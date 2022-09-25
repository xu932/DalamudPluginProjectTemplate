using System;
using System.Numerics;

using ImGuiNET;

namespace CottonCollector.Util
{
    internal class Ui
    {
        public enum UiType
        {
            BUTTON = 0,
            COMBO = 1,
            INPUT = 2,
            TABLE = 3,
            CHILD = 4,
        };

        public static string GenUid(string label, Type type, UiType uiType, string description)
        {
            var uid = label + "##" + type.Name + "__" + uiType.ToString() + "__";
            if(!string.IsNullOrEmpty(description))
            {
                uid += "__" + description;
            }
            return uid;
        }

        public static Vector3? GetCurrPosBtn(string label, Type type, string hash)
        {
            if(ImGui.Button(GenUid(label, type, UiType.BUTTON, "GetCurrPos" + hash)))
            {
                var player = CottonCollectorPlugin.ClientState.LocalPlayer;
                return player.Position;
            }
            return null;
        }
    }
}
