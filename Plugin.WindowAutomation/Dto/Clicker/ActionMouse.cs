using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Plugin.WindowAutomation.Native;
using Plugin.WindowAutomation.UI;

namespace Plugin.WindowAutomation.Dto.Clicker
{
	internal class ActionMouse : ActionBase
	{
		private Point _location;
		private UInt32 _repeat = 1;

		/// <summary>Какую кнопку нажимать</summary>
		[DefaultValue(MouseButtons.Left)]
		[Description("What button to click")]
		public MouseButtons Button { get; set; }

		[Editor(typeof(ColumnEditor<Input.ClickFlags>), typeof(UITypeEditor))]
		[Description("How to perform click")]
		public Input.ClickFlags Click { get; set; } = Input.ClickFlags.Down | Input.ClickFlags.Up;

		/// <summary>Где конпку нажимать</summary>
		[Editor(typeof(PointSelectorEditor), typeof(UITypeEditor))]
		[Description("Specify mouse coordinates where mouse event will be invoked")]
		public Point Location
		{
			get => this._location;
			set
			{
				this._location = value;
				if(!value.IsEmpty && this.Button == MouseButtons.None)
					this.Button = MouseButtons.Left;
			}
		}

		/// <summary>Сколько раз кнопку нажимать</summary>
		[DefaultValue(1)]
		[Description("How many times to press the mouse buttons")]
		public UInt32 Repeat
		{
			get => this._repeat;
			set => this._repeat = value < 1 ? 1 : value;
		}

		[DefaultValue(true)]
		[Description("This mouse event can be executed")]
		public override Boolean IsValid => !this.Location.IsEmpty && this.Click != 0;

		public override void Invoke()
		{
			/*Native.Input.INPUT mouseMode = MoveMouse(this.Location.X, this.Location.Y);
			Native.Input.DispatchInput(mouseMode);*/
			if(Input.SetCursorPos(this.Location.X, this.Location.Y) == 0)
				throw new Win32Exception();

			System.Threading.Thread.Sleep(100);

			List<Input.INPUT> clicks = new List<Input.INPUT>(2);
			if((this.Click & Input.ClickFlags.Down) == Input.ClickFlags.Down)
				clicks.Add(new Input.INPUT(this.Button, Input.ClickFlags.Down));
			if((this.Click & Input.ClickFlags.Up) == Input.ClickFlags.Up)
				clicks.Add(new Input.INPUT(this.Button, Input.ClickFlags.Up));

			for(Int32 loop = 0; loop < this.Repeat; loop++)
			{
				Input.DispatchInput(clicks.ToArray());
				if(loop + 1 < this.Repeat)
					System.Threading.Thread.Sleep(100);
			}
		}
	}
}