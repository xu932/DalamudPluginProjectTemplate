using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CottonCollector.Commands.Conditions
{
    internal class ConditionConverter : JsonConverter<Condition>
    {
        public override Condition ReadJson(JsonReader reader, Type objectType, Condition existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            JObject jo = JObject.Load(reader);
            Type type = Type.GetType(jo["$type"].ToString());

            object ret = Activator.CreateInstance(type);
            serializer.Populate(jo.CreateReader(), ret);

            return (Condition)ret;

        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, Condition value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
