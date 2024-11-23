using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Plugin.WindowAutomation.Native
{
	internal class GlobalWindowsHook : IDisposable
	{
		#region EventArgs
		public class KeyEventArgs2 : KeyEventArgs
		{
			public Input.ClickFlags Click { get; private set; }

			public KeyEventArgs2(Keys keyData, Input.ClickFlags click)
				: base(keyData)
				=> this.Click = click;
		}

		public class MouseEventArgs2 : MouseEventArgs
		{
			public Input.ClickFlags Click { get; private set; }

			public MouseEventArgs2(MouseButtons button, Hook.MOUSEHOOKSTRUCT.Point point, Input.ClickFlags click)
				: base(button, 1, point.X, point.Y, 0)
				=> this.Click = click;
		}
		#endregion EventArgs

		private static IntPtr _moduleHandle = IntPtr.Zero;
		private readonly IntPtr _hHook;
		private Hook.HookProc wm_CallbackDelegate = null;
		private event EventHandler<KeyEventArgs2> WindowsKeysPressed;
		private event EventHandler<MouseEventArgs2> WindowsMouseClicked;

		public static IntPtr ModuleHandle
			=> _moduleHandle == IntPtr.Zero
				? _moduleHandle = Hook.GetModuleHandle()
				: _moduleHandle;

		public GlobalWindowsHook(EventHandler<KeyEventArgs2> onKeyDown)
			: this(GlobalWindowsHook.ModuleHandle, onKeyDown)
		{ }

		public GlobalWindowsHook(EventHandler<MouseEventArgs2> onMouseDown)
			: this(GlobalWindowsHook.ModuleHandle, onMouseDown)
		{ }

		public GlobalWindowsHook(IntPtr hModule, EventHandler<KeyEventArgs2> onKeyDown)
		{
			this.WindowsKeysPressed = onKeyDown;
			this.wm_CallbackDelegate = new Hook.HookProc(this.WM_KeyPress);

			this._hHook = hModule == IntPtr.Zero
				? Hook.SetWindowsHookEx(Hook.WH.KEYBOARD, this.wm_CallbackDelegate, IntPtr.Zero, Hook.GetCurrentThreadId())
				: Hook.SetWindowsHookEx(Hook.WH.KEYBOARD_LL, this.wm_CallbackDelegate, hModule, 0);

			if(this._hHook == IntPtr.Zero)
				throw new Win32Exception();
		}

		public GlobalWindowsHook(IntPtr hModule, EventHandler<MouseEventArgs2> onMouseDown)
		{
			this.WindowsMouseClicked = onMouseDown;
			this.wm_CallbackDelegate = new Hook.HookProc(this.WM_MouseClick);

			this._hHook = hModule == IntPtr.Zero
				? Hook.SetWindowsHookEx(Hook.WH.MOUSE, this.wm_CallbackDelegate, IntPtr.Zero, Hook.GetCurrentThreadId())
				: Hook.SetWindowsHookEx(Hook.WH.MOUSE_LL, this.wm_CallbackDelegate, hModule, 0);

			if(this._hHook == IntPtr.Zero)
				throw new Win32Exception();
		}

		private IntPtr WM_KeyPress(Int32 code, Int32 wParam, IntPtr lParam)
		{
			if(code >= 0)
				switch((Window.WM)wParam)
				{
				case Window.WM.KEYDOWN:
				case Window.WM.KEYUP:
					Hook.KBDLLHOOKSTRUCT keyData = (Hook.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Hook.KBDLLHOOKSTRUCT));

					Keys modifier = Control.ModifierKeys;
					Input.ClickFlags click = wParam == (Int32)Window.WM.KEYUP ? Input.ClickFlags.Up : Input.ClickFlags.Down;
					System.Threading.ThreadPool.QueueUserWorkItem(delegate (Object state)
					{
						this.WindowsKeysPressed.Invoke(this, new KeyEventArgs2(keyData.vkCode | modifier, click));
					}, keyData);
					break;
				}

			return Hook.CallNextHookEx(this._hHook, code, wParam, lParam);
		}

		private IntPtr WM_MouseClick(Int32 code, Int32 wParam, IntPtr lParam)
		{
			if(code >= 0)
				switch((Window.WM)wParam)
				{
				case Window.WM.LBUTTONDOWN:
				case Window.WM.LBUTTONUP:
				case Window.WM.RBUTTONDOWN:
				case Window.WM.RBUTTONUP:
					Hook.MOUSEHOOKSTRUCT mouseData = (Hook.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Hook.MOUSEHOOKSTRUCT));

					Window.WM wm = (Window.WM)wParam;
					MouseButtons button = (wm == Window.WM.LBUTTONDOWN || wm == Window.WM.LBUTTONUP) ? MouseButtons.Left : MouseButtons.Right;
					Input.ClickFlags click = (wm == Window.WM.LBUTTONUP || wm == Window.WM.RBUTTONUP) ? Input.ClickFlags.Up : Input.ClickFlags.Down;
					System.Threading.ThreadPool.QueueUserWorkItem(delegate (Object state)
					{
						this.WindowsMouseClicked.Invoke(this, new MouseEventArgs2(button, mouseData.pt, click));
					}, mouseData);
					break;
				}
			return Hook.CallNextHookEx(this._hHook, code, wParam, lParam);
		}

		private static IntPtr GetModuleHandle()
		{
			using(Process process = Process.GetCurrentProcess())
			using(ProcessModule module = process.MainModule)
				return Hook.GetModuleHandle(module.ModuleName);
		}

		public void Dispose()
		{
			this.WindowsKeysPressed = null;
			this.WindowsMouseClicked = null;
			//Hook.UnhookWindowsHookEx(this._hHook);
			if(!Hook.UnhookWindowsHookEx(this._hHook))
				throw new Win32Exception();
		}
	}
}