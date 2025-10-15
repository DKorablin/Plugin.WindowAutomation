using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Plugin.WindowAutomation.UI
{
	public class CompilerMethodEditor : UITypeEditor
	{
		private IWindowsFormsEditorService _editorService;

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			=> UITypeEditorEditStyle.DropDown;

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			this._editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			// use a list box
			ListBox lb = new ListBox()
			{
				SelectionMode = SelectionMode.One,
				DisplayMember = "Key",
				ValueMember = "Value",
			};
			lb.SelectedValueChanged += (sender, e) => this._editorService.CloseDropDown();// close the drop down as soon as something is clicked

			foreach(String item in Plugin.Instance.Compiler.GetMethods())
			{
				Int32 index = lb.Items.Add(item);
				if(item.Equals(value))
					lb.SelectedIndex = index;
			}

			Int32 newItemIndex = lb.Items.Add("Add...");

			// show this model stuff
			this._editorService.DropDownControl(lb);

			String result = (String)lb.SelectedItem;
			if(result == null)
				return value;

			result = lb.SelectedIndex == newItemIndex
				? Plugin.Instance.GetUniqueMethodName()
				: result;
			Plugin.Instance.Compiler.CreateCompilerWindow(result, null);

			return result;
		}
	}
}