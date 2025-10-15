using System;
using Plugin.WindowAutomation.Dto;

namespace Plugin.WindowAutomation.Native
{
	internal class GlobalWindowsHookAntiDebounce : GlobalWindowsHookBase
	{
		// Array indexed by virtual key (0-255) holding last accepted tick count (Environment.TickCount) for fast O(1) lookup.
		private static readonly Int32[] _lastAcceptedTicks = new Int32[256];

		// Mouse buttons: 0=Left,1=Right,2=Middle,3=XButton1,4=XButton2
		private static readonly Int32[] _lastMouseAcceptedTicks = new Int32[5];

		// Threshold in milliseconds for debouncing (key presses arriving sooner are suppressed).
		private readonly UInt32 _thresholdMs;

		// Extension points for tracing / diagnostics (no-op by default)
		protected virtual void OnSuppressedKey(Int32 virtualKey, Int32 nowTick) { }
		protected virtual void OnSuppressedMouse(Int32 buttonIndex, Int32 nowTick) { }

		static GlobalWindowsHookAntiDebounce()
		{
			// Initialize with very negative value so first press always passes.
			for(Int32 i = 0; i < _lastAcceptedTicks.Length; i++)
				_lastAcceptedTicks[i] = Int32.MinValue / 2;
			for(Int32 i = 0; i < _lastMouseAcceptedTicks.Length; i++)
				_lastMouseAcceptedTicks[i] = Int32.MinValue / 2;
		}

		public GlobalWindowsHookAntiDebounce(HookType hookType, UInt32 thresholdMs = 50)
			: base(GlobalWindowsHookBase.ModuleHandle, hookType)
		{
			this._thresholdMs = thresholdMs;
		}

		protected override Boolean OnKeyPress(Window.WM wParam, Hook.KBDLLHOOKSTRUCT keyData)
		{
			if(wParam == Window.WM.KEYDOWN || wParam == Window.WM.SYSKEYDOWN)
			{
				Int32 vk = ((Int32)keyData.vkCode) & 0xFF; // ensure in range 0-255
				Int32 now = Environment.TickCount; // fast, sufficient resolution (~15.6ms default)
				Int32 last = _lastAcceptedTicks[vk];
				// unchecked subtraction handles Environment.TickCount wrap-around every ~24.9 days
				if(unchecked(now - last) < _thresholdMs)
				{
					this.OnSuppressedKey(vk, now);
					return false; // suppress chatter
				}
				_lastAcceptedTicks[vk] = now; // accept
			}
			return true;
		}

		protected override Boolean OnMouseClick(Window.WM wParam, Hook.MOUSEHOOKSTRUCT mouseData)
		{
			// NOTE: Current hook type is WH.KEYBOARD so this method will not be invoked unless hook type changes.
			Int32 index;
			switch(wParam)
			{
			case Window.WM.LBUTTONDOWN: index = 0; break;
			case Window.WM.RBUTTONDOWN: index = 1; break;
			case Window.WM.MBUTTONDOWN: index = 2; break;
			case Window.WM.XBUTTONDOWN:
				// Determine which X button by high word of mouseData? Structure provided does not expose; treat as generic.
				index = 3; break;
			default:
				return true; // ignore other messages
			}

			Int32 now = Environment.TickCount;
			Int32 last = _lastMouseAcceptedTicks[index];
			if(unchecked(now - last) < _thresholdMs)
			{
				this.OnSuppressedMouse(index, now);
				return false; // suppress chatter
			}

			_lastMouseAcceptedTicks[index] = now;
			return true;
		}
	}
}