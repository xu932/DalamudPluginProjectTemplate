using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;

using ImGuiNET;

using WindowsInput;
using WindowsInput.Native;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

using CottonCollector.Commands.Structures;
using System.Diagnostics;
using CottonCollector.CameraManager;

namespace CottonCollector.Commands.Impls
{
    internal unsafe class FooCommand : Command
    {
        public enum ActionType
        {
            KEY_DOWN = 0,
            KEY_UP = 1,
            KEY_PRESS = 2,
        }

        public enum KeyCode
        {
            W = 0x57,
            A = 0x41,
            D = 0x44,
            LEFT = 0x25,
            RIGHT = 0x27,
        }

        public ActionType actionType = ActionType.KEY_PRESS;
        private Stopwatch stopwatch = new Stopwatch();

        private InputSimulator sim = new();
        private Vector3 target;

        private int state = 0;
        private int xMove = 0;
        private int yMove = 0;
        private int turn = 0; // + left - right

        public FooCommand(Vector3 target)
        {
            this.target = target;
            this.Repeate = true;
        }

        public override bool TerminateCondition()
        {
            return state == 4;
        }

        public override string Description()
        {
            string description = base.Description();
            description += $"Move to {this.target.ToString()}";
            return description;
        }

        private Vector3 decide(double angle, double dist)
        {
            Vector3 v = new Vector3(0, 0, 0);
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
                if (angle > 0)
                {
                    v.X = 1;
                }
                else if (angle < 0)
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

        public int StopAndStart(int cur, int next, KeyCode pos, KeyCode neg)
        {
            if (cur != next)
            {
                if (cur == -1)
                {
                    sim.Keyboard.KeyUp((VirtualKeyCode)neg);
                    Thread.Sleep(1);
                }
                else if (cur == 1)
                {
                    sim.Keyboard.KeyUp((VirtualKeyCode)pos);
                    Thread.Sleep(1);
                }

                if (next == 1)
                {
                    sim.Keyboard.KeyDown((VirtualKeyCode)pos);
                    Thread.Sleep(1);
                }
                else if (next == -1)
                {
                    sim.Keyboard.KeyDown((VirtualKeyCode)neg);
                    Thread.Sleep(1);
                }
            }
            return next != 0 ? 1 : 0;
        }

        public override void Do()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var camera = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Y, 0);
            var angle = MyMath.angle2d(player.Position, camera, this.target);
            var dist = MyMath.dist(player.Position, target);


            if (dist < 3)
            {
                state = 4;
            }

            Vector3 next = decide(angle, dist);
            int newState = 0;


            PluginLog.Log($"Angle: {angle}\t\tDist: {dist}");
            PluginLog.Log($"<{xMove}, {yMove}, {turn}> -> <{next.X}, {next.Y}, {next.Z}>");
            switch (state)
            {
                case 0:
                    // stopped
                    switch (next.X)
                    {
                        case 1:
                            sim.Keyboard.KeyDown((VirtualKeyCode)KeyCode.D);
                            Thread.Sleep(1);
                            newState |= 0x1;
                            break;
                        case -1:
                            sim.Keyboard.KeyDown((VirtualKeyCode)KeyCode.A);
                            Thread.Sleep(1);
                            newState |= 0x1;
                            break;
                    }
                    if (next.Y == 1)
                    {
                        sim.Keyboard.KeyDown((VirtualKeyCode)KeyCode.W);
                        Thread.Sleep(1);
                        newState |= 0x1;
                    }
                    switch (next.Z)
                    {
                        case 1:
                            sim.Keyboard.KeyDown((VirtualKeyCode)KeyCode.LEFT);
                            Thread.Sleep(1);
                            newState |= 0x2;
                            break;
                        case -1:
                            sim.Keyboard.KeyDown((VirtualKeyCode)KeyCode.RIGHT);
                            Thread.Sleep(1);
                            newState |= 0x2;
                            break;
                    }
                    break;
                case 1:
                case 2:
                case 3:
                    // moving
                    newState |= StopAndStart(xMove, (int)next.X, KeyCode.D, KeyCode.A);
                    newState |= StopAndStart(yMove, (int)next.Y, KeyCode.W, KeyCode.W);
                    newState |= StopAndStart(turn, (int)next.Z, KeyCode.LEFT, KeyCode.RIGHT) << 1;
                    break;
                case 4:
                    StopAndStart(xMove, 0, KeyCode.D, KeyCode.A);
                    StopAndStart(yMove, 0, KeyCode.W, KeyCode.W);
                    StopAndStart(turn, 0, KeyCode.LEFT, KeyCode.RIGHT);
                    newState = 4;
                    break;
            }

            PluginLog.Log($"State {state} -> {newState}");
            xMove = (int)next.X;
            yMove = (int)next.Y;
            turn = (int)next.Z;
            state = newState;
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            // Type selector
            ImGui.Text("Type:");
            ImGui.SameLine();
            List<ActionType> actionTypes = Enum.GetValues(typeof(ActionType)).Cast<ActionType>().ToList();
            int newActionTypeIndex = actionTypes.IndexOf(actionType);
            if (ImGui.Combo($"##KeyboardCommand__ActionTypeSelector__{GetHashCode()}", ref newActionTypeIndex,
                Enum.GetNames(typeof(ActionType)), actionTypes.Count))
            {
                actionType = actionTypes[newActionTypeIndex];
            }

            // Key Selector
            ImGui.SameLine();
            ImGui.Text("Key:");
            ImGui.SameLine();
            List<VirtualKey> keys = Enum.GetValues(typeof(VirtualKey)).Cast<VirtualKey>().ToList();

            // TODO: add key modifier selector
            ImGui.PopItemWidth();
        }
    }
}
