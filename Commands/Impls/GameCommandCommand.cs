using System;
using System.Runtime.InteropServices;
using System.Text;
using Newtonsoft.Json;

using ImGuiNET;

using Dalamud.Logging;
using Dalamud.Hooking;
using Dalamud.Game;

using CottonCollector.Commands.Structures;
using CottonCollector.Util;

namespace CottonCollector.Commands.Impls
{
    class SeFunctionBase<T> where T : Delegate
    {
        public IntPtr Address;
        protected T FuncDelegate;

        public SeFunctionBase(SigScanner sigScanner, int offset)
        {
            Address = sigScanner.Module.BaseAddress + offset;
            PluginLog.Debug($"{GetType().Name} address 0x{Address.ToInt64():X16}, baseOffset 0x{offset:X16}.");
        }

        public SeFunctionBase(SigScanner sigScanner, string signature, int offset = 0)
        {
            Address = sigScanner.ScanText(signature);
            if (Address != IntPtr.Zero)
                Address += offset;
            var baseOffset = (ulong)Address.ToInt64() - (ulong)sigScanner.Module.BaseAddress.ToInt64();
            PluginLog.Debug($"{GetType().Name} address 0x{Address.ToInt64():X16}, baseOffset 0x{baseOffset:X16}.");
        }

        public T Delegate()
        {
            if (FuncDelegate != null)
                return FuncDelegate;

            if (Address != IntPtr.Zero)
            {
                FuncDelegate = Marshal.GetDelegateForFunctionPointer<T>(Address);
                return FuncDelegate;
            }

            PluginLog.Error($"Trying to generate delegate for {GetType().Name}, but no pointer available.");
            return null;
        }

        public dynamic Invoke(params dynamic[] parameters)
        {
            if (FuncDelegate != null)
                return FuncDelegate.DynamicInvoke(parameters);

            if (Address != IntPtr.Zero)
            {
                FuncDelegate = Marshal.GetDelegateForFunctionPointer<T>(Address);
                return FuncDelegate!.DynamicInvoke(parameters);
            }
            else
            {
                PluginLog.Error($"Trying to call {GetType().Name}, but no pointer available.");
                return null;
            }
        }

        public Hook<T> CreateHook(T detour)
        {
            if (Address != IntPtr.Zero)
            {
                var hook = Hook<T>.FromAddress(Address, detour);
                hook.Enable();
                PluginLog.Debug($"Hooked onto {GetType().Name} at address 0x{Address.ToInt64():X16}.");
                return hook;
            }

            PluginLog.Error($"Trying to create Hook for {GetType().Name}, but no pointer available.");
            return null;
        }
    }

    public delegate IntPtr ProcessChatBoxDelegate(IntPtr uiModule, IntPtr message, IntPtr unk1, byte unk2);

    sealed class ProcessChatBox : SeFunctionBase<ProcessChatBoxDelegate>
    {
        public ProcessChatBox(SigScanner sigScanner)
            : base(sigScanner, "48 89 5C 24 ?? 57 48 83 EC 20 48 8B FA 48 8B D9 45 84 C9")
        { }
    }

    internal class GameCommandCommand : Command
    {
        [JsonProperty] public string cmd = "";

        private static readonly IntPtr uiModulePtr =
            CottonCollectorPlugin.GameGui.GetUIModule();

        protected override int MinTimeMili { get; } = 500;

        protected override bool TerminateCondition() => true;

        protected override void Do()
        {
            if (CottonCollectorPlugin.CommandManager.ProcessCommand(cmd))
            {
                PluginLog.Log($"Executed Command: {cmd}");
            }
            else
            {
                var (text, length) = PrepareString(cmd);
                var payload = PrepareContainer(text, length);
                ProcessChatBox processChatBox = new(new SigScanner());

                processChatBox.Invoke(uiModulePtr, payload, IntPtr.Zero, (byte)0);

                Marshal.FreeHGlobal(payload);
                Marshal.FreeHGlobal(text);
            }
        }

        private static (IntPtr, long) PrepareString(string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var mem = Marshal.AllocHGlobal(bytes.Length + 30);
            Marshal.Copy(bytes, 0, mem, bytes.Length);
            Marshal.WriteByte(mem + bytes.Length, 0);
            return (mem, bytes.Length + 1);
        }
        private static IntPtr PrepareContainer(IntPtr message, long length)
        {
            var mem = Marshal.AllocHGlobal(400);
            Marshal.WriteInt64(mem, message.ToInt64());
            Marshal.WriteInt64(mem + 0x8, 64);
            Marshal.WriteInt64(mem + 0x10, length);
            Marshal.WriteInt64(mem + 0x18, 0);
            return mem;
        }

        #region GUI
        internal override void SelectorGui()
        {
            ImGui.Text("Game Command:");
            ImGui.SameLine();

            ImGui.PushItemWidth(100);
            ImGui.InputText(Ui.Uid(index: uid), ref cmd, 100);
            ImGui.PopItemWidth();
        }

        internal override void MinimalInfo()
        {
            base.MinimalInfo();
            ImGui.Text($"cmd: {cmd}");
        }
        #endregion
    }
}
