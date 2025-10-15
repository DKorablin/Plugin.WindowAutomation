using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Plugin.WindowAutomation.Dto.Clicker;
using Plugin.WindowAutomation.Native;
using Plugin.WindowAutomation.Properties;
using SAL.Flatbed;
using SAL.Windows;

namespace Plugin.WindowAutomation
{
	public partial class PanelWindowClicker : UserControl, IPluginSettings<PanelWindowClickerSettings>
	{
		private const String Caption = "Window Clicker";
		private PanelWindowClickerSettings _settings;
		private StringBuilder _recordText = null;//TODO: Test code to try to assemble the input buttons into a string
		private GlobalWindowsHookListener _keyboardHook = null;
		private GlobalWindowsHookListener _mouseHook = null;

		private Plugin Plugin => (Plugin)this.Window.Plugin;

		private IWindow Window => (IWindow)base.Parent;

		Object IPluginSettings.Settings => this.Settings;

		public PanelWindowClickerSettings Settings
			=> this._settings ?? (this._settings = new PanelWindowClickerSettings());

		public PanelWindowClicker()
			=> this.InitializeComponent();

		/// <summary>Clean up any resources being used.</summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(Boolean disposing)
		{
			if(disposing && (components != null))
				components.Dispose();

			if(this._keyboardHook != null)
			{
				this._keyboardHook.Dispose();
				this._keyboardHook = null;
			}
			if(this._mouseHook != null)
			{
				this._mouseHook.Dispose();
				this._mouseHook = null;
			}

			base.Dispose(disposing);
		}

		protected override void OnCreateControl()
		{
			this.SetWindowCaption();
			this.Window.SetTabPicture(Resources.Application_Clicker);
			tsmiActionMethod.Enabled = this.Plugin.Compiler.PluginInstance != null;
			lvActions.Plugin = this.Plugin;
			this.Plugin.Settings.PropertyChanged += this.Settings_PropertyChanged;
			this.Settings_PropertyChanged(null, new PropertyChangedEventArgs(nameof(WindowAutomation.Settings.Start)));

			this.LoadActions();
			base.OnCreateControl();
		}

		private void SetWindowCaption(ToolStripButton ctrl = null)
		{
			List<String> captions = new List<String>();
			if(this.Settings.ProjectFileName != null)
				captions.Add(Path.GetFileName(this.Settings.ProjectFileName));
			captions.Add(PanelWindowClicker.Caption);

			if(ctrl == null)
			{
				lvActions.IsEnabled = true;
				bnActionsRecord.Checked = bnActionsStart.Checked = false;
				ddlActionAdd.Enabled = bnActions.Enabled = bnActionsRecord.Enabled = bnActionsStart.Enabled = true;
				bnActionsSave.Enabled = lvActions.IsDirty;
				bnActionRemove.Enabled = lvActions.SelectedItems.Count > 0;
			} else if(ctrl == bnActionsRecord)
			{
				lvActions.IsEnabled = false;
				bnActionsRecord.Checked = true;
				bnActionsStart.Checked = false;
				ddlActionAdd.Enabled = bnActionRemove.Enabled = bnActions.Enabled = bnActionsSave.Enabled = bnActionsStart.Enabled = false;
				captions.Add("Recording...");
			} else if(ctrl == bnActionsStart)
			{
				lvActions.IsEnabled = false;
				bnActionsRecord.Checked = false;
				bnActionsStart.Checked = true;
				ddlActionAdd.Enabled = bnActionRemove.Enabled = bnActions.Enabled = bnActionsSave.Enabled = bnActionsRecord.Enabled = false;
				captions.Add("Running...");
			}

			this.Window.Caption = String.Join(" - ", captions.ToArray());
		}

		private void LoadActions()
		{
			String filePath = this.Settings.ProjectFileName;
			if(filePath != null && !File.Exists(filePath))
			{
				Plugin.Trace.TraceEvent(TraceEventType.Warning, 7, "File {0} not found", filePath);
				filePath = null;
			}

			lvActions.LoadProject(filePath);
			this.SetWindowCaption();
			tsmiProjectExport.Visible = filePath == null;
			tsmiProjectImport.Visible = filePath != null;
		}

		private void Settings_PropertyChanged(Object sender, PropertyChangedEventArgs e)
		{
			switch(e.PropertyName)
			{
			case nameof(WindowAutomation.Settings.Start):
			case nameof(WindowAutomation.Settings.Record):
				KeysConverter converter = new KeysConverter();
				this.bnActionsRecord.ToolTipText = this.Plugin.Settings.Record == Keys.None
					? "Record"
					: String.Format("Record ({0})", converter.ConvertToString(this.Plugin.Settings.Record));
				this.bnActionsStart.ToolTipText = this.Plugin.Settings.Start == Keys.None
					? "Start"
					: String.Format("Start ({0})", converter.ConvertToString(this.Plugin.Settings.Start));

				if(this.Plugin.Settings.Start != Keys.None || this.Plugin.Settings.Record != Keys.None)
				{
					if(this._keyboardHook == null)
						this._keyboardHook = new GlobalWindowsHookListener(this.keyboardHook_WindowsKeysPressed);
				} else
				{
					if(this._keyboardHook != null)
					{
						this._keyboardHook.Dispose();
						this._keyboardHook = null;
					}
				}
				break;
			}
		}

		private void keyboardHook_WindowsKeysPressed(Object sender, GlobalWindowsHookListener.KeyEventArgs2 e)
		{
			if(e.Click == Input.ClickFlags.Up)
				switch(e.KeyData)
				{
				case Keys.Alt | Keys.Tab:
					break;//Alt+Tab only comes as KeyUp
				default:
					return;
				}

			if(base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate { this.keyboardHook_WindowsKeysPressed(sender, e); });
				return;
			}

			if(e.KeyData == this.Plugin.Settings.Record)
			{
				Boolean recordState = !bnActionsRecord.Checked;
				bnActionsRecord.Checked = recordState;
				this.bnActionsRecord_Click(sender, e);

				Plugin.Trace.TraceInformation("{0} recording", recordState ? "Start" : "Stop");
			} else if(bnActionsRecord.Checked)//Priority recording
			{
				if(!Input.IsExtendedKey(e.KeyCode))
				{
					String data = Input.KeyCodeToChar(e.KeyData);
					if(!String.IsNullOrEmpty(data))
						this._recordText.Append(data);
				}

				switch(e.KeyData)
				{
				case Keys.Control:
				case Keys.Control | Keys.LControlKey:
				case Keys.Control | Keys.RControlKey:
				case Keys.LControlKey:
				case Keys.RControlKey:
				case Keys.Alt:
				case Keys.Shift:
				case Keys.Shift | Keys.LShiftKey:
				case Keys.Shift | Keys.RShiftKey:
				case Keys.LShiftKey:
				case Keys.RShiftKey:
					break;//I cut off phantom combinations
				default:
					ActionKey action = new ActionKey() { Key = e.KeyData };//TODO: If multiple characters are entered, it must be converted to ActionText.
					lvActions.AddAction(action);
					break;
				}
			} else if(e.KeyData == this.Plugin.Settings.Start)//Launching the created process
			{
				Boolean runState = !bnActionsStart.Checked;
				bnActionsStart.Checked = runState;
				this.bnActionsRun_CheckedChanged(sender, e);
				Plugin.Trace.TraceInformation("{0} clicker", runState ? "Starting" : "Stopping");
			}
		}

		private void mouseHook_WindowsMouseClicked(Object sender, GlobalWindowsHookListener.MouseEventArgs2 e)
		{
			if(e.Click == Input.ClickFlags.Up)
				return;

			if(base.InvokeRequired)
			{
				base.Invoke((MethodInvoker)delegate { this.mouseHook_WindowsMouseClicked(sender, e); });
				return;
			}

			/*IntPtr wndPtr = Native.Window.WindowFromPoint(e.Location);
			if(wndPtr == IntPtr.Zero)
				return;
			Point pt1 = e.Location;
			Point pt2 = e.Location;
			Boolean r1 = Native.Window.ClientToScreen(wndPtr, ref pt1);
			Boolean r2 =Native.Window.ScreenToClient(wndPtr, ref pt2);*/

			ActionMouse action = new ActionMouse() { Button = e.Button, Location = e.Location, };
			//Point s2c, c2s;
			//Native.Window.ScreenToClient()
			lvActions.AddAction(action);
		}

