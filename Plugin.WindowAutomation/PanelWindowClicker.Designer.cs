namespace Plugin.WindowAutomation
{
	partial class PanelWindowClicker
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelWindowClicker));
			this.tsMain = new System.Windows.Forms.ToolStrip();
			this.ddlActionAdd = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiActionMouse = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiActionKey = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiActionKeysArray = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiActionMethod = new System.Windows.Forms.ToolStripMenuItem();
			this.bnActionRemove = new System.Windows.Forms.ToolStripButton();
			this.bnActionsStart = new System.Windows.Forms.ToolStripButton();
			this.bnActionsRecord = new System.Windows.Forms.ToolStripButton();
			this.bnActionsSave = new System.Windows.Forms.ToolStripButton();
			this.bnActions = new System.Windows.Forms.ToolStripDropDownButton();
			this.tsmiProjectOpen = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectExport = new System.Windows.Forms.ToolStripMenuItem();
			this.tsmiProjectImport = new System.Windows.Forms.ToolStripMenuItem();
			this.pgActions = new System.Windows.Forms.PropertyGrid();
			this.splitMain = new System.Windows.Forms.SplitContainer();
			this.lvActions = new global::Plugin.WindowAutomation.UI.ListViewActions();
			this.ilActions = new System.Windows.Forms.ImageList(this.components);
			this.bwClicker = new System.ComponentModel.BackgroundWorker();
			toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.tsMain.SuspendLayout();
			this.splitMain.Panel1.SuspendLayout();
			this.splitMain.Panel2.SuspendLayout();
			this.splitMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new System.Drawing.Size(149, 6);
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// tsMain
			// 
			this.tsMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this.tsMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ddlActionAdd,
            this.bnActionRemove,
            toolStripSeparator1,
            this.bnActionsStart,
            this.bnActionsRecord,
            this.bnActionsSave,
            this.bnActions});
			this.tsMain.Location = new System.Drawing.Point(0, 0);
			this.tsMain.Name = "tsMain";
			this.tsMain.Size = new System.Drawing.Size(202, 25);
			this.tsMain.TabIndex = 1;
			// 
			// ddlActionAdd
			// 
			this.ddlActionAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ddlActionAdd.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiActionMouse,
            this.tsmiActionKey,
            this.tsmiActionKeysArray,
            this.tsmiActionMethod});
			this.ddlActionAdd.Image = global::Plugin.WindowAutomation.Properties.Resources.add;
			this.ddlActionAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ddlActionAdd.Name = "ddlActionAdd";
			this.ddlActionAdd.Size = new System.Drawing.Size(29, 22);
			this.ddlActionAdd.Text = "Add Action";
			this.ddlActionAdd.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.ddlAddAction_DropDownItemClicked);
			// 
			// tsmiActionMouse
			// 
			this.tsmiActionMouse.Name = "tsmiActionMouse";
			this.tsmiActionMouse.Size = new System.Drawing.Size(139, 22);
			this.tsmiActionMouse.Text = "Mouse &Click";
			// 
			// tsmiActionKey
			// 
			this.tsmiActionKey.Name = "tsmiActionKey";
			this.tsmiActionKey.Size = new System.Drawing.Size(139, 22);
			this.tsmiActionKey.Text = "Key &Press";
			// 
			// tsmiActionKeysArray
			// 
			this.tsmiActionKeysArray.Name = "tsmiActionKeysArray";
			this.tsmiActionKeysArray.Size = new System.Drawing.Size(139, 22);
			this.tsmiActionKeysArray.Text = "Keys &Array";
			// 
			// tsmiActionMethod
			// 
			this.tsmiActionMethod.Name = "tsmiActionMethod";
			this.tsmiActionMethod.Size = new System.Drawing.Size(139, 22);
			this.tsmiActionMethod.Text = "&Method";
			// 
			// bnActionRemove
			// 
			this.bnActionRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnActionRemove.Enabled = false;
			this.bnActionRemove.Image = global::Plugin.WindowAutomation.Properties.Resources.iconDelete;
			this.bnActionRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnActionRemove.Name = "bnActionRemove";
			this.bnActionRemove.Size = new System.Drawing.Size(23, 22);
			this.bnActionRemove.Text = "Remove";
			this.bnActionRemove.ToolTipText = "Remove action";
			this.bnActionRemove.Click += new System.EventHandler(this.bnActionRemove_Click);
			// 
			// bnActionsStart
			// 
			this.bnActionsStart.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.bnActionsStart.CheckOnClick = true;
			this.bnActionsStart.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnActionsStart.Image = global::Plugin.WindowAutomation.Properties.Resources.iconDebug;
			this.bnActionsStart.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnActionsStart.Name = "bnActionsStart";
			this.bnActionsStart.Size = new System.Drawing.Size(23, 22);
			this.bnActionsStart.Text = "Start";
			this.bnActionsStart.CheckedChanged += new System.EventHandler(this.bnActionsRun_CheckedChanged);
			// 
			// bnActionsRecord
			// 
			this.bnActionsRecord.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.bnActionsRecord.CheckOnClick = true;
			this.bnActionsRecord.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnActionsRecord.Image = global::Plugin.WindowAutomation.Properties.Resources.record;
			this.bnActionsRecord.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnActionsRecord.Name = "bnActionsRecord";
			this.bnActionsRecord.Size = new System.Drawing.Size(23, 22);
			this.bnActionsRecord.Text = "Record";
			this.bnActionsRecord.Click += new System.EventHandler(this.bnActionsRecord_Click);
			// 
			// bnActionsSave
			// 
			this.bnActionsSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnActionsSave.Image = global::Plugin.WindowAutomation.Properties.Resources.FileSave;
			this.bnActionsSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnActionsSave.Name = "bnActionsSave";
			this.bnActionsSave.Size = new System.Drawing.Size(23, 22);
			this.bnActionsSave.Text = "Save";
			this.bnActionsSave.Click += new System.EventHandler(this.bnActionsSave_Click);
			// 
			// bnActions
			// 
			this.bnActions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.bnActions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiProjectOpen,
            toolStripSeparator2,
            this.tsmiProjectExport,
            this.tsmiProjectImport});
			this.bnActions.Image = global::Plugin.WindowAutomation.Properties.Resources.iconOpen;
			this.bnActions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.bnActions.Name = "bnActions";
			this.bnActions.Size = new System.Drawing.Size(29, 22);
			this.bnActions.Text = "Open";
			this.bnActions.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.bnActions_DropDownItemClicked);
			// 
			// tsmiProjectOpen
			// 
			this.tsmiProjectOpen.Image = global::Plugin.WindowAutomation.Properties.Resources.iconOpen;
			this.tsmiProjectOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.tsmiProjectOpen.Name = "tsmiProjectOpen";
			this.tsmiProjectOpen.Size = new System.Drawing.Size(152, 22);
			this.tsmiProjectOpen.Text = "&Open...";
			// 
			// tsmiProjectExport
			// 
			this.tsmiProjectExport.Name = "tsmiProjectExport";
			this.tsmiProjectExport.Size = new System.Drawing.Size(152, 22);
			this.tsmiProjectExport.Text = "&Export...";
			// 
			// tsmiProjectImport
			// 
			this.tsmiProjectImport.Name = "tsmiProjectImport";
			this.tsmiProjectImport.Size = new System.Drawing.Size(152, 22);
			this.tsmiProjectImport.Text = "&Import";
			// 
			// pgActions
			// 
			this.pgActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgActions.Location = new System.Drawing.Point(0, 0);
			this.pgActions.Name = "pgActions";
			this.pgActions.Size = new System.Drawing.Size(202, 59);
			this.pgActions.TabIndex = 2;
			this.pgActions.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgActions_PropertyValueChanged);
			// 
			// splitMain
			// 
			this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitMain.Location = new System.Drawing.Point(0, 25);
			this.splitMain.Name = "splitMain";
			this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitMain.Panel1
			// 
			this.splitMain.Panel1.Controls.Add(this.lvActions);
			// 
			// splitMain.Panel2
			// 
			this.splitMain.Panel2.Controls.Add(this.pgActions);
			this.splitMain.Size = new System.Drawing.Size(202, 125);
			this.splitMain.SplitterDistance = 62;
			this.splitMain.TabIndex = 3;
			// 
			// lvActions
			// 
			this.lvActions.AllowDrop = true;
			this.lvActions.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvActions.FullRowSelect = true;
			this.lvActions.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.lvActions.HideSelection = false;
			this.lvActions.IsEnabled = true;
			this.lvActions.Location = new System.Drawing.Point(0, 0);
			this.lvActions.Name = "lvActions";
			this.lvActions.Plugin = null;
			this.lvActions.Size = new System.Drawing.Size(202, 62);
			this.lvActions.StateImageList = this.ilActions;
			this.lvActions.TabIndex = 0;
			this.lvActions.UseCompatibleStateImageBehavior = false;
			this.lvActions.View = System.Windows.Forms.View.Details;
			this.lvActions.DirtyChanged += new System.EventHandler<System.EventArgs>(this.LvActions_DirtyChanged);
			this.lvActions.SelectedIndexChanged += new System.EventHandler(this.lvActions_SelectedIndexChanged);
			this.lvActions.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lvActions_KeyDown);
			// 
			// ilActions
			// 
			this.ilActions.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilActions.ImageStream")));
			this.ilActions.TransparentColor = System.Drawing.Color.Magenta;
			this.ilActions.Images.SetKeyName(0, "Pending");
			this.ilActions.Images.SetKeyName(1, "Valid");
			this.ilActions.Images.SetKeyName(2, "Invalid");
			this.ilActions.Images.SetKeyName(3, "Active");
			// 
			// bwClicker
			// 
			this.bwClicker.WorkerReportsProgress = true;
			this.bwClicker.WorkerSupportsCancellation = true;
			this.bwClicker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bwClicker_DoWork);
			this.bwClicker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bwClicker_ProgressChanged);
			this.bwClicker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bwClicker_RunWorkerCompleted);
			// 
			// PanelWindowClicker
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitMain);
			this.Controls.Add(this.tsMain);
			this.Name = "PanelWindowClicker";
			this.Size = new System.Drawing.Size(202, 150);
			this.tsMain.ResumeLayout(false);
			this.tsMain.PerformLayout();
			this.splitMain.Panel1.ResumeLayout(false);
			this.splitMain.Panel2.ResumeLayout(false);
			this.splitMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.ToolStrip tsMain;
		private System.Windows.Forms.ToolStripDropDownButton ddlActionAdd;
		private System.Windows.Forms.ToolStripMenuItem tsmiActionMouse;
		private System.Windows.Forms.ToolStripMenuItem tsmiActionKey;
		private System.Windows.Forms.PropertyGrid pgActions;
		private System.Windows.Forms.ToolStripButton bnActionRemove;
		private System.Windows.Forms.SplitContainer splitMain;
		private UI.ListViewActions lvActions;
		private System.Windows.Forms.ToolStripButton bnActionsStart;
		private System.ComponentModel.BackgroundWorker bwClicker;
		private System.Windows.Forms.ImageList ilActions;
		private System.Windows.Forms.ToolStripMenuItem tsmiActionKeysArray;
		private System.Windows.Forms.ToolStripMenuItem tsmiActionMethod;
		private System.Windows.Forms.ToolStripButton bnActionsRecord;
		private System.Windows.Forms.ToolStripButton bnActionsSave;
		private System.Windows.Forms.ToolStripDropDownButton bnActions;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectOpen;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectExport;
		private System.Windows.Forms.ToolStripMenuItem tsmiProjectImport;
	}
}
