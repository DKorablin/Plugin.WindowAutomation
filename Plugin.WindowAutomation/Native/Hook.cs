using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plugin.WindowAutomation.Native
{
	internal static class Hook
	{
		/// <summary>Enumerates the valid hook types passed as the idHook parameter into a call to SetWindowsHookEx.</summary>
		public enum WH
		{
			/// <summary>Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar.</summary>
			/// <see>For more information, see the MessageProc hook procedure.</see>
			MSGFILTER = -1,
			/// <summary>Installs a hook procedure that records input messages posted to the system message queue.</summary>
			/// <remarks>This hook is useful for recording macros.</remarks>
			/// <see>For more information, see the JournalRecordProc hook procedure.</see>
			JOURNALRECORD = 0,
			/// <summary>Installs a hook procedure that posts messages previously recorded by a WH_JOURNALRECORD hook procedure.</summary>
			/// <see>For more information, see the JournalPlaybackProc hook procedure.</see>
			JOURNALPLAYBACK = 1,
			/// <summary>Installs a hook procedure that monitors keystroke messages.</summary>
			/// <see>For more information, see the KeyboardProc hook procedure.</see>
			KEYBOARD = 2,
			/// <summary>Installs a hook procedure that monitors messages posted to a message queue.</summary>
			/// <see>For more information, see the GetMsgProc hook procedure.</see>
			GETMESSAGE = 3,
			/// <summary>Installs a hook procedure that monitors messages before the system sends them to the destination window procedure.</summary>
			/// <see>For more information, see the CallWndProc hook procedure.</see>
			CALLWNDPROC = 4,
			/// <summary>Installs a hook procedure that receives notifications useful to a CBT application.</summary>
			/// <see>For more information, see the CBTProc hook procedure.</see>
			CBT = 5,
			/// <summary>
			/// Installs a hook procedure that monitors messages generated as a result of an input event in a dialog box, message box, menu, or scroll bar.
			/// The hook procedure monitors these messages for all applications in the same desktop as the calling thread.
			/// </summary>
			/// <see>For more information, see the SysMsgProc hook procedure.</see>
			SYSMSGFILTER = 6,
			/// <summary>Installs a hook procedure that monitors mouse messages.</summary>
			/// <see>For more information, see the MouseProc hook procedure.</see>
			MOUSE = 7,
			/// <summary>Installs a hook procedure useful for debugging other hook procedures.</summary>
			/// <see>For more information, see the DebugProc hook procedure.</see>
			DEBUG = 9,
			/// <summary>Installs a hook procedure that receives notifications useful to shell applications.</summary>
			/// <see>For more information, see the ShellProc hook procedure.</see>
			SHELL = 10,
			/// <summary>Installs a hook procedure that will be called when the application's foreground thread is about to become idle.</summary>
			/// <remarks>This hook is useful for performing low priority tasks during idle time.</remarks>
			/// <see>For more information, see the ForegroundIdleProc hook procedure.</see>
			FOREGROUNDIDLE = 11,
			/// <summary>
			/// Installs a hook procedure that monitors messages after they have been processed by the destination window procedure.
			/// </summary>
			/// <see>For more information, see the CallWndRetProc hook procedure.</see>
			CALLWNDPROCRET = 12,
			/// <summary>Installs a hook procedure that monitors low-level keyboard input events.</summary>
			/// <see>For more information, see the LowLevelKeyboardProc hook procedure.</see>
			KEYBOARD_LL = 13,
			/// <summary>Installs a hook procedure that monitors low-level mouse input events.</summary>
			/// <see>For more information, see the LowLevelMouseProc hook procedure.</see>
			MOUSE_LL = 14,
		}

		[StructLayout(LayoutKind.Sequential)]
		public class KBDLLHOOKSTRUCT
		{
			/// <summary>A virtual-key code</summary>
			/// <remarks>The code must be a value in the range 1 to 254</remarks>
			public Keys vkCode;
			/// <summary>A hardware scan code for the key</summary>
			public UInt32 scanCode;
			/// <summary>
			/// The extended-key flag, event-injected flags, context code, and transition-state flag.
			/// This member is specified as follows.
			/// An application can use the following values to test the keystroke flags.
			/// Testing LLKHF_INJECTED (bit 4) will tell you whether the event was injected.
			/// If it was, then testing LLKHF_LOWER_IL_INJECTED (bit 1) will tell you whether or not the event was injected from a process running at lower integrity level.
			/// </summary>
			public LLKHF flags;
			/// <summary>The time stamp for this message, equivalent to what GetMessageTime would return for this message</summary>
			public UInt32 time;
			/// <summary>Additional information associated with the message.</summary>
			public UIntPtr dwExtraInfo;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MOUSEHOOKSTRUCT
		{
			[StructLayout(LayoutKind.Sequential)]
			public struct Point
			{
				public Int32 X;
				public Int32 Y;
			}

			public Point pt;
			public IntPtr hwnd;
			public UInt32 wHitTestCode;
			public IntPtr dwExtraInfo;
		}

		[Flags]
		public enum LLKHF : UInt32
		{
			/// <summary>Test the extended-key flag.</summary>
			EXTENDED = 0x01,
			/// <summary>Test the event-injected (from any process) flag.</summary>
			INJECTED = 0x10,
			/// <summary>Test the context code.</summary>
			ALTDOWN = 0x20,
			/// <summary>Test the transition-state flag.</summary>
			UP = 0x80,
		}

		/// <summary>
		/// Installs an application-defined hook procedure into a hook chain.
		/// You would install a hook procedure to monitor the system for certain types of events.
		/// These events are associated either with a specific thread or with all threads in the same desktop as the calling thread.
		/// </summary>
		/// <param name="code">The type of hook procedure to be installed.</param>
		/// <param name="func">
		/// A pointer to the hook procedure.
		/// If the dwThreadId parameter is zero or specifies the identifier of a thread created by a different process, the lpfn parameter must point to a hook procedure in a DLL.
		/// Otherwise, lpfn can point to a hook procedure in the code associated with the current process.
		/// </param>
		/// <param name="hInstance">
		/// A handle to the DLL containing the hook procedure pointed to by the lpfn parameter.
		/// The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process.
		/// </param>
		/// <param name="threadID">
		/// The identifier of the thread with which the hook procedure is to be associated.
		/// For desktop apps, if this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread.
		/// </param>
		/// <returns>
		/// If the function succeeds, the return value is the handle to the hook procedure.
		/// If the function fails, the return value is NULL.
		/// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr SetWindowsHookEx(WH code, HookProc func, IntPtr hInstance, Int32 threadID = 0);

		/// <summary>
		/// Passes the hook information to the next hook procedure in the current hook chain.
		/// A hook procedure can call this function either before or after processing the hook information.
		/// </summary>
		/// <param name="hHook">This parameter is ignored</param>
		/// <param name="nCode">
		/// The hook code passed to the current hook procedure.
		/// The next hook procedure uses this code to determine how to process the hook information.
		/// </param>
		/// <param name="wParam">
		/// The wParam value passed to the current hook procedure.
		/// The meaning of this parameter depends on the type of hook associated with the current hook chain.
		/// </param>
		/// <param name="lParam">
		/// The lParam value passed to the current hook procedure.
		/// The meaning of this parameter depends on the type of hook associated with the current hook chain.
		/// </param>
		/// <returns>
		/// This value is returned by the next hook procedure in the chain.
		/// The current hook procedure must also return this value.
		/// The meaning of the return value depends on the hook type.
		/// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr CallNextHookEx(IntPtr hHook, Int32 nCode, Int32 wParam, IntPtr lParam);

		/// <summary>Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.</summary>
		/// <param name="hHook">
		/// A handle to the hook to be removed.
		/// This parameter is a hook handle obtained by a previous call to SetWindowsHookEx.
		/// </param>
		/// <remarks>
		/// The hook procedure can be in the state of being called by another thread even after UnhookWindowsHookEx returns. If the hook procedure is not being called concurrently, the hook procedure is removed immediately before UnhookWindowsHookEx returns.
		/// </remarks>
		/// <returns>
		/// If the function succeeds, the return value is nonzero.
		/// If the function fails, the return value is zero.
		/// </returns>
		[DllImport("user32.dll", SetLastError = true)]
		public static extern Boolean UnhookWindowsHookEx(IntPtr hHook);

		public delegate IntPtr HookProc(Int32 code, Int32 wParam, IntPtr lParam);

		/// <summary>Retrieves a module handle for the specified module. The module must have been loaded by the calling process.</summary>
		/// <param name="lpModuleName">
		/// The name of the loaded module (either a .dll or .exe file). If the file name extension is omitted, the default library extension .dll is appended. The file name string can include a trailing point character (.) to indicate that the module name has no extension. The string does not have to specify a path. When specifying a path, be sure to use backslashes (\), not forward slashes (/). The name is compared (case independently) to the names of modules currently mapped into the address space of the calling process.
		/// If this parameter is NULL, GetModuleHandle returns a handle to the file used to create the calling process (.exe file).
		/// </param>
		/// <remarks>
		/// If lpModuleName does not include a path and there is more than one loaded module with the same base name and extension, you cannot predict which module handle will be returned.
		/// To work around this problem, you could specify a path, use side-by-side assemblies, or use GetModuleHandleEx to specify a memory location rather than a DLL name.
		/// </remarks>
		/// <returns>
		/// If the function succeeds, the return value is a handle to the specified module.
		/// If the function fails, the return value is NULL.
		/// </returns>
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr GetModuleHandle(String lpModuleName = null);

		[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
		public static extern Int32 GetCurrentThreadId();
	}
}