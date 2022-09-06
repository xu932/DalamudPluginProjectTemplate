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

            if (ImGui.Button("Look to Atheryte"))
            {
                commands.commands.Enqueue(new Command(Commands.Type.KEY_DOWN, VirtualKey.LEFT, 0, () =>
                {
                    var playerPos = CottonCollectorPlugin.ClientState.LocalPlayer.Position;
                    var cameraPos = new Vector3(CameraHelpers.collection->WorldCamera->X, CameraHelpers.collection->WorldCamera->Z,
                        CameraHelpers.collection->WorldCamera->Y);

                    GameObject aetheryte = null;
                    foreach (GameObject obj in CottonCollectorPlugin.ObjectTable)
                    {
                        if (obj.ObjectKind == ObjectKind.Aetheryte) aetheryte = obj;
                    }

                    if (aetheryte == null) return true;
                    var aetheratePos = aetheryte.Position;

                    var v1 = Vector3.Normalize(cameraPos - playerPos);
                    var v2 = Vector3.Normalize(aetheratePos - playerPos);
                    PluginLog.Log($"v1:{v1}");
                    PluginLog.Log($"v2:{v2}");

                    var cross = v1.X * v2.Z - v1.Z * v2.X;
                    PluginLog.Log($"cross:{cross:000.00}");

                    return cross < 1e-2;
                }));
                commands.commands.Enqueue(new Command(Commands.Type.KEY_UP, VirtualKey.LEFT));
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
