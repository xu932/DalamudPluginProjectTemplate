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
        [JsonProperty] public Condition condition = null;

        internal static Type[] AllTypes = Assembly.GetAssembly(typeof(Command)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Command))).ToArray();
        private static int nextUid = 0;

        internal bool IsCurrent { get; private set; } = false;
        internal virtual bool ShouldRepeat { get; } = false;
        protected virtual int MinTimeMili { get; } = 0;
        protected virtual int TimeOutMili { get; } = 1000 * 60 * 5;
        protected virtual long MinRefireMili { get; } = 10;

        protected readonly int uid;
        private int conditionIndex = 0;
        private int runnedTimes = 0;
        private long lastFireTime = -1;

        private readonly Stopwatch timer = new();

        internal bool TriggerCondition() {
            bool triggerCondition = condition?.triggeringCondition() ?? true;
            bool refireCondition = runnedTimes == 0 || MinRefireMili < timer.ElapsedMilliseconds - lastFireTime;
            return triggerCondition && refireCondition;
        }

        internal Command()
        {
            uid = nextUid++;
            ResetExecutionState();
        }

        internal bool IsCommandSet() => this is CommandSet;

        protected abstract bool TerminateCondition();
        protected abstract void Do();
        protected virtual void OnStart() { }
        protected virtual void OnFinish() { }

        #region wrapper methods
        internal virtual void ResetExecutionState()
        {
            timer.Stop();
            timer.Reset();
            runnedTimes = 0;
            lastFireTime = -1;
            IsCurrent = false;
        }

        internal void Execute()
        {
            if (runnedTimes == 0)
            {
                PluginLog.Log($"Executing Command {this.GetType()}");
                IsCurrent = true;
                OnStart();
                timer.Reset();
                timer.Start();
            }
            lastFireTime = timer.ElapsedMilliseconds; 

            Do();
            runnedTimes++;
        }

        internal bool IsFinished()
        {
            bool timedout = TimeOutMili != -1 && timer.ElapsedMilliseconds >= TimeOutMili;
            bool finished = timer.ElapsedMilliseconds >= MinTimeMili && TerminateCondition() || timedout;

            if (finished)
            {
                OnFinish();
                ResetExecutionState();
                PluginLog.Log($"Finished Command {this.GetType()}");
            }
            return finished;
        }

        #region GUI
        internal virtual void MinimalInfo()
        {
            if (condition != null)
            {
                ImGui.Text(condition.Description());
            }
        }

        internal abstract void SelectorGui();

        internal void BuilderGui()
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
        #endregion
        #endregion
    }
}
