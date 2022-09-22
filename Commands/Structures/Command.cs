using Newtonsoft.Json;

using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

using ImGuiNET;

using Dalamud.Logging;
using CottonCollector.Commands.Conditions;

namespace CottonCollector.Commands.Structures
{
    [Serializable]
    [JsonConverter(typeof(CommandConverter))]
    internal abstract class Command
    {
        public static Type[] AllTypes = Assembly.GetAssembly(typeof(Command)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Command))).ToArray();

        public Condition condition = null;

        private static int nextUid = 0;

        private readonly int uid;

        protected int minTimeMili { set; get; }
        protected int timeOutMili { set; get; }

        public bool TriggerCondition() {
            return this.condition == null || condition.triggeringCondition();
        }

        private bool isCurrent = false;
        private readonly Stopwatch timer = new();

        public Command()
        {
            uid = nextUid++;
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
        public void BuilderGui()
        {
            ImGui.Text($"{this.GetType().Name}");
            ImGui.SameLine();
            SelectorGui();

            if (condition == null)
            {
                ImGui.Text("Condition");
                ImGui.SameLine();

                var types = Condition.AllTypes.ToArray();
                var conditionIndex = 0;
                ImGui.SetNextItemWidth(200);
                ImGui.Combo($"##ConditionTypeSelector__Command__{uid}", ref conditionIndex,
                    types.Select(t => t.Name).ToArray(), types.Length);

                ImGui.SameLine();
                if (ImGui.Button($"Add##Condition__Command__{uid}"))
                {
                    condition = (Condition)Activator.CreateInstance(types[conditionIndex], null);
                }
            } 
            else
            {
                ImGui.Text($"On {condition.GetType().Name}");
                ImGui.SameLine();
                condition.SelectorGui();
                ImGui.SameLine();
                if (ImGui.Button($"Remove##Condition__Command__{uid}")) {
                    condition = null;
                }
            }
        }

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
