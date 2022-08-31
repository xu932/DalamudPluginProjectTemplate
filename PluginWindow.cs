using Dalamud.Interface.Windowing;
using ImGuiNET;
using System.Numerics;

namespace CottonCollector
{
    public class PluginWindow : Window
    {
        public PluginWindow() : base("TemplateWindow")
        {
            IsOpen = true;
            Size = new Vector2(810, 520);
            SizeCondition = ImGuiCond.FirstUseEver;
        }

        public override void Draw()
        {
            ImGui.Text("Hello, world!");
        }
    }
}
