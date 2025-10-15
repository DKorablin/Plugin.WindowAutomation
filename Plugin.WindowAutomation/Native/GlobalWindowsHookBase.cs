using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Plugin.WindowAutomation.Dto;

namespace Plugin.WindowAutomation.Native
{
	internal abstract class GlobalWindowsHookBase : IDisposable
	{
		private static IntPtr _moduleHandle = IntPtr.Zero;
		private readonly IntPtr _hHookKeyboard;
		private readonly IntPtr _hHookMouse;
		private readonly Hook.HookProc _callback;
		private Boolean _disposed;

		protected static IntPtr ModuleHandle
			=> _moduleHandle == IntPtr.Zero
				? _moduleHandle = Hook.GetModuleHandle()
				: _moduleHandle;

		protected GlobalWindowsHookBase(IntPtr hModule, HookType hookType)
		{
			if(hookType == HookType.None)
				throw new ArgumentException("At least one hook flag must be specified", nameof(hookType));

			this._callback = new Hook.HookProc(this.WM_HookProc);

			try
			{
				if((hookType & HookType.Keyboard) != 0)
				{
					this._hHookKeyboard = hModule == IntPtr.Zero
						? Hook.SetWindowsHookEx(Hook.WH.KEYBOARD, this._callback, IntPtr.Zero, Hook.GetCurrentThreadId())
						: Hook.SetWindowsHookEx(Hook.WH.KEYBOARD_LL, this._callback, hModule, 0);
					if(this._hHookKeyboard == IntPtr.Zero)
						throw new Win32Exception();
				}
				if((hookType & HookType.Mouse) != 0)
				{
					this._hHookMouse = hModule == IntPtr.Zero
						? Hook.SetWindowsHookEx(Hook.WH.MOUSE, this._callback, IntPtr.Zero, Hook.GetCurrentThreadId())
						: Hook.SetWindowsHookEx(Hook.WH.MOUSE_LL, this._callback, hModule, 0);
					if(this._hHookMouse == IntPtr.Zero)
						throw new Win32Exception();
				}
			}
			catch(Win32Exception)
			{
				// Clean up any successfully created hooks before rethrowing.
				if(this._hHookKeyboard != IntPtr.Zero)
					Hook.UnhookWindowsHookEx(this._hHookKeyboard);
				if(this._hHookMouse != IntPtr.Zero)
					Hook.UnhookWindowsHookEx(this._hHookMouse);
				throw;
			}
		}

		private IntPtr WM_HookProc(Int32 code, Int32 wParam, IntPtr lParam)
		{
			Window.WM wm = (Window.WM)wParam;
			if(code >= 0)
				switch(wm)
				{
				case Window.WM.KEYDOWN:
				case Window.WM.KEYUP:
				case Window.WM.SYSKEYDOWN:
				case Window.WM.SYSKEYUP:
					Hook.KBDLLHOOKSTRUCT keyData = (Hook.KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Hook.KBDLLHOOKSTRUCT));

					if(!this.OnKeyPress(wm, keyData))
						return (IntPtr)1;
					break;
				case Window.WM.LBUTTONDOWN:
				case Window.WM.LBUTTONUP:
				case Window.WM.RBUTTONDOWN:
				case Window.WM.RBUTTONUP:
					Hook.MOUSEHOOKSTRUCT mouseData = (Hook.MOUSEHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(Hook.MOUSEHOOKSTRUCT));

					if(!this.OnMouseClick(wm, mouseData))
						return (IntPtr)1;
					break;
				}

			// We do not differentiate which hook invoked us; passing IntPtr.Zero is acceptable for low-level hooks.
			return Hook.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
		}

		protected abstract Boolean OnKeyPress(Window.WM wParam, Hook.KBDLLHOOKSTRUCT keyData);

		protected abstract Boolean OnMouseClick(Window.WM wParam, Hook.MOUSEHOOKSTRUCT mouseData);

		private static IntPtr GetModuleHandle()
		{
			using(Process process = Process.GetCurrentProcess())
			using(ProcessModule module = process.MainModule)
				return Hook.GetModuleHandle(module.ModuleName);
		}

		protected virtual void Dispose(Boolean disposing)
		{
			if(this._disposed)
				return;

			// Always unhook (managed vs unmanaged distinction not critical here)
			if(this._hHookKeyboard != IntPtr.Zero)
			{
				Boolean ok = Hook.UnhookWindowsHookEx(this._hHookKeyboard);
				if(disposing && !ok)
					throw new Win32Exception();
			}
			if(this._hHookMouse != IntPtr.Zero)
			{
				Boolean ok = Hook.UnhookWindowsHookEx(this._hHookMouse);
				if(disposing && !ok)
					throw new Win32Exception();
			}

			this._disposed = true;
		}

		~GlobalWindowsHookBase()
			=> this.Dispose(false);

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}