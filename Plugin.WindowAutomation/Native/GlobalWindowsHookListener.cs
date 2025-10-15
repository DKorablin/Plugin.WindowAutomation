using System;
using System.Windows.Forms;
using Plugin.WindowAutomation.Dto;

namespace Plugin.WindowAutomation.Native
{
	internal class GlobalWindowsHookListener : GlobalWindowsHookBase
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

		private event EventHandler<KeyEventArgs2> WindowsKeysPressed;
		private event EventHandler<MouseEventArgs2> WindowsMouseClicked;

		public GlobalWindowsHookListener(EventHandler<KeyEventArgs2> onKeyDown)
			: this(GlobalWindowsHookBase.ModuleHandle, onKeyDown)
		{ }

		public GlobalWindowsHookListener(EventHandler<MouseEventArgs2> onMouseDown)
			: this(GlobalWindowsHookBase.ModuleHandle, onMouseDown)
		{ }

		public GlobalWindowsHookListener(IntPtr hModule, EventHandler<KeyEventArgs2> onKeyDown)
			: base(hModule, HookType.Keyboard)
			=> this.WindowsKeysPressed = onKeyDown ?? throw new ArgumentNullException(nameof(onKeyDown));

		public GlobalWindowsHookListener(IntPtr hModule, EventHandler<MouseEventArgs2> onMouseDown)
			: base(hModule, HookType.Mouse)
			=> this.WindowsMouseClicked = onMouseDown ?? throw new ArgumentNullException(nameof(onMouseDown));

		protected override Boolean OnKeyPress(Window.WM wParam, Hook.KBDLLHOOKSTRUCT keyData)
		{
			Keys modifier = Control.ModifierKeys;
			Input.ClickFlags click = wParam == Window.WM.KEYUP ? Input.ClickFlags.Up : Input.ClickFlags.Down;
			System.Threading.ThreadPool.QueueUserWorkItem(
				state => this.WindowsKeysPressed.Invoke(this, new KeyEventArgs2(keyData.vkCode | modifier, click)),
				keyData);

			return true;
		}

		protected override Boolean OnMouseClick(Window.WM wParam, Hook.MOUSEHOOKSTRUCT mouseData)
		{
			//TODO: Check this condition
			MouseButtons button = (wParam == Window.WM.LBUTTONDOWN || wParam == Window.WM.LBUTTONUP) ? MouseButtons.Left : MouseButtons.Right;
			Input.ClickFlags click = (wParam == Window.WM.LBUTTONUP || wParam == Window.WM.RBUTTONUP) ? Input.ClickFlags.Up : Input.ClickFlags.Down;
			System.Threading.ThreadPool.QueueUserWorkItem(
				state => this.WindowsMouseClicked.Invoke(this, new MouseEventArgs2(button, mouseData.pt, click)),
				mouseData);

			return true;
		}

		protected override void Dispose(Boolean disposing)
		{
			this.WindowsKeysPressed = null;
			this.WindowsMouseClicked = null;

			base.Dispose(disposing);
		}
	}
}