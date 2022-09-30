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
    internal unsafe class MoveToCommand : Command
    {
        [JsonProperty] private Vector3 targetPos;
        [JsonProperty] private bool shouldFaceTarget = false;
        [JsonProperty] private bool swim = false;

        public const uint STOP = 0;
        public const uint LEFT = 1;
        public const uint RIGHT = 1 << 1;
        public const uint FORWARD = 1 << 2;
        public const uint ROTATE_LEFT = 1 << 4;
        public const uint ROTATE_RIGHT = 1 << 5;
        public const uint LOOK_UP = 1 << 6;
        public const uint LOOK_DOWN = 1 << 7;

        internal override bool ShouldRepeat { get; } = true;

        private bool finished = false;
        private bool faceTarget = false;
        private uint state = 0;

        protected override bool TerminateCondition() => finished;

        internal void SetCurrOrTargetPos()
        {
            var target = CottonCollectorPlugin.TargetManager.Target;
            if (target != null)
            {
                targetPos = target.Position;
                shouldFaceTarget = true;
            }
            else
            {
                targetPos = CottonCollectorPlugin.ClientState.LocalPlayer.Position;
            }
        }

        private uint Decide(double angle, double dist)
        {
            uint ret = 0;

            if (dist < 5)   // check if we are close enough to the target
            {
                if ((state & (ROTATE_LEFT | ROTATE_RIGHT)) == 0)  // check we are turning or not, if we are not turning
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
                            ret |= ROTATE_LEFT;
                        }
                        else if (angle > Math.PI / 18)
                        {
                            ret |= ROTATE_RIGHT;
                        }
                    }
                }
                else if ((angle > 0 && (state & ROTATE_LEFT) > 0) || (angle < 0 && (state & ROTATE_RIGHT) > 0))
                {
                    // we have turn passed the target
                    finished = true;
                    ret = STOP;
                }
                else
                {
                    ret |= (state & (ROTATE_LEFT | ROTATE_RIGHT));
                }
            }
            else if (dist < 20)
            {
                // if we are too close to the target, just use W/A/D without turning camera
                ret |= FORWARD;
                // use A/D when we still have good distance
                if (angle > -Math.PI / 72)
                {
                    ret |= RIGHT;
                }
                else if (angle > Math.PI / 72)
                {
                    ret |= LEFT;
                }
            }
            else
            {
                // we are far away, we can move forward
                // if we are 45 degree away, then turn while running forward
                if (angle > Math.PI / 12)
                {
                    ret |= ROTATE_RIGHT;
                }
                else if (angle < -Math.PI / 12)
                {
                    ret |= ROTATE_LEFT;
                }
                else
                {
                    ret |= FORWARD;
                }
            }


            // update turn logic to avoid shaking
            // if we were turning before and we haven't reach target angle, keep turning in that direction
            if (dist > 15 && (angle < -Math.PI / 72 || angle > Math.PI / 72))
            {
                if ((state & (ROTATE_LEFT | ROTATE_RIGHT)) == 0 || (angle > 0 && (state & ROTATE_RIGHT) > 0) || (angle < 0 && (state & ROTATE_LEFT) > 0))
                {
                    ret |= state & (ROTATE_LEFT | ROTATE_RIGHT);
                }
            }
            return ret;
        }

        private void UpdateMove(uint cur, uint next, VirtualKey higher, VirtualKey lower)
        {
            //PluginLog.Log($"cur: {cur}\t\tnext: {next}");
            if (cur != next)
            {
                if ((cur & 0x1) > 0)
                {
                    BgInput.KeyUp(lower);
                }
                else if ((cur & 0x2) > 0)
                {
                    BgInput.KeyUp(higher);
                }

                if ((next & 0x1) > 0)
                {
                    BgInput.KeyDown(lower);
                }
                else if ((next & 0x2) > 0)
                {
                    BgInput.KeyDown(higher);
                }
            }
        }

        protected override void Do()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var camera = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Y, 0);
            var angle = MyMath.angle2d(player.Position, camera, this.targetPos);
            var dist = MyMath.dist(player.Position, targetPos);

            // PluginLog.Log("start moving");

            uint next = Decide(angle, dist);
            PluginLog.Log($"next: {next}");
            if (finished)
            {
                UpdateMove(state & 0x3, 0, VirtualKey.D, VirtualKey.A);
                UpdateMove((state >> 2) & 0x3, 0, VirtualKey.W, VirtualKey.W);
                UpdateMove((state >> 4) & 0x3, 0, VirtualKey.RIGHT, VirtualKey.LEFT);
                finished = true;
                return;
            }

            // PluginLog.Log($"Angle: {angle}\t\tDist: {dist}");
            // PluginLog.Log($"<{xMove}, {yMove}, {turn}> -> <{next.X}, {next.Y}, {next.Z}>");

            //PluginLog.Log("Left right movement");
            UpdateMove(next & 0x3, next & 0x3, VirtualKey.D, VirtualKey.A);
            //PluginLog.Log("forward movement");
            UpdateMove((state >> 2) & 0x3, (next >> 2) & 0x3, VirtualKey.W, VirtualKey.W);
            //PluginLog.Log("rotate movement");
            UpdateMove((state >> 4) & 0x3, (next >> 4) & 0x3, VirtualKey.RIGHT, VirtualKey.LEFT);
            Thread.Sleep(1);

            state = next;
        }

        internal override void ResetExecutionState()
        {
            base.ResetExecutionState();
            finished = false;
            faceTarget = !shouldFaceTarget;
            state = 0;
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
                shouldFaceTarget = true;
            }

            ImGui.SameLine();
            ImGui.Checkbox(Ui.Uid("face target?", uid), ref shouldFaceTarget);
            ImGui.SameLine();
            ImGui.Checkbox(Ui.Uid("face target?", uid), ref swim);
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
