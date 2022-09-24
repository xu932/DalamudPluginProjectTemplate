using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using System;

using CottonCollector.Commands.Impls;
namespace CottonCollector.Commands.Structures
{
    internal class CommandConverter : JsonConverter<Command>
    {
        public override Command ReadJson(JsonReader reader, Type objectType, Command existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            object ret;
            JObject jo = JObject.Load(reader);
            string id = (string)jo["$ref"];

            if (id != null)
            {
                return (Command)serializer.ReferenceResolver.ResolveReference(serializer, id);
            }

            Type type = Type.GetType(jo["$type"].ToString());

            if (type.Equals(typeof(CommandSet)))
            {
                ret = Activator.CreateInstance(type, jo["uniqueId"].Value<string>());
            }
            else
            {
                ret = Activator.CreateInstance(type);
            }

            if (ret == null) return null;

            serializer.Populate(jo.CreateReader(), ret);
            return (Command)ret;
        }

        public override bool CanWrite => false;

        public override void WriteJson(JsonWriter writer, Command value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
