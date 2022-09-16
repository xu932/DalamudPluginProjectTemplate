using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace CottonCollector.CharacterControl.Commands
{
    [Serializable]
    [JsonConverter(typeof(CommandConverter))]
    internal abstract class Command
    {
        public enum Type
        {
            KEYBOARD_COMMAND = 0,
            SLEEP_COMMAND = 1,
            TILL_LOOKED_AT_COMMAND = 2,
            TILL_MOVED_TO_COMMAND = 3,

            COMBINED_COMMAND = 4,
        }

        protected int minTimeMili { set; get; }
        protected int timeOutMili { set; get; }

        private bool isCurrent = false;
        private Stopwatch timer = new Stopwatch();

        public Type type;

        public Command(Type type)
        {
            minTimeMili = 0;
            timeOutMili = 1000 * 60 * 5; // 5 mins 
            this.type = type;
        }

        public bool IsCurrent() => isCurrent;

        public abstract void Do();

        public abstract bool TerminateCondition();

        public abstract void SelectorGui();

        public virtual void OnStart() { }
        public virtual void OnFinish() { }

        public void Execute()
        {
            isCurrent = true;
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
                isCurrent = false;
            }
            return finished;
        }
    }
}
