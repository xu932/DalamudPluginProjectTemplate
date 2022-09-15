using System;
using System.Diagnostics;

using ImGuiNET;

namespace CottonCollector.CharacterControl.Commands
{
    [Serializable]
    internal class SleepCommand : Command
    {
        int mili = 1000;
        private Stopwatch stopwatch = new Stopwatch();

        public override bool TerminateCondition() {
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
            ImGui.InputInt($"##SleepCommand__Input__{this.GetHashCode()}", ref mili);

            ImGui.SameLine();
            ImGui.Text(" mili seconds");
            ImGui.PopItemWidth();
        }
    }
}
