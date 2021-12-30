using System;

namespace Framework.Player.DShow
{
    /// <summary>
    /// From AM_SEEKING_SeekingCapabilities
    /// </summary>
    [Flags]
    public enum AMSeekingSeekingCapabilities
    {
        None = 0,
        CanSeekAbsolute = 0x001,
        CanSeekForwards = 0x002,
        CanSeekBackwards = 0x004,
        CanGetCurrentPos = 0x008,
        CanGetStopPos = 0x010,
        CanGetDuration = 0x020,
        CanPlayBackwards = 0x040,
        CanDoSegments = 0x080,
        Source = 0x100
    }
}