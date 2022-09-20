using System;
using System.Numerics;

using CottonCollector.CameraManager;

namespace CottonCollector.Commands.Structures
{
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

        public static bool CameraOnLeft(Vector3 target)
        {
            return CameraPlayerCrossTargetPlayer(target) < -CROSS_THRESHOLD;
        }
        public static bool CameraOnRight(Vector3 target)
        {
            return CameraPlayerCrossTargetPlayer(target) > CROSS_THRESHOLD;
        }
    }
}
