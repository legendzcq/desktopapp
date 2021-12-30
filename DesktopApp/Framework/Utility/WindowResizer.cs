using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Framework.Utility
{
	/// <summary>
	/// 窗口改变大小的类
	/// </summary>
	/// <remarks>修改了以下几个问题：
	/// 1、当WPF中有Form控件或Win32窗体带来的鼠标捕获不到的问题
	/// 2、当鼠标拖动到的位置使得计算结果为0或负值，引起崩溃
	/// 3、对窗体的MinWidth、MinHeight的设置无响应</remarks>
	public class WindowResizer
	{
		/// <summary>
		/// 目标窗口
		/// </summary>
		private Window target = null;

		private readonly List<UIElement> _handlerElements;

		#region 窗口大小改变的方向

		/// <summary>
		/// 通过右边的控件
		/// </summary>
		private bool resizeRight = false;

		/// <summary>
		/// 通过左边的控件
		/// </summary>
		private bool resizeLeft = false;

		/// <summary>
		/// 通过上边的控件
		/// </summary>
		private bool resizeUp = false;

		/// <summary>
		/// 通过下边的控件
		/// </summary>
		private bool resizeDown = false;

		#endregion

		#region 控件容器

		private Dictionary<UIElement, short> leftElements = new Dictionary<UIElement, short>();
		private Dictionary<UIElement, short> rightElements = new Dictionary<UIElement, short>();
		private Dictionary<UIElement, short> upElements = new Dictionary<UIElement, short>();
		private Dictionary<UIElement, short> downElements = new Dictionary<UIElement, short>();

		#endregion

		#region 坐标和尺寸

		/// <summary>
		/// 鼠标坐标
		/// </summary>
		private NativeMethod.PointAPI resizePoint = new NativeMethod.PointAPI();

		/// <summary>
		/// 窗口的尺寸
		/// </summary>
		private Size resizeSize = new Size();

		/// <summary>
		/// 窗口的位置
		/// </summary>
		private Point resizeWindowPoint = new Point();

		#endregion

		/// <summary>
		/// 无参的刷新委托
		/// </summary>
		private delegate void RefreshDelegate();

		public WindowResizer(Window target)
		{
			this.target = target;

			if (target == null)
			{
				throw new Exception("Invalid Window handle");
			}

			_handlerElements = new List<UIElement>();
		}

		#region 添加拖动组件

		/// <summary>
		/// 添加拖动控件的事件
		/// </summary>
		/// <param name="element">要添加事件的控件</param>
		private void connectMouseHandlers(UIElement element)
		{
			element.MouseLeftButtonDown += new MouseButtonEventHandler(element_MouseLeftButtonDown);
			element.MouseLeftButtonUp += new MouseButtonEventHandler(element_PreviewMouseLeftButtonUp);
			element.MouseEnter += new MouseEventHandler(element_MouseEnter);
			element.MouseLeave += new MouseEventHandler(element_MouseLeave);
		}

		private void disconnectMouseHandlers(UIElement element)
		{
			element.MouseLeftButtonDown -= new MouseButtonEventHandler(element_MouseLeftButtonDown);
			element.MouseLeftButtonUp -= new MouseButtonEventHandler(element_PreviewMouseLeftButtonUp);
			element.MouseEnter -= new MouseEventHandler(element_MouseEnter);
			element.MouseLeave -= new MouseEventHandler(element_MouseLeave);
		}

		public void ConnectResizer()
		{
			foreach (var element in _handlerElements)
			{
				connectMouseHandlers(element);
			}
		}

		public void RemoveResizer()
		{
			foreach (var element in _handlerElements)
			{
				disconnectMouseHandlers(element);
			}
		}

		#region 添加拖动控件

		public void AddResizers(UIElement left, UIElement right, UIElement top, UIElement bottom
			, UIElement leftTop, UIElement leftBottom, UIElement rightTop, UIElement rightBottom)
		{
			AddResizerRight(right);
			AddResizerLeft(left);
			AddResizerUp(top);
			AddResizerDown(bottom);
			AddResizerLeftUp(leftTop);
			AddResizerRightUp(rightTop);
			AddResizerLeftDown(leftBottom);
			AddResizerRightDown(rightBottom);
		}

		public void AddResizerRight(UIElement element)
		{
			_handlerElements.Add(element);
			rightElements.Add(element, 0);
		}

		public void AddResizerLeft(UIElement element)
		{
			_handlerElements.Add(element);
			leftElements.Add(element, 0);
		}

		public void AddResizerUp(UIElement element)
		{
			_handlerElements.Add(element);
			upElements.Add(element, 0);
		}

		public void AddResizerDown(UIElement element)
		{
			_handlerElements.Add(element);
			downElements.Add(element, 0);
		}

		public void AddResizerRightDown(UIElement element)
		{
			_handlerElements.Add(element);
			rightElements.Add(element, 0);
			downElements.Add(element, 0);
		}

		public void AddResizerLeftDown(UIElement element)
		{
			_handlerElements.Add(element);
			leftElements.Add(element, 0);
			downElements.Add(element, 0);
		}

		public void AddResizerRightUp(UIElement element)
		{
			_handlerElements.Add(element);
			rightElements.Add(element, 0);
			upElements.Add(element, 0);
		}

		public void AddResizerLeftUp(UIElement element)
		{
			_handlerElements.Add(element);
			leftElements.Add(element, 0);
			upElements.Add(element, 0);
		}

		#endregion

		#endregion

		#region 更新大小

		/// <summary>
		/// 鼠标按下事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//强制使鼠标捕获到此元素
			Mouse.Capture((UIElement)sender);

			//获取鼠标位置
			NativeMethod.GetCursorPos(out resizePoint);

			//保存窗口位置和大小
			resizeSize = new Size(target.Width, target.Height);
			resizeWindowPoint = new Point(target.Left, target.Top);

			#region 更新改变大小的方向

			UIElement sourceSender = (UIElement)sender;
			if (leftElements.ContainsKey(sourceSender))
			{
				resizeLeft = true;
			}
			if (rightElements.ContainsKey(sourceSender))
			{
				resizeRight = true;
			}
			if (upElements.ContainsKey(sourceSender))
			{
				resizeUp = true;
			}
			if (downElements.ContainsKey(sourceSender))
			{
				resizeDown = true;
			}

			#endregion

			SetCursorShape(resizeLeft, resizeRight, resizeUp, resizeDown);

			//启动一个更新窗口大小的线程
			//Thread t = new Thread(updateSizeLoop);
			//t.Name = "Mouse Position Poll Thread";
			//t.Start();
			SystemInfo.StartBackGroundThread("更新窗口大小线程", updateSizeLoop);
		}

		/// <summary>
		/// 鼠标弹起事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void element_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			//释放鼠标对此元素的捕获
			Mouse.Capture(null);

			RaiseMouseUpEvent();

			SetElementCursor((UIElement)sender);
		}

		/// <summary>
		/// 释放控件按下状态，停止拖动
		/// </summary>
		public void RaiseMouseUpEvent()
		{
			resizeRight = false;
			resizeLeft = false;
			resizeUp = false;
			resizeDown = false;
		}

		/// <summary>
		/// 更新大小的循环
		/// </summary>
		private void updateSizeLoop()
		{
			try
			{
				while (resizeDown || resizeLeft || resizeRight || resizeUp)
				{
					target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
						new RefreshDelegate(updateSize));
					//设置线程响应的时间间隔，越短缩放延迟越小，也越耗CPU
					//Thread.Sleep(20);
				}
				target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render,
					new RefreshDelegate(setArrowCursor));
			}
			catch (Exception)
			{
			}
		}

		#region updates

		/// <summary>
		/// 更新窗口大小
		/// </summary>
		private void updateSize()
		{
			NativeMethod.PointAPI p = new NativeMethod.PointAPI();
			//获取鼠标新的位置
			NativeMethod.GetCursorPos(out p);

			if (resizeRight)
			{
				double tW = this.resizeSize.Width - (resizePoint.X - p.X);

				if (0 < tW && tW >= target.MinWidth)
				{
					target.Width = tW;
				}
			}

			if (resizeDown)
			{
				double tH = resizeSize.Height - (resizePoint.Y - p.Y);
				if (0 < tH && tH >= target.MinHeight)
				{
					target.Height = tH;
				}
			}

			if (resizeLeft)
			{
				double tL = resizeWindowPoint.X - (resizePoint.X - p.X);
				double tW = target.ActualWidth + target.Left - tL;

				if (0 < tW && tW >= target.MinWidth)
				{
					target.Width = tW;
					target.Left = tL;
				}
			}

			if (resizeUp)
			{
				double tT = resizeWindowPoint.Y - (resizePoint.Y - p.Y);
				;
				double tH = target.ActualHeight + target.Top - tT;

				if (0 < tH && tH >= target.MinHeight)
				{
					target.Height = tH;
					target.Top = tT;
				}
			}
		}

		#endregion

		#endregion

		#region 鼠标状态更新

		/// <summary>
		/// 鼠标进入控件事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void element_MouseEnter(object sender, MouseEventArgs e)
		{
			SetElementCursor((UIElement)sender);
		}

		/// <summary>
		/// 鼠标离开控件事件
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void element_MouseLeave(object sender, MouseEventArgs e)
		{
			//改回鼠标原来的样式
			if (!resizeDown && !resizeLeft && !resizeRight && !resizeUp)
			{
				setArrowCursor();
			}
		}

		/// <summary>
		/// 设置当鼠标在控件上的形状
		/// </summary>
		/// <param name="sourceSender"></param>
		private void SetElementCursor(UIElement sourceSender)
		{
			bool resizeRight = false;
			bool resizeLeft = false;
			bool resizeUp = false;
			bool resizeDown = false;

			if (leftElements.ContainsKey(sourceSender))
			{
				resizeLeft = true;
			}
			if (rightElements.ContainsKey(sourceSender))
			{
				resizeRight = true;
			}
			if (upElements.ContainsKey(sourceSender))
			{
				resizeUp = true;
			}
			if (downElements.ContainsKey(sourceSender))
			{
				resizeDown = true;
			}

			SetCursorShape(resizeLeft, resizeRight, resizeUp, resizeDown);
		}

		/// <summary>
		/// 改回鼠标原来的样式
		/// </summary>
		private void setArrowCursor()
		{
			target.Cursor = Cursors.Arrow;
		}

		/// <summary>
		/// 根据窗口改变的方向设定鼠标形状
		/// </summary>
		private void SetCursorShape(bool resizeLeft, bool resizeRight, bool resizeUp, bool resizeDown)
		{
			if ((resizeLeft && resizeDown) || (resizeRight && resizeUp))
			{
				target.Cursor = Cursors.SizeNESW;
			}
			else if ((resizeRight && resizeDown) || (resizeLeft && resizeUp))
			{
				target.Cursor = Cursors.SizeNWSE;
			}
			else if (resizeLeft || resizeRight)
			{
				target.Cursor = Cursors.SizeWE;
			}
			else if (resizeUp || resizeDown)
			{
				target.Cursor = Cursors.SizeNS;
			}
		}

		#endregion
	}
}
