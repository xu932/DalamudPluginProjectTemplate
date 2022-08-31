using Dalamud.Data;
using Dalamud.IoC;
using Dalamud.Game.ClientState;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.Interface.Windowing;
using Dalamud.Logging;
using Dalamud.Plugin;

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
        internal static ClientState clientState { get; private set; }

        [PluginService]
        internal static CommandManager commandManager { get; private set; }

        [PluginService]
        internal static ChatGui chatGui { get; private set; }

        internal static CottonCollectorConfig config { get; set; }
        private static ConfigWindow ConfigWindow;

        public string Name => "Cotton Collector";

        public CottonCollectorPlugin()
        {
            config = DalamudPluginInterface.GetPluginConfig() as CottonCollectorConfig ?? new CottonCollectorConfig();
            ConfigWindow = new ConfigWindow();
            DalamudPluginInterface.UiBuilder.Draw += ConfigWindow.Draw;
            DalamudPluginInterface.UiBuilder.OpenConfigUi += ConfigWindow.Open;
        }

        public void Dispose()
        {
        }
    }
}
