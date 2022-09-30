using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Dalamud.Logging;

using ImGuiNET;

namespace CottonCollector.Util
{
    internal class Ui
    {
        private static int cnt = 0;
        private static Dictionary<string, string> hashToUid = new();
        private static Dictionary<string, int> hashCnt = new();

        public enum UiType
        {
            BUTTON = 0,
            COMBO = 1,
            INPUT = 2,
            TABLE = 3,
            CHILD = 4,
        };

        public static string Uid(string label = "", int index = -1, 
            [CallerLineNumber] int lineNumber = 0, [CallerMemberName] string caller = null)
        {
            string traceHash = caller + "__" + lineNumber + "__" + label;
            if (index != -1)
            {
                traceHash += "__" + index;
            }

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

        public static Vector3? GetTargetPosBtn(string uid)
        {
            if(ImGui.Button(uid))
            {
                var target = CottonCollectorPlugin.TargetManager.Target;
                if (target != null)
                {
                    return target.Position;
                }
                else
                {
                    PluginLog.Log("No Target Selected!");
                }
            }
            return null;
        }
    }
}
