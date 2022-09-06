using System;

using Dalamud.Game;
using Dalamud.Hooking;
using Dalamud.Game.ClientState.Conditions;

namespace CottonCollector.CameraManager
{
    public static unsafe class CameraHelpers
    {
        public static CameraCollection* collection;
        public static float cameraHeightOffset = 0;
        public static float cameraSideOffset = 0;

        private delegate void GetCameraPositionDelegate(GameCamera* camera, IntPtr target, float* vectorPosition, bool swapPerson);
        private static Hook<GetCameraPositionDelegate> GetCameraPositionHook;
        private static void GetCameraPositionDetour(GameCamera* camera, IntPtr target, float* vectorPosition, bool swapPerson)
        {
            if (CottonCollectorPlugin.ClientState.IsLoggedIn)
            {
                GetCameraPositionHook.Original(camera, target, vectorPosition, swapPerson);
                vectorPosition[1] += cameraHeightOffset;

                if (cameraSideOffset == 0 || camera->Mode != 1) return;

                const float halfPI = MathF.PI / 2f;
                var a = collection->WorldCamera->CurrentHRotation - halfPI;
                vectorPosition[0] += -cameraSideOffset * MathF.Sin(a);
                vectorPosition[2] += -cameraSideOffset * MathF.Cos(a);
            }
        }

        public static void Initialize()
        {
            // Ini CameraCollection
            var sigScanner = new SigScanner();
            collection = (CameraCollection*)sigScanner.GetStaticAddressFromSig("4C 8D 35 ?? ?? ?? ?? 85 D2");

            var vtbl = collection->WorldCamera->VTable;
            GetCameraPositionHook = Hook<GetCameraPositionDelegate>.FromAddress(vtbl[15], GetCameraPositionDetour);
            GetCameraPositionHook.Enable();
        }

        public static void Dispose()
        {
            GetCameraPositionHook?.Dispose();
        }
    }
}
