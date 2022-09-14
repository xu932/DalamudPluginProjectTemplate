using System;
using System.Diagnostics;

namespace CottonCollector.CharacterControl.Commands
{
    internal abstract class Command
    {
        protected int minTimeMili { set; get; }
        protected int timeOutMili { set; get; }

        private Stopwatch timer = new Stopwatch();

        public Command()
        {
            minTimeMili = 50;
            timeOutMili = 1000 * 60 * 5; // 5 mins 
        }

        public abstract void Do();

        public abstract bool TerminateCondition();

        public abstract void SelectorGui();

        public virtual void OnStart() { }
        public virtual void OnFinish() { }

        public void Execute()
        {
            OnStart();
            timer.Reset();
            timer.Start();

            Do();
        }

        // This should be called per frame.
        public bool IsFinished()
        {
            bool timedout = timer.ElapsedMilliseconds >= timeOutMili;
            bool finished = timer.ElapsedMilliseconds >= minTimeMili && TerminateCondition() || timedout;

            if (finished)
            {
                timer.Stop();
                OnFinish();
            }
            return finished;
        }
    }
}
