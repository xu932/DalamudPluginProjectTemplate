using System.Numerics;

using ImGuiNET;

using CottonCollector.CameraManager;
using CottonCollector.Config;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using System;

namespace CottonCollector.Interface
{
    internal unsafe class CameraInfoTab : ConfigTab
    {
        public CameraInfoTab(ref CottonCollectorConfig config) : base ("Camera Info", ref config) { }

        public override void TabContent()
        {
            var playerPos = CottonCollectorPlugin.ClientState.LocalPlayer.Position;
            ImGui.Text($"Camera X: {CameraHelpers.collection->WorldCamera->X:#.00}");
            ImGui.SameLine();
            ImGui.Text($"Player X: {playerPos.X:#.00}");
            ImGui.SameLine();
            ImGui.Text($"delta X: {playerPos.X - CameraHelpers.collection->WorldCamera->X:#.00}");
            // For cameras, Z is actually Y
            ImGui.Text($"Camera Y: {CameraHelpers.collection->WorldCamera->Z:#.00}");
            ImGui.SameLine();
            ImGui.Text($"Player Y: {playerPos.Y:#.00}");
            ImGui.SameLine();
            ImGui.Text($"delta Y: {playerPos.Y - CameraHelpers.collection->WorldCamera->Z:#.00}");
            // Ditto
            ImGui.Text($"Camera Z: {CameraHelpers.collection->WorldCamera->Y:#.00}");
            ImGui.SameLine();
            ImGui.Text($"Player Z: {playerPos.Z:#.00}");
            ImGui.SameLine();
            ImGui.Text($"delta Z: {playerPos.Z - CameraHelpers.collection->WorldCamera->Y:#.00}");

            GameObject target = CottonCollectorPlugin.ClientState.LocalPlayer.TargetObject;

            if (target == null) return;

            var playerPos2 = new Vector2(playerPos.X, playerPos.Z);
            var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                CameraHelpers.collection->WorldCamera->Y);
            var aetherytePos = new Vector2(target.Position.X, target.Position.Z);
            var v = Vector2.Normalize(cameraPos - playerPos2);
            var u = Vector2.Normalize(aetherytePos - playerPos2);
            var is_right = (v.X * u.Y - u.X * v.Y) < 0;


            ImGui.Text($"player to camera:{v}");
            ImGui.Text($"player to traget:{u}");
            ImGui.Text($"diff:{(v + u).LengthSquared()}");
            ImGui.Text(is_right ? "left" : "right");
        }
    }
}
