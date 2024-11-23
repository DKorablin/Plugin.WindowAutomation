using System;
using System.ComponentModel;
using System.Windows.Automation;

namespace Plugin.WindowAutomation.Dto.Automation
{
	internal class ValuePatternDto
	{
		private readonly WindowInfo _window;
		private readonly ValuePattern _pattern;

		[DefaultValue(false)]
		public Boolean IsReadOnly
		{
			get => this._pattern.Current.IsReadOnly;
			set => this._window.SendMessage(Native.Window.WM.EM_SETREADONLY, new IntPtr(value ? 1 : 0), IntPtr.Zero);
		}

		public String Value
		{
			get => this._pattern.Current.Value;
			set => this._pattern.SetValue(value);
		}

		public ValuePatternDto(WindowInfo window, ValuePattern pattern)
		{
			this._window = window;
			this._pattern = pattern;
		}
	}
}