using System;
using System.Diagnostics;

using Dalamud.Game.ClientState.Keys;

namespace CottonCollector.CharacterControl
{
    internal class TimedCommand : Command
    {
        private Stopwatch timer = new Stopwatch();
        private readonly int mili;

        public TimedCommand(Commands.Type type, VirtualKey vk, int mili) : base(type, vk)
        {
            this.mili = mili;
        }

        public override bool IsFinished()
        {
            return timer.ElapsedMilliseconds > mili;
        }

        public override void OnStart()
        {
            timer.Start(); 
        }

        public override void OnFinish()
        {
            timer.Stop();
        }
    }
}