		private void ddlAddAction_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			ActionBase action = null;
			if(e.ClickedItem == tsmiActionKey)
				action = new ActionKey();
			else if(e.ClickedItem == tsmiActionMouse)
				action = new ActionMouse();
			else if(e.ClickedItem == tsmiActionKeysArray)
				action = new ActionText();
			else if(e.ClickedItem == tsmiActionMethod)
				action = new ActionMethod();

			if(action != null)
				lvActions.AddAction(action);
		}

		private void bnActionRemove_Click(Object sender, EventArgs e)
		{
			if(MessageBox.Show("Are you sure you want to delete selected items?", this.Window.Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				while(lvActions.SelectedItems.Count > 0)
					lvActions.RemoveAction(lvActions.SelectedItems[0]);
		}

		private void lvActions_SelectedIndexChanged(Object sender, EventArgs e)
		{
			bnActionRemove.Enabled = lvActions.SelectedItems.Count > 0;
			ActionBase action = lvActions.SelectedItems.Count == 1
				? (ActionBase)lvActions.SelectedItems[0].Tag
				: null;

			if(action is ActionMethod && this.Plugin.Compiler.PluginInstance == null)
				action = new ActionMethod.MethodFake();
			pgActions.SelectedObject = action;
		}

		private void pgActions_PropertyValueChanged(Object s, PropertyValueChangedEventArgs e)
			=> lvActions.UpdateAction(lvActions.SelectedItems[0], (ActionBase)pgActions.SelectedObject);

		private void bnActionsRecord_Click(Object sender, EventArgs e)
		{
			if(bnActionsRecord.Checked)
			{
				this._recordText = new StringBuilder();
				if(this._mouseHook == null)//This can only be done on the UI Thread, otherwise mouse click events will not be received.
					this._mouseHook = new GlobalWindowsHookListener(this.mouseHook_WindowsMouseClicked);
				this.SetWindowCaption(bnActionsRecord);
			} else
			{
				this._mouseHook.Dispose();
				this._mouseHook = null;
				this.SetWindowCaption();
			}
		}

		private void bnActionsRun_CheckedChanged(Object sender, EventArgs e)
		{
			if(bnActionsStart.Checked)
			{
				if(bwClicker.IsBusy)
				{
					bnActionsStart.Checked = false;
					Plugin.Trace.TraceInformation("Clicker is busy");
					return;
				}

				if(lvActions.Project.IsValid)
				{
					foreach(ListViewItem item in lvActions.Items)
						item.ImageIndex = item.StateImageIndex = (Int32)UI.ListViewActions.ActionsIcons.Pending;
					this.SetWindowCaption(bnActionsStart);
				}else
				{
					bnActionsStart.Checked = false;
					return;
				}

				bwClicker.RunWorkerAsync(lvActions.Project);
			} else
				bwClicker.CancelAsync();
		}

		private void bwClicker_DoWork(Object sender, DoWorkEventArgs e)
		{
			e.Result = null;
			ActionsProject project = (ActionsProject)e.Argument;
			Boolean result = project.Invoke(p =>
				{
					if(bwClicker.CancellationPending)
						return false;
					else
					{
						bwClicker.ReportProgress(1, p);
						return true;
					}
				});

			e.Result = result;

			/*for(Int32 loop = 0; loop < project.Actions.Count; loop++)
			{
				if(bwClicker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}

				ActionBase action = project.Actions[loop];
				bwClicker.ReportProgress(loop);
				if(action.Disabled)
					continue;

				if(action is ActionMethod)//Calls runtime method in UI thread
				{
					ActionMethod mAction = (ActionMethod)action;
					base.Invoke((MethodInvoker)delegate {
						try
						{
							mAction.Invoke();
						}catch(Exception exc)
						{//Invoke is loosing BackgroundWorker context
							PluginWindows.Trace.TraceData(TraceEventType.Error, 10, exc);
						}
					});

					if(mAction.Result == false)
					{
						e.Result = false;
						return;
					} else if(loop + 1 == project.Actions.Count)
						loop = -1;//If last action is runtime method and it returns True, then start from beginning
				} else
					action.Invoke();
				if(action.Timeout > 0)
					System.Threading.Thread.Sleep((Int32)action.Timeout);
			}*/
		}

		private void bwClicker_ProgressChanged(Object sender, ProgressChangedEventArgs e)
		{
			ActionBase action = (ActionBase)e.UserState;
			Int32 index = lvActions.Project.Actions.FindIndex(p => p == action);
			ListViewItem item = lvActions.Items[index];
			item.ImageIndex = item.StateImageIndex = (Int32)UI.ListViewActions.ActionsIcons.Active;
			item.EnsureVisible();

			ListViewItem prevItem = lvActions.Items[index == 0 ? lvActions.Items.Count - 1 : index - 1];
			prevItem.ImageIndex = prevItem.StateImageIndex = (Int32)UI.ListViewActions.ActionsIcons.Pending;
		}

		private void bwClicker_RunWorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
		{
			if(e.Error != null)
				Plugin.Trace.TraceData(TraceEventType.Error, 10, e.Error);

			if(e.Cancelled || e.Result == null)
			{
				foreach(ListViewItem item in lvActions.Items)
					item.ImageIndex = item.StateImageIndex = (Int32)UI.ListViewActions.ActionsIcons.Valid;
			}
			this.SetWindowCaption();
		}
		private void lvActions_KeyDown(Object sender, KeyEventArgs e)
		{
			if(!lvActions.IsEnabled)
				return;

			//if(e.KeyData == this.Plugin.Settings.Record || e.KeyData == this.Plugin.Settings.Start)
			//	return;//So as not to interfere with the process launch shortcuts

			switch(e.KeyData)
			{
			case Keys.Control | Keys.Z:
				if(MessageBox.Show("Are you sure you want to revert actions to last saved state?", this.Window.Caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					this.LoadActions();
				e.Handled = true;
				break;
			case Keys.R:
			case Keys.L:
				if(pgActions.SelectedObject is ActionMouse action)
				{
					action.Button = e.KeyCode == Keys.R
						? MouseButtons.Right
						: MouseButtons.Left;
					action.Location = Cursor.Position;
					lvActions.UpdateAction(lvActions.SelectedItems[0], action);
					e.Handled = true;
				}
				break;
			case Keys.Delete:
				this.bnActionRemove_Click(sender, e);
				e.Handled = true;
				break;
			}
		}

		private void LvActions_DirtyChanged(Object sender, EventArgs e)
		{
			bnActionsSave.Enabled = lvActions.IsDirty;

			if(!lvActions.IsDirty)
			{
				this.Settings.SetValues(lvActions.FilePath);
				this.SetWindowCaption();
				tsmiProjectExport.Visible = lvActions.FilePath == null;
				tsmiProjectImport.Visible = lvActions.FilePath != null;
			}
		}

		private void bnActionsSave_Click(Object sender, EventArgs e)
			=> lvActions.SaveProject();

		private void bnActions_DropDownItemClicked(Object sender, ToolStripItemClickedEventArgs e)
		{
			bnActions.DropDown.Close(ToolStripDropDownCloseReason.ItemClicked);

			if(e.ClickedItem == tsmiProjectOpen)
			{
				using(OpenFileDialog dlg = new OpenFileDialog()
				{
					Filter = "Actions file (json) (*.clc)|*.clc|All files (*.*)|*.*",
					DefaultExt = "clc"
				})
					if(dlg.ShowDialog() == DialogResult.OK)
						if(dlg.FileName == this.Settings.ProjectFileName)
							this.Settings.ProjectFileName = dlg.FileName;
						else if(this.Plugin.CreateWindow(
								typeof(PanelWindowClicker).ToString(),
								true,
								new PanelWindowClickerSettings() { ProjectFileName = dlg.FileName }) == null)
							Plugin.Trace.TraceEvent(TraceEventType.Warning, 1, "Error opening window");
			} else if(e.ClickedItem == tsmiProjectExport)
				lvActions.SaveProjectToFile();
			else if(e.ClickedItem == tsmiProjectImport)
				lvActions.SaveProject(null);
			else
				throw new NotImplementedException("Unknown item: " + e.ClickedItem);
		}
	}
}