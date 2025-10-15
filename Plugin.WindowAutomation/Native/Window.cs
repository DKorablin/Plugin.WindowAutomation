using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace Plugin.WindowAutomation.Native
{
	internal static class Window
	{
		public enum WM
		{
			/// <summary>Copies the text that corresponds to a window into a buffer provided by the caller.</summary>
			GETTEXT = 0x000D,
			/// <summary>Sets the text of a window.</summary>
			SETTEXT = 0x000C,
			/// <summary>Sent to a window to retrieve a handle to the large or small icon associated with a window.</summary>
			/// <remarks>The system displays the large icon in the ALT+TAB dialog, and the small icon in the window caption.</remarks>
			GETICON = 0x007F,
			/// <summary>
			/// Sent to a minimized (iconic) window.
			/// The window is about to be dragged by the user but does not have an icon defined for its class.
			/// </summary>
			/// <remarks>
			/// An application can return a handle to an icon or cursor.
			/// The system displays this cursor or icon while the user drags the icon.
			/// </remarks>
			QUERYDRAGICON = 0x0037,
			/// <summary>Posted to the window with the keyboard focus when a non system key is pressed.</summary>
			/// <remarks>A no system key is a key that is pressed when the ALT key is not pressed.</remarks>
			KEYDOWN = 0x0100,
			/// <summary>Posted to the window with the keyboard focus when a non system key is released.</summary>
			/// <remarks>A non system key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus.</remarks>
			KEYUP = 0x0101,
			/// <summary>
			/// Posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds down the ALT key and then presses another key.
			/// </summary>
			/// <remarks>
			/// It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window.
			/// The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
			/// </remarks>
			SYSKEYDOWN = 0x0104,
			/// <summary>Posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down.</summary>
			/// <remarks>
			/// It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window.
			/// The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
			/// </remarks>
			SYSKEYUP = 0x0105,
			/// <summary>Posted when the user presses the left mouse button while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			LBUTTONDOWN = 0x0201,
			/// <summary>Posted when the user releases the left mouse button while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			LBUTTONUP = 0x0202,
			/// <summary>Posted when the user presses the right mouse button while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			RBUTTONDOWN = 0x0204,
			/// <summary>Posted when the user releases the right mouse button while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			RBUTTONUP = 0x0205,
			/// <summary>Posted when the user presses the middle mouse button while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			MBUTTONDOWN = 0x0206,
			/// <summary>Posted when the user releases the middle mouse button while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			MBUTTONUP = 0x0207,
			/// <summary>Sent to the focus window when the mouse wheel is rotated.</summary>
			/// <remarks>
			/// The DefWindowProc function propagates the message to the window's parent.
			/// There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
			/// </remarks>
			MOUSEWHEEL = 0x020A,
			/// <summary>Posted when the user presses either XBUTTON1 or XBUTTON2 while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			XBUTTONDOWN = 0x020B,
			/// <summary>Posted when the user releases either XBUTTON1 or XBUTTON2 while the cursor is in the client area of a window.</summary>
			/// <remarks>
			/// If the mouse is not captured, the message is posted to the window beneath the cursor.
			/// Otherwise, the message is posted to the window that has captured the mouse.
			/// </remarks>
			XBUTTONUP = 0x020C,
			/// <summary>
			/// Sets or removes the read-only style (ES_READONLY) of an edit control.
			/// You can send this message to either an edit control or a rich edit control.
			/// </summary>
			EM_SETREADONLY = 0x00CF,
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct RECT
		{
			public Int32 Left;
			public Int32 Top;
			public Int32 Right;
			public Int32 Bottom;
			public Rectangle ToRectangle()
				=> new Rectangle(Left, Top, Right - Left, Bottom - Top);
		}

		public enum SM
		{
			/// <summary>The width of a window border, in pixels. This is equivalent to the SM_CXEDGE value for windows with the 3-D look.</summary>
			CXBORDER = 5,
			/// <summary>
			/// The width of the client area for a full-screen window on the primary display monitor, in pixels.
			/// To get the coordinates of the portion of the screen that is not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			CXFULLSCREEN = 16,
			/// <summary>
			/// The height of the client area for a full-screen window on the primary display monitor, in pixels.
			/// To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars, call the SystemParametersInfo function with the SPI_GETWORKAREA value.
			/// </summary>
			CYFULLSCREEN = 17
		}

		/// <summary>Retrieves the specified system metric or system configuration setting.</summary>
		/// <param name="nIndex">The system metric or configuration setting to be retrieved.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		public static extern Int32 GetSystemMetrics(SM nIndex);

		/// <summary>
		/// Retrieves a handle to the desktop window.
		/// The desktop window covers the entire screen.
		/// The desktop window is the area on top of which other windows are painted.
		/// </summary>
		/// <returns>The return value is a handle to the desktop window.</returns>
		[DllImport("user32.dll")]
		public static extern IntPtr GetDesktopWindow();

		/// <summary>Determines the visibility state of the specified window.</summary>
		/// <param name="hWnd">A handle to the window to be tested.</param>
		/// <returns>
		/// If the specified window, its parent window, its parent's parent window, and so forth, have the WS_VISIBLE style, the return value is nonzero.
		/// Otherwise, the return value is zero.
		/// 
		/// Because the return value specifies whether the window has the WS_VISIBLE style, it may be nonzero even if the window is totally obscured by other windows.
		/// </returns>
		[DllImport("user32.dll")]
		public static extern Boolean IsWindowVisible(IntPtr hWnd);

		/// <summary>Specified window's show state</summary>
		public enum SW
		{
			/// <summary>Hides the window and activates another window.</summary>
			HIDE = 0,
			/// <summary>
			/// Activates and displays a window.
			/// If the window is minimized or maximized, the system restores it to its original size and position.
			/// An application should specify this flag when displaying the window for the first time.
			/// </summary>
			SHOWNORMAL = NORMAL,
			/// <summary>
			/// Activates and displays a window.
			/// If the window is minimized or maximized, the system restores it to its original size and position.
			/// An application should specify this flag when displaying the window for the first time.
			/// </summary>
			NORMAL = 1,
			/// <summary>Activates the window and displays it as a minimized window</summary>
			SHOWMINIMIZED = 2,
			/// <summary>Activates the window and displays it as a maximized window</summary>
			SHOWMAXIMIZED = MAXIMIZE,
			/// <summary>Activates the window and displays it as a maximized window</summary>
			MAXIMIZE = 3,
			/// <summary>Displays a window in its most recent size and position</summary>
			/// <remarks>This value is similar to SW_SHOWNORMAL, except that the window is not activated</remarks>
			SHOWNOACTIVATE = 4,
			/// <summary>Activates the window and displays it in its current size and position</summary>
			SHOW = 5,
			/// <summary>Minimizes the specified window and activates the next top-level window in the Z order</summary>
			MINIMIZE = 6,
			/// <summary>
			/// Displays the window as a minimized window.
			/// This value is similar to SW_SHOWMINIMIZED, except the window is not activated.
			/// </summary>
			SHOWMINNOACTIVE = 7,
			/// <summary>
			/// Displays the window in its current size and position.
			/// This value is similar to SW_SHOW, except that the window is not activated.
			/// </summary>
			SHOWNA = 8,
			/// <summary>
			/// Activates and displays the window.
			/// If the window is minimized or maximized, the system restores it to its original size and position.
			/// An application should specify this flag when restoring a minimized window.
			/// </summary>
			RESTORE = 9,
			/// <summary>
			/// Sets the show state based on the SW_ value specified in the STARTUPINFO structure passed to the CreateProcess function by the program that started the application.
			/// </summary>
			SHOWDEFAULT = 10,
			/// <summary>
			/// Minimizes a window, even if the thread that owns the window is not responding.
			/// This flag should only be used when minimizing windows from a different thread.
			/// </summary>
			FORCEMINIMIZE = 11,
		}

		/// <summary>Sets the specified window's show state</summary>
		/// <param name="hWnd">A handle to the window</param>
		/// <param name="nCmdShow">
		/// Controls how the window is to be shown.
		/// This parameter is ignored the first time an application calls ShowWindow, if the program that launched the application provides a STARTUPINFO structure.
		/// Otherwise, the first time ShowWindow is called, the value should be the value obtained by the WinMain function in its nCmdShow parameter.
		/// </param>
		/// <returns>
		/// If the window was previously visible, the return value is nonzero.
		/// If the window was previously hidden, the return value is zero.
		/// </returns>
		[DllImport("user32.dll")]
		public static extern Boolean ShowWindow(IntPtr hWnd, SW nCmdShow);

		/// <summary>The behavior of SendMessageTimeout function</summary>
		[Flags]
		public enum SMTO : UInt32
		{
			/// <summary>The function returns without waiting for the time-out period to elapse if the receiving thread appears to not respond or "hangs."</summary>
			ABORTIFHUNG = 0x0002,
			/// <summary>Prevents the calling thread from processing any other requests until the function returns.</summary>
			BLOCK = 0x0001,
			/// <summary>The calling thread is not prevented from processing other requests while waiting for the function to return.</summary>
			NORMAL = 0x0000,
			/// <summary>The function does not enforce the time-out period as long as the receiving thread is processing messages.</summary>
			NOTIMEOUTIFNOTHUNG = 0x0008,
			/// <summary>The function should return 0 if the receiving window is destroyed or its owning thread dies while the message is being processed.</summary>
			ERRORONEXIT = 0x0020
		}

		public const Int32 ICON_SMALL = 0;
		public const Int32 ICON_BIG = 1;

		[DllImport("user32.dll", CharSet = CharSet.Auto,SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, WM msg, IntPtr wParam, IntPtr lParam);

		/// <summary>Sends the specified message to one or more windows</summary>
		/// <param name="hWnd">
		/// A handle to the window whose window procedure will receive the message.
		/// If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including disabled or invisible unowned windows.
		/// The function does not return until each window has timed out.
		/// Therefore, the total wait time can be up to the value of uTimeout multiplied by the number of top-level windows.
		/// </param>
		/// <remarks>
		/// The function calls the window procedure for the specified window and, if the specified window belongs to a different thread,
		/// does not return until the window procedure has processed the message or the specified time-out period has elapsed.
		/// If the window receiving the message belongs to the same queue as the current thread,
		/// the window procedure is called directly—the time-out value is ignored.
		/// </remarks>
		/// <param name="Msg">The message to be sent.</param>
		/// <param name="wParam">Any additional message-specific information.</param>
		/// <param name="lParam">Any additional message-specific information.</param>
		/// <param name="fuFlags">The behavior of this function.</param>
		/// <param name="uTimeout">
		/// The duration of the time-out period, in milliseconds.
		/// If the message is a broadcast message, each window can use the full time-out period.
		/// For example, if you specify a five second time-out period and there are three top-level windows that fail to process the message, you could have up to a 15 second delay.
		/// </param>
		/// <param name="lpdwResult">The result of the message processing. The value of this parameter depends on the message that is specified.</param>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// SendMessageTimeout does not provide information about individual windows timing out if HWND_BROADCAST is used.
		/// </returns>
		[DllImport("User32.dll",SetLastError = true)]
		public static extern Int32 SendMessageTimeout(IntPtr hWnd,
			WM Msg,
			Int32 wParam,
			Int32 lParam,
			SMTO fuFlags,
			UInt32 uTimeout,
			out IntPtr lpdwResult);

		[DllImport("user32.dll", EntryPoint = "SendMessageTimeout", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern UInt32 SendMessageTimeoutText(IntPtr hWnd,
			WM Msg,
			Int32 countOfChars,
			StringBuilder text,
			SMTO fuFlags,
			UInt32 uTimeout,
			out IntPtr lpdwResult);

		/// <summary>Retrieves a handle to the window that contains the specified point</summary>
		/// <remarks>
		/// The WindowFromPoint function does not retrieve a handle to a hidden or disabled window, even if the point is within the window.
		/// An application should use the ChildWindowFromPoint function for a nonrestrictive search.
		/// </remarks>
		/// <param name="point">The point to be checked</param>
		/// <returns>
		/// The return value is a handle to the window that contains the point.
		/// If no window exists at the given point, the return value is NULL.
		/// If the point is over a static text control, the return value is a handle to the window under the static text control.
		/// </returns>
		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(Point point);

		/// <summary>To retrieve a value from the extra class memory</summary>
		public enum GCL
		{
			/// <summary>
			/// Retrieves an ATOM value that uniquely identifies the window class.
			/// This is the same atom that the RegisterClassEx function returns.
			/// </summary>
			GCW_ATOM = (-32),
			/// <summary>Retrieves the size, in bytes, of the extra memory associated with the class.</summary>
			GCL_CBCLSEXTRA = (-20),
			/// <summary>
			/// Retrieves the size, in bytes, of the extra window memory associated with each window in the class.
			/// For information on how to access this memory, see <see cref="GetWindowLongPtr"/>.
			/// </summary>
			GCL_CBWNDEXTRA = (-18),
			/// <summary>Retrieves a handle to the background brush associated with the class.</summary>
			GCLP_HBRBACKGROUND = (-10),
			/// <summary>Retrieves a handle to the cursor associated with the class.</summary>
			GCLP_HCURSOR = (-12),
			/// <summary>Retrieves a handle to the icon associated with the class.</summary>
			GCLP_HICON = (-14),
			/// <summary>Retrieves a handle to the small icon associated with the class.</summary>
			GCLP_HICONSM = (-34),
			/// <summary>Retrieves a handle to the module that registered the class.</summary>
			GCLP_HMODULE = (-16),
			/// <summary>
			/// Retrieves the pointer to the menu name string.
			/// The string identifies the menu resource associated with the class.
			/// </summary>
			GCLP_MENUNAME = (-8),
			/// <summary>Retrieves the window-class style bits.</summary>
			GCL_STYLE = (-26),
			/// <summary>
			/// Retrieves the address of the window procedure, or a handle representing the address of the window procedure.
			/// You must use the CallWindowProc function to call the window procedure.
			/// </summary>
			GCLP_WNDPROC = (-24),
		}

		/// <summary>Retrieves the specified value from the WNDCLASSEX structure associated with the specified window.</summary>
		/// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
		/// <param name="nIndex">
		/// The value to be retrieved.
		/// To retrieve a value from the extra class memory, specify the positive, zero-based byte offset of the value to be retrieved.
		/// Valid values are in the range zero through the number of bytes of extra class memory, minus eight;
		/// for example, if you specified 24 or more bytes of extra class memory, a value of 16 would be an index to the third integer.
		/// </param>
		/// <returns>If the function succeeds, the return value is the requested value.</returns>
		public static IntPtr GetClassLong(IntPtr hWnd, GCL nIndex)
			=> IntPtr.Size > 4
				? GetClassLongPtr64(hWnd, nIndex)
				: new IntPtr(GetClassLongPtr32(hWnd, nIndex));

		[DllImport("user32.dll", EntryPoint = "GetClassLong")]
		private static extern UInt32 GetClassLongPtr32(IntPtr hWnd, GCL nIndex);

		[DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
		private static extern IntPtr GetClassLongPtr64(IntPtr hWnd, GCL nIndex);

		/// <summary>The ScreenToClient function converts the screen coordinates of a specified point on the screen to client-area coordinates.</summary>
		/// <param name="handle">A handle to the window whose client area will be used for the conversion.</param>
		/// <param name="point">A pointer to a POINT structure that specifies the screen coordinates to be converted.</param>
		/// <returns>If the function succeeds, the return value is nonzero.</returns>
		[DllImport("user32.dll")]
		public static extern Boolean ScreenToClient(IntPtr handle, ref Point point);

		[DllImport("user32.dll")]
		public static extern IntPtr ChildWindowFromPointEx(IntPtr hWndParent, Point pt, UInt32 uFlags);

		[DllImport("user32.dll")]
		public static extern Boolean ClientToScreen(IntPtr hwnd, ref Point lpPoint);

		[DllImport("user32.dll")]
		public static extern Boolean IsChild(IntPtr hWndParent, IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr GetParent(IntPtr hWnd);

		/// <summary>The relationship between the specified window and the window whose handle is to be retrieved.</summary>
		public enum GW : UInt32
		{
			/// <summary>The retrieved handle identifies the window of the same type that is highest in the Z order.</summary>
			/// <remarks>
			/// If the specified window is a topmost window, the handle identifies a topmost window.
			/// If the specified window is a top-level window, the handle identifies a top-level window.
			/// If the specified window is a child window, the handle identifies a sibling window.
			/// </remarks>
			HWNDFIRST = 0,
			/// <summary>The retrieved handle identifies the window of the same type that is lowest in the Z order.</summary>
			/// <remarks>
			/// If the specified window is a topmost window, the handle identifies a topmost window.
			/// If the specified window is a top-level window, the handle identifies a top-level window.
			/// If the specified window is a child window, the handle identifies a sibling window.
			/// </remarks>
			HWNDLAST = 1,
			/// <summary>The retrieved handle identifies the window below the specified window in the Z order.</summary>
			/// <remarks>
			/// If the specified window is a topmost window, the handle identifies a topmost window.
			/// If the specified window is a top-level window, the handle identifies a top-level window.
			/// If the specified window is a child window, the handle identifies a sibling window.
			/// </remarks>
			HWNDNEXT = 2,
			/// <summary>The retrieved handle identifies the window above the specified window in the Z order.</summary>
			/// <remarks>
			/// If the specified window is a topmost window, the handle identifies a topmost window.
			/// If the specified window is a top-level window, the handle identifies a top-level window.
			/// If the specified window is a child window, the handle identifies a sibling window.
			/// </remarks>
			HWNDPREV = 3,
			/// <summary>The retrieved handle identifies the specified window's owner window, if any.</summary>
			OWNER = 4,
			/// <summary>
			/// The retrieved handle identifies the child window at the top of the Z order, if the specified window is a parent window; otherwise, the retrieved handle is NULL.
			/// </summary>
			/// <remarks>
			/// The function examines only child windows of the specified window.
			/// It does not examine descendant windows.
			/// </remarks>
			CHILD = 5,
			/// <summary>
			/// The retrieved handle identifies the enabled popup window owned by the specified window
			/// (the search uses the first such window found using GW_HWNDNEXT);
			/// otherwise, if there are no enabled popup windows, the retrieved handle is that of the specified window.
			/// </summary>
			ENABLEDPOPUP = 6
		}

		/// <summary>Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified window</summary>
		/// <param name="hWnd">
		/// A handle to a window.
		/// The window handle retrieved is relative to this window, based on the value of the uCmd parameter.
		/// </param>
		/// <param name="uCmd">
		/// The relationship between the specified window and the window whose handle is to be retrieved.
		/// This parameter can be one of the following values.
		/// </param>
		/// <returns>If the function succeeds, the return value is a window handle.</returns>
		[DllImport("user32.dll",SetLastError = true)]
		public static extern IntPtr GetWindow(IntPtr hWnd, GW uCmd);

		public enum WindowLongFlags
		{
			/// <summary>Sets a new extended window style.</summary>
			GWL_EXSTYLE = -20,
			/// <summary>Sets a new application instance handle.</summary>
			GWLP_HINSTANCE = -6,
			GWLP_HWNDPARENT = -8,
			/// <summary>Sets a new identifier of the child window. The window cannot be a top-level window.</summary>
			GWL_ID = -12,
			/// <summary>Sets a new window style.</summary>
			GWL_STYLE = -16,
			/// <summary>
			/// Sets the user data associated with the window.
			/// This data is intended for use by the application that created the window.
			/// Its value is initially zero.
			/// </summary>
			GWL_USERDATA = -21,
			/// <summary>Sets a new address for the window procedure.</summary>
			GWL_WNDPROC = -4,
			DWLP_USER = 0x8,
			DWLP_MSGRESULT = 0x0,
			DWLP_DLGPROC = 0x4
		}

		/// <summary>The following are the window styles</summary>
		/// <remarks>After the window has been created, these styles cannot be modified, except as noted.</remarks>
		[Flags]
		public enum WS : Int64
		{
			/// <summary>The window has a thin-line border.</summary>
			BORDER = 0x00800000L,
			/// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
			CAPTION = 0x00C00000L,
			/// <summary>
			/// The window is a child window.
			/// A window with this style cannot have a menu bar.
			/// </summary>
			/// <remarks>This style cannot be used with the WS_POPUP style.</remarks>
			CHILD = 0x40000000L,
			/// <summary>Same as the WS_CHILD style</summary>
			CHILDWINDOW = 0x40000000L,
			/// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window</summary>
			/// <remarks>This style is used when creating the parent window</remarks>
			CLIPCHILDREN = 0x02000000L,
			/// <summary>
			/// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message,
			/// the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
			/// </summary>
			/// <remarks>
			/// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
			/// </remarks>
			CLIPSIBLINGS = 0x04000000L,
			/// <summary>The window is initially disabled.</summary>
			/// <remarks>
			/// A disabled window cannot receive input from the user.
			/// To change this after a window has been created, use the EnableWindow function.
			/// </remarks>
			DISABLED = 0x08000000L,
			/// <summary>The window has a border of a style typically used with dialog boxes.</summary>
			/// <remarks>A window with this style cannot have a title bar.</remarks>
			DLGFRAME = 0x00400000L,
			/// <summary>
			/// The window is the first control of a group of controls.
			/// The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
			/// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group.
			/// The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
			/// </summary>
			/// <remarks>
			/// You can turn this style on and off to change dialog box navigation.
			/// To change this style after a window has been created, use the SetWindowLong function.
			/// </remarks>
			GROUP = 0x00020000L,
			/// <summary>The window has a horizontal scroll bar.</summary>
			HSCROLL = 0x00100000L,
			/// <summary>The window is initially minimized.</summary>
			/// <remarks>Same as the WS_MINIMIZE style.</remarks>
			ICONIC = 0x20000000L,
			/// <summary>The window is initially maximized.</summary>
			MAXIMIZE = 0x01000000L,
			/// <summary>The window has a maximize button.</summary>
			/// <remarks>
			/// Cannot be combined with the WS_EX_CONTEXTHELP style.
			/// The WS_SYSMENU style must also be specified.
			/// </remarks>
			MAXIMIZEBOX = 0x00010000L,
			/// <summary>The window is initially minimized</summary>
			/// <remarks>Same as the WS_ICONIC style</remarks>
			MINIMIZE = 0x20000000L,
			/// <summary>The window has a minimize button</summary>
			/// <remarks>
			/// Cannot be combined with the WS_EX_CONTEXTHELP style.
			/// The WS_SYSMENU style must also be specified.
			/// </remarks>
			MINIMIZEBOX = 0x00020000L,
			/// <summary>The window is an overlapped window</summary>
			/// <remarks>
			/// An overlapped window has a title bar and a border.
			/// Same as the WS_TILED style.
			/// </remarks>
			OVERLAPPED = 0x00000000L,
			/// <summary>The window is an overlapped window.</summary>
			/// <remarks>Same as the WS_TILEDWINDOW style.</remarks>
			OVERLAPPEDWINDOW = OVERLAPPED| CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
			/// <summary>The window is a pop-up window.</summary>
			/// <remarks>This style cannot be used with the WS_CHILD style.</remarks>
			POPUP = 0x80000000L,
			/// <summary>The window is a pop-up window.</summary>
			/// <remarks>The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</remarks>
			POPUPWINDOW = POPUP | BORDER | SYSMENU,
			/// <summary>The window has a sizing border.</summary>
			/// <remarks>Same as the WS_THICKFRAME style.</remarks>
			SIZEBOX = 0x00040000L,
			/// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
			SYSMENU = 0x00080000L,
			/// <summary>
			/// The window is a control that can receive the keyboard focus when the user presses the TAB key.
			/// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.
			/// 
			/// You can turn this style on and off to change dialog box navigation.
			/// To change this style after a window has been created, use the SetWindowLong function.
			/// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
			/// </summary>
			TABSTOP =0x00010000L,
			/// <summary>The window has a sizing border.</summary>
			/// <remarks>Same as the WS_SIZEBOX style.</remarks>
			THICKFRAME = 0x00040000L,
			/// <summary>
			/// The window is an overlapped window.
			/// An overlapped window has a title bar and a border.
			/// </summary>
			/// <remarks>Same as the WS_OVERLAPPED style.</remarks>
			TILED = 0x00000000L,
			/// <summary>The window is an overlapped window.</summary>
			/// <remarks>Same as the WS_OVERLAPPEDWINDOW style.</remarks>
			TILEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | THICKFRAME | MINIMIZEBOX | MAXIMIZEBOX,
			/// <summary>The window is initially visible.</summary>
			/// <remarks>This style can be turned on and off by using the ShowWindow or SetWindowPos function.</remarks>
			VISIBLE = 0x10000000L,
			/// <summary>The window has a vertical scroll bar</summary>
			VSCROLL = 0x00200000L,
		}

		/// <summary>The following are the extended window styles</summary>
		[Flags]
		public enum WS_EX : Int64
		{
			/// <summary>The window accepts drag-drop files</summary>
			ACCEPTFILES = 0x00000010L,

			/// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
			APPWINDOW = 0x00040000L,

			/// <summary>The window has a border with a sunken edge.</summary>
			CLIENTEDGE = 0x00000200L,

			/// <summary>
			/// Paints all descendants of a window in bottom-to-top painting order using double-buffering.
			/// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects, but only if the descendent window also has the <see cref="WS_EX.TRANSPARENT"/> bit set.
			/// </summary>
			/// <remarks>
			/// Double-buffering allows the window and its descendants to be painted without flicker.
			/// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
			/// </remarks>
			COMPOSITED = 0x02000000L,

			/// <summary>
			/// The title bar of the window includes a question mark.
			/// When the user clicks the question mark, the cursor changes to a question mark with a pointer.
			/// </summary>
			/// <remarks>
			/// If the user then clicks a child window, the child receives a WM_HELP message.
			/// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
			/// The Help application displays a pop-up window that typically contains help for the child window.
			/// <see cref="WS_EX.CONTEXTHELP"/> cannot be used with the <see cref="WS.MAXIMIZEBOX"/> or <see cref="WS.MINIMIZEBOX"/> styles.
			/// </remarks>
			CONTEXTHELP = 0x00000400L,

			/// <summary>
			/// The window itself contains child windows that should take part in dialog box navigation.
			/// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations such as handling the TAB key, an arrow key, or a keyboard mnemonic.
			/// </summary>
			CONTROLPARENT = 0x00010000L,

			/// <summary>The window has a double border;</summary>
			/// <remarks>the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.</remarks>
			DLGMODALFRAME = 0x00000001L,

			/// <summary>The window is a layered window</summary>
			/// <remarks>This style cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.</remarks>
			LAYERED = 0x00080000,

			/// <summary>
			/// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the horizontal origin of the window is on the right edge.
			/// Increasing horizontal values advance to the left.
			/// </summary>
			LAYOUTRTL = 0x00400000L,

			/// <summary>The window has generic left-aligned properties. This is the default.</summary>
			LEFT = 0x00000000L,

			/// <summary>
			/// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area.
			/// </summary>
			/// <remarks>For other languages, the style is ignored.</remarks>
			LEFTSCROLLBAR = 0x00004000L,

			/// <summary>
			/// The window text is displayed using left-to-right reading-order properties. This is the default.
			/// </summary>
			LTRREADING = 0x00000000L,

			/// <summary>The window is a MDI child window.</summary>
			MDICHILD = 0x00000040L,

			/// <summary>
			/// A top-level window created with this style does not become the foreground window when the user clicks it.
			/// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
			/// The window should not be activated through programmatic access or via keyboard navigation by accessible technology, such as Narrator.
			/// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
			/// </summary>
			/// <remarks>
			/// The window does not appear on the taskbar by default.
			/// To force the window to appear on the taskbar, use the <see cref="WS_EX.APPWINDOW"/> style
			/// </remarks>
			NOACTIVATE = 0x08000000L,

			/// <summary>The window does not pass its window layout to its child windows.</summary>
			NOINHERITLAYOUT = 0x00100000L,

			/// <summary>The child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.</summary>
			NOPARENTNOTIFY = 0x00000004L,

			/// <summary>The window does not render to a redirection surface.</summary>
			/// <remarks>This is for windows that do not have visible content or that use mechanisms other than surfaces to provide their visual.</remarks>
			NOREDIRECTIONBITMAP = 0x00200000L,

			/// <summary>The window is an overlapped window.</summary>
			OVERLAPPEDWINDOW = WINDOWEDGE | CLIENTEDGE,

			/// <summary>The window is palette window, which is a modeless dialog box that presents an array of commands.</summary>
			PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST,

			/// <summary>
			/// The window has generic "right-aligned" properties. This depends on the window class.
			/// </summary>
			/// <remarks>
			/// This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored.
			/// Using the <see cref="WS_EX.RIGHT"/> style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
			/// </remarks>
			RIGHT = 0x00001000L,

			/// <summary>The vertical scroll bar (if present) is to the right of the client area. This is the default.</summary>
			RIGHTSCROLLBAR = 0x00000000L,

			/// <summary>If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties.</summary>
			/// <remarks>For other languages, the style is ignored.</remarks>
			RTLREADING = 0x00002000L,

			/// <summary>The window has a three-dimensional border style intended to be used for items that do not accept user input.</summary>
			STATICEDGE = 0x00020000L,

			/// <summary>
			/// The window is intended to be used as a floating toolbar.
			/// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
			/// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
			/// If a tool window has a system menu, its icon is not displayed on the title bar.
			/// However, you can display the system menu by right-clicking or by typing ALT+SPACE.
			/// </summary>
			TOOLWINDOW = 0x00000080L,

			/// <summary>
			/// The window should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
			/// To add or remove this style, use the SetWindowPos function.
			/// </summary>
			TOPMOST = 0x00000008L,

			/// <summary>
			/// The window should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
			/// The window appears transparent because the bits of underlying sibling windows have already been painted.
			/// To achieve transparency without these restrictions, use the SetWindowRgn function.
			/// </summary>
			TRANSPARENT = 0x00000020L,

			/// <summary>The window has a border with a raised edge.</summary>
			WINDOWEDGE = 0x00000100L
		}

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowLongPtr(IntPtr hWnd, WindowLongFlags nIndex);

		/// <summary>
		/// Changes an attribute of the specified window.
		/// The function also sets a value at the specified offset in the extra window memory.
		/// </summary>
		/// <param name="hWnd">
		/// A handle to the window and, indirectly, the class to which the window belongs.
		/// The SetWindowLongPtr function fails if the process that owns the window specified by the hWnd parameter is at a higher process privilege in the UIPI hierarchy than the process the calling thread resides in.
		/// </param>
		/// <param name="nIndex">The zero-based offset to the value to be set</param>
		/// <param name="dwNewLong">The replacement value.</param>
		/// <returns>If the function succeeds, the return value is the previous value of the specified offset.</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetWindowLongPtr(IntPtr hWnd, WindowLongFlags nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern Boolean GetWindowRect(IntPtr hWnd, ref RECT lpRect);

		[Flags]
		public enum SWP : UInt32
		{
			/// <summary>If the calling thread and the thread that owns the window are attached to different input queues, the system posts the request to the thread that owns the window.</summary>
			/// <remarks>This prevents the calling thread from blocking its execution while other threads process the request.</remarks>
			ASYNCWINDOWPOS = 0x4000,
			/// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
			DEFERERASE = 0x2000,
			/// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
			DRAWFRAME = 0x0020,
			/// <summary>Applies new frame styles set using the SetWindowLong function.</summary>
			/// <remarks>
			/// Sends a WM_NCCALCSIZE message to the window, even if the window's size is not being changed.
			/// If this flag is not specified, WM_NCCALCSIZE is sent only when the window's size is being changed.
			/// </remarks>
			FRAMECHANGED = 0x0020,
			/// <summary>Hides the window.</summary>
			HIDEWINDOW = 0x0080,
			/// <summary>Does not activate the window.</summary>
			/// <remarks>If this flag is not set, the window is activated and moved to the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter parameter).</remarks>
			NOACTIVATE = 0x0010,
			/// <summary>Discards the entire contents of the client area.</summary>
			/// <remarks>If this flag is not specified, the valid contents of the client area are saved and copied back into the client area after the window is sized or repositioned.</remarks>
			NOCOPYBITS = 0x0100,
			/// <summary>Retains the current position (ignores X and Y parameters).</summary>
			NOMOVE = 0x0002,
			/// <summary>Does not change the owner window's position in the Z order.</summary>
			NOOWNERZORDER = 0x0200,
			/// <summary>Does not redraw changes</summary>
			/// <remarks>
			/// If this flag is set, no repainting of any kind occurs.
			/// This applies to the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent window uncovered as a result of the window being moved.
			/// When this flag is set, the application must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
			/// </remarks>
			NOREDRAW = 0x0008,
			/// <summary>Same as the <see cref="SWP.NOOWNERZORDER"/> flag.</summary>
			NOREPOSITION = 0x0200,
			/// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
			NOSENDCHANGING = 0x0400,
			/// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
			NOSIZE = 0x0001,
			/// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
			NOZORDER = 0x0004,
			/// <summary>Displays the window.</summary>
			SHOWWINDOW = 0x0040,
		}

		public enum InsertAfter
		{
			TOPMOST = -1,
			NOTOPMOST = -2,
			TOP = 0,
			BOTTOM = 1,
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern Boolean SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, SWP uFlags);

		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowDC(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern Int32 ReleaseDC(IntPtr hWnd, IntPtr hdc);

		[DllImport("gdi32.dll")]
		public static extern IntPtr CreateDC(IntPtr lpszDriver, String lpszDevice, IntPtr lpszOutput, IntPtr lpInitData);

		[DllImport("gdi32.dll")]
		public static extern IntPtr DeleteDC(IntPtr hdc);

		[DllImport("user32.dll")]
		private static extern Int32 GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern Int32 GetClassName(IntPtr hWnd, StringBuilder lpClassName, Int32 nMaxCount);

		/// <summary>Retrieves the full path and file name of the module associated with the specified window handle.</summary>
		/// <param name="hWnd">A handle to the window whose module file name is to be retrieved.</param>
		/// <param name="lpString">The path and file name.</param>
		/// <param name="nMaxCount">The maximum number of characters that can be copied into the lpszFileName buffer.</param>
		/// <returns>The return value is the total number of characters copied into the buffer.</returns>
		[DllImport("user32.dll")]
		private static extern Int32 GetWindowModuleFileName(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

		public static String GetClassName(IntPtr hWnd)
		{
			Debug.Assert(hWnd != IntPtr.Zero);

			StringBuilder result = new StringBuilder(256);
			Native.Window.GetClassName(hWnd, result, result.Capacity);
			return result.ToString();
		}

		public static String GetWindowModuleFileName(IntPtr hWnd)
		{
			Debug.Assert(hWnd != IntPtr.Zero);

			StringBuilder result = new StringBuilder(256);
			Native.Window.GetWindowModuleFileName(hWnd, result, result.Capacity);
			return result.ToString();
		}

		// helper function return directly a Rectangle object
		public static Rectangle GetWindowRect(IntPtr hWnd)
		{
			Debug.Assert(hWnd != IntPtr.Zero);

			RECT rect = new RECT();
			if(!Window.GetWindowRect(hWnd, ref rect))
				throw new Win32Exception();
			return rect.ToRectangle();
		}

		public static void SetWindowPos(IntPtr hWnd,InsertAfter insertAfter, Rectangle rect, SWP uFlags)
		{
			Debug.Assert(hWnd != IntPtr.Zero);

			IntPtr hwndInsertAfter = new IntPtr((Int32)insertAfter);
			if(!Window.SetWindowPos(hWnd, hwndInsertAfter, rect.X, rect.Y, rect.Width, rect.Height, uFlags))
				throw new Win32Exception();
		}

		public static String GetWindowText(IntPtr hWnd)
		{
			Debug.Assert(hWnd != IntPtr.Zero);

			StringBuilder result = new StringBuilder(Window.GetWindowTextLength(hWnd) + 1);
			Window.GetWindowText(hWnd, result, result.Capacity);
			return result.ToString();
		}

		[DllImport("user32.dll")]
		public static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);
	}
}