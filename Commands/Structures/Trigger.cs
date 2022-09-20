using Newtonsoft.Json;
using System;

namespace CottonCollector.Commands.Structures
{
    [Serializable]
    internal class Trigger
    {
        public delegate bool TriggerConditionDelegate();

        [JsonIgnore]
        public TriggerConditionDelegate TriggerCondition = () => false;

        public Command command { private set; get; }

        public Trigger(Command command)
        {
            this.command = command;
        }
    }
}
