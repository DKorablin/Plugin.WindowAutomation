using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Plugin.WindowAutomation.UI
{
	internal class PointSelectorEditor : UITypeEditor
	{
		private PointSelectorControl _control;

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			if(this._control == null)
				this._control = new PointSelectorControl();

			this._control.Result = (Point)value;
			((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService))).DropDownControl(this._control);
			return this._control.Result;
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown;

		private sealed class PointSelectorControl : UserControl
		{
			private readonly Label _lblLocation = new Label();

			private readonly TargetWindowCtrl _ctlWindowFinder = new TargetWindowCtrl();

			public Point Result { get; set; }

			public PointSelectorControl()
			{
				base.SuspendLayout();
				base.BackColor = SystemColors.Control;
				this._lblLocation.Dock = DockStyle.Top;
				this._lblLocation.Size = new Size(base.Size.Width, 16);
				this._ctlWindowFinder.Location = new Point(0, this._lblLocation.Height);
				this._ctlWindowFinder.Searching += ctlWindowFinder_Searching;
				this._ctlWindowFinder.SearchFinished += ctlWindowFinder_SearchFinished;
				base.Size = new Size(this._ctlWindowFinder.Width, this._ctlWindowFinder.Height + this._lblLocation.Height);

				base.Controls.AddRange(new Control[] { this._lblLocation, this._ctlWindowFinder });
				this._ctlWindowFinder.Focus();
				base.ResumeLayout();
			}

			private void ctlWindowFinder_Searching(Object sender, MouseEventArgs e)
				=> _lblLocation.Text = Cursor.Position.ToString();

			private void ctlWindowFinder_SearchFinished(Object sender, MouseEventArgs e)
				=> this.Result = Cursor.Position;
		}
	}
}