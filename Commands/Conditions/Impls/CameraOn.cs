﻿using System;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;

using ImGuiNET;

using CottonCollector.CameraManager;

namespace CottonCollector.Commands.Conditions.Impls
{
    internal unsafe class CameraOn : Condition
    {
        public enum ConditionType
        {
            ON_LEFT_OF = 0,
            ON_RIGHT_OF = 1,
            FACING = 2,
        };

        [JsonProperty]
        private ConditionType type = ConditionType.FACING;

        [JsonProperty]
        private Vector3 targetPos = new();

        private static float CROSS_THRESHOLD = 0.2f;

        public override bool triggeringCondition()
        {
            var cross = CameraPlayerCrossTargetPlayer(targetPos);
            var dist = CameraPlayerToTargetPlayerDist(targetPos);
            switch(type)
            {
                case ConditionType.ON_LEFT_OF:
                    return cross < -CROSS_THRESHOLD;
                case ConditionType.ON_RIGHT_OF:
                    return cross > CROSS_THRESHOLD;
                case ConditionType.FACING:
                    return cross >= -CROSS_THRESHOLD && cross <= CROSS_THRESHOLD && dist < 1.414;
                default:
                    // Not ever happening.
                    return false;
            }
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);
            var conditionTypes = Enum.GetValues(typeof(ConditionType)).Cast<ConditionType>().ToList();
            var typeIndex = conditionTypes.IndexOf(type);
            if (ImGui.Combo("##CameraOn__TypeSelector", ref typeIndex, conditionTypes.Select(t=>t.ToString()).ToArray(), 
                conditionTypes.Count))
            {
                type = conditionTypes[typeIndex];
            }

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
            return $"On camera {type} of target:{targetPos}";
        }

        private double CameraPlayerCrossTargetPlayer(Vector3 target)
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var targetPos2 = new Vector2(targetPos.X, targetPos.Z);
            var playerPos2 = new Vector2(player.Position.X, player.Position.Z);
            var cameraPos2 = new Vector2(CameraHelpers.collection->WorldCamera->X,
                CameraHelpers.collection->WorldCamera->Y);
            var v = Vector2.Normalize(cameraPos2 - playerPos2);
            var u = Vector2.Normalize(targetPos2 - playerPos2);

            return v.X * u.Y - v.Y * u.X;
        }
        private double CameraPlayerToTargetPlayerDist(Vector3 target)
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var targetPos2 = new Vector2(targetPos.X, targetPos.Z);
            var playerPos2 = new Vector2(player.Position.X, player.Position.Z);
            var cameraPos2 = new Vector2(CameraHelpers.collection->WorldCamera->X,
                CameraHelpers.collection->WorldCamera->Y);
            var v = Vector2.Normalize(cameraPos2 - playerPos2);
            var u = Vector2.Normalize(targetPos2 - playerPos2);

            return Vector2.Distance(u, v);
        }
    }
}
