using System;
using System.Numerics;
using System.Threading;
using Newtonsoft.Json;

using ImGuiNET;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

using CottonCollector.BackgroundInputs;
using CottonCollector.Commands.Structures;
using CottonCollector.CameraManager;
using CottonCollector.Util;

namespace CottonCollector.Commands.Impls
{
    internal unsafe class MoveToCommand: Command
    {
        [JsonProperty] public Vector3 targetPos;

        private bool finished = false;
        private int xMove = 0;
        private int yMove = 0;
        private int turn = 0; // + left - right

        public MoveToCommand()
        {
            shouldRepeat = true;
        }

        public override bool TerminateCondition()
        {
            return finished;
        }

        public override void MinimalInfo()
        {
            base.MinimalInfo();
            ImGui.Text($"Move to {this.targetPos}");
            ImGui.SameLine(ImGui.GetColumnWidth() - 90);
            Vector3? currPos = Ui.GetCurrPosBtn("GetCurrPos", this.GetType(), this.GetHashCode().ToString());
            if (currPos != null)
            {
                targetPos = currPos.Value;
            }
        }

        private static Vector3 Decide(double angle, double dist)
        {
            Vector3 v = new();
            if (angle > Math.PI / 3)
            {
                v.Z = -1;
            }
            else if (angle < -Math.PI / 3)
            {
                v.Z = 1;
            }
            else if (dist < 10)
            {
                v.Y = 1;
                if (dist > 10 && angle > 0)
                {
                    v.X = 1;
                }
                else if (dist > 10 && angle < 0)
                {
                    v.X = -1;
                }
            }
            else
            {
                v.Y = 1;
                if (angle > Math.PI / 4)
                {
                    v.Z = -1;
                }
                else if (angle < -Math.PI / 4)
                {
                    v.Z = 1;
                }
            }
            return v;
        }

        public void UpdateMove(int cur, int next, VirtualKey pos, VirtualKey neg)
        {
            if (cur != next)
            {
                if (cur == -1)
                {
                    BgInput.KeyUp(neg);
                    Thread.Sleep(1);
                }
                else if (cur == 1)
                {
                    BgInput.KeyUp(pos);
                    Thread.Sleep(1);
                }

                if (next == 1)
                {
                    BgInput.KeyDown(pos);
                    Thread.Sleep(1);
                }
                else if (next == -1)
                {
                    BgInput.KeyDown(neg);
                    Thread.Sleep(1);
                }
            }
        }

        public override void OnStart()
        {
            finished = false;
            xMove = yMove = turn = 0;
        }

        public override void Do()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var camera = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Y, 0);
            var angle = MyMath.angle2d(player.Position, camera, this.targetPos);
            var dist = MyMath.dist(player.Position, targetPos);

            PluginLog.Log("start moving");
            if (dist < 3)
            {
                UpdateMove(xMove, 0, VirtualKey.D, VirtualKey.A);
                UpdateMove(yMove, 0, VirtualKey.W, VirtualKey.W);
                UpdateMove(turn, 0, VirtualKey.LEFT, VirtualKey.RIGHT);
                finished = true;
                return;
            }

            Vector3 next = Decide(angle, dist);

            // update turn logic to avoid shaking
            if (turn != 0 && dist > 10 && (angle < -Math.PI / 72 || angle > Math.PI / 72))
            {
                if (next.Z == 0 || angle * next.Z < 0)
                {
                    next.Z = turn;
                }
            }

            PluginLog.Log($"Angle: {angle}\t\tDist: {dist}");
            PluginLog.Log($"<{xMove}, {yMove}, {turn}> -> <{next.X}, {next.Y}, {next.Z}>");

            UpdateMove(xMove, (int)next.X, VirtualKey.D, VirtualKey.A);
            UpdateMove(yMove, (int)next.Y, VirtualKey.W, VirtualKey.W);
            UpdateMove(turn, (int)next.Z, VirtualKey.LEFT, VirtualKey.RIGHT);

            xMove = (int)next.X;
            yMove = (int)next.Y;
            turn = (int)next.Z;
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
    }
}
