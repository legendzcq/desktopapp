using System;

namespace Framework.Player.DShow
{
    static public class FindDirection
    {
        /// <summary> LOOK_UPSTREAM_ONLY </summary>
        public static readonly Guid UpstreamOnly = new Guid(0xac798be0, 0x98e3, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);

        /// <summary> LOOK_DOWNSTREAM_ONLY </summary>
        public static readonly Guid DownstreamOnly = new Guid(0xac798be1, 0x98e3, 0x11d1, 0xb3, 0xf1, 0x00, 0xaa, 0x00, 0x37, 0x61, 0xc5);
    }
}