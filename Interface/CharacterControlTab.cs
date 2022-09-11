using System;
using System.Collections.Generic;
using System.Numerics;

using ImGuiNET;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Logging;

using CottonCollector.CameraManager;
using CottonCollector.Config;
using CottonCollector.CharacterControl;
using System.Collections;

namespace CottonCollector.Interface
{
    internal unsafe class CharacterControlTab : ConfigTab
    {
        private Commands commands;

        // temporary
        private Queue<Vector3> collectedObj = new Queue<Vector3>();

        public CharacterControlTab(ref CottonCollectorConfig config, ref Commands commands) : base("Character Control", ref config) {
            this.commands = commands;
        }

        private Vector2 ToVector2(Vector3 v3)
        {
            return new Vector2(v3.X, v3.Z);
        }

        public override void TabContent()
        {
            KeyState keyState = CottonCollectorPlugin.KeyState;

            if (ImGui.Button("Huton"))
            {
                var huton = new Queue<Command>();
                huton.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.R, 500));
                huton.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.E, 500));
                huton.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.Q, 500));
                huton.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.T, 500));
                commands.commands.Enqueue(huton);
            }

            if (ImGui.Button("Face camera at target"))
            {

                GameObject target = CottonCollectorPlugin.ClientState.LocalPlayer.TargetObject;
                if (target != null)
                {
                    var playerPos = ToVector2(CottonCollectorPlugin.ClientState.LocalPlayer.Position);
                    var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                        CameraHelpers.collection->WorldCamera->Y);
                    var targetPos = ToVector2(target.Position);
                    var v = Vector2.Normalize(cameraPos - playerPos);
                    var u = Vector2.Normalize(targetPos - playerPos);
                    var camera_on_left = (v.X * u.Y - u.X * v.Y) < 0;

                    var turn_to_target = new Queue<Command>();
                    turn_to_target.Enqueue(new Command(Commands.Type.KEY_DOWN, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT, 0, () =>
                    { 
                        var playerPos = ToVector2(CottonCollectorPlugin.ClientState.LocalPlayer.Position);
                        var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                            CameraHelpers.collection->WorldCamera->Y);
                        var targetPos = ToVector2(target.Position);
                        var v = Vector2.Normalize(cameraPos - playerPos);
                        var u = Vector2.Normalize(targetPos - playerPos);
                        return (v + u).LengthSquared() < 1e-3f;
                    }));
                    turn_to_target.Enqueue(new Command(Commands.Type.KEY_UP, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT));

                    commands.commands.Enqueue(turn_to_target);
                }
            }

            if (ImGui.Button("collect closest resource"))
            {
                var minDist = 10e6f;
                GameObject target = null;
                foreach(GameObject obj in CottonCollectorPlugin.ObjectTable)
                {
                    if (obj.ObjectKind != ObjectKind.CardStand) continue;
                    if (collectedObj.Contains(obj.Position)) continue;
                    var dist = Vector3.Distance(obj.Position, CottonCollectorPlugin.ClientState.LocalPlayer.Position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        target = obj;
                    }
                }
                if (target != null)
                {
                    PluginLog.Log($"target name: {target.Name}, pos: {target.Position}, dist: {minDist}");
                    collectedObj.Enqueue(target.Position);
                    if (collectedObj.Count > 20)
                    {
                        collectedObj.Dequeue();
                    }
                    var playerPos = ToVector2(CottonCollectorPlugin.ClientState.LocalPlayer.Position);
                    var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                        CameraHelpers.collection->WorldCamera->Y);
                    var targetPos = ToVector2(target.Position);
                    var v = Vector2.Normalize(cameraPos - playerPos);
                    var u = Vector2.Normalize(targetPos - playerPos);
                    var camera_on_left = (v.X * u.Y - u.X * v.Y) < 0;

                    var turn_to_target = new Queue<Command>();
                    turn_to_target.Enqueue(new Command(Commands.Type.KEY_DOWN, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT, 0, () =>
                    { 
                        var playerPos = ToVector2(CottonCollectorPlugin.ClientState.LocalPlayer.Position);
                        var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                            CameraHelpers.collection->WorldCamera->Y);
                        var targetPos = ToVector2(target.Position);
                        var v = Vector2.Normalize(cameraPos - playerPos);
                        var u = Vector2.Normalize(targetPos - playerPos);
                        return (v + u).LengthSquared() < 1e-3f;
                    }));
                    turn_to_target.Enqueue(new Command(Commands.Type.KEY_UP, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT));

                    commands.commands.Enqueue(turn_to_target);

                    var go_to_target = new Queue<Command>();
                    go_to_target.Enqueue(new Command(Commands.Type.KEY_DOWN, VirtualKey.W, isFinished: () => {
                        return Vector3.Distance(target.Position, CottonCollectorPlugin.ClientState.LocalPlayer.Position) < 3;
                    }));
                    go_to_target.Enqueue(new Command(Commands.Type.KEY_UP, VirtualKey.W));
                    commands.commands.Enqueue(go_to_target);

                    var select_and_collect = new Queue<Command>();
                    select_and_collect.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.OEM_MINUS, 1000));
                    select_and_collect.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.OEM_MINUS));
                    commands.commands.Enqueue(select_and_collect);
                }
            }

            if (ImGui.Button("Stop All"))
            {
                commands.KillSwitch();
            }
        }
    }
}
