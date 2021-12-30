using System;
using System.Runtime.InteropServices;

namespace Framework.Player.DShow
{
    /// <summary>
    /// From AM_MEDIA_TYPE - When you are done with an instance of this class,
    /// it should be released with FreeAMMediaType() to avoid leaking
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class AMMediaType
    {
        public Guid majorType;
        public Guid subType;
        [MarshalAs(UnmanagedType.Bool)]
        public bool fixedSizeSamples;
        [MarshalAs(UnmanagedType.Bool)]
        public bool temporalCompression;
        public int sampleSize;
        public Guid formatType;
        public IntPtr unkPtr; // IUnknown Pointer
        public int formatSize;
        public IntPtr formatPtr; // Pointer to a buff determined by formatType
    }

    /// <summary>
    /// CLSID_VideoMixingRenderer9
    /// </summary>
    [ComImport, Guid("51b4abf3-748f-4e3b-a276-c828330e926a")]
    public class VideoMixingRenderer9
    {
    }

	[ComImport, Guid("6BC1CFFA-8FC1-4261-AC22-CFB4CC38DB50")]
	public class VideoRendererDefault
	{
	}

	[ComImport, Guid("e30629d1-27e5-11ce-875d-00608cb78066")]
	public class AudioRender
	{
	}
}