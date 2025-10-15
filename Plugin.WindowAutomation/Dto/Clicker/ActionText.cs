using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Plugin.WindowAutomation.Native;

namespace Plugin.WindowAutomation.Dto.Clicker
{
	internal class ActionText : ActionBase
	{
		private String _text;
		private Keys[] _textCodes;
		private CultureInfo _culture = System.Threading.Thread.CurrentThread.CurrentUICulture;

		public String Text
		{
			get => this._text;
			set
			{
				this._text = value == null || value.Trim().Length == 0
					? null
					: value;

				this._textCodes = null;
			}
		}

		public Keys[] TextCodes
		{
			get
			{
				if(this._textCodes == null && this.Text != null)
					this._textCodes = Convert(this._culture, this.Text).ToArray();
				return this._textCodes;
			}
		}

		public String CurrentCulture
		{
			get => this._culture.Name;
			set
			{
				this._culture = String.IsNullOrEmpty(value)
					? System.Threading.Thread.CurrentThread.CurrentUICulture
					: new CultureInfo(value);
			}
		}

		[DefaultValue(true)]
		public override Boolean IsValid => this.TextCodes != null && this.TextCodes.Length > 0;

		public override void Invoke()
		{
			List<Input.INPUT> keys = new List<Input.INPUT>();
			foreach(Keys code in this.TextCodes)
			{
				keys.AddRange(ActionKey.Convert(code));
				/*Boolean isShift = (code & Keys.Shift) != Keys.None;
				Boolean isCtrl = (code & Keys.Control) != Keys.None;
				Boolean isAlt = (code & Keys.Alt) != Keys.None;


				Native.VirtualKeyCode keyCode = (Native.VirtualKeyCode)(code & Keys.KeyCode);
				if(isCtrl)
					keys.Add(ActionKey.Convert(VirtualKeyCode.CONTROL, true));
				if(isAlt)
					keys.Add(ActionKey.Convert(VirtualKeyCode.MENU, true));
				if(isShift)
					keys.Add(ActionKey.Convert(VirtualKeyCode.SHIFT, true));

				keys.Add(ActionKey.Convert(keyCode, true));
				keys.Add(ActionKey.Convert(keyCode, false));

				if(isCtrl)
					keys.Add(ActionKey.Convert(VirtualKeyCode.CONTROL, false));
				if(isAlt)
					keys.Add(ActionKey.Convert(VirtualKeyCode.MENU, false));
				if(isShift)
					keys.Add(ActionKey.Convert(VirtualKeyCode.SHIFT, false));*/
			}

			Native.Input.DispatchInput(keys.ToArray());
		}

		public static IEnumerable<Keys> Convert(CultureInfo culture, String text)
		{
			IntPtr kbPtr = Input.LoadKeyboardLayoutW(culture.KeyboardLayoutId.ToString("X8"), Input.KLF.ACTIVATE);
			try
			{
				foreach(Char ch in text)
				{
					UInt16 keyCode = Input.VkKeyScanExW(ch, kbPtr);
					Keys key = (Keys)(((keyCode & 0xFF00) << 8) | (keyCode & 0xFF));
					yield return key;
				}
			} finally
			{
				Native.Input.UnloadKeyboardLayout(kbPtr);
			}
		}
	}
}