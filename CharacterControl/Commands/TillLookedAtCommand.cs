using System.Numerics;

using ImGuiNET;

using CottonCollector.CameraManager;

namespace CottonCollector.CharacterControl.Commands
{
    internal unsafe class TillLookedAtCommand : Command
    {
        private double X, Y, Z;

        public override bool TerminateCondition()
        {
            var targetPos = new Vector2((float)X, (float)Z);
            var playerPos3 = CottonCollectorPlugin.ClientState.LocalPlayer.Position;
            var playerPos = new Vector2(playerPos3.X, playerPos3.Z);
            var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                CameraHelpers.collection->WorldCamera->Y);
            var v = Vector2.Normalize(cameraPos - playerPos);
            var u = Vector2.Normalize(targetPos - playerPos);
            return (v + u).LengthSquared() < 1e-3f;
        }

        public override void Do() { }

        public override void SelectorGui()
        {            
            ImGui.PushItemWidth(100);

            ImGui.InputDouble("X:##TillLookedAtCommand__X", ref X);

            ImGui.SameLine();
            ImGui.InputDouble("Y:##TillLookedAtCommand__Y", ref Y);

            ImGui.SameLine();
            ImGui.InputDouble("Z:##TillLookedAtCommand__Z", ref Y);

            ImGui.SameLine();
            if (ImGui.Button($"GetCurrentPos##TillLookedAtCommand__getpos"))
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
