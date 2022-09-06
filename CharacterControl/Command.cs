using System;

using WindowsInput;
using System.Diagnostics;

using WindowsInput.Native;

using Dalamud.Game.ClientState.Keys;

namespace CottonCollector.CharacterControl
{
    internal class Command
    {
        private Commands.Type type;
        private readonly VirtualKey vk;
        private readonly int mili;

        private Stopwatch timer = new Stopwatch();
        private InputSimulator sim = new InputSimulator();
        protected Func<bool> isFinished;

        public Command(Commands.Type type, VirtualKey vk, int mili = 100, Func<bool> isFinished = null)
        {
            this.type = type;
            this.vk = vk;
            this.isFinished = isFinished;
        }

        public void OnStart()
        {
            timer.Start(); 
        }

        public void OnFinish()
        {
            // These are more than likely useless but for completeness.
            timer.Stop();
            timer.Reset();
        }

        public bool IsFinished()
        {
            if (timer.ElapsedMilliseconds < mili)
            {
                return false;
            }

            if (isFinished != null)
            {
                return isFinished();
            }
            return true;
        }

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
