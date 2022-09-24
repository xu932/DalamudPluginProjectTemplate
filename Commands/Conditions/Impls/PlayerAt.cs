using System;
using System.Numerics;
using Newtonsoft.Json;

using ImGuiNET;

namespace CottonCollector.Commands.Conditions.Impls
{
    internal class PlayerAtTarget : Condition
    {
        [JsonProperty] private Vector3 targetPos = new();

        [JsonProperty] private float distThreshold = 1.0f;

        public override bool triggeringCondition()
        {
            return Vector3.Distance(targetPos, CottonCollectorPlugin.ClientState.LocalPlayer.Position) < distThreshold;
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            ImGui.Text("X:");
            ImGui.SameLine();
            ImGui.InputFloat("##TillMovedToCommand__X", ref targetPos.X);

            ImGui.SameLine();
            ImGui.Text("Y:");
            ImGui.SameLine();
            ImGui.InputFloat("##TillMovedToCommand__Y", ref targetPos.Y);

            ImGui.SameLine();
            ImGui.Text("Z:");
            ImGui.SameLine();
            ImGui.InputFloat("##TillMovedToCommand__Z", ref targetPos.Z);

            ImGui.Text("threshold:");
            ImGui.SameLine();
            ImGui.InputFloat("##TillMovedToCommand__threshold", ref distThreshold);

            ImGui.SameLine();
            if (ImGui.Button($"GetCurrentPos##TillMovedToCommand__getpos"))
            {
                var localPlayer = CottonCollectorPlugin.ClientState.LocalPlayer;
                targetPos.X = localPlayer.Position.X;
                targetPos.Y = localPlayer.Position.Y;
                targetPos.Z = localPlayer.Position.Z;
            }

            ImGui.PopItemWidth();
        }

        public override string Description()
        {
            return $"On player within threshold:{distThreshold} of target:{targetPos}";
        }
    }
}
