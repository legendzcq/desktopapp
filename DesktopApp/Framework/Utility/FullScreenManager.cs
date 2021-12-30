using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace Framework.Utility
{
	/// <summary>
	/// 全屏
	/// </summary>
	public static class FullScreenManager
	{
		public static void RepairWpfWindowFullScreenBehavior(Window wpfWindow)
		{
			if (wpfWindow == null)
			{
				return;
			}

			if (wpfWindow.WindowState == WindowState.Maximized)
			{
				wpfWindow.WindowState = WindowState.Normal;
				wpfWindow.Loaded += delegate { wpfWindow.WindowState = WindowState.Maximized; };
			}

			wpfWindow.SourceInitialized += delegate
			{
				IntPtr handle = (new WindowInteropHelper(wpfWindow)).Handle;
				HwndSource source = HwndSource.FromHwnd(handle);
				if (source != null)
				{
					source.AddHook(WindowProc);
				}
			};
		}

		[DebuggerStepThrough]
		private static IntPtr WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			switch (msg)
			{
				case 0x0024:
					WmGetMinMaxInfo(hwnd, lParam);
					handled = true;
					break;
			}

			return (IntPtr)0;
		}

		private static void WmGetMinMaxInfo(IntPtr hwnd, IntPtr lParam)
		{
			var mmi = (NativeMethod.MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(NativeMethod.MINMAXINFO));

			// Adjust the maximized size and position to fit the work area of the correct monitor
			int MONITOR_DEFAULTTONEAREST = 0x00000002;
			IntPtr monitor = NativeMethod.MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);

			if (monitor != IntPtr.Zero)
			{
				var monitorInfo = new NativeMethod.MONITORINFO();
				NativeMethod.GetMonitorInfo(monitor, monitorInfo);
				NativeMethod.RECT rcWorkArea = monitorInfo.rcWork;
				NativeMethod.RECT rcMonitorArea = monitorInfo.rcMonitor;
				mmi.ptMaxPosition.X = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
				mmi.ptMaxPosition.Y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
				mmi.ptMaxSize.X = Math.Abs(rcWorkArea.right - rcWorkArea.left);
				mmi.ptMaxSize.Y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
			}

			Marshal.StructureToPtr(mmi, lParam, true);
		}
	}
}
