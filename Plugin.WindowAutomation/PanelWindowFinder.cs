using System;
using System.Drawing;
using System.Windows.Automation;
using System.Windows.Forms;
using Plugin.WindowAutomation.Dto;
using Plugin.WindowAutomation.Dto.Automation;
using Plugin.WindowAutomation.Properties;
using SAL.Windows;

namespace Plugin.WindowAutomation
{
	public partial class PanelWindowFinder : UserControl
	{
		private WindowInfo _lastWindow = new WindowInfo(IntPtr.Zero);

		private Plugin Plugin { get => (Plugin)this.Window.Plugin; }
		private IWindow Window { get => (IWindow)base.Parent; }

		public PanelWindowFinder()
		{
			InitializeComponent();
			gridSearch.TreeView = tvWindows;
		}

		protected override void OnCreateControl()
		{
			this.Window.Caption = "Window Finder";
			this.Window.SetTabPicture(Resources.Application_Finder);
			this.Window.SetDockAreas(DockAreas.DockBottom | DockAreas.DockLeft | DockAreas.DockRight | DockAreas.DockTop | DockAreas.Document | DockAreas.Float);

			this.tsbnWindowsRefresh_Click(null, null);
			base.OnCreateControl();
			ctlWindowFinder.Searching += this.ctlWindowFinder_Searching;
			ctlWindowFinder.SearchCancelled += this.ctlWindowFinder_SearchCancelled;
			ctlWindowFinder.SearchFinished += this.ctlWindowFinder_SearchFinished;
		}

		private void tsbnWindowsRefresh_Click(Object sender, EventArgs e)
		{
			tvWindows.Nodes.Clear();
			WindowInfo desktop = new WindowInfo();
			TreeNode node = new TreeNode(desktop.ToString()) { Tag = desktop };
			node.Nodes.Add(new TreeNode());
			tvWindows.Nodes.Add(node);
		}

		private void tvWindows_MouseClick(Object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Right)
			{
				TreeViewHitTestInfo info = tvWindows.HitTest(e.Location);
				if(info.Node != null)
				{
					tvWindows.SelectedNode = info.Node;
					cmsWindows.Show(tvWindows, e.Location);
				}
			}
		}

		private void tvWindows_MouseDoubleClick(Object sender, MouseEventArgs e)
		{
			if(tvWindows.SelectedNode != null && tabAutomate.Parent != null)
				tabMain.SelectedTab = tabAutomate;
		}

		private void tvWindows_BeforeExpand(Object sender, TreeViewCancelEventArgs e)
		{
			this.PopulateWindowsChildNodes(e.Node);
		}

		private void tvWindows_AfterSelect(Object sender, TreeViewEventArgs e)
		{
			WindowInfo window = (WindowInfo)e.Node.Tag;
			this.DisplayWindowInfo(window);
			AutomationElement element = AutomationElement.FromHandle(window.Handle);
			if(element.GetSupportedPatterns().Length > 0)
			{
				if(tabAutomate.Parent == null)
					tabMain.TabPages.Add(tabAutomate);
			} else if(tabAutomate.Parent != null)
				tabMain.TabPages.Remove(tabAutomate);
		}

		/// <summary>Show information about the given window</summary>
		/// <param name="window"></param>
		private void DisplayWindowInfo(WindowInfo window)
		{
			if(window.IsEmpty)
			{
				txtCaption.Text = String.Empty;
				txtRect.Text = String.Empty;
				txtClassName.Text = String.Empty;
				txtModuleName.Text = String.Empty;
			} else
			{
				txtCaption.Text = window.Caption;
				txtClassName.Text = window.ClassName;
				txtRect.Text = window.Rect.ToString();
				txtModuleName.Text = window.ModuleFileName;
				String cName = window.ClassName;
				/*var element = System.Windows.Automation.AutomationElement.FromHandle(window.Handle);
				if(element != null)
				{
					Object pattern;
					if(element.TryGetCurrentPattern(ValuePattern.Pattern, out pattern))
					{
						ValuePattern vPattern = (ValuePattern)pattern;
						vPattern.Current.
					}
				}*/
			}
		}

		private void ctlWindowFinder_Searching(Object sender, MouseEventArgs e)
		{
			WindowInfo foundWindow = new WindowInfo(Cursor.Position);

			// not this application
			//if(Control.FromHandle(foundWindow) == null)
			{
				if(foundWindow != this._lastWindow)
				{
					// clear old window
					this._lastWindow.ToggleBorder();
					// set new window
					this._lastWindow = foundWindow;
					// paint new window
					this._lastWindow.ToggleBorder();
				}
				DisplayWindowInfo(this._lastWindow);
			}
			// show global mouse cursor
			tsCursor.Text = "Cursor: " + Cursor.Position.ToString();
		}

		private void ctlWindowFinder_SearchCancelled(Object sender, MouseEventArgs e)
			=> this.ClearSelectedWindow();

		private void ctlWindowFinder_SearchFinished(Object sender, EventArgs e)
		{
			WindowInfo wnd = this._lastWindow;
			this.ClearSelectedWindow();
			if(!wnd.IsEmpty)
			{
				TreeNode wndNode = this.FindTreeNodeParent(wnd);
				if(wndNode == null)
				{
					this.tsbnWindowsRefresh_Click(sender, e);
					wndNode = this.FindTreeNodeParent(wnd);
				}
				if(wndNode != null)
				{
					tvWindows.SelectedNode = wndNode;
					tvWindows.Focus();
				}
			}
		}

		/// <summary>Remove window selection</summary>
		private void ClearSelectedWindow()
		{
			// reset all done things from mouse_down and mouse_move ...
			this._lastWindow.ToggleBorder();
			this._lastWindow = new WindowInfo(IntPtr.Zero);
		}

		private void PopulateWindowsChildNodes(TreeNode node)
		{
			if(node.Nodes.Count != 1 || node.Nodes[0].Tag != null)
				return;

			WindowInfo wnd = (WindowInfo)node.Tag;
			foreach(WindowInfo child in wnd.GetChildWindows())
			{
				TreeNode childNode = new TreeNode(child.ToString()) { Tag = child, ForeColor = child.IsVisible ? Color.Black : Color.Gray, };
				childNode.Nodes.Add(new TreeNode());
				node.Nodes.Add(childNode);
			}
			node.Nodes.RemoveAt(0);
		}

		private TreeNode FindTreeNodeParent(WindowInfo wnd)
		{
			TreeNode foundNode = this.FindTreeNode(wnd);
			if(foundNode != null)
				return foundNode;

			WindowInfo wndParent = wnd.GetParentWindow();
			if(wndParent != null)
			{
				foundNode = this.FindTreeNodeParent(wndParent);
				if(foundNode != null)
					foreach(TreeNode childNode in foundNode.Nodes)
					{
						if(childNode.Tag.Equals(wnd))
							return childNode;
					}
			}
			return null;
		}

		private TreeNode FindTreeNode(WindowInfo tableType)
		{
			foreach(TreeNode node in tvWindows.Nodes)
				if(node.Tag.Equals(tableType))
					return node;
				else
				{
					TreeNode result = this.FindTreeNode(node, tableType);
					if(result != null)
						return result;
				}
			return null;
		}

		private TreeNode FindTreeNode(TreeNode root, WindowInfo wnd)
		{
			this.PopulateWindowsChildNodes(root);
			foreach(TreeNode node in root.Nodes)
				if(node.Tag?.Equals(wnd) == true)
					return node;
				else
				{
					TreeNode result = this.FindTreeNode(node, wnd);
					if(result != null)
						return result;
				}
			return null;
		}

		private void cmsWindows_ItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			if(e.ClickedItem == cmsWindowsRefresh)
				this.tsbnWindowsRefresh_Click(sender, e);
			else if(e.ClickedItem == cmsWindowsPrintScreen)
			{
				WindowInfo window = (WindowInfo)tvWindows.SelectedNode.Tag;
				Bitmap bmp = window.GetWindowBitmap();
				if(bmp != null)
					Clipboard.SetImage(bmp);
			}
		}

		private void cbAutomationPattern_SelectedIndexChanged(Object sender, EventArgs e)
		{
			WindowInfo window = (WindowInfo)cbAutomationPattern.Tag;
			AutomationElement element = AutomationElement.FromHandle(window.Handle);
			if(element == null)
			{
				cbAutomationPattern.Tag = null;
				cbAutomationPattern.Items.Clear();
				pgAutomation.SelectedObject = null;
			} else
			{
				AutomationPattern marker = ((AutomationPatternDto)cbAutomationPattern.SelectedItem).Automation;
				Object pattern = element.GetCurrentPattern(marker);
				if(marker == ValuePattern.Pattern)
					pgAutomation.SelectedObject = new ValuePatternDto(window, (ValuePattern)pattern);
				else if(marker == ExpandCollapsePattern.Pattern)
					pgAutomation.SelectedObject = new ExpandCollapsePatternDto((ExpandCollapsePattern)pattern);
				else if(marker == TextPattern.Pattern)
					pgAutomation.SelectedObject = new TextPatternDto((TextPattern)pattern);
				else if(marker == WindowPattern.Pattern)
					pgAutomation.SelectedObject = new WindowPatternDto(window, (WindowPattern)pattern);
				else if(marker == SelectionPattern.Pattern)
					pgAutomation.SelectedObject = new SelectionPatternDto((SelectionPattern)pattern);
				else if(marker == TransformPattern.Pattern)
					pgAutomation.SelectedObject = new TransformPatternDto((TransformPattern)pattern);
				else
					pgAutomation.SelectedObject = pattern;
			}
		}

		private void tabMain_SelectedIndexChanged(Object sender, EventArgs e)
		{
			if(tabMain.SelectedTab == tabAutomate)
			{
				TreeNode selectedNode = tvWindows.SelectedNode;
				if(selectedNode == null)
				{
					cbAutomationPattern.Items.Clear();
					cbAutomationPattern.Tag = null;
				} else
				{
					WindowInfo window = (WindowInfo)selectedNode.Tag;
					if(window.Equals(cbAutomationPattern.Tag))
						return;

					AutomationElement element = AutomationElement.FromHandle(window.Handle);
					cbAutomationPattern.Items.Clear();
					cbAutomationPattern.Tag = window;
					pgAutomation.SelectedObject = null;

					if(element != null)
						foreach(AutomationPattern pattern in element.GetSupportedPatterns())
							cbAutomationPattern.Items.Add(new AutomationPatternDto(pattern));
				}
			}
		}
	}
}