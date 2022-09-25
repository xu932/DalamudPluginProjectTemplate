﻿using System;
using System.Numerics;
using System.Threading;
using Newtonsoft.Json;

using ImGuiNET;

using WindowsInput;
using WindowsInput.Native;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Logging;

using CottonCollector.Commands.Structures;
using CottonCollector.CameraManager;

namespace CottonCollector.Commands.Impls
{
    internal unsafe class MoveToCommand: Command
    {
        [JsonProperty] private Vector3 targetPos;

        private readonly InputSimulator sim = new();
        private ushort state = 0;
        private int xMove = 0;
        private int yMove = 0;
        private int turn = 0; // + left - right

        public MoveToCommand()
        {
            shouldRepeat = true;
        }

        public override bool TerminateCondition()
        {
            return state == 4;
        }

        public override string Description()
        {
            string description = base.Description();
            description += $"Move to {this.targetPos}";
            return description;
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

        public ushort StopAndStart(int cur, int next, VirtualKey pos, VirtualKey neg)
        {
            if (cur != next)
            {
                if (cur == -1)
                {
                    sim.Keyboard.KeyUp((VirtualKeyCode)(int)neg);
                    Thread.Sleep(1);
                }
                else if (cur == 1)
                {
                    sim.Keyboard.KeyUp((VirtualKeyCode)(int)pos);
                    Thread.Sleep(1);
                }

                if (next == 1)
                {
                    sim.Keyboard.KeyDown((VirtualKeyCode)(int)pos);
                    Thread.Sleep(1);
                }
                else if (next == -1)
                {
                    sim.Keyboard.KeyDown((VirtualKeyCode)(int)neg);
                    Thread.Sleep(1);
                }
            }
            return (ushort)(next != 0 ? 0x1u : 0u);
        }

        public override void OnStart()
        {
            state = 0;
        }

        public override void Do()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var camera = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Y, 0);
            var angle = MyMath.angle2d(player.Position, camera, this.targetPos);
            var dist = MyMath.dist(player.Position, targetPos);


            if (dist < 3)
            {
                state = 4;
            }

            Vector3 next = Decide(angle, dist);
            ushort newState = 0;


            PluginLog.Log($"Angle: {angle}\t\tDist: {dist}");
            PluginLog.Log($"<{xMove}, {yMove}, {turn}> -> <{next.X}, {next.Y}, {next.Z}>");
            switch (state)
            {
                case 0:
                    // stopped
                    switch (next.X)
                    {
                        case 1:
                            sim.Keyboard.KeyDown(VirtualKeyCode.VK_D);
                            Thread.Sleep(1);
                            newState |= 0x1;
                            break;
                        case -1:
                            sim.Keyboard.KeyDown(VirtualKeyCode.VK_A);
                            Thread.Sleep(1);
                            newState |= 0x1;
                            break;
                    }
                    if (next.Y == 1)
                    {
                        sim.Keyboard.KeyDown(VirtualKeyCode.VK_W);
                        Thread.Sleep(1);
                        newState |= 0x1;
                    }
                    switch (next.Z)
                    {
                        case 1:
                            sim.Keyboard.KeyDown(VirtualKeyCode.LEFT);
                            Thread.Sleep(1);
                            newState |= 0x2;
                            break;
                        case -1:
                            sim.Keyboard.KeyDown(VirtualKeyCode.RIGHT);
                            Thread.Sleep(1);
                            newState |= 0x2;
                            break;
                    }
                    break;
                case 1:
                case 2:
                case 3:
                    // moving
                    if (dist > 10 && (angle < -Math.PI / 72 || angle > Math.PI / 72))
                    {
                        if (next.Z == 0 || angle * next.Z < 0)
                        {
                            next.Z = turn;
                        }
                    }
                    newState |= StopAndStart(xMove, (int)next.X, VirtualKey.D, VirtualKey.A);
                    newState |= StopAndStart(yMove, (int)next.Y, VirtualKey.W, VirtualKey.W);
                    newState |= (ushort)(StopAndStart(turn, (int)next.Z, VirtualKey.LEFT, VirtualKey.RIGHT) << 1);
                    break;
                case 4:
                    StopAndStart(xMove, 0, VirtualKey.D, VirtualKey.A);
                    StopAndStart(yMove, 0, VirtualKey.W, VirtualKey.W);
                    StopAndStart(turn, 0, VirtualKey.LEFT, VirtualKey.RIGHT);
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