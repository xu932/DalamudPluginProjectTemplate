using Newtonsoft.Json;

using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

using ImGuiNET;

using Dalamud.Logging;

using CottonCollector.Commands.Conditions;
using CottonCollector.Util;
using Dalamud.Interface;

namespace CottonCollector.Commands.Structures
{
    [JsonConverter(typeof(CommandConverter))]
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    internal abstract class Command
    {
        public static Type[] AllTypes = Assembly.GetAssembly(typeof(Command)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Command))).ToArray();

        [JsonProperty] public Condition condition = null;

        private static int nextUid = 0;

        private int conditionIndex = 0;

        protected readonly int uid;
        protected int minTimeMili { set; get; }
        protected int timeOutMili { set; get; }

        public bool TriggerCondition() {
            return this.condition == null || condition.triggeringCondition();
        }

        private bool isCurrent = false;
        public bool shouldRepeat { get; protected set; } = false;
        private int runnedTimes = 0;

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

        public virtual void MinimalInfo()
        {
            if (condition != null)
            {
                ImGui.Text(condition.Description());
            }
        }

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
                ImGui.SetNextItemWidth(200);
                ImGui.Combo(Ui.Uid(index: uid), ref conditionIndex,
                    types.Select(t => t.Name).ToArray(), types.Length);

                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Plus.ToIconString()}", uid)))
                {
                    condition = (Condition)Activator.CreateInstance(types[conditionIndex], null);
                }
                ImGui.PopFont();
            } 
            else
            {
                ImGui.Text($"On {condition.GetType().Name}");
                ImGui.SameLine();
                condition.SelectorGui();

                ImGui.SameLine();
                ImGui.PushFont(UiBuilder.IconFont);
                if (ImGui.Button(Ui.Uid($"{FontAwesomeIcon.Trash.ToIconString()}", uid))) {
                    condition = null;
                }
                ImGui.PopFont();
            }
        }

        public void Execute()
        {
            if (runnedTimes == 0)
            {
                PluginLog.Log($"Executing Command {this.GetType()}");
                isCurrent = true;
                OnStart();
                timer.Reset();
                timer.Start();
            }

            Do();
            runnedTimes++;
        }

        // This should be called per frame.
        public bool IsFinished()
        {
            bool timedout = timeOutMili != -1 && timer.ElapsedMilliseconds >= timeOutMili;
            bool finished = timer.ElapsedMilliseconds >= minTimeMili && TerminateCondition() || timedout;

            if (finished)
            {
                timer.Stop();

                runnedTimes = 0;
                OnFinish();
                isCurrent = false;
                PluginLog.Log($"Finished Command {this.GetType()}");
            }
            return finished;
        }
        #endregion
    }
}
