using System;
using System.Numerics;

using ImGuiNET;

using Dalamud.Game.ClientState.Keys;
using Dalamud.Game.ClientState.Objects.Types;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Logging;

using CottonCollector.CameraManager;
using CottonCollector.Config;
using CottonCollector.CharacterControl;

namespace CottonCollector.Interface
{
    internal unsafe class CharacterControlTab : ConfigTab
    {
        private Commands commands;

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

            if (ImGui.Button("Press W for 1 sec"))
            {
                commands.commands.Enqueue(new Command(Commands.Type.KEY_DOWN, VirtualKey.W, 1000));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_UP, VirtualKey.W));
            }

            if (ImGui.Button("Press W for 1 sec then Press D for 1 sec"))
            {
                commands.commands.Enqueue(new Command(Commands.Type.KEY_DOWN, VirtualKey.W, 1000));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_UP, VirtualKey.W));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_DOWN, VirtualKey.D, 1000));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_UP, VirtualKey.D));
            }

            if (ImGui.Button("Huton"))
            {
                commands.commands.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.R, 500));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.E, 500));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.Q, 500));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_PRESS, VirtualKey.T, 500));
            }

            if (ImGui.Button("face camera at target"))
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
                    commands.commands.Enqueue(new Command(Commands.Type.KEY_DOWN, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT, 0, () =>
                    { 
                        var playerPos = ToVector2(CottonCollectorPlugin.ClientState.LocalPlayer.Position);
                        var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                            CameraHelpers.collection->WorldCamera->Y);
                        var targetPos = ToVector2(target.Position);
                        var v = Vector2.Normalize(cameraPos - playerPos);
                        var u = Vector2.Normalize(targetPos - playerPos);
                        return (v + u).LengthSquared() < 1e-3f;
                    }));
                    commands.commands.Enqueue(new Command(Commands.Type.KEY_UP, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT));
                }
            }

            if (keyState[VirtualKey.W]) {
                ImGui.Text("W");
            }
            if (keyState[VirtualKey.S]) {
                ImGui.Text("S");
            }
            if (keyState[VirtualKey.A]) {
                ImGui.Text("A");
            }
            if (keyState[VirtualKey.D]) {
                ImGui.Text("D");
            }

        }
    }
}
