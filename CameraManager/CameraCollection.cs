using System.Runtime.InteropServices;

namespace CottonCollector.CameraManager
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct CameraCollection
    {
        [FieldOffset(0x0)] public GameCamera* WorldCamera;
        [FieldOffset(0x8)] public GameCamera* IdleCamera;
        [FieldOffset(0x10)] public GameCamera* MenuCamera;
        [FieldOffset(0x18)] public GameCamera* SpectatorCamera;
    }
}

