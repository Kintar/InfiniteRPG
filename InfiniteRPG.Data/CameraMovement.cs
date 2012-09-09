using System;

namespace InfiniteRPG.Data
{
    [Flags]
    public enum CameraMovement
    {
        Left = 1,
        Right = 2,
        Up = 4,
        Down = 8,
        ZoomIn = 16,
        ZoomOut = 32,
        RotateCCW = 64,
        RotateCW = 128
    }
}