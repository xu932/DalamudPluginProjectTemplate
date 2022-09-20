using System;
using System.Numerics;
using Newtonsoft.Json;

using ImGuiNET;

using CottonCollector.Commands.Structures;

namespace CottonCollector.Commands.Impls
{
    // This is no more than a shitty demo. User shall be able to construct this with GUI.
    [Serializable]
    [JsonObject(IsReference = true)]
    internal class MoveToCommand : CommandSet
    {
        public double X, Y, Z, threshold = 2.0f;

        // temp
        static int uid = 0;
        CommandSet adjustRightCommand = null;
        CommandSet adjustLeftCommand = null;
        TillMovedToCommand tillMovedToCommand = null;
        TillLookedAtCommand tillLookedAtCommand = null;
        Trigger adjustRightTrigger = null;
        Trigger adjustLeftTrigger = null;

        public MoveToCommand() : base("MoveTo" + uid) {
            tillMovedToCommand = new TillMovedToCommand();
            tillLookedAtCommand = new TillLookedAtCommand();

            // Move
            var pressW = new KeyboardCommand
            {
                vk = Dalamud.Game.ClientState.Keys.VirtualKey.W,
                actionType = KeyboardCommand.ActionType.KEY_DOWN
            };
            var releaseW = new KeyboardCommand
            {
                vk = Dalamud.Game.ClientState.Keys.VirtualKey.W,
                actionType = KeyboardCommand.ActionType.KEY_UP
            };

            subCommands.AddLast(pressW);
            subCommands.AddLast(tillMovedToCommand);
            subCommands.AddLast(releaseW);


            // Adjust Right Trigger
            adjustRightCommand = new CommandSet("adjust right" + uid);
            var pressLeft = new KeyboardCommand
            {
                vk = Dalamud.Game.ClientState.Keys.VirtualKey.LEFT,
                actionType = KeyboardCommand.ActionType.KEY_DOWN,
            };
            var releaseLeft = new KeyboardCommand
            {
                vk = Dalamud.Game.ClientState.Keys.VirtualKey.LEFT,
                actionType = KeyboardCommand.ActionType.KEY_UP
            };

            adjustRightCommand.subCommands.AddLast(pressLeft);
            adjustRightCommand.subCommands.AddLast(tillLookedAtCommand);
            adjustRightCommand.subCommands.AddLast(releaseLeft);

            adjustRightTrigger = new Trigger(adjustRightCommand);

            // Adjust Left Trigger
            adjustLeftCommand = new CommandSet("adjust left" + uid);
            var pressRight = new KeyboardCommand
            {
                vk = Dalamud.Game.ClientState.Keys.VirtualKey.RIGHT,
                actionType = KeyboardCommand.ActionType.KEY_DOWN,
            };
            var releaseRight = new KeyboardCommand
            {
                vk = Dalamud.Game.ClientState.Keys.VirtualKey.RIGHT,
                actionType = KeyboardCommand.ActionType.KEY_UP,
            };

            adjustLeftCommand.subCommands.AddLast(pressRight);
            adjustLeftCommand.subCommands.AddLast(tillLookedAtCommand);
            adjustLeftCommand.subCommands.AddLast(releaseRight);

            adjustLeftTrigger = new Trigger(adjustLeftCommand);

            // Add triggers to collection.
            triggers.AddLast(adjustRightTrigger);
            triggers.AddLast(adjustLeftTrigger);

            uid++;
        }

        public override void OnStart()
        {
            tillMovedToCommand.X = X;
            tillMovedToCommand.Y = Y;
            tillMovedToCommand.Z = Z;
            tillMovedToCommand.threshold = threshold;

            tillLookedAtCommand.X = X;
            tillLookedAtCommand.Y = Y;
            tillLookedAtCommand.Z = Z;

            adjustRightTrigger.TriggerCondition =
                () => Conditions.CameraOnRight(new Vector3((float)X, (float)Y, (float)Z));
            adjustLeftTrigger.TriggerCondition =
                () => Conditions.CameraOnLeft(new Vector3((float)X, (float)Y, (float)Z));

            base.OnStart();
        }

        public override void SelectorGui()
        {
            ImGui.PushItemWidth(100);

            ImGui.Text("X:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__X", ref X);

            ImGui.SameLine();
            ImGui.Text("Y:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__Y", ref Y);

            ImGui.SameLine();
            ImGui.Text("Z:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__Z", ref Z);

            ImGui.SameLine();
            ImGui.Text("threshold:");
            ImGui.SameLine();
            ImGui.InputDouble("##TillMovedToCommand__threshold", ref threshold);

            ImGui.SameLine();
            if (ImGui.Button($"GetCurrentPos##TillMovedToCommand__getpos"))
            {
                var localPlayer = CottonCollectorPlugin.ClientState.LocalPlayer;
                X = localPlayer.Position.X;
                Y = localPlayer.Position.Y;
                Z = localPlayer.Position.Z;
            }

            ImGui.PopItemWidth();
        }
    }
}
