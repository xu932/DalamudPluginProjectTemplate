using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

using Dalamud.Logging;
using CottonCollector.Commands.Impls;

namespace CottonCollector.Commands.Structures
{
    internal class CommandConverter : JsonConverter<Command>
    {
        public override Command ReadJson(JsonReader reader, Type objectType, Command existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            object ret = null;
            JObject jo = JObject.Load(reader);
            var type = Enum.ToObject(typeof(Command.Type), jo["type"].Value<int>());
            switch (type)
            {
                case
                    Command.Type.KEYBOARD_COMMAND:
                    ret = new KeyboardCommand();
                    break;
                case Command.Type.SLEEP_COMMAND:
                    ret = new SleepCommand();
                    break;
                case Command.Type.TILL_LOOKED_AT_COMMAND:
                    ret = new TillLookedAtCommand();
                    break;
                case Command.Type.TILL_MOVED_TO_COMMAND:
                    ret = new TillMovedToCommand();
                    break;
                case Command.Type.COMMAND_SET:
                    var id = jo["uniqueId"].Value<string>();
                    if (CommandSet.CommandSetMap.ContainsKey(id))
                    {
                        ret = CommandSet.CommandSetMap[id];
                        return (Command)ret;
                    }
                    else
                    {
                        ret = new CommandSet(jo["uniqueId"].Value<string>());
                    }
                    break;
            }

            if (ret == null)
            {
                return null;
            }

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
