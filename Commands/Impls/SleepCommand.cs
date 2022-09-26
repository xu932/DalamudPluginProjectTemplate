using System.Diagnostics;
using Newtonsoft.Json;

using ImGuiNET;

using CottonCollector.Util;
using CottonCollector.Commands.Structures;

namespace CottonCollector.Commands.Impls
{
    internal class SleepCommand : Command
    {
        [JsonProperty] public int mili = 1000;
        private Stopwatch stopwatch = new Stopwatch();

        public override void MinimalInfo()
        {
            base.MinimalInfo();
            ImGui.Text($"Sleep for {mili} miliseconds");
        }

        public override bool TerminateCondition()
        {
            return stopwatch.ElapsedMilliseconds > mili;
        }

        public override void Do()
        {
            stopwatch.Reset();
            stopwatch.Start();
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);
            ImGui.Text("Sleep for ");

            ImGui.SameLine();
            ImGui.InputInt(Ui.Uid(), ref mili);

            ImGui.SameLine();
            ImGui.Text(" mili seconds");
            ImGui.PopItemWidth();
        }
    }
}
