using System;
using System.Runtime.InteropServices;

namespace Plugin.WindowAutomation.Native
{
	internal static class Mouse
	{
		public enum MOUSEEVENTF
		{
			/// <summary>mouse move</summary>
			MOVE = 0x0001,
			/// <summary>left button down</summary>
			LEFTDOWN = 0x0002,
			/// <summary>left button up</summary>
			LEFTUP = 0x0004,
			/// <summary>right button down</summary>
			RIGHTDOWN = 0x0008,
			/// <summary>right button up</summary>
			RIGHTUP = 0x0010,
			/// <summary>middle button down</summary>
			MIDDLEDOWN = 0x0020,
			/// <summary>middle button up</summary>
			MIDDLEUP = 0x0040,
			/// <summary>x button down</summary>
			XDOWN = 0x0080,
			/// <summary>x button down</summary>
			XUP = 0x0100,
			/// <summary>wheel button rolled</summary>
			WHEEL = 0x0800,
			/// <summary>map to entire virtual desktop</summary>
			VIRTUALDESK = 0x4000,
			/// <summary>absolute move</summary>
			ABSOLUTE = 0x8000,
		}

		[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
		public static extern void mouse_event(MOUSEEVENTF dwFlags, Int32 dx, Int32 dy, Int32 cButtons, Int32 dwExtraInfo);

		[DllImport("user32")]
		public static extern Int32 SetCursorPos(Int32 x, Int32 y);

		

		public static void MouseLClick()
		{
			mouse_event(MOUSEEVENTF.LEFTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF.LEFTUP, 0, 0, 0, 0);
		}

		public static void MouseRClick()
		{
			mouse_event(MOUSEEVENTF.RIGHTDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF.RIGHTUP, 0, 0, 0, 0);
		}

		public static void MouseMClick()
		{
			mouse_event(MOUSEEVENTF.MIDDLEDOWN, 0, 0, 0, 0);
			mouse_event(MOUSEEVENTF.MIDDLEUP, 0, 0, 0, 0);
		}
	}
}