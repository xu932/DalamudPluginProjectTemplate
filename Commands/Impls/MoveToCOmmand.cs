using System.Collections.Generic;
using System.Numerics;

using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.CameraManager;
using CottonCollector.Commands.Structures;

namespace CottonCollector.Commands.Impls
{
    /*
    internal unsafe class MoveToCommand: Command
    {
        private static readonly double CROSS_THRESHOLD = 1e-3;

        public double X, Y, Z, threshold = 1.0f;

        private LinkedListNode<Command> nextCommandNode = null;
        private int isAdjusting = 0; // 0 not adjusting
                                     // 1 is adjusting left
                                     // 2 is adjusting right

        public override bool TerminateCondition()
        {
            var target = new Vector3((float)X, (float)Y, (float)Z);
            return Vector3.Distance(CottonCollectorPlugin.ClientState.LocalPlayer.Position, target) < threshold;
        }

        public override void OnStart()
        {
            var pressRight = CameraOnLeft();
            var pressLeft = CameraOnRight();

            if (!pressLeft && !pressRight)
            {
                return;
            }

            var cmdlist = new List<Command>
            {
                new KeyboardCommand(),
                new TillLookedAtCommand(),
                new KeyboardCommand()
            };

            var vk = pressLeft ? Dalamud.Game.ClientState.Keys.VirtualKey.RIGHT
                : Dalamud.Game.ClientState.Keys.VirtualKey.LEFT;
            ((KeyboardCommand)cmdlist[0]).vk = vk;
            ((KeyboardCommand)cmdlist[2]).vk = vk;
            ((KeyboardCommand)cmdlist[0]).actionType = KeyboardCommand.ActionType.KEY_DOWN;
            ((KeyboardCommand)cmdlist[2]).actionType = KeyboardCommand.ActionType.KEY_UP;

            nextCommandNode = cmdManager.ScheduleFront(cmdlist);
        }

        public override void Do()
        {
            var pressW = new KeyboardCommand();

            pressW.vk = Dalamud.Game.ClientState.Keys.VirtualKey.W;
            pressW.actionType = KeyboardCommand.ActionType.KEY_DOWN;

            cmdManager.ScheduleBefore(nextCommandNode, pressW);
        }

        public override void OnUpdate()
        {
            var pressRight = CameraOnLeft();
            var pressLeft = CameraOnRight();

            if (!pressLeft && !pressRight)
            {
                isAdjusting = 0;
                return;
            }

            if (pressLeft && isAdjusting != 1)
            {
                isAdjusting = 1;

            }
            else if (pressRight && isAdjusting != 2)
            {
                isAdjusting = 2;

            }
        }

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

        private double CameraPlayerCrossTargetPlayer()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var targetPos2 = new Vector2((float)X, (float)Z);
            var playerPos2 = new Vector2(player.Position.X, player.Position.Z);
            var cameraPos2 = new Vector2(CameraHelpers.collection->WorldCamera->X, 
                CameraHelpers.collection->WorldCamera->Y);
            var v = Vector2.Normalize(cameraPos2 - playerPos2);
            var u = Vector2.Normalize(targetPos2 - playerPos2);

            return v.X * u.Y - v.Y * u.X;
        }

        private bool CameraOnLeft()
        {
            return CameraPlayerCrossTargetPlayer() < -CROSS_THRESHOLD;
        }

        private bool CameraOnRight()
        {
            return CameraPlayerCrossTargetPlayer() > CROSS_THRESHOLD;
        }
    }
    */
}
