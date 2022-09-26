using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using Dalamud.Logging;

using ImGuiNET;

namespace CottonCollector.Util
{
    internal class Ui
    {
        private static int cnt = 0;
        private static Dictionary<string, string> hashToUid = new();

        public enum UiType
        {
            BUTTON = 0,
            COMBO = 1,
            INPUT = 2,
            TABLE = 3,
            CHILD = 4,
        };

        public static string Uid(string label = "")
        {
            var trace = new System.Diagnostics.StackTrace();
            string traceHash = string.Join(',', trace.GetFrames().Take(3).Select(t => t.ToString()));
            if (!hashToUid.ContainsKey(traceHash)) {
                hashToUid.Add(traceHash, label + "##" + cnt++);
            }
            return hashToUid[traceHash];
        }

        public static Vector3? GetCurrPosBtn(string uid)
        {
            if(ImGui.Button(uid))
            {
                var player = CottonCollectorPlugin.ClientState.LocalPlayer;
                return player.Position;
            }
            return null;
        }
    }
}
