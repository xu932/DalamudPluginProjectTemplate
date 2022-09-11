using System;
using System.Numerics;
using System.Collections.Generic;

using Dalamud.Logging;

using Dalamud.Game.ClientState.Keys;

using CottonCollector.CameraManager;

namespace CottonCollector.CharacterControl
{
    internal unsafe class Commands
    {
        public enum Type {
            KEY_DOWN = 0,
            KEY_UP = 1,
            KEY_PRESS = 2,
            MOUSE_LBTN_DOWN = 3,
            MOUSE_LBTN_UP = 4,
            MOUSE_LBTN_CLICK = 5,
            MOUSE_RBTN_DOWN = 6,
            MOUSE_RBTN_UP = 7,
            MOUSE_RBTN_CLICK = 8,
            MOUSE_MOVE = 9,
        }

        private bool done = true;
        private Command currCommand;
        private Queue<Command> currCommandQueue = null; // A atomic list of commands.

        public Queue<Queue<Command>> commands;

        public Commands ()
        {
            commands = new Queue<Queue<Command>>();
        }

        public void Update()
        {
            if (done)
            {
                if (currCommandQueue == null || currCommandQueue.Count == 0)
                {
                    if (commands.Count != 0) {
                        currCommandQueue = commands.Dequeue();
                    }
                }

                if (currCommandQueue != null && currCommandQueue.Count != 0)
                {
                    done = false;
                    PluginLog.Log("Exectuing Command");
                    currCommand = currCommandQueue.Dequeue();
                    currCommand.Execute();
                }
            }

            if (!done && currCommand != null && currCommand.IsFinished())
            {
                done = true;
                PluginLog.Log("Finished Command");
            }
        }

        public void KillSwitch()
        {
            commands.Clear();
        }

        public static Queue<Command> FaceCameraCommands(Vector3 targetPos3)
        {
            var cmdQueue = new Queue<Command>();

            var playerPos3 = CottonCollectorPlugin.ClientState.LocalPlayer.Position;
            var playerPos = new Vector2(playerPos3.X, playerPos3.Z);
            var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                CameraHelpers.collection->WorldCamera->Y);
            var targetPos = new Vector2(targetPos3.X, targetPos3.Z);
            var v = Vector2.Normalize(cameraPos - playerPos);
            var u = Vector2.Normalize(targetPos - playerPos);
            var camera_on_left = (v.X * u.Y - u.X * v.Y) < 0;

            cmdQueue.Enqueue(new Command(Commands.Type.KEY_DOWN, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT, 0, () =>
            { 
                var playerPos3 = CottonCollectorPlugin.ClientState.LocalPlayer.Position;
                var playerPos = new Vector2(playerPos3.X, playerPos3.Z);
                var cameraPos = new Vector2(CameraHelpers.collection->WorldCamera->X,
                    CameraHelpers.collection->WorldCamera->Y);
                var v = Vector2.Normalize(cameraPos - playerPos);
                var u = Vector2.Normalize(targetPos - playerPos);
                return (v + u).LengthSquared() < 1e-3f;
            }));
            cmdQueue.Enqueue(new Command(Commands.Type.KEY_UP, camera_on_left ? VirtualKey.RIGHT : VirtualKey.LEFT));

            return cmdQueue;
        }

        public static Queue<Command> MoveToCommands(Vector3 targetPos3)
        {
            var cmdQueue = new Queue<Command>();
            cmdQueue.Enqueue(new Command(Commands.Type.KEY_DOWN, VirtualKey.W, isFinished: () => {
                return Vector3.Distance(targetPos3, CottonCollectorPlugin.ClientState.LocalPlayer.Position) < 3;
            }));
            cmdQueue.Enqueue(new Command(Commands.Type.KEY_UP, VirtualKey.W));
            return cmdQueue;
        }
    }
}
