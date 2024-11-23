using System;
using System.Drawing;
using System.Windows.Automation;

namespace Plugin.WindowAutomation.Dto.Automation
{
	internal class WindowPatternDto
	{
		private readonly WindowInfo _window;
		private readonly WindowPattern _pattern;

		public Boolean CanMaximize
			=> this._pattern.Current.CanMaximize;

		public Boolean CanMinimize
			=> this._pattern.Current.CanMinimize;

		public Boolean IsTopmost
			=> this._pattern.Current.IsTopmost;

		public Boolean IsModal
			=> this._pattern.Current.IsModal;

		public WindowInteractionState WindowInteractionState
			=> this._pattern.Current.WindowInteractionState;

		public WindowVisualState WindowVisualState
		{
			get => this._pattern.Current.WindowVisualState;
			set => this._pattern.SetWindowVisualState(value);
		}

		public Boolean IsVisible
		{
			get => this._window.IsVisible;
			set => this._window.IsVisible = value;
		}

		public Native.Window.WS WindowStyle
		{
			get
			{
				IntPtr styles = Native.Window.GetWindowLongPtr(this._window.Handle, Native.Window.WindowLongFlags.GWL_STYLE);
				return styles == IntPtr.Zero
					? 0
					: (Native.Window.WS)styles.ToInt64();
			}
		}

		public Rectangle Rect
		{
			get => this._window.Rect;
			set => this._window.Rect = value;
		}

		public String Caption
		{
			get => this._window.Caption;
			set => this._window.Caption = value;
		}

		public Native.Window.WS_EX WindowStyleEx
		{
			get
			{
				IntPtr styles = Native.Window.GetWindowLongPtr(this._window.Handle, Native.Window.WindowLongFlags.GWL_EXSTYLE);
				return styles == IntPtr.Zero
					? 0
					: (Native.Window.WS_EX)styles.ToInt64();
			}
		}

		public WindowPatternDto(WindowInfo window, WindowPattern pattern)
		{
			this._window = window;
			this._pattern = pattern;
		}
	}
}