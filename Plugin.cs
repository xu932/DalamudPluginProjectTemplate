using System;
using System.Numerics;

using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Aetherytes;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Gui;
using Dalamud.Logging;
using Dalamud.Plugin;

using CottonCollector.Attributes;
using CottonCollector.CameraManager;
using CottonCollector.Config;
using CottonCollector.Interface;
using CottonCollector.Commands.Impls;
using CottonCollector.Commands.Structures;
using CottonCollector.BackgroundInputs;
using CottonCollector.Commands.Conditions;

namespace CottonCollector
{
    internal unsafe class CottonCollectorPlugin : IDalamudPlugin
    {
        [PluginService]
        internal static DalamudPluginInterface DalamudPluginInterface { get; private set; }

        [PluginService]
        internal static DataManager DataManager { get; private set; }

        [PluginService]
        internal static ClientState ClientState { get; private set; }

        [PluginService]
        internal static Dalamud.Game.Command.CommandManager CommandManager { get; private set; }

        [PluginService]
        internal static GameGui GameGui { get; private set; }

        [PluginService]
        internal static ChatGui ChatGui { get; private set; }

        [PluginService]
        internal static Framework Framework { get; private set; }

        [PluginService]
        internal static ObjectTable ObjectTable { get; private set; }

        [PluginService]
        internal static KeyState KeyState { get; private set; }

        [PluginService]
        internal static AetheryteList AetheryteList { get; private set; }

        [PluginService]
        internal static TargetManager TargetManager { get; private set; }

        [PluginService]
        internal static Dalamud.Game.ClientState.Conditions.Condition GameCondition { get; private set; }

        internal static CommandManager rootCmdManager = new(true);

        internal static CottonCollectorConfig config { get; set; }

        internal static CommandSet selectedCommandSet = null;

        private readonly ConfigWindow configWindow;
        private readonly PluginCommandManager<CottonCollectorPlugin> pluginCommandManager;


        public string Name => "Cotton Collector";

        public CottonCollectorPlugin()
        {
            // Ini CameraHelpers
            CameraHelpers.Initialize();
            BgInput.Initialize();

            // Load config window
            config = DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            configWindow = new ConfigWindow();
            DalamudPluginInterface.UiBuilder.Draw += configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi += configWindow.Open;
            Framework.Update += rootCmdManager.Update;

            // Load all commands
            pluginCommandManager = new PluginCommandManager<CottonCollectorPlugin>(this);
        }

        [Command("/cottoncollectorconfig")]
        [Aliases("/ccc")]
        [HelpMessage("Open cotton collector config.")]
        public void OpenMonitorWindowCommand(string command, string args)
        {
            configWindow.Toggle();
        }

        [Command("/ccselect")]
        [HelpMessage("/ccselect <command set name>")]
        public void SelectPreset(string command, string args)
        {
            CommandSet.CommandSetMap.TryGetValue(args, out selectedCommandSet);
            if (selectedCommandSet == null)
            {
                ChatGui.Print($"CommandSet {args} does not exist.");
            }
            else
            {
                ChatGui.Print($"Selected CommandSet {args}");
            }
        }

        [Command("/ccaddwaymark")]
        [Aliases("/ccaw")]
        [HelpMessage("Add a waymark to selected CommandSet.")]
        public void RecordPos(string command, string args)
        {
            if (selectedCommandSet != null)
            {
                var player = ClientState.LocalPlayer;
                var moveTo = new MoveToCommand();
                moveTo.SetTarget(player.Position);

                PluginLog.Log($"added waymark {player.Position} in {selectedCommandSet.uniqueId} at index {selectedCommandSet.subCommands.Count}");
                selectedCommandSet.subCommands.AddLast(moveTo);
            }
            else
            {
                ChatGui.Print("No selected CommandSet.");
            }
        }

        [Command("/ccplay")]
        [HelpMessage("/ccplay <integer>. Play selected CommandSet <n> times. Play once if you do not pass anything.")]
        public void PlaySelectedCommandSet(string command, string args)
        {
            if (selectedCommandSet != null)
            {
                if (string.IsNullOrEmpty(args))
                {
                    rootCmdManager.Schedule(selectedCommandSet);
                    ChatGui.Print($"Playing CommandSet {selectedCommandSet.uniqueId}.");
                }
                else if (int.TryParse(args, out int times))
                {
                    for (int i = 0; i < times; i++)
                    {
                        rootCmdManager.Schedule(selectedCommandSet);
                    }
                    ChatGui.Print($"Playing CommandSet {selectedCommandSet.uniqueId} {args} times.");
                }
                else 
                {
                    ChatGui.Print($"{args} is not a valid integer.");
                }
            }
            else
            {
                ChatGui.Print("No selected CommandSet.");
            }
        }

        [Command("/cckill")]
        [HelpMessage("Kill all current running and queued commands.")]
        public void Kill(string command, string args)
        {
            rootCmdManager.KillSwitch();
            BgInput.Clear();
            KeyState.ClearAll();
        }

        public void Dispose()
        {
            BgInput.Dispose();
            DalamudPluginInterface.UiBuilder.Draw -= configWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi -= configWindow.Open;
            Framework.Update -= rootCmdManager.Update;
            pluginCommandManager?.Dispose();
            CommandSet.CommandSetMap.Clear();

            GC.SuppressFinalize(this);
        }
    }
}
