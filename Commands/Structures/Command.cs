﻿using Newtonsoft.Json;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Dalamud.Logging;

namespace CottonCollector.Commands.Structures
{
    [Serializable]
    [JsonConverter(typeof(CommandConverter))]
    internal abstract class Command
    {
        public static Type[] AllTypes = Assembly.GetAssembly(typeof(Command)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Command))).ToArray();

        protected int minTimeMili { set; get; }
        protected int timeOutMili { set; get; }

        private bool isCurrent = false;
        private Stopwatch timer = new Stopwatch();

        public Command()
        {
            minTimeMili = 0;
            timeOutMili = 1000 * 60 * 5; // 5 mins 
        }

        public bool IsCommandSet() => this is CommandSet;

        public bool IsCurrent() => isCurrent;


        public abstract void SelectorGui();

        public abstract bool TerminateCondition();

        public virtual void OnStart() { }

        public abstract void Do();

        public virtual void OnFinish() { }

        #region wrapper methods
        public void Execute()
        {
            PluginLog.Log($"Executing Command {this.GetType()}");
            isCurrent = true;
            OnStart();
            timer.Reset();
            timer.Start();

            Do();
        }

        // This should be called per frame.
        public bool IsFinished()
        {
            bool timedout = timeOutMili != -1 && timer.ElapsedMilliseconds >= timeOutMili;
            bool finished = timer.ElapsedMilliseconds >= minTimeMili && TerminateCondition() || timedout;

            if (finished)
            {
                timer.Stop();
                OnFinish();
                isCurrent = false;
                PluginLog.Log($"Finished Command {this.GetType()}");
            }
            return finished;
        }
        #endregion
    }
}
