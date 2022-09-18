using System;
using System.Numerics;

using ImGuiNET;

using CottonCollector.Commands.Structures;

namespace CottonCollector.Commands.Impls
{
    [Serializable]
    internal class TillMovedToCommand : Command
    {
        public double X, Y, Z, threshold = 1.0f;

        public TillMovedToCommand() : base(Type.TILL_MOVED_TO_COMMAND) { }
        public override bool TerminateCondition()
        {
            var target = new Vector3((float)X, (float)Y, (float)Z);
            return Vector3.Distance(CottonCollectorPlugin.ClientState.LocalPlayer.Position, target) < threshold;
        }

        public override void Do() { }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            ImGui.Text("X:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__X", ref X);

            ImGui.SameLine();
            ImGui.Text("Y:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__Y", ref Y);

            ImGui.SameLine();
            ImGui.Text("Z:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__Z", ref Z);

            ImGui.SameLine();
            ImGui.Text("threshold:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__threshold", ref threshold);

            ImGui.SameLine();
            if (ImGui.Button($"GetCurrentPos##TillMovedToCommand__getpos"))
            {
                var localPlayer = CottonCollectorPlugin.ClientState.LocalPlayer;
                X = localPlayer.Position.X;
                Y = localPlayer.Position.Y;
                Z = localPlayer.Position.Z;
            }

            ImGui.PopItemWidth();
        }
    }
}
