using System.Numerics;

using ImGuiNET;

namespace CottonCollector.CharacterControl.Commands
{
    internal class TillMovedToCommand : Command
    {
        private double X, Y, Z, threshold = 1.0f;

        public override bool TerminateCondition()
        {
            var target = new Vector3((float)X, (float)Y, (float)Z);
            return Vector3.Distance(CottonCollectorPlugin.ClientState.LocalPlayer.Position, target) < threshold;
        }

        public override void Do() { }

        public override void SelectorGui()
        {            
            ImGui.PushItemWidth(100);

            ImGui.InputDouble("X:##TillMovedToCommand__X", ref X);

            ImGui.SameLine();
            ImGui.InputDouble("Y:##TillMovedToCommand__Y", ref Y);

            ImGui.SameLine();
            ImGui.InputDouble("Z:##TillMovedToCommand__Z", ref Y);

            ImGui.SameLine();
            ImGui.InputDouble("threshold:##TillMovedToCommand__threshold", ref threshold);

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
