using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImGuiNET;

using CottonCollector.CameraManager;
using CottonCollector.Config;

namespace CottonCollector.Interface
{
    internal unsafe class CameraInfoTab : ConfigTab
    {
        public CameraInfoTab(ref CottonCollectorConfig config) : base ("Camera Info", ref config)
        {

        }

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
        }
    }
}
