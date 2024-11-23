using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Plugin.WindowAutomation.UI
{
	internal class ColumnEditor<T> : UITypeEditor
	{
		private ColumnEditorControl _control;

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			if(this._control == null)
				this._control = new ColumnEditorControl(typeof(T));

			this._control.SetStatus((Int32)value);
			((IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService))).DropDownControl(this._control);
			return this._control.Result; //return base.EditValue(context, provider, value);
		}

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown; //return base.GetEditStyle(context);

		private class ColumnEditorControl : UserControl
		{
			private readonly CheckedListBox cblColumns = new CheckedListBox();

			public Int32 Result
			{
				get
				{
					Boolean[] columns = new Boolean[this.cblColumns.Items.Count];
					for(Int32 loop = 0; loop < columns.Length; loop++)
						columns[loop] = this.cblColumns.GetItemChecked(loop);

					return BitToInt(columns)[0];
				}
			}

			public ColumnEditorControl(Type enumType)
			{
				base.SuspendLayout();
				base.BackColor = SystemColors.Control;
				this.cblColumns.FormattingEnabled = true;
				this.cblColumns.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
				this.cblColumns.BorderStyle = BorderStyle.None;
				base.Size = new Size(this.cblColumns.Width, this.cblColumns.Height);

				foreach(Object value in Enum.GetValues(enumType))
					this.cblColumns.Items.Add(value.ToString());

				base.Controls.AddRange(new Control[] { this.cblColumns });
				this.cblColumns.Focus();
				base.ResumeLayout();
			}

			public void SetStatus(Int32 flags)
			{
				for(Int32 loop = 0; loop < this.cblColumns.Items.Count; loop++)
					cblColumns.SetItemChecked(loop, ((flags >> loop) & 0x01) == 1);
			}

			public static Int32[] BitToInt(params Boolean[] bits)
			{
				Int32[] result = new Int32[] { };
				Int32 counter = 0;
				for(Int32 loop = 0; loop < bits.Length; loop++)
				{
					if(result.Length <= loop)//Увеличиваю массив на один, если не помещается значение
						Array.Resize<Int32>(ref result, result.Length + 1);

					for(Int32 innerLoop = 0; innerLoop < 32; innerLoop++)
					{
						result[loop] |= Convert.ToInt32(bits[counter++]) << innerLoop;
						if(counter >= bits.Length)
							break;
					}
					if(counter >= bits.Length)
						break;
				}
				return result;
			}
		}
	}
}