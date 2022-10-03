using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

namespace CottonCollector.BackgroundInputs
{
    internal static class BgInput
    {

        #region user32 imports
        [StructLayout(LayoutKind.Sequential)]
        public struct MSG
        {
            public IntPtr handle;
            public uint msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point p;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWinEventHook(int eventMin, int eventMax, IntPtr hmodWinEventProc, 
            WinEventProc lpfnWinEventProc, int idProcess, int idThread, SetWinEventHookFlags dwflags);
        [DllImport("user32.dll")]
        static extern bool UnhookWinEvent(IntPtr hWinEventHook);
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, Int32 lParam);
        [DllImport("user32.dll")]
        private static extern bool PeekMessage(out MSG lpMsg, IntPtr hwnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        [DllImport("user32.dll")]
        private static extern bool TranslateMessage(ref MSG lpMsg);
        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage(ref MSG lpMsg);
        #endregion

        #region delegates
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);
        private delegate void WinEventProc(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, 
            int idChild, int dwEventThread, int dwmsEventTime);
        #endregion

        #region const and enums
        private const string FF_WND_NAME = "FINAL FANTASY XIV";

        private const uint WM_KEYUP = 0x101;
        private const uint WM_KEYDOWN = 0x100;
        private const int EVENT_SYSTEM_FOREGROUND = 0x0003;

        private enum SetWinEventHookFlags
        {
            WINEVENT_INCONTEXT = 4,
            WINEVENT_OUTOFCONTEXT = 0,
            WINEVENT_SKIPOWNPROCESS = 2,
            WINEVENT_SKIPOWNTHREAD = 1
        }
        #endregion

        #region window utils
        private static string GetWindowTextFromHandle(IntPtr hWnd)
        {
            int len = GetWindowTextLength(hWnd);
            if (len > 0)
            {
                var builder = new StringBuilder(len + 1);
                GetWindowText(hWnd, builder, len + 1);
                return builder.ToString();
            }
            return string.Empty;
        }

        private static IntPtr FindFFXIVWindow()
        {
            List<IntPtr> wins = new();
            Regex rex = new(FF_WND_NAME);

            EnumWindows((hWnd, lParam) =>
                {
                    try
                    {
                        string t = GetWindowTextFromHandle(hWnd);
                        Match m = rex.Match(t);
                        if (m.Success == true)
                        {
                            wins.Add(hWnd);
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return true;
                }, IntPtr.Zero);
            return wins.Where(hWnd => {
                _ = GetWindowThreadProcessId(hWnd, out uint wpid);
                return wpid == Process.GetCurrentProcess().Id;
            }).FirstOrDefault((IntPtr)0);
        }
        #endregion

        private static IntPtr hWndGame = FindFFXIVWindow();
        private static IntPtr hFocusHook = IntPtr.Zero;
        private static WinEventProc focusListener = new(ForegroundChangedCallback);
        private static bool gameIsFocused = true;
        private static bool disposing = false;
        private static Thread t;

        private static HashSet<VirtualKey> pressedKeys = new();

        private static void ThreadProc ()
        {
            PluginLog.Verbose($"Initializing BgInput hWndGame {hWndGame}");
            hFocusHook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
                focusListener, 0, 0, SetWinEventHookFlags.WINEVENT_OUTOFCONTEXT);
            PluginLog.Verbose($"hFocusHook {hFocusHook}");
            MSG msg;
            while(!disposing)
            {
                if (PeekMessage(out msg, IntPtr.Zero, 0, 0, 1))
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
                Thread.Sleep(0);
            }
            var ret = UnhookWinEvent(hFocusHook);
            PluginLog.Verbose($"Unhook BgInput {hFocusHook}? {ret}");
        }

        private static void ResumePressedKeys()
        {
            foreach (VirtualKey pressedKey in pressedKeys)
            {
                KeyDown(pressedKey);
                while (!CottonCollectorPlugin.KeyState[pressedKey])
                {
                    Thread.Sleep(5);
                    KeyDown(pressedKey);
                }
            }
        }

        internal static void ForegroundChangedCallback(IntPtr hWinEventHook, int iEvent, IntPtr hWnd, int idObject, 
            int idChild, int dwEventThread, int dwmsEventTime)
        {
            PluginLog.Log($"Something Happened hWnd {hWnd}, hWndGame {hWndGame}");
            if (hWnd == hWndGame)
            {
                PluginLog.Log("Re-Focused FFXIV!");
                gameIsFocused = true;
                ResumePressedKeys();
            }
            else if (gameIsFocused)
            {
                PluginLog.Log("Un-Focused FFXIV!");
                gameIsFocused = false;
                ResumePressedKeys();
            }
        }

        public static void Initialize()
        {
            t = new(new ThreadStart(ThreadProc));
            t.Start();
        }

        public static void Clear()
        {
            pressedKeys.Clear();
        }

        public static void Dispose()
        {
            disposing = true;
            t.Join();
        }

        internal enum Modifier
        {
            NONE = 0,
            SHIFT = 1,
            CTRL = 2,
            ALT = 3,
        }

        private static VirtualKey? ModifierToVk(Modifier mod)
        {
            if (mod == Modifier.NONE) return null;
            switch (mod)
            {
                case Modifier.SHIFT:
                    return VirtualKey.LSHIFT;
                case Modifier.CTRL:
                    return VirtualKey.LCONTROL;
                default:
                    return VirtualKey.LMENU;
            }
        }

        public static void KeyDown(VirtualKey vk, Modifier mod = Modifier.NONE)
        {
            var modVk = ModifierToVk(mod);
            if (modVk != null)
            {
                pressedKeys.Add(modVk.Value);
                SendMessage(hWndGame, WM_KEYDOWN, (IntPtr)modVk, (IntPtr)0);
            }
            pressedKeys.Add(vk);
            SendMessage(hWndGame, WM_KEYDOWN, (IntPtr)vk, (IntPtr)0);
        }

        public static void KeyUp(VirtualKey vk, Modifier mod = Modifier.NONE)
        {
            SendMessage(hWndGame, WM_KEYUP, (IntPtr)vk, (IntPtr)0);
            pressedKeys.Remove(vk);
            var modVk = ModifierToVk(mod);
            if (modVk != null)
            {
                SendMessage(hWndGame, WM_KEYUP, (IntPtr)modVk, (IntPtr)0);
                pressedKeys.Remove(modVk.Value);
            }
        }

        public static void KeyPress(VirtualKey vk, Modifier mod = Modifier.NONE)
        {
            var modVk = ModifierToVk(mod);
            if (modVk != null)
            {
                SendMessage(hWndGame, WM_KEYDOWN, (IntPtr)modVk, (IntPtr)0);
            }
            SendMessage(hWndGame, WM_KEYDOWN, (IntPtr)vk, (IntPtr)0);
            Thread.Sleep(10);
            SendMessage(hWndGame, WM_KEYUP, (IntPtr)vk, (IntPtr)0);
            if (modVk != null)
            {
                SendMessage(hWndGame, WM_KEYUP, (IntPtr)modVk, (IntPtr)0);
            }
        }
    }
}
