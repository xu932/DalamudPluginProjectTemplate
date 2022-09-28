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
        private readonly Stopwatch stopwatch = new Stopwatch();

        protected override bool TerminateCondition() =>
            stopwatch.ElapsedMilliseconds > mili;

        protected override void Do()
        {
            stopwatch.Start();
        }

        internal override void ResetExecutionState()
        {
            stopwatch.Stop();
            stopwatch.Reset();
        }

        #region GUI
        internal override void MinimalInfo()
        {
            base.MinimalInfo();
            ImGui.Text($"Sleep for {mili} miliseconds");
        }

        internal override void SelectorGui()
        {
            ImGui.PushItemWidth(100);
            ImGui.Text("Sleep for ");

            ImGui.SameLine();
            ImGui.InputInt(Ui.Uid(index: uid), ref mili);

            ImGui.SameLine();
            ImGui.Text(" mili seconds");
            ImGui.PopItemWidth();
        }
        #endregion
    }
}
