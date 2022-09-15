using System.Numerics;

using ImGuiNET;

using CottonCollector.CameraManager;

namespace CottonCollector.CharacterControl.Commands
{
    internal unsafe class TillLookedAtCommand : Command
    {
        public double X, Y, Z;

        public TillLookedAtCommand() : base(Type.TILL_LOOKED_AT_COMMAND) { }

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

            ImGui.Text("X:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillLookedAtCommand__X", ref X);

            ImGui.SameLine();
            ImGui.Text("Y:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillLookedAtCommand__Y", ref Y);

            ImGui.SameLine();
            ImGui.Text("Z:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillLookedAtCommand__Z", ref Z);

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
