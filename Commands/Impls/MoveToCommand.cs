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

        private Random rand = new();

        public const uint STOP = 0;
        public const uint LEFT = 1;
        public const uint RIGHT = 1 << 1;
        public const uint FORWARD = 1 << 2;
        public const uint ROTATE_LEFT = 1 << 4;
        public const uint ROTATE_RIGHT = 1 << 5;
        public const uint UP = 1 << 6;
        public const uint DOWN = 1 << 7;
        public const uint JUMP = 1 << 8;

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

        private uint Decide(double angle, double dist, double height)
        {
            uint ret = 0;

            if (dist < 5 && (!swim || Math.Abs(height) < 0.5))   // check if we are close enough to the target
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
                // use A/D when we still have good distance
                if (dist > 5)
                {
                    ret |= FORWARD;
                }
                if (angle < -Math.PI / 72)
                {
                    ret |= RIGHT;
                }
                else if (angle > Math.PI / 72)
                {
                    ret |= LEFT;
                }
                if (height > 0.4)
                {
                    ret |= DOWN;
                }
                else if (height < -0.4)
                {
                    ret |= UP;
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
                    if (height > 5)
                    {
                        ret |= DOWN;
                    }
                    else if (height < -5)
                    {
                        ret |= UP;
                    }
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

            if (((state & DOWN) > 0 && height > 0.1) || (state & UP) > 0 && height < 0.1)
            {
                ret |= state & (DOWN | UP);
            }

            if (!swim)
            {
                int r = rand.Next(1000);
                if (r == 0)
                {
                    PluginLog.Log("register jump!");
                    ret |= JUMP;
                }
            }

            return ret;
        }

        private void UpdateMove(uint cur, uint next, Tuple<BgInput.Modifier, VirtualKey> higher, 
            Tuple<BgInput.Modifier, VirtualKey> lower)
        {
            if (cur != next)
            {
                if ((cur & 0x1) > 0)
                {
                    BgInput.KeyUp(lower.Item2, lower.Item1);
                }
                else if ((cur & 0x2) > 0)
                {
                    BgInput.KeyUp(higher.Item2, higher.Item1);
                }

                if ((next & 0x1) > 0)
                {
                    BgInput.KeyDown(lower.Item2, lower.Item1);
                }
                else if ((next & 0x2) > 0)
                {
                    BgInput.KeyDown(higher.Item2, higher.Item1);
                }
            }
        }

        protected override void Do()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var camera = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Y, 0);
            var angle = MyMath.angle2d(player.Position, camera, this.targetPos);
            var dist = MyMath.dist(player.Position, targetPos);
            var keybind = CottonCollectorPlugin.config.keybind;

            uint next = Decide(angle, dist, this.targetPos.Y - player.Position.Y);
            if (finished)
            {
                UpdateMove(state & 0x3, 0, keybind.moveRight, keybind.moveLeft);
                UpdateMove((state >> 2) & 0x3, 0, keybind.moveForward, keybind.moveForward);
                UpdateMove((state >> 4) & 0x3, 0, keybind.rotateCameraRight, keybind.rotateCameraLeft);
                if (swim)
                {
                    UpdateMove((state >> 6) & 0x3, 0, keybind.moveUpward, keybind.moveDownward);
                }
                finished = true;
                return;
            }

            UpdateMove(next & 0x3, next & 0x3, keybind.moveRight, keybind.moveLeft);
            UpdateMove((state >> 2) & 0x3, (next >> 2) & 0x3, keybind.moveForward, keybind.moveForward);
            UpdateMove((state >> 4) & 0x3, (next >> 4) & 0x3, keybind.rotateCameraRight, keybind.rotateCameraLeft);
            if (swim)
            {
                UpdateMove((state >> 6) & 0x3, (next >> 6) & 0x3, keybind.moveUpward, keybind.moveDownward);
            }
            else
            {
                UpdateMove((state >> 8) & 0x1, (next >> 8) & 0x1, keybind.jump, keybind.jump);
            }

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
            ImGui.Checkbox(Ui.Uid("swim?", uid), ref swim);
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
