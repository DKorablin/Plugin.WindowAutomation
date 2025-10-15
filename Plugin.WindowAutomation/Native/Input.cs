using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace Plugin.WindowAutomation.Native
{
	internal static class Input
	{
		/// <summary>Тип нажатия</summary>
		[Flags]
		public enum ClickFlags
		{
			/// <summary>Нажать на кнопку</summary>
			Down = 0x1,
			/// <summary>Отжать кнопку</summary>
			Up = 0x2,
		}

		/// <summary>Specifies the type of the input event.</summary>
		public enum InputType : UInt32
		{
			/// <summary>The event is a mouse event. Use the mi structure of the union.</summary>
			Mouse = 0,
			/// <summary>The event is a keyboard event. Use the ki structure of the union.</summary>
			Keyboard = 1,
			/// <summary>Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.</summary>
			Hardware = 2,
		}

		/// <summary>
		/// The INPUT structure is used by SendInput to store information for synthesizing input events such as keystrokes, mouse movement, and mouse clicks. (see: http://msdn.microsoft.com/en-us/library/ms646270(VS.85).aspx)
		/// Declared in Winuser.h, include Windows.h
		/// </summary>
		/// <remarks>
		/// This structure contains information identical to that used in the parameter list of the keybd_event or mouse_event function.
		/// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard input methods, such as handwriting recognition or voice recognition, as if it were text input by using the KEYEVENTF_UNICODE flag. For more information, see the remarks section of KEYBDINPUT.
		/// </remarks>
		public struct INPUT
		{
			/// <summary>
			/// Specifies the type of the input event. This member can be one of the following values. 
			/// <see cref="InputType.Mouse"/> - The event is a mouse event. Use the mi structure of the union.
			/// <see cref="InputType.Keyboard"/> - The event is a keyboard event. Use the ki structure of the union.
			/// <see cref="InputType.Hardware"/> - Windows 95/98/Me: The event is from input hardware other than a keyboard or mouse. Use the hi structure of the union.
			/// </summary>
			public InputType Type;
			public INPUT_Union Union;

			public INPUT(Keys key, ClickFlags click)
			{
				this.Type = InputType.Keyboard;
				this.Union = new INPUT_Union
				{
					Keyboard = new KEYBDINPUT()
					{
						KeyCode = (UInt16)key,
						Scan = (UInt16)(Input.MapVirtualKey((UInt32)key, MAPVK.VK_TO_VSC) & 0xFFU),
						time = 0,
						dwExtraInfo = UIntPtr.Zero,
					}
				};

				switch(click)
				{
				case ClickFlags.Down:
					this.Union.Keyboard.dwFlags = Input.IsExtendedKey(key)
						? KEYBDINPUT.KEYEVENTF.ExtendedKey
						: KEYBDINPUT.KEYEVENTF.None;
					break;
				case ClickFlags.Up:
					this.Union.Keyboard.dwFlags = Input.IsExtendedKey(key)
						? KEYBDINPUT.KEYEVENTF.KeyUp | KEYBDINPUT.KEYEVENTF.ExtendedKey
						: KEYBDINPUT.KEYEVENTF.KeyUp;
					break;
				}
			}

			public INPUT(MouseButtons button, ClickFlags click)
			{
				this.Type = InputType.Mouse;
				this.Union = new INPUT_Union();
				switch(button)
				{
				case MouseButtons.Left:
					this.Union.Mouse.dwFlags = click == ClickFlags.Down
						? MOUSEINPUT.MOUSEEVENTF.LeftDown
						: MOUSEINPUT.MOUSEEVENTF.LeftUp;
					break;
				case MouseButtons.Middle:
					this.Union.Mouse.dwFlags = click == ClickFlags.Down
						? MOUSEINPUT.MOUSEEVENTF.MiddleDown
						: MOUSEINPUT.MOUSEEVENTF.MiddleUp;
					break;
				case MouseButtons.Right:
					this.Union.Mouse.dwFlags = click == ClickFlags.Down
						? MOUSEINPUT.MOUSEEVENTF.RightDown
						: MOUSEINPUT.MOUSEEVENTF.RightUp;
					break;
				default:
					throw new NotImplementedException();
				}
			}

			/// <summary>Move the mouse to an absolute position.</summary>
			/// <param name="absoluteX"></param>
			/// <param name="absoluteY"></param>
			public static INPUT MoveMouse(Int32 absoluteX, Int32 absoluteY)
			{
				INPUT input = new INPUT() { Type = InputType.Mouse };
				input.Union.Mouse.dwFlags = MOUSEINPUT.MOUSEEVENTF.Move | MOUSEINPUT.MOUSEEVENTF.Absolute;
				input.Union.Mouse.x = absoluteX;
				input.Union.Mouse.y = absoluteY;

				return input;
			}
		}

		[StructLayout(LayoutKind.Explicit)]
		public struct INPUT_Union
		{
			[FieldOffset(0)]
			public MOUSEINPUT Mouse;
			[FieldOffset(0)]
			public KEYBDINPUT Keyboard;
			[FieldOffset(0)]
			public HARDWAREINPUT Hardware;
		}

		/// <summary>
		/// The MOUSEINPUT structure contains information about a simulated mouse event.
		/// Declared in Winuser.h, include Windows.h
		/// </summary>
		/// <see>http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx</see>
		/// <remarks>
		/// If the mouse has moved, indicated by MOUSEEVENTF_MOVE, dx and dy specify information about that movement. The information is specified as absolute or relative integer values. 
		/// If MOUSEEVENTF_ABSOLUTE value is specified, dx and dy contain normalized absolute coordinates between 0 and 65,535. The event procedure maps these coordinates onto the display surface. Coordinate (0,0) maps onto the upper-left corner of the display surface; coordinate (65535,65535) maps onto the lower-right corner. In a multimonitor system, the coordinates map to the primary monitor. 
		/// Windows 2000/XP: If MOUSEEVENTF_VIRTUALDESK is specified, the coordinates map to the entire virtual desktop.
		/// If the MOUSEEVENTF_ABSOLUTE value is not specified, dx and dy specify movement relative to the previous mouse event (the last reported position). Positive values mean the mouse moved right (or down); negative values mean the mouse moved left (or up). 
		/// Relative mouse motion is subject to the effects of the mouse speed and the two-mouse threshold values. A user sets these three values with the Pointer Speed slider of the Control Panel's Mouse Properties sheet. You can obtain and set these values using the SystemParametersInfo function. 
		/// The system applies two tests to the specified relative mouse movement. If the specified distance along either the x or y axis is greater than the first mouse threshold value, and the mouse speed is not zero, the system doubles the distance. If the specified distance along either the x or y axis is greater than the second mouse threshold value, and the mouse speed is equal to two, the system doubles the distance that resulted from applying the first threshold test. It is thus possible for the system to multiply specified relative mouse movement along the x or y axis by up to four times.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEINPUT
			{
				/// <summary>The set of MouseFlags for use in the Flags property of the <see cref="MOUSEINPUT"/> structure.</summary>
				/// <see>http://msdn.microsoft.com/en-us/library/ms646273(VS.85).aspx</see>
				[Flags]
				public enum MOUSEEVENTF : UInt32
				{
					/// <summary>Specifies that movement occurred.</summary>
					Move = 0x0001,
					/// <summary>Specifies that the left button was pressed.</summary>
					LeftDown = 0x0002,
					/// <summary>Specifies that the left button was released</summary>
					LeftUp = 0x0004,
					/// <summary>Specifies that the right button was pressed</summary>
					RightDown = 0x0008,
					/// <summary>Specifies that the right button was released</summary>
					RightUp = 0x0010,
					/// <summary>Specifies that the middle button was pressed</summary>
					MiddleDown = 0x0020,
					/// <summary>Specifies that the middle button was released</summary>
					MiddleUp = 0x0040,
					/// <summary>Windows 2000/XP: Specifies that an X button was pressed</summary>
					XDown = 0x0080,
					/// <summary>Windows 2000/XP: Specifies that an X button was released.</summary>
					XUp = 0x0100,
					/// <summary>
					/// Windows NT/2000/XP: Specifies that the wheel was moved, if the mouse has a wheel.
					/// The amount of movement is specified in mouseData
					/// </summary>
					VerticalWheel = 0x0800,
					/// <summary>
					/// Specifies that the wheel was moved horizontally, if the mouse has a wheel.
					/// The amount of movement is specified in mouseData.
					/// Windows 2000/XP:  Not supported.
					/// </summary>
					HorizontalWheel = 0x01000,
					MOVE_NOCOALESCE = 0x2000,
					/// <summary>
					/// Windows 2000/XP: Maps coordinates to the entire desktop.
					/// Must be used with MOUSEEVENTF_ABSOLUTE.
					/// </summary>
					VirtualDesk = 0x4000,
					/// <summary>
					/// Specifies that the dx and dy members contain normalized absolute coordinates.
					/// If the flag is not set, dxand dy contain relative data (the change in position since the last reported position).
					/// This flag can be set, or not set, regardless of what kind of mouse or other pointing device, if any, is connected to the system.
					/// For further information about relative mouse motion, see the following Remarks section.
					/// </summary>
					Absolute = 0x8000,
				}

				/// <summary>
				/// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member.
				/// Absolute data is specified as the x coordinate of the mouse; relative data is specified as the number of pixels moved.
				/// </summary>
				public Int32 x;
				/// <summary>
				/// Specifies the absolute position of the mouse, or the amount of motion since the last mouse event was generated, depending on the value of the dwFlags member.
				/// Absolute data is specified as the y coordinate of the mouse; relative data is specified as the number of pixels moved.
				/// </summary>
				public Int32 y;
				/// <summary>
				/// If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated forward, away from the user; a negative value indicates that the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is 120. 
				/// Windows Vista: If dwFlags contains MOUSEEVENTF_HWHEEL, then dwData specifies the amount of wheel movement. A positive value indicates that the wheel was rotated to the right; a negative value indicates that the wheel was rotated to the left. One wheel click is defined as WHEEL_DELTA, which is 120.
				/// Windows 2000/XP: IfdwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData should be zero. 
				/// If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were pressed or released. This value may be any combination of the following flags. 
				/// </summary>
				public Int32 mouseData;
				/// <summary>
				/// A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this member can be any reasonable combination of the following values. 
				/// The bit flags that specify mouse button status are set to indicate changes in status, not ongoing conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN is set when the left button is first pressed, but not for subsequent motions. Similarly, MOUSEEVENTF_LEFTUP is set only when the button is first released. 
				/// You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field. 
				/// </summary>
				public MOUSEEVENTF dwFlags;
				/// <summary>Time stamp for the event, in milliseconds.</summary>
				/// <remarks>If this parameter is 0, the system will provide its own time stamp.</remarks>
				public UInt32 time;
				/// <summary>Specifies an additional value associated with the mouse event.</summary>
				/// <remarks>An application calls GetMessageExtraInfo to obtain this extra information.</remarks>
				public UIntPtr dwExtraInfo;
			}

		/// <summary>
		/// The KEYBDINPUT structure contains information about a simulated keyboard event.  (see: http://msdn.microsoft.com/en-us/library/ms646271(VS.85).aspx)
		/// Declared in Winuser.h, include Windows.h
		/// </summary>
		/// <remarks>
		/// Windows 2000/XP: INPUT_KEYBOARD supports nonkeyboard-input methodssuch as handwriting recognition or voice recognitionas if it were text input by using the KEYEVENTF_UNICODE flag. If KEYEVENTF_UNICODE is specified, SendInput sends a WM_KEYDOWN or WM_KEYUP message to the foreground thread's message queue with wParam equal to VK_PACKET. Once GetMessage or PeekMessage obtains this message, passing the message to TranslateMessage posts a WM_CHAR message with the Unicode character originally specified by wScan. This Unicode character will automatically be converted to the appropriate ANSI value if it is posted to an ANSI window.
		/// Windows 2000/XP: Set the KEYEVENTF_SCANCODE flag to define keyboard input in terms of the scan code. This is useful to simulate a physical keystroke regardless of which keyboard is currently being used. The virtual key value of a key may alter depending on the current keyboard layout or what other keys were pressed, but the scan code will always be the same.
		/// </remarks>
		[StructLayout(LayoutKind.Sequential)]
		public struct KEYBDINPUT
			{
				[Flags]
				public enum KEYEVENTF : UInt32
				{
					None=0x0000,
					/// <summary>If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).</summary>
					ExtendedKey = 0x0001,

					/// <summary>If specified, the key is being released. If not specified, the key is being pressed.</summary>
					KeyUp = 0x0002,

					/// <summary>If specified, wScan identifies the key and wVk is ignored.</summary>
					Unicode = 0x0004,

					/// <summary>
					/// Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke.
					/// The wVk parameter must be zero.
					/// This flag can only be combined with the KEYEVENTF_KEYUP flag.
					/// For more information, see the Remarks section.
					/// </summary>
					ScanCode = 0x0008,
				}

				/// <summary>Specifies a virtual-key code.</summary>
				/// <remarks>
				/// The code must be a value in the range 1 to 254.
				/// The Winuser.h header file provides macro definitions (VK_*) for each value.
				/// If the dwFlags member specifies KEYEVENTF_UNICODE, wVk must be 0.
				/// </remarks>
				public UInt16 KeyCode;
				/// <summary>
				/// Specifies a hardware scan code for the key.
				/// If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode character which is to be sent to the foreground application.
				/// </summary>
				public UInt16 Scan;
				/// <summary>
				/// Specifies various aspects of a keystroke. This member can be certain combinations of the following values.
				/// KEYEVENTF_EXTENDEDKEY - If specified, the scan code was preceded by a prefix byte that has the value 0xE0 (224).
				/// KEYEVENTF_KEYUP - If specified, the key is being released. If not specified, the key is being pressed.
				/// KEYEVENTF_SCANCODE - If specified, wScan identifies the key and wVk is ignored. 
				/// KEYEVENTF_UNICODE - Windows 2000/XP: If specified, the system synthesizes a VK_PACKET keystroke. The wVk parameter must be zero. This flag can only be combined with the KEYEVENTF_KEYUP flag. For more information, see the Remarks section. 
				/// </summary>
				public KEYEVENTF dwFlags;
				/// <summary>Time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its own time stamp.</summary>
				public Int32 time;
				/// <summary>Specifies an additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this information.</summary>
				public UIntPtr dwExtraInfo;
			}

		/// <summary>
		/// The HARDWAREINPUT structure contains information about a simulated message generated by an input device other than a keyboard or mouse.  (see: http://msdn.microsoft.com/en-us/library/ms646269(VS.85).aspx)
		/// Declared in Winuser.h, include Windows.h
		/// </summary>
		public struct HARDWAREINPUT
			{
				/// <summary>Value specifying the message generated by the input hardware.</summary>
				public UInt32 Msg;

				/// <summary>Specifies the low-order word of the lParam parameter for uMsg.</summary>
				public UInt16 ParamL;

				/// <summary>Specifies the high-order word of the lParam parameter for uMsg.</summary>
				public UInt16 ParamH;
			}

		/// <summary>
		/// The SendInput function synthesizes keystrokes, mouse motions, and button clicks.
		/// </summary>
		/// <param name="numberOfInputs">Number of structures in the Inputs array.</param>
		/// <param name="inputs">Pointer to an array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
		/// <param name="sizeOfInputStructure">Specifies the size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function fails.</param>
		/// <returns>The function returns the number of events that it successfully inserted into the keyboard or mouse input stream. If the function returns zero, the input was already blocked by another thread. To get extended error information, call GetLastError.Microsoft Windows Vista. This function fails when it is blocked by User Interface Privilege Isolation (UIPI). Note that neither GetLastError nor the return value will indicate the failure was caused by UIPI blocking.</returns>
		/// <remarks>
		/// Microsoft Windows Vista. This function is subject to UIPI. Applications are permitted to inject input only into applications that are at an equal or lesser integrity level.
		/// The SendInput function inserts the events in the INPUT structures serially into the keyboard or mouse input stream. These events are not interspersed with other keyboard or mouse input events inserted either by the user (with the keyboard or mouse) or by calls to keybd_event, mouse_event, or other calls to SendInput.
		/// This function does not reset the keyboard's current state. Any keys that are already pressed when the function is called might interfere with the events that this function generates. To avoid this problem, check the keyboard's state with the GetAsyncKeyState function and correct as necessary.
		/// </remarks>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern UInt32 SendInput(UInt32 numberOfInputs, INPUT[] inputs, Int32 sizeOfInputStructure);

		/// <summary>The translation to be performed</summary>
		public enum MAPVK : UInt32
		{
			/// <summary>
			/// The uCode parameter is a virtual-key code and is translated into a scan code.
			/// If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned.
			/// If there is no translation, the function returns 0.
			/// </summary>
			VK_TO_VSC = 0,
			/// <summary>
			/// The uCode parameter is a scan code and is translated into a virtual-key code that does not distinguish between left- and right-hand keys.
			/// If there is no translation, the function returns 0.
			/// </summary>
			VSC_TO_VK = 1,
			/// <summary>
			/// The uCode parameter is a virtual-key code and is translated into an unshifted character value in the low order word of the return value.
			/// Dead keys (diacritics) are indicated by setting the top bit of the return value.
			/// If there is no translation, the function returns 0.
			/// </summary>
			VK_TO_CHAR = 2,
			/// <summary>
			/// The uCode parameter is a scan code and is translated into a virtual-key code that distinguishes between left- and right-hand keys.
			/// If there is no translation, the function returns 0.
			/// </summary>
			VSC_TO_VK_EX = 3,
			/// <summary>
			/// Windows Vista and later: The uCode parameter is a virtual-key code and is translated into a scan code.
			/// If it is a virtual-key code that does not distinguish between left- and right-hand keys, the left-hand scan code is returned.
			/// If the scan code is an extended scan code, the high byte of the uCode value can contain either 0xe0 or 0xe1 to specify the extended scan code.
			/// If there is no translation, the function returns 0.
			/// </summary>
			VK_TO_VSC_EX = 4,
		}

		/// <summary>
		/// Translates (maps) a virtual-key code into a scan code or character value, or translates a scan code into a virtual-key code.
		/// </summary>
		/// <param name="uCode">The virtual key code or scan code for a key. How this value is interpreted depends on the value of the uMapType parameter.</param>
		/// <param name="uMapType">The translation to be performed</param>
		/// <returns>
		/// The return value is either a scan code, a virtual-key code, or a character value, depending on the value of uCode and uMapType.
		/// If there is no translation, the return value is zero.
		/// </returns>
		[DllImport("user32.dll")]
		public static extern UInt32 MapVirtualKey(UInt32 uCode, MAPVK uMapType);

		/// <summary>
		/// Translates a character to the corresponding virtual-key code and shift state.
		/// The function translates the character using the input language and physical keyboard layout identified by the input locale identifier.
		/// </summary>
		/// <param name="ch">The character to be translated into a virtual-key code.</param>
		/// <param name="dwhkl">
		/// Input locale identifier used to translate the character.
		/// This parameter can be any input locale identifier previously returned by the LoadKeyboardLayout function.
		/// </param>
		/// <returns>If the function succeeds, the low-order byte of the return value contains the virtual-key code and the high-order byte contains the shift state, which can be a combination of the following flag bits.</returns>
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern UInt16 VkKeyScanExW(Char ch, IntPtr dwhkl);

		/// <summary>Unloads an input locale identifier (formerly called a keyboard layout).</summary>
		/// <param name="hkl">The input locale identifier to be unloaded.</param>
		/// <remarks>The input locale identifier is a broader concept than a keyboard layout, since it can also encompass a speech-to-text converter, an Input Method Editor (IME), or any other form of input.</remarks>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero.The function can fail for the following reasons:
		/// - An invalid input locale identifier was passed.
		/// - The input locale identifier was preloaded.
		/// - The input locale identifier is in use.
		/// </returns>
		[DllImport("user32.dll")]
		public static extern Boolean UnloadKeyboardLayout(IntPtr hkl);

		/// <summary>Specifies how the input locale identifier is to be loaded</summary>
		public enum KLF : UInt32
		{
			/// <summary>If the specified input locale identifier is not already loaded, the function loads and activates the input locale identifier for the system</summary>
			ACTIVATE = 0x00000001,
			/// <summary>In this scenario, the last input locale identifier is set for the entire system.</summary>
			NOTELLSHELL = 0x00000080,
			/// <summary>
			/// Moves the specified input locale identifier to the head of the input locale identifier list, making that locale identifier the active locale identifier for the system.
			/// This value reorders the input locale identifier list even if KLF_ACTIVATE is not provided.
			/// </summary>
			REORDER = 0x00000008,
			/// <summary>
			/// If the new input locale identifier has the same language identifier as a current input locale identifier, the new input locale identifier replaces the current one as the input locale identifier for that language.
			/// If this value is not provided and the input locale identifiers have the same language identifiers, the current input locale identifier is not replaced and the function returns NULL.
			/// </summary>
			REPLACELANG = 0x00000010,
			/// <summary>
			/// Substitutes the specified input locale identifier with another locale preferred by the user.
			/// The system starts with this flag set, and it is recommended that your application always use this flag.
			/// The substitution occurs only if the registry key HKEY_CURRENT_USER\Keyboard\Layout\Substitutes explicitly defines a substitution locale.
			/// For example, if the key includes the value name "00000409" with value "00010409", loading the U.S.
			/// English layout ("00000409") causes the Dvorak U.S. English layout ("00010409") to be loaded instead.
			/// The system uses KLF_SUBSTITUTE_OK when booting, and it is recommended that all applications use this value when loading input locale identifiers to ensure that the user's preference is selected.
			/// </summary>
			SUBSTITUTE_OK = 0x00000002,
			/// <summary>
			/// Beginning in Windows 8: This flag is not used. LoadKeyboardLayout always activates an input locale identifier for the entire system if the current process owns the window with keyboard focus.
			/// </summary>
			SETFORPROCESS = 0x00000100,
		}

		/// <summary>Loads a new input locale identifier (formerly called the keyboard layout) into the system.</summary>
		/// <param name="pwszKLID">The name of the input locale identifier to load. This name is a string composed of the hexadecimal value of the Language Identifier (low word) and a device identifier (high word).</param>
		/// <param name="flags">Specifies how the input locale identifier is to be loaded</param>
		/// <returns></returns>
		[DllImport("user32.dll", CharSet = CharSet.Unicode)]
		public static extern IntPtr LoadKeyboardLayoutW(String pwszKLID, KLF flags);

		/// <summary>Moves the cursor to the specified screen coordinates.</summary>
		/// <param name="x">The new x-coordinate of the cursor, in screen coordinates</param>
		/// <param name="y">The new y-coordinate of the cursor, in screen coordinates</param>
		/// <returns>Returns nonzero if successful or zero otherwise</returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern Int32 SetCursorPos(Int32 x, Int32 y);

		/// <summary>Determines if the <see cref="Keys"/> is an ExtendedKey</summary>
		/// <param name="keyCode">The key code.</param>
		/// <returns>true if the key code is an extended key; otherwise, false.</returns>
		/// <remarks>
		/// The extended keys consist of the ALT and CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP, PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in the numeric keypad.
		/// 
		/// See http://msdn.microsoft.com/en-us/library/ms646267(v=vs.85).aspx Section "Extended-Key Flag"
		/// </remarks>
		public static Boolean IsExtendedKey(Keys keyCode)
		{
			switch(keyCode)
			{
			case Keys.Menu:
			case Keys.LMenu:
			case Keys.RMenu:
			case Keys.ControlKey:
			case Keys.LControlKey:
			case Keys.RControlKey:
			case Keys.Insert:
			case Keys.Delete:
			case Keys.Home:
			case Keys.End:
			case Keys.Prior:
			case Keys.Next:
			case Keys.Right:
			case Keys.Up:
			case Keys.Left:
			case Keys.Down:
			case Keys.NumLock:
			case Keys.Cancel:
			case Keys.Snapshot:
			case Keys.Divide:
				return true;
			default:
				return false;
			}
		}

		/// <summary>
		/// Dispatches the specified list of <see cref="INPUT"/> messages in their specified order by issuing a single called to <see cref="NativeMethods.SendInput"/>.
		/// </summary>
		/// <param name="inputs">The list of <see cref="INPUT"/> messages to be dispatched.</param>
		/// <exception cref="ArgumentException">If the <paramref name="inputs"/> array is empty.</exception>
		/// <exception cref="ArgumentNullException">If the <paramref name="inputs"/> array is null.</exception>
		/// <exception cref="Exception">If the any of the commands in the <paramref name="inputs"/> array could not be sent successfully.</exception>
		public static void DispatchInput(params INPUT[] inputs)
		{
			UInt32 sentInputs = Input.SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
			if(sentInputs != inputs.Length)
				throw new ArgumentException("Some simulated input commands were not sent successfully. The most common reason for this happening are the security features of Windows including User Interface Privacy Isolation (UIPI). Your application can only send commands to applications of the same or lower elevation. Similarly certain commands are restricted to Accessibility/UIAutomation applications. Refer to the project home page and the code samples for more information.");
		}

		#region KeyCodeToChar
		/// <summary>Copies the status of the 256 virtual keys to the specified buffer</summary>
		/// <param name="lpKeyState">The 256-byte array that receives the status data for each virtual key</param>
		/// <remarks>
		/// An application can call this function to retrieve the current status of all the virtual keys.
		/// The status changes as a thread removes keyboard messages from its message queue.
		/// 
		/// The status does not change as keyboard messages are posted to the thread's message queue, nor does it change as keyboard messages are posted to or retrieved from message queues of other threads.
		/// (Exception: Threads that are connected through AttachThreadInput share the same keyboard state.)
		/// </remarks>
		/// <returns>If the function succeeds, the return value is nonzero</returns>
		[DllImport("user32.dll")]
		static extern Boolean GetKeyboardState(Byte[] lpKeyState);

		/// <summary>Retrieves the active input locale identifier (formerly called the keyboard layout)</summary>
		/// <param name="idThread">The identifier of the thread to query, or 0 for the current thread</param>
		/// <remarks>
		/// The input locale identifier is a broader concept than a keyboard layout, since it can also encompass a speech-to-text converter, an Input Method Editor (IME), or any other form of input.
		/// Since the keyboard layout can be dynamically changed, applications that cache information about the current keyboard layout should process the WM_INPUTLANGCHANGE message to be informed of changes in the input language.
		/// </remarks>
		/// <returns>
		/// The return value is the input locale identifier for the thread.
		/// The low word contains a Language Identifier for the input language and the high word contains a device handle to the physical layout of the keyboard.
		/// </returns>
		[DllImport("user32.dll")]
		static extern IntPtr GetKeyboardLayout(UInt32 idThread = 0);

		/// <summary>Translates the specified virtual-key code and keyboard state to the corresponding Unicode character or characters</summary>
		/// <param name="wVirtKey">The virtual-key code to be translated</param>
		/// <param name="wScanCode">
		/// The hardware scan code of the key to be translated.
		/// The high-order bit of this value is set if the key is up
		/// </param>
		/// <param name="lpKeyState">
		/// A pointer to a 256-byte array that contains the current keyboard state. Each element (byte) in the array contains the state of one key.
		/// If the high-order bit of a byte is set, the key is down
		/// </param>
		/// <param name="pwszBuff">
		/// The buffer that receives the translated Unicode character or characters.
		/// However, this buffer may be returned without being null-terminated even though the variable name suggests that it is null-terminated.
		/// </param>
		/// <param name="cchBuff">The size, in characters, of the buffer pointed to by the pwszBuff parameter.</param>
		/// <param name="wFlags">
		/// The behavior of the function.
		/// If bit 0 is set, a menu is active.
		/// If bit 2 is set, keyboard state is not changed(Windows 10, version 1607 and newer)
		/// All other bits(through 31) are reserved.
		/// </param>
		/// <param name="dwhkl">
		/// The input locale identifier used to translate the specified code.
		/// This parameter can be any input locale identifier previously returned by the LoadKeyboardLayout function.
		/// </param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		static extern Int32 ToUnicodeEx(UInt32 wVirtKey, UInt32 wScanCode, Byte[] lpKeyState, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pwszBuff, Int32 cchBuff, UInt32 wFlags, IntPtr dwhkl);

		/// <summary>Converts Keys to Chars</summary>
		/// <param name="key">keyCode</param>
		/// <returns>Chars</returns>
		public static String KeyCodeToChar(Keys key)
		{
			Byte[] keyboardState = new Byte[255];
			if(!Input.GetKeyboardState(keyboardState))
				return null;

			UInt32 vKeyCode = (UInt32)key;
			UInt32 scanCode = Input.MapVirtualKey(vKeyCode, MAPVK.VK_TO_VSC);
			IntPtr hLayout = Input.GetKeyboardLayout(0);

			StringBuilder result = new StringBuilder();
			Input.ToUnicodeEx(vKeyCode, scanCode, keyboardState, result, 5, 0, hLayout);
			return result.ToString();
		}
		#endregion KeyCodeToChar
	}
}