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

        public enum ActionMode
        {
            STOP = 0,
            LEFT = -1,
            RIGHT = 1,
            FORWARD = 1,
            ROTATE_LEFT = 1,
            ROTATE_RIGHT = -1,
        }

        internal override bool ShouldRepeat { get; } = true;

        private bool finished = false;
        private bool faceTarget = false;
        private int xMove = 0;
        private int yMove = 0;
        private int turn = 0; // + left - right

        protected override bool TerminateCondition() => finished;

        private Vector3 Decide(double angle, double dist)
        {
            Vector3 v = new();
            
            if (dist < 5)   // check if we are close enough to the target
            {
                if (turn == 0)  // check we are turning or not, if we are not turning
                {
                    if (faceTarget)     // if we are facing target, then we are good
                    {
                        finished = true;
                    }
                    else        // we are close but not facing target
                    {
                        // start turning towards target
                        faceTarget = true;
                        if (angle < -Math.PI / 18)
                        {
                            v.Z = (int) ActionMode.ROTATE_LEFT;
                        }
                        else if (angle > Math.PI / 18)
                        {
                            v.Z = (int) ActionMode.ROTATE_RIGHT;
                        }
                    }
                }
                else if (turn * angle > 0)
                {
                    // we have turn passed the target
                    finished = true;
                    v.Z = (int) ActionMode.STOP;
                }
                else
                {
                    v.Z = turn;
                }
            }
            else if (dist < 20)
            {
                // if we are too close to the target, just use W/A/D without turning camera
                v.Y = (int) ActionMode.FORWARD;
                // use A/D when we still have good distance
                if (angle > -Math.PI / 72)
                {
                    v.X = (int) ActionMode.RIGHT;
                }
                else if (angle > Math.PI / 72)
                {
                    v.X = (int) ActionMode.LEFT;
                }
            }
            else
            {
                // we are far away, we can move forward
                // if we are 45 degree away, then turn while running forward
                v.Y = (int) ActionMode.FORWARD;
                if (angle > Math.PI / 4)
                {
                    v.Z = (int) ActionMode.ROTATE_RIGHT;
                }
                else if (angle < -Math.PI / 4)
                {
                    v.Z = (int) ActionMode.ROTATE_LEFT;
                }
            }


            // update turn logic to avoid shaking
            // if we were turning before and we haven't reach target angle, keep turning in that direction
            if (dist > 15 && (angle < -Math.PI / 72 || angle > Math.PI / 72))
            {
                if (v.Z == (int) ActionMode.STOP || angle * turn < 0)
                {
                    v.Z = turn;
                }
            }
            return v;
        }

        private void UpdateMove(int cur, int next, VirtualKey pos, VirtualKey neg)
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

        protected override void Do()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var camera = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Y, 0);
            var angle = MyMath.angle2d(player.Position, camera, this.targetPos);
            var dist = MyMath.dist(player.Position, targetPos);

            PluginLog.Log("start moving");
            
            Vector3 next = Decide(angle, dist);

            if (finished)
            {
                UpdateMove(xMove, 0, VirtualKey.D, VirtualKey.A);
                UpdateMove(yMove, 0, VirtualKey.W, VirtualKey.W);
                UpdateMove(turn, 0, VirtualKey.LEFT, VirtualKey.RIGHT);
                finished = true;
                return;
            }

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

        internal override void ResetExecutionState()
        {
            base.ResetExecutionState();
            finished = faceTarget = false;
            xMove = yMove = turn = 0;
        }

        internal void SetTarget(Vector3 target)
        {
            targetPos = target;
        }

        #region GUI
        internal override void MinimalInfo()
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

        internal override void SelectorGui()
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
        #endregion
    }
}
