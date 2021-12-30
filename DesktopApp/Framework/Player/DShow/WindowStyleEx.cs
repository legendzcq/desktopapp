using System;

namespace Framework.Player.DShow
{
    /// <summary>
    /// From WS_EX_* defines
    /// </summary>
    [Flags]
    public enum WindowStyleEx
    {
        DlgModalFrame = 0x00000001,
        NoParentNotify = 0x00000004,
        Topmost = 0x00000008,
        AcceptFiles = 0x00000010,
        Transparent = 0x00000020,
        MDIChild = 0x00000040,
        ToolWindow = 0x00000080,
        WindowEdge = 0x00000100,
        ClientEdge = 0x00000200,
        ContextHelp = 0x00000400,
        Right = 0x00001000,
        Left = 0x00000000,
        RTLReading = 0x00002000,
        LTRReading = 0x00000000,
        LeftScrollBar = 0x00004000,
        RightScrollBar = 0x00000000,
        ControlParent = 0x00010000,
        StaticEdge = 0x00020000,
        APPWindow = 0x00040000,
        Layered = 0x00080000,
        NoInheritLayout = 0x00100000,
        LayoutRTL = 0x00400000,
        Composited = 0x02000000,
        NoActivate = 0x08000000
    }
}