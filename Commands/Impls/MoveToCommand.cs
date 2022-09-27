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
using static System.Net.Mime.MediaTypeNames;

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
            ImGui.Text($"Move to {this.targetPos:#.00}");
            ImGui.SameLine();
            Vector3? currPos = Ui.GetCurrPosBtn(Ui.Uid("CurrPos", uid));
            if (currPos != null)
            {
                targetPos = currPos.Value;
            }

            ImGui.SameLine();
            Vector3? currTargetPos = Ui.GetTargetPosBtn(Ui.Uid("TargetPos", uid));
            if (currTargetPos != null)
            {
                targetPos = currTargetPos.Value;
            }
        }

        private Vector3 Decide(double angle, double dist)
        {
            Vector3 v = new();
            // if we are more than 60 degree away from target, just turn camera
            if (angle > Math.PI / 3)
            {
                v.Z = -1;
            }
            else if (angle < -Math.PI / 3)
            {
                v.Z = 1;
            }
            else if (dist < 20)
            {
                // if we are too close to the target, just use W/A/D without turning camera
                v.Y = 1;
                // use A/D when we still have good distance
                if (angle > -Math.PI / 72)
                {
                    v.X = 1;
                }
                else if (angle > Math.PI / 72)
                {
                    v.X = -1;
                }
            }
            else
            {
                // we are far away, we can move forward
                // if we are 45 degree away, then turn while running forward
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


            // update turn logic to avoid shaking
            // if we were turning before and we haven't reach target angle, keep turning in that direction
            if (dist > 15 && (angle < -Math.PI / 72 || angle > Math.PI / 72))
            {
                if (v.Z == 0 || angle * turn < 0)
                {
                    v.Z = turn;
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
                }
                else if (cur == 1)
                {
                    BgInput.KeyUp(pos);
                }

                if (next == 1)
                {
                    BgInput.KeyDown(pos);
                }
                else if (next == -1)
                {
                    BgInput.KeyDown(neg);
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
            if (dist < 5)
            {
                UpdateMove(xMove, 0, VirtualKey.D, VirtualKey.A);
                UpdateMove(yMove, 0, VirtualKey.W, VirtualKey.W);
                UpdateMove(turn, 0, VirtualKey.LEFT, VirtualKey.RIGHT);
                finished = true;
                return;
            }

            Vector3 next = Decide(angle, dist);

            PluginLog.Log($"Angle: {angle}\t\tDist: {dist}");
            PluginLog.Log($"<{xMove}, {yMove}, {turn}> -> <{next.X}, {next.Y}, {next.Z}>");

            UpdateMove(xMove, (int)next.X, VirtualKey.D, VirtualKey.A);
            UpdateMove(yMove, (int)next.Y, VirtualKey.W, VirtualKey.W);
            UpdateMove(turn, (int)next.Z, VirtualKey.LEFT, VirtualKey.RIGHT);
            Thread.Sleep(1);

            xMove = (int)next.X;
            yMove = (int)next.Y;
            turn = (int)next.Z;
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            ImGui.Text("X:");
            ImGui.SameLine();
            ImGui.InputFloat(Ui.Uid(index: uid), ref targetPos.X);

            ImGui.SameLine();
            ImGui.Text("Y:");
            ImGui.SameLine();
            ImGui.InputFloat(Ui.Uid(index: uid), ref targetPos.Y);

            ImGui.SameLine();
            ImGui.Text("Z:");
            ImGui.SameLine();
            ImGui.InputFloat(Ui.Uid(index: uid), ref targetPos.Z);

            ImGui.SameLine();
            Vector3? currPos = Ui.GetCurrPosBtn(Ui.Uid("CurrPos", uid));
            if (currPos != null)
            {
                targetPos = currPos.Value;
            }

            ImGui.SameLine();
            Vector3? currTargetPos = Ui.GetTargetPosBtn(Ui.Uid("TargetPos", uid));
            if (currTargetPos != null)
            {
                targetPos = currTargetPos.Value;
            }

            ImGui.PopItemWidth();
        }

        public void SetTarget(Vector3 target)
        {
            targetPos = target;
        }
    }
}
