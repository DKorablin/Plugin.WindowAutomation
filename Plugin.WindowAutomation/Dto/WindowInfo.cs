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
	/// <summary>Provides rich information and helper operations for a native (Win32) window handle.</summary>
	public class WindowInfo
	{
		/// <summary>Well-known native class identifiers.</summary>
		public enum ClassType
		{
			/// <summary>Class name is not mapped to a known constant.</summary>
			Unknown,
			/// <summary>Popup menu window (#32768).</summary>
			Menu,
			/// <summary>Desktop window (#32769).</summary>
			Desktop,
			/// <summary>Standard dialog window (#32770).</summary>
			Dialog,
			/// <summary>Alt+Tab task switch window (#32771).</summary>
			TaskSwitch,
			/// <summary>Icon title window (#32772).</summary>
			IconTitle,
		}
		private String _className;
		/// <summary>Gets the native window handle (HWND).</summary>
		public IntPtr Handle { get; }

		/// <summary>Gets whether the handle is zero (no window).</summary>
		public Boolean IsEmpty => this.Handle == IntPtr.Zero;

		/// <summary>Gets or sets the visibility of the window using ShowWindow/IsWindowVisible.</summary>
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

		/// <summary>Gets or sets the window caption text (uses WM_SETTEXT).</summary>
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

		/// <summary>Gets the full path of the module (executable or DLL) that owns the window.</summary>
		public String ModuleFileName { get => this.IsEmpty ? null : Native.Window.GetWindowModuleFileName(this.Handle); }

		/// <summary>Gets the cached native class name of the window.</summary>
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

		/// <summary>Gets the mapped <see cref="ClassType"/> for well-known native class names.</summary>
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

		/// <summary>Gets or sets the window rectangle (screen coordinates); setter uses SetWindowPos.</summary>
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

		/// <summary>Gets the current cursor position translated into this window's client coordinates.</summary>
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

		/// <summary>Initializes a new instance wrapping the desktop window.</summary>
		public WindowInfo()
			: this(Native.Window.GetDesktopWindow())
		{ }

		/// <summary>Initializes a new instance with a specific window handle.</summary>
		public WindowInfo(IntPtr hWnd)
			=> this.Handle = hWnd;

		/// <summary>Initializes a new instance for the smallest child window located at a given screen point.</summary>
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

			// create a list to hold all children under the point
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

		/// <summary>Retrieves the live window text using SendMessageTimeout (more reliable than cached caption).</summary>
		public String GetWindowText()
		{
			StringBuilder result = new StringBuilder(256);
			if(Native.Window.SendMessageTimeoutText(this.Handle, Native.Window.WM.GETTEXT, result.Capacity, result, Native.Window.SMTO.ABORTIFHUNG, 100, out IntPtr dwResult) == 0)
			{
				Int32 errorCode = Marshal.GetLastWin32Error();
				return errorCode == 0//If the function returns 0, and GetLastError returns ERROR_SUCCESS, then treat it as a generic failure.
					? String.Empty
					: throw new Win32Exception();
			}

			return result.ToString();
		}

		/// <summary>Sends a window message and throws if it fails.</summary>
		internal void SendMessage(Native.Window.WM message, IntPtr wParam, IntPtr lParam)
		{
			if(Native.Window.SendMessage(this.Handle, message, wParam, lParam) == IntPtr.Zero)
				throw new Win32Exception();
		}

		/// <summary>Toggles a temporary border highlight by drawing an XOR rectangle on the window DC.</summary>
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

		/// <summary>Gets the owner (parent) window.</summary>
		public WindowInfo GetParentWindow()
			=> this.IsEmpty
				? null
				: new WindowInfo(Native.Window.GetWindow(this.Handle, Native.Window.GW.OWNER));

		/// <summary>Enumerates direct child windows.</summary>
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

		/// <summary>Captures a bitmap of this window's bounds using BitBlt; falls back to full desktop if empty.</summary>
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

		/// <summary>Captures a bitmap of the entire virtual desktop across all monitors.</summary>
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

				IntPtr hdcDestination = g.GetHdc();
				try
				{
					foreach(Screen screen in screens)
					{
						IntPtr hdcSource = Native.Window.CreateDC(IntPtr.Zero, screen.DeviceName, IntPtr.Zero, IntPtr.Zero);

						try
						{
							Int32 xDestination = screen.Bounds.X - rcScreen.X;
							Int32 yDestination = screen.Bounds.Y - rcScreen.Y;

							if(!Native.Gdi.StretchBlt(hdcDestination,
								xDestination, yDestination,
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
					g.ReleaseHdc(hdcDestination);
				}
			}
			return result;
		}

		/// <summary>Gets the large or small icon associated with the window (using WM_GETICON or class icon).</summary>
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

		/// <summary>Computes hash code based on the window handle.</summary>
		public override Int32 GetHashCode()
			=> this.Handle.GetHashCode();

		/// <summary>Determines equality by comparing underlying handles.</summary>
		public override Boolean Equals(Object obj)
			=> obj is WindowInfo wndInfo && this.Handle == wndInfo.Handle;

		/// <summary>Returns a readable string including handle, class, and (trimmed) caption text.</summary>
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