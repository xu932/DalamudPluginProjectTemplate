using System;
using System.Diagnostics;

using WindowsInput;
using WindowsInput.Native;

using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Plugin;
using Dalamud.Logging;
using Dalamud.Game.ClientState.Objects;

using CottonCollector.Attributes;
using CottonCollector.Config;
using CottonCollector.Interface;

namespace CottonCollector
{
    internal class CottonCollectorPlugin : IDalamudPlugin
    {
        [PluginService]
        internal static DalamudPluginInterface DalamudPluginInterface { get; private set; }

        [PluginService]
        internal static DataManager DataManager { get; private set; }

        [PluginService]
        internal static ClientState ClientState { get; private set; }

        [PluginService]
        internal static CommandManager CommandManager { get; private set; }

        [PluginService]
        internal static ChatGui ChatGui { get; private set; }

        [PluginService]
        internal static Framework Framework { get; private set; }

        [PluginService]
        internal static ObjectTable ObjectTable { get; private set; }

        [PluginService]
        internal static KeyState KeyState { get; private set; }

        internal static CottonCollectorConfig config { get; set; }
        private readonly ConfigWindow configWindow;
        private readonly PluginCommandManager<CottonCollectorPlugin> pluginCommandManager;

        public CharacterControl.Commands Commands;

        public string Name => "Cotton Collector";

        // Temporary
        private Tuple<VirtualKey, int> command;
        private bool done = true;
        private Stopwatch timer = new Stopwatch();
        private InputSimulator sim = new InputSimulator();

        public CottonCollectorPlugin()
        {
            // Ini commands manager
            Commands = new CharacterControl.Commands();

            // Load config window
            config = DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            configWindow = new ConfigWindow(this);
            DalamudPluginInterface.UiBuilder.Draw += configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi += configWindow.Open;
            Framework.Update += Update;

            // Load all commands
            pluginCommandManager = new PluginCommandManager<CottonCollectorPlugin>(this);

        }

        [Command("/cottoncollectormonitor")]
        [Aliases("/ccm")]
        public void OpenMonitorWindowCommand(string command, string args)
        {
            configWindow.Toggle();
        }

        private void Update(Framework framework)
        {
            if (done && Commands.commands.Count != 0)
            {
                PluginLog.Log("Exectuing Command");
                command = Commands.commands.Dequeue();
                done = false;
                timer.Start();
                
                sim.Keyboard.KeyDown((VirtualKeyCode)((int)command.Item1));
            }

            //PluginLog.Log($"{(DateTime.UtcNow - timestamp).Milliseconds}");

            if (!done && command != null) {
                if (timer.ElapsedMilliseconds > command.Item2)
                {
                    sim.Keyboard.KeyUp((VirtualKeyCode)((int)command.Item1));
                    PluginLog.Log($"Command Finished, time elapsed: {timer.Elapsed}");
                    timer.Stop();
                    timer.Reset();
                    done = true;
                }
            }
        }

        public void Dispose()
        {
            DalamudPluginInterface.UiBuilder.Draw -= configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi -= configWindow.Open;
            Framework.Update -= Update;
            if (pluginCommandManager != null) pluginCommandManager.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
