using System;
using System.Numerics;

using CottonCollector.CameraManager;
using Dalamud.Logging;

namespace CottonCollector.Commands.Conditions
{
    /*
    internal static unsafe class Conditions
    {
        private static readonly double CROSS_THRESHOLD = 5e-2;

        private static double CameraPlayerCrossTargetPlayer(Vector3 target)
        {
            var player = CottonCollectorPlugin.ClientState.LocalPlayer;
            var targetPos2 = new Vector2(target.X, target.Z);
            var playerPos2 = new Vector2(player.Position.X, player.Position.Z);
            var cameraPos2 = new Vector2(CameraHelpers.collection->WorldCamera->X,
                CameraHelpers.collection->WorldCamera->Y);
            var v = Vector2.Normalize(cameraPos2 - playerPos2);
            var u = Vector2.Normalize(targetPos2 - playerPos2);

            return v.X * u.Y - v.Y * u.X;
        }

        public delegate bool Condition(ConditionConfig config);

        public static bool CameraOnLeft(ConditionConfig config)
        {
            return CameraPlayerCrossTargetPlayer(config.targetPos) < -CROSS_THRESHOLD;
        }

        public static bool CameraOnRight(ConditionConfig config)
        {
            return CameraPlayerCrossTargetPlayer(config.targetPos) > CROSS_THRESHOLD;
        }

        public static bool PlayerAt(Vector3 target, ref double threshold)
        {
            var dist = Vector3.Distance(CottonCollectorPlugin.ClientState.LocalPlayer.Position, target);
            PluginLog.Log($"player: {CottonCollectorPlugin.ClientState.LocalPlayer.Position}, target: {target}, threshold: {threshold}, dist: {dist}");
            return dist < threshold;
        }
    }
    */
}
