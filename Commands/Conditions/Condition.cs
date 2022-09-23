using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace CottonCollector.Commands.Conditions
{
    [Serializable]
    [JsonConverter(typeof(ConditionConverter))]
    [JsonObject(MemberSerialization=MemberSerialization.OptIn)]
    internal abstract class Condition
    {
        public static Type[] AllTypes = Assembly.GetAssembly(typeof(Condition)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Condition))).ToArray();

        public abstract bool triggeringCondition();

        public abstract void SelectorGui();
    }
}
