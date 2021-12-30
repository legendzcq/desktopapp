using System;

namespace Framework.Player.DShow
{
    /// <summary>
    /// From AM_SEEKING_SeekingFlags
    /// </summary>
    [Flags]
    public enum AMSeekingSeekingFlags
    {
        NoPositioning = 0x00,
        AbsolutePositioning = 0x01,
        RelativePositioning = 0x02,
        IncrementalPositioning = 0x03,
        PositioningBitsMask = 0x03,
        SeekToKeyFrame = 0x04,
        ReturnTime = 0x08,
        Segment = 0x10,
        NoFlush = 0x20
    }
}