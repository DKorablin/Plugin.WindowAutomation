using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using Plugin.WindowAutomation.Native;
using Plugin.WindowAutomation.UI;

namespace Plugin.WindowAutomation.Dto.Clicker
{
	internal class ActionKey : ActionBase
	{//https://github.com/michaelnoonan/inputsimulator/blob/a61df64303cb76b005a3ced19eae4cb4755e2e42/WindowsInput/InputBuilder.cs
		[DefaultValue(Input.ClickFlags.Down | Input.ClickFlags.Up)]
		[Editor(typeof(ColumnEditor<Input.ClickFlags>), typeof(UITypeEditor))]
		[Description("Button action: press, unpress or click")]
		public Input.ClickFlags Click { get; set; } = Input.ClickFlags.Down | Input.ClickFlags.Up;

		[Description("Key combination to emulate")]
		public Keys Key { get; set; }

		[DefaultValue(true)]
		public override Boolean IsValid => this.Key != Keys.None && this.Click != 0;

		public override void Invoke()
		{
			List<Input.INPUT> keys = new List<Input.INPUT>(Convert(this.Key, this.Click));
			Native.Input.DispatchInput(keys.ToArray());
		}

		internal static IEnumerable<Input.INPUT> Convert(Keys key, Input.ClickFlags click = Input.ClickFlags.Down | Input.ClickFlags.Up)
		{
			Boolean isShift = (key & Keys.Shift) != Keys.None;
			Boolean isCtrl = (key & Keys.Control) != Keys.None;
			Boolean isAlt = (key & Keys.Alt) != Keys.None;

			Keys keyCode = key & Keys.KeyCode;
			if((click & Input.ClickFlags.Down) == Input.ClickFlags.Down)
			{//Emulate key down
				if(isCtrl)
					yield return new Input.INPUT(Keys.ControlKey, Input.ClickFlags.Down);
				if(isAlt)
					yield return new Input.INPUT(Keys.Alt, Input.ClickFlags.Down);
				if(isShift)
					yield return new Input.INPUT(Keys.ShiftKey, Input.ClickFlags.Down);

				yield return new Input.INPUT(keyCode, Input.ClickFlags.Down);
			}

			if((click & Input.ClickFlags.Up) == Input.ClickFlags.Up)
			{//Emulate key up
				yield return new Input.INPUT(keyCode, Input.ClickFlags.Up);

				if(isCtrl)
					yield return new Input.INPUT(Keys.ControlKey, Input.ClickFlags.Up);
				if(isAlt)
					yield return new Input.INPUT(Keys.Alt, Input.ClickFlags.Up);
				if(isShift)
					yield return new Input.INPUT(Keys.ShiftKey, Input.ClickFlags.Up);
			}
		}
	}
}