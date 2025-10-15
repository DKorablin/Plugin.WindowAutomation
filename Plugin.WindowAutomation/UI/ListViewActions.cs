using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AlphaOmega.Windows.Forms;
using Plugin.WindowAutomation.Dto.Clicker;

namespace Plugin.WindowAutomation.UI
{
	internal class ListViewActions : DbListView
	{
		public enum ActionsIcons
		{
			Pending = 0,
			Valid = 1,
			Invalid = 2,
			Active = 3,
		}

		private ActionsProject _project;

		private readonly ColumnHeader colActionName = new ColumnHeader() { Text = "Name" };
		private readonly ColumnHeader colActionData = new ColumnHeader() { Text = "Data" };
		private readonly ColumnHeader colActionTimeout = new ColumnHeader() { Text = "Timeout" };

		public ActionsProject Project
		{
			get => this._project ?? (this._project = new ActionsProject());
			private set => this._project = value;
		}

		public String FilePath { get; private set; }

		public Boolean IsDirty { get; private set; } = false;

		public Boolean IsEnabled { get; set; }

		public event EventHandler<EventArgs> DirtyChanged;

		public Plugin Plugin { get; set; }

		public ListViewActions()
		{
			this.IsEnabled = true;
			base.AllowDrop = true;
			base.FullRowSelect = true;
			base.HeaderStyle = ColumnHeaderStyle.Nonclickable;
			base.HideSelection = false;

			base.Columns.AddRange(new ColumnHeader[] { this.colActionName, this.colActionTimeout, this.colActionData});
		}

		public void SaveProjectToFile()
		{
			using(SaveFileDialog dlg = new SaveFileDialog()
			{
				Filter = "Actions file (json) (*.clc)|*.clc",
				DefaultExt = "clc",
				AddExtension = true,
				FileName = this.FilePath,
			})
				if(dlg.ShowDialog() == DialogResult.OK)
					this.SaveProject(dlg.FileName);
		}

		public void SaveProject()
		{
			if(this.IsDirty)
				if(this.FilePath == null)
					this.SaveProject(null);
				else
					this.SaveProjectToFile();
		}

		public void SaveProject(String filePath)
		{
			if(filePath == null)
				this.Plugin.Settings.SaveActions(this.Project);
			else
				this.Project.Save(filePath);

			this.FilePath = filePath;
			this.ToggleDirty(false);
		}

		public void LoadProject(String filePath)
		{
			this.Project = filePath == null
				? this.Plugin.Settings.ClickerActions
				: new ActionsProject(this.FilePath);

			base.Items.Clear();
			if(this.Project.Actions.Count > 0)
			{
				List<ListViewItem> itemsToAdd = new List<ListViewItem>(this.Project.Actions.Count);
				foreach(ActionBase action in this.Project.Actions)
					itemsToAdd.Add(this.CreateListItemFromAction(action));

				base.Items.AddRange(itemsToAdd.ToArray());
				base.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}

			this.FilePath = filePath;
			this.ToggleDirty(false);
		}

		public void AddAction(ActionBase action)
		{
			this.Project.Actions.Add(action);
			base.Items.Add(this.CreateListItemFromAction(action));
			this.ToggleDirty(true);
		}

		public void RemoveAction(ListViewItem item)
		{
			ActionBase action = (ActionBase)item.Tag;
			if(this.Project.Actions.Remove(action))
				base.Items.Remove(item);
		}

		public void UpdateAction(ListViewItem item, ActionBase action)
		{
			this.PopulateListItemAction(item, action);
			this.ToggleDirty(true);
		}

		private ListViewItem CreateListItemFromAction(ActionBase action)
		{
			String[] subItems = Array.ConvertAll<String, String>(new String[base.Columns.Count + 1], (str) => { return String.Empty; });

			ListViewItem result = new ListViewItem(subItems);
			this.PopulateListItemAction(result, action);
			return result;
		}

		private void PopulateListItemAction(ListViewItem item, ActionBase baseAction)
		{
			item.Tag = baseAction;
			if(baseAction is ActionKey keyAction)
			{
				KeysConverter converter = new KeysConverter();
				item.SubItems[colActionName.Index].Text = converter.ConvertToString(keyAction.Key);
				item.SubItems[colActionData.Index].Text = String.Empty;
			} else if(baseAction is ActionMouse mouseAction)
			{
				item.SubItems[colActionName.Index].Text = $"{mouseAction.Repeat}x {mouseAction.Button} {mouseAction.Location}";
			} else if(baseAction is ActionText textAction)
			{
				item.SubItems[colActionName.Index].Text = String.Format("String[{0:N0}]", textAction.Text == null ? 0 : textAction.Text.Length);

				String text = textAction.Text == null ? String.Empty : textAction.Text;
				item.SubItems[colActionName.Index].Text = text.Length < 50 ? text : text.Substring(0, 50) + "...";
			} else if(baseAction is ActionMethod methodAction)
			{
				item.SubItems[colActionName.Index].Text = methodAction.MethodName;
			} else
				item.SubItems[colActionName.Index].Text = baseAction.ToString();

			item.SubItems[colActionTimeout.Index].Text = baseAction.Timeout.ToString("N0");
			item.SubItems[colActionData.Index].Text = baseAction.Description;
			if(baseAction.Disabled)
				item.ForeColor = Color.LightGray;
			else if(baseAction.IsValid)
				item.ForeColor = SystemColors.WindowText;
			else
				item.ForeColor = Color.DarkRed;

			item.ImageIndex = item.StateImageIndex = (Int32)(baseAction.IsValid ? ActionsIcons.Valid : ActionsIcons.Invalid);
		}

		private void ToggleDirty(Boolean isDirty)
		{
			this.IsDirty = isDirty;
			if(this.IsEnabled)
				this.DirtyChanged?.Invoke(this, EventArgs.Empty);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			if(!this.IsEnabled)
				return;

			switch(e.KeyData)
			{
			case Keys.Control | Keys.S:
				base.Cursor = Cursors.WaitCursor;
				try
				{
					this.SaveProject();
				} finally
				{
					base.Cursor = Cursors.Default;
				}
				e.Handled = true;
				break;
			case Keys.Control | Keys.A:
				foreach(ListViewItem item in base.Items)
					item.Selected = true;
				e.Handled = true;
				break;
			}
			base.OnKeyDown(e);
		}

		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			if(!this.IsEnabled)
				return;

			ActionBase[] items = new ActionBase[base.SelectedItems.Count];
			for(Int32 loop = 0; loop < items.Length; loop++)
				items[loop] = (ActionBase)base.SelectedItems[loop].Tag;

			base.DoDragDrop(items, DragDropEffects.Move);
			base.OnItemDrag(e);
		}

		protected override void OnDragEnter(DragEventArgs drgevent)
		{
			drgevent.Effect = drgevent.Data.GetDataPresent(typeof(ActionBase[])) ? DragDropEffects.Move : DragDropEffects.None;
			base.OnDragEnter(drgevent);
		}

		protected override void OnDragDrop(DragEventArgs drgevent)
		{
			Point pos = base.PointToClient(new Point(drgevent.X, drgevent.Y));
			ListViewHitTestInfo hit = base.HitTest(pos);
			if(hit.Item != null && !hit.Item.Selected)
			{
				ListViewItem[] items = new ListViewItem[base.SelectedItems.Count];
				Int32 index = hit.Item.Index;

				for(Int32 loop = 0; loop < items.Length; loop++)
				{
					items[loop] = base.SelectedItems[loop];
					ActionBase action = (ActionBase)items[loop].Tag;
					if(this.Project.Actions.Remove(action))
						items[loop].Remove();

					this.Project.Actions.Insert(index, action);
					base.Items.Insert(index, items[loop]);
					index = items[loop].Index;
				}
				this.ToggleDirty(true);
			}

			base.OnDragDrop(drgevent);
		}
	}
}