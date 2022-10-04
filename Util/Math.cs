using Dalamud.Logging;
using System;
using System.Linq;
using System.Numerics;

namespace CottonCollector
{
    public static class MyMath
    {
        public static double angle2d(Vector3 player, Vector3 camera, Vector3 target)
        {
            // PluginLog.Log($"{player.ToString()} {camera.ToString()} {target.ToString()}");
            double facing_x = player.X - camera.X, facing_y = player.Z - camera.Y;
            double dir_x = target.X - player.X, dir_y = target.Z - player.Z;
            // player xz
            // camera xy
            // PluginLog.Log($"facing <{facing_x}, {facing_y}>");
            // PluginLog.Log($"direction <{dir_x}, {dir_y}>");
            var cross = (facing_x * dir_y - facing_y * dir_x);
            var dot = (facing_x * dir_x + facing_y * dir_y) / (Math.Sqrt(dir_x * dir_x + dir_y * dir_y) * Math.Sqrt(facing_x * facing_x + facing_y * facing_y));
            dot = Math.Acos(dot);
            if (cross < 0)
            {
                dot = -dot;
            }
            return dot;
        }

        public static double dist(Vector3 player, Vector3 target)
        {
            double dir_x = target.X - player.X, dir_y = target.Z - player.Z;
            return dir_x * dir_x + dir_y * dir_y;
        }

        public static double phi(Vector3 v)
        {
            var dot = (v.X * v.X + v.Z * v.Z) / (Math.Sqrt(v.X * v.X + v.Z * v.Z) * Math.Sqrt(v.X * v.X + v.Z * v.Z + v.Y * v.Y));
            var phi = Math.Acos(dot);
            if (v.Y > 0)
            {
                return phi;
            }
            return -phi;
        }
    }
}