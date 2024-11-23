using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Plugin.WindowAutomation.UI
{
	internal class TargetWindowCtrl : PictureBox
	{
		private Cursor _hoverCursor;
		private readonly ImageList imageList;

		private Cursor HoverCursor
		{
			get
			{
				if(this._hoverCursor == null)
				{
					MemoryStream stream = new MemoryStream(Properties.Resources.winfinder);
					this._hoverCursor = new Cursor(stream);
				}
				return this._hoverCursor;
			}
		}

		public event EventHandler<MouseEventArgs> Searching;
		public event EventHandler<MouseEventArgs> SearchFinished;
		public event EventHandler<MouseEventArgs> SearchCancelled;

		public TargetWindowCtrl()
		{
			this.imageList = new ImageList
			{
				ImageStream = Plugin.WindowAutomation.Properties.Resources.imageList_ImageStream,
				TransparentColor = System.Drawing.Color.Transparent
			};
			this.imageList.Images.SetKeyName(0, String.Empty);
			this.imageList.Images.SetKeyName(1, String.Empty);

			base.Cursor = Cursors.Hand;
			base.Image = Plugin.WindowAutomation.Properties.Resources.Icon1;
			base.Size = new System.Drawing.Size(31, 28);
			base.SizeMode = PictureBoxSizeMode.AutoSize;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			switch(e.Button)
			{
			case MouseButtons.Left:
				base.Cursor = this.HoverCursor;
				base.Image = imageList.Images[0];
				break;
			case MouseButtons.Right:
				if(base.Cursor == this.HoverCursor)
				{
					base.Cursor = Cursors.Default;
					base.Image = imageList.Images[1];
					this.SearchCancelled?.Invoke(this, e);
				}
				break;
			}

			base.OnMouseDown(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			if(base.Cursor == this.HoverCursor)
			{
				base.Cursor = Cursors.Default;
				base.Image = imageList.Images[1];
				this.SearchFinished?.Invoke(this, e);
			}
			base.OnMouseUp(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(base.Cursor == this.HoverCursor)
				this.Searching?.Invoke(this, e);
			base.OnMouseMove(e);
		}
	}
}