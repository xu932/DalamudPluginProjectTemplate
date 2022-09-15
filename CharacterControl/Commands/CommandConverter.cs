using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

using Dalamud.Logging;

namespace CottonCollector.CharacterControl.Commands
{
    internal class CommandConverter : JsonConverter<Command>
    {
        public override Command ReadJson(JsonReader reader, Type objectType, Command existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
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
