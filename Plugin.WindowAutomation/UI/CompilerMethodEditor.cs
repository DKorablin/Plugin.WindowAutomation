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
			=> UITypeEditorEditStyle.DropDown; //base.GetEditStyle(context);

		public override Object EditValue(ITypeDescriptorContext context, IServiceProvider provider, Object value)
		{
			_editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

			// use a list box
			ListBox lb = new ListBox()
			{
				SelectionMode = SelectionMode.One,
				DisplayMember = "Key",
				ValueMember = "Value",
			};
			lb.SelectedValueChanged += OnListBoxSelectedValueChanged;

			foreach(String item in PluginWindows.Instance.Compiler.GetMethods())
			{
				Int32 index = lb.Items.Add(item);
				if(item.Equals(value))
					lb.SelectedIndex = index;
			}

			Int32 newItemIndex = lb.Items.Add("Add...");

			// show this model stuff
			_editorService.DropDownControl(lb);

			String result = (String)lb.SelectedItem;
			if(result == null)
				return value;

			result = lb.SelectedIndex == newItemIndex
				? PluginWindows.Instance.GetUniqueMethodName()
				: result;
			PluginWindows.Instance.Compiler.CreateCompilerWindow(result, null);

			return result;
		}

		private void OnListBoxSelectedValueChanged(Object sender, EventArgs e)
			=> _editorService.CloseDropDown();// close the drop down as soon as something is clicked
	}
}