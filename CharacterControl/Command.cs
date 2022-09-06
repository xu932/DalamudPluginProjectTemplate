using System;

using WindowsInput;
using WindowsInput.Native;

using Dalamud.Game.ClientState.Keys;

namespace CottonCollector.CharacterControl
{
    internal abstract class Command
    {

        private Commands.Type type;
        private VirtualKey vk;
        private InputSimulator sim = new InputSimulator();

        public Command(Commands.Type type, VirtualKey vk)
        {
            this.type = type;
            this.vk = vk;
        }

        public abstract bool IsFinished();
        public abstract void OnStart();
        public abstract void OnFinish();

        public void Execute()
        {
            OnStart();
            switch (type)
            {
                case Commands.Type.KEY_DOWN:
                    sim.Keyboard.KeyDown((VirtualKeyCode)((int)vk));
                    break;
                case Commands.Type.KEY_UP:
                    sim.Keyboard.KeyUp((VirtualKeyCode)((int)vk));
                    break;
                case Commands.Type.KEY_PRESS:
                    sim.Keyboard.KeyPress((VirtualKeyCode)((int)vk));
                    break;
                case Commands.Type.MOUSE_LBTN_DOWN:
                    sim.Mouse.LeftButtonDown();
                    break;
                case Commands.Type.MOUSE_LBTN_UP:
                    sim.Mouse.LeftButtonUp();
                    break;
                case Commands.Type.MOUSE_LBTN_CLICK:
                    sim.Mouse.LeftButtonClick();
                    break;
                case Commands.Type.MOUSE_RBTN_DOWN:
                    sim.Mouse.RightButtonDown();
                    break;
                case Commands.Type.MOUSE_RBTN_UP:
                    sim.Mouse.RightButtonUp();
                    break;
                case Commands.Type.MOUSE_RBTN_CLICK:
                    sim.Mouse.RightButtonClick();
                    break;
            }
        }
    }
}
