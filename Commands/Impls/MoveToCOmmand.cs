using System.Collections.Generic;
using System.Numerics;

using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.CameraManager;
using CottonCollector.Commands.Structures;

namespace CottonCollector.Commands.Impls
{
    internal unsafe class MoveToCommand: Command
    {
        public double X, Y, Z, threshold = 1.0f;

        public override bool TerminateCondition() => true;

        public override void Do()
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var targetPos2 = new Vector2((float)X, (float)Z);
            var playerPos2 = new Vector2(player.Position.X, player.Position.Z);
            var cameraPos2 = new Vector2(CameraHelpers.collection->WorldCamera->X, 
                CameraHelpers.collection->WorldCamera->Y);
            var v = Vector2.Normalize(cameraPos2 - playerPos2);
            var u = Vector2.Normalize(targetPos2 - playerPos2);

            var cross = v.X * u.Y - v.Y * u.X;

            var cmdlist = new List<Command>();
            cmdlist.Add(new KeyboardCommand());
            cmdlist.Add(new TillLookedAtCommand());
            cmdlist.Add(new KeyboardCommand());
            cmdlist.Add(new KeyboardCommand());
            cmdlist.Add(new TillMovedToCommand());
            cmdlist.Add(new KeyboardCommand());

            var vk = cross > 0 ? Dalamud.Game.ClientState.Keys.VirtualKey.LEFT
                : Dalamud.Game.ClientState.Keys.VirtualKey.RIGHT;
            ((KeyboardCommand)cmdlist[0]).vk = vk;
            ((KeyboardCommand)cmdlist[2]).vk = vk;
            ((KeyboardCommand)cmdlist[3]).vk = Dalamud.Game.ClientState.Keys.VirtualKey.W;
            ((KeyboardCommand)cmdlist[5]).vk = Dalamud.Game.ClientState.Keys.VirtualKey.W;

            ((KeyboardCommand)cmdlist[0]).actionType = KeyboardCommand.ActionType.KEY_DOWN;
            ((KeyboardCommand)cmdlist[2]).actionType = KeyboardCommand.ActionType.KEY_UP;
            ((KeyboardCommand)cmdlist[3]).actionType = KeyboardCommand.ActionType.KEY_DOWN;
            ((KeyboardCommand)cmdlist[5]).actionType = KeyboardCommand.ActionType.KEY_UP;

            ((TillLookedAtCommand)cmdlist[1]).X = X;
            ((TillLookedAtCommand)cmdlist[1]).Y = Y;
            ((TillLookedAtCommand)cmdlist[1]).Z = Z;

            ((TillMovedToCommand)cmdlist[4]).X = X;
            ((TillMovedToCommand)cmdlist[4]).Y = Y;
            ((TillMovedToCommand)cmdlist[4]).Z = Z;
            ((TillMovedToCommand)cmdlist[4]).threshold = threshold;

            PluginLog.Log($"Scheduling {cmdlist.Count} commands to the front.");
            cmdManager.ScheduleFront(cmdlist);
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
