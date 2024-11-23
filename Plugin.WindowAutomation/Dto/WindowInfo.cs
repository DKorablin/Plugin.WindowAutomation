using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Plugin.WindowAutomation.Dto
{
	public class WindowInfo
	{
		public enum ClassType
		{
			Unknown,
			Menu,
			Desktop,
			Dialog,
			TaskSwitch,
			IconTitle,
		}
		private String _className;
		public IntPtr Handle { get; }

		public Boolean IsEmpty => this.Handle == IntPtr.Zero;

		public Boolean IsVisible
		{
			get => !this.IsEmpty && Native.Window.IsWindowVisible(this.Handle);
			set
			{
				if(this.IsEmpty)
					return;

				Native.Window.ShowWindow(this.Handle, value ? Native.Window.SW.SHOWNORMAL : Native.Window.SW.HIDE);
			}
		}

		public String Caption
		{
			get => this.IsEmpty ? null : Native.Window.GetWindowText(this.Handle);
			set
			{
				IntPtr lParam = Marshal.StringToHGlobalUni(value);
				try
				{
					this.SendMessage(Native.Window.WM.SETTEXT, IntPtr.Zero, lParam);
				} finally
				{
					Marshal.FreeHGlobal(lParam);
				}
			}
		}

		public String ModuleFileName { get => this.IsEmpty ? null : Native.Window.GetWindowModuleFileName(this.Handle); }

		public String ClassName
		{
			get
			{
				if(this._className != null)
					return this._className;

				return this._className = this.IsEmpty
					? String.Empty
					: Native.Window.GetClassName(this.Handle);
			}
		}

		public ClassType ClassNameType
		{
			get
			{
				switch(this.ClassName)
				{
				case "#32768":
					return ClassType.Menu;
				case "#32769":
					return ClassType.Desktop;
				case "#32770":
					return ClassType.Dialog;
				case "#32771":
					return ClassType.TaskSwitch;
				case "#32772":
					return ClassType.IconTitle;
				default:
					return ClassType.Unknown;
				}
			}
		}

		public Rectangle Rect
		{
			get => this.IsEmpty ? Rectangle.Empty : Native.Window.GetWindowRect(this.Handle);
			set
			{
				if(this.IsEmpty)
					return;

				Native.Window.SetWindowPos(this.Handle, Native.Window.InsertAfter.TOP, value, Native.Window.SWP.NOACTIVATE | Native.Window.SWP.NOOWNERZORDER);
			}
		}

		public Point CursorPosition
		{
			get
			{
				if(this.IsEmpty)
					return Point.Empty;
				else
				{
					Point point = Cursor.Position;
					if(!Native.Window.ScreenToClient(this.Handle, ref point))
						throw new InvalidOperationException();
					return point;
				}
			}
		}

		public WindowInfo()
			: this(Native.Window.GetDesktopWindow())
		{ }

		public WindowInfo(IntPtr hWnd)
			=> this.Handle = hWnd;

		public WindowInfo(Point point)
		{
			this.Handle = IntPtr.Zero;

			IntPtr wndPtr = Native.Window.WindowFromPoint(point);
			if(wndPtr == IntPtr.Zero)
				return;

			if(!Native.Window.ScreenToClient(wndPtr, ref point))
				return;

			this.Handle = Native.Window.ChildWindowFromPointEx(wndPtr, point, 0);
			if(this.Handle == IntPtr.Zero)
				return;

			if(!Native.Window.ClientToScreen(wndPtr, ref point))
				return;

			if(!Native.Window.IsChild(Native.Window.GetParent(this.Handle), this.Handle))
				return;

			// create a list to hold all childs under the point
			List<IntPtr> windowList = new List<IntPtr>();
			while(this.Handle != IntPtr.Zero)
			{
				Rectangle rect = Native.Window.GetWindowRect(this.Handle);
				if(rect.Contains(point))
					windowList.Add(this.Handle);
				this.Handle = Native.Window.GetWindow(this.Handle, Native.Window.GW.HWNDNEXT);
			}

			// search for the smallest window in the list
			//Int32 minPixelOld = Native.Window.GetSystemMetrics(Native.Window.SM.CXFULLSCREEN) * Native.Window.GetSystemMetrics(Native.Window.SM.CYFULLSCREEN);
			Rectangle screenBounds = Screen.PrimaryScreen.Bounds;
			Int32 minPixel = screenBounds.Width * screenBounds.Height;
			for(Int32 loop = 0; loop < windowList.Count; ++loop)
			{
				Rectangle rect = Native.Window.GetWindowRect(windowList[loop]);
				Int32 childPixel = rect.Width * rect.Height;
				if(childPixel < minPixel)
				{
					minPixel = childPixel;
					this.Handle = windowList[loop];
				}
			}
		}

		public String GetWindowText()
		{
			StringBuilder result = new StringBuilder(256);
			if(Native.Window.SendMessageTimeoutText(this.Handle, Native.Window.WM.GETTEXT, result.Capacity, result, Native.Window.SMTO.ABORTIFHUNG, 100, out IntPtr dwResult) == 0)
				throw new Win32Exception();

			return result.ToString();
		}

		internal void SendMessage(Native.Window.WM message, IntPtr wParam, IntPtr lParam)
		{
			if(Native.Window.SendMessage(this.Handle, message, wParam, lParam) == IntPtr.Zero)
				throw new Win32Exception();
		}

		/// <summary>Подсветить рамки окна</summary>
		public void ToggleBorder()
		{
			if(this.IsEmpty)
				return;

			Rectangle rect = this.Rect;
			IntPtr dc = Native.Window.GetWindowDC(this.Handle);
			Debug.Assert(dc != IntPtr.Zero);

			try
			{
				if(Native.Gdi.SetROP2(dc, (Int32)Native.Gdi.RopMode.R2_NOT) == 0)
					throw new InvalidOperationException();

				Color color = Color.FromArgb(0, 255, 0);
				IntPtr pen = Native.Gdi.CreatePen((Int32)Native.Gdi.PenStyles.PS_INSIDEFRAME, 3 * Native.Window.GetSystemMetrics(Native.Window.SM.CXBORDER), (UInt32)color.ToArgb());
				Debug.Assert(pen != IntPtr.Zero);

				try
				{
					// Draw the rectangle around the window
					IntPtr OldPen = Native.Gdi.SelectObject(dc, pen);
					IntPtr OldBrush = Native.Gdi.SelectObject(dc, Native.Gdi.GetStockObject(Native.Gdi.StockObjects.NULL_BRUSH));
					Native.Gdi.Rectangle(dc, 0, 0, rect.Width, rect.Height);

					Native.Gdi.SelectObject(dc, OldBrush);
					Native.Gdi.SelectObject(dc, OldPen);
				} finally
				{
					Native.Gdi.DeleteObject(pen);
				}
			} finally
			{
				Native.Window.ReleaseDC(this.Handle, dc);
			}
		}

		public WindowInfo GetParentWindow()
			=> this.IsEmpty
				? null
				: new WindowInfo(Native.Window.GetWindow(this.Handle, Native.Window.GW.OWNER));

		public IEnumerable<WindowInfo> GetChildWindows()
		{
			if(this.IsEmpty)
				yield break;

			WindowInfo child = new WindowInfo(Native.Window.GetWindow(this.Handle, Native.Window.GW.CHILD));
			while(!child.IsEmpty)
			{
				yield return child;
				child = new WindowInfo(Native.Window.GetWindow(child.Handle, Native.Window.GW.HWNDNEXT));
			}
		}

		public Bitmap GetWindowBitmap()
		{
			if(this.IsEmpty)
				return GetDesktopBitmap();

			IntPtr wndDc = Native.Window.GetWindowDC(this.Handle);
			Debug.Assert(wndDc != IntPtr.Zero);

			Rectangle rect = Native.Window.GetWindowRect(this.Handle);
			Bitmap result = new Bitmap(rect.Width, rect.Height);
			try
			{
				using(Graphics g = Graphics.FromImage(result))
				{
					IntPtr hDc = g.GetHdc();
					try
					{
						if(!Native.Gdi.BitBlt(hDc, 0, 0, rect.Width, rect.Height, wndDc, 0, 0, Native.Gdi.RasterOperationCode.SRCCOPY))
							throw new Win32Exception();
					} finally
					{
						g.ReleaseHdc(hDc);
					}
				}
			} finally
			{
				Native.Window.ReleaseDC(this.Handle, wndDc);
			}

			return result;
		}

		public static Bitmap GetDesktopBitmap()
		{
			Rectangle rcScreen = Rectangle.Empty;
			Screen[] screens = Screen.AllScreens;

			foreach(Screen screen in screens)
				rcScreen = Rectangle.Union(rcScreen, screen.Bounds);

			Bitmap result = new Bitmap(rcScreen.Width, rcScreen.Height);

			using(Graphics g = Graphics.FromImage(result))
			{
				g.CompositingQuality = CompositingQuality.HighSpeed;
				g.FillRectangle(SystemBrushes.Desktop, 0, 0, rcScreen.Width - rcScreen.X, rcScreen.Height - rcScreen.Y);

				IntPtr hdcDest = g.GetHdc();
				try
				{
					foreach(Screen screen in screens)
					{
						IntPtr hdcSource = Native.Window.CreateDC(IntPtr.Zero, screen.DeviceName, IntPtr.Zero, IntPtr.Zero);

						try
						{
							Int32 xDest = screen.Bounds.X - rcScreen.X;
							Int32 yDest = screen.Bounds.Y - rcScreen.Y;

							if(!Native.Gdi.StretchBlt(hdcDest,
								xDest, yDest,
								screen.Bounds.Width, screen.Bounds.Height,
								hdcSource,
								0, 0,
								screen.Bounds.Width, screen.Bounds.Height,
								Native.Gdi.RasterOperationCode.SRCCOPY))
								throw new Win32Exception();

						} finally
						{
							Native.Window.DeleteDC(hdcSource);
						}
					}
				} finally
				{
					g.ReleaseHdc(hdcDest);
				}
			}
			return result;
		}

		public Bitmap GetWindowIcon(Boolean isLargeIcon)
		{
			if(this.IsEmpty)
				return null;

			Int32 errorCode = Native.Window.SendMessageTimeout(
				this.Handle,
				Native.Window.WM.GETICON,
				isLargeIcon ? Native.Window.ICON_BIG : Native.Window.ICON_SMALL,
				0,
				Native.Window.SMTO.ABORTIFHUNG,
				1000,
				out IntPtr hIcon);

			if(hIcon == IntPtr.Zero)
				hIcon = Native.Window.GetClassLong(this.Handle,
					isLargeIcon ? Native.Window.GCL.GCLP_HICON : Native.Window.GCL.GCLP_HICONSM);

			if(hIcon == IntPtr.Zero)
			{
				errorCode = Native.Window.SendMessageTimeout(
					this.Handle,
					Native.Window.WM.QUERYDRAGICON,
					0,
					0,
					Native.Window.SMTO.ABORTIFHUNG,
					1000,
					out hIcon);
				if(errorCode == 0)
					throw new Win32Exception();
			}

			return hIcon == IntPtr.Zero
				? null
				: Bitmap.FromHicon(hIcon);
		}

		public override Int32 GetHashCode()
			=> this.Handle.GetHashCode();

		public override Boolean Equals(Object obj)
			=> obj is WindowInfo wndInfo && this.Handle == wndInfo.Handle;

		public override String ToString()
		{
			ClassType cType = this.ClassNameType;
			String classType = cType == ClassType.Unknown ? this.ClassName : cType.ToString();
			String wndTitle = this.GetWindowText();

			return String.IsNullOrEmpty(wndTitle)
				? String.Format("0x{0:X8} [{1}]", this.Handle.ToInt64(), classType)
				: String.Format("0x{0:X8} [{1}] \"{2}\"", this.Handle.ToInt64(), classType, wndTitle.Length > 255 ? wndTitle.Substring(0, 255) : wndTitle);
		}
	}
}