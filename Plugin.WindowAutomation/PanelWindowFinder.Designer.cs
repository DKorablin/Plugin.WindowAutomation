namespace Plugin.WindowAutomation
{
	partial class PanelWindowFinder
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private UI.TargetWindowCtrl ctlWindowFinder;
		private System.Windows.Forms.ToolTip toolTip;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label lblRect;
			System.Windows.Forms.Label lblCaption;
			System.Windows.Forms.Label lblModuleName;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PanelWindowFinder));
			System.Windows.Forms.Label lblClass;
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.txtClassName = new System.Windows.Forms.TextBox();
			this.txtRect = new System.Windows.Forms.TextBox();
			this.txtCaption = new System.Windows.Forms.TextBox();
			this.ssMain = new System.Windows.Forms.StatusStrip();
			this.tsCursor = new System.Windows.Forms.ToolStripStatusLabel();
			this.tabMain = new System.Windows.Forms.TabControl();
			this.tabFinder = new System.Windows.Forms.TabPage();
			this.tvWindows = new System.Windows.Forms.TreeView();
			this.cmsWindows = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.cmsWindowsRefresh = new System.Windows.Forms.ToolStripMenuItem();
			this.cmsWindowsPrintScreen = new System.Windows.Forms.ToolStripMenuItem();
			this.tabAutomate = new System.Windows.Forms.TabPage();
			this.pgAutomation = new System.Windows.Forms.PropertyGrid();
			this.cbAutomationPattern = new System.Windows.Forms.ComboBox();
			this.txtModuleName = new System.Windows.Forms.TextBox();
			this.gridSearch = new AlphaOmega.Windows.Forms.SearchGrid();
			this.ctlWindowFinder = new Plugin.WindowAutomation.UI.TargetWindowCtrl();
			lblRect = new System.Windows.Forms.Label();
			lblCaption = new System.Windows.Forms.Label();
			lblModuleName = new System.Windows.Forms.Label();
			lblClass = new System.Windows.Forms.Label();
			this.ssMain.SuspendLayout();
			this.tabMain.SuspendLayout();
			this.tabFinder.SuspendLayout();
			this.cmsWindows.SuspendLayout();
			this.tabAutomate.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.ctlWindowFinder)).BeginInit();
			this.SuspendLayout();
			// 
			// lblRect
			// 
			lblRect.Location = new System.Drawing.Point(51, 9);
			lblRect.Name = "lblRect";
			lblRect.Size = new System.Drawing.Size(40, 16);
			lblRect.TabIndex = 11;
			lblRect.Text = "Rect:";
			// 
			// lblCaption
			// 
			lblCaption.Location = new System.Drawing.Point(3, 57);
			lblCaption.Name = "lblCaption";
			lblCaption.Size = new System.Drawing.Size(48, 16);
			lblCaption.TabIndex = 9;
			lblCaption.Text = "Caption:";
			// 
			// txtClassName
			// 
			this.txtClassName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtClassName.Location = new System.Drawing.Point(97, 30);
			this.txtClassName.Name = "txtClassName";
			this.txtClassName.ReadOnly = true;
			this.txtClassName.Size = new System.Drawing.Size(219, 20);
			this.txtClassName.TabIndex = 14;
			// 
			// txtRect
			// 
			this.txtRect.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtRect.Location = new System.Drawing.Point(97, 6);
			this.txtRect.Name = "txtRect";
			this.txtRect.ReadOnly = true;
			this.txtRect.Size = new System.Drawing.Size(219, 20);
			this.txtRect.TabIndex = 12;
			// 
			// txtCaption
			// 
			this.txtCaption.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtCaption.Location = new System.Drawing.Point(59, 54);
			this.txtCaption.Name = "txtCaption";
			this.txtCaption.ReadOnly = true;
			this.txtCaption.Size = new System.Drawing.Size(257, 20);
			this.txtCaption.TabIndex = 10;
			// 
			// ssMain
			// 
			this.ssMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsCursor});
			this.ssMain.Location = new System.Drawing.Point(0, 345);
			this.ssMain.Name = "ssMain";
			this.ssMain.Size = new System.Drawing.Size(330, 22);
			this.ssMain.TabIndex = 15;
			// 
			// tsCursor
			// 
			this.tsCursor.Name = "tsCursor";
			this.tsCursor.Size = new System.Drawing.Size(0, 17);
			// 
			// tabMain
			// 
			this.tabMain.Controls.Add(this.tabFinder);
			this.tabMain.Controls.Add(this.tabAutomate);
			this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabMain.Location = new System.Drawing.Point(0, 0);
			this.tabMain.Name = "tabMain";
			this.tabMain.SelectedIndex = 0;
			this.tabMain.Size = new System.Drawing.Size(330, 345);
			this.tabMain.TabIndex = 16;
			this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
			// 
			// tabFinder
			// 
			this.tabFinder.Controls.Add(lblModuleName);
			this.tabFinder.Controls.Add(this.txtModuleName);
			this.tabFinder.Controls.Add(this.gridSearch);
			this.tabFinder.Controls.Add(this.tvWindows);
			this.tabFinder.Controls.Add(this.ctlWindowFinder);
			this.tabFinder.Controls.Add(lblRect);
			this.tabFinder.Controls.Add(this.txtCaption);
			this.tabFinder.Controls.Add(lblClass);
			this.tabFinder.Controls.Add(this.txtRect);
			this.tabFinder.Controls.Add(lblCaption);
			this.tabFinder.Controls.Add(this.txtClassName);
			this.tabFinder.Location = new System.Drawing.Point(4, 22);
			this.tabFinder.Name = "tabFinder";
			this.tabFinder.Padding = new System.Windows.Forms.Padding(3);
			this.tabFinder.Size = new System.Drawing.Size(322, 319);
			this.tabFinder.TabIndex = 0;
			this.tabFinder.Text = "Finder";
			this.tabFinder.UseVisualStyleBackColor = true;
			// 
			// tvWindows
			// 
			this.tvWindows.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tvWindows.ContextMenuStrip = this.cmsWindows;
			this.tvWindows.HideSelection = false;
			this.tvWindows.Location = new System.Drawing.Point(3, 106);
			this.tvWindows.Name = "tvWindows";
			this.tvWindows.Size = new System.Drawing.Size(316, 210);
			this.tvWindows.TabIndex = 17;
			this.tvWindows.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvWindows_BeforeExpand);
			this.tvWindows.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvWindows_AfterSelect);
			this.tvWindows.MouseClick += new System.Windows.Forms.MouseEventHandler(this.tvWindows_MouseClick);
			this.tvWindows.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.tvWindows_MouseDoubleClick);
			// 
			// cmsWindows
			// 
			this.cmsWindows.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmsWindowsRefresh,
            this.cmsWindowsPrintScreen});
			this.cmsWindows.Name = "cmsWindows";
			this.cmsWindows.Size = new System.Drawing.Size(138, 48);
			this.cmsWindows.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsWindows_ItemClicked);
			// 
			// cmsWindowsRefresh
			// 
			this.cmsWindowsRefresh.Name = "cmsWindowsRefresh";
			this.cmsWindowsRefresh.Size = new System.Drawing.Size(137, 22);
			this.cmsWindowsRefresh.Text = "&Refresh";
			// 
			// cmsWindowsPrintScreen
			// 
			this.cmsWindowsPrintScreen.Name = "cmsWindowsPrintScreen";
			this.cmsWindowsPrintScreen.Size = new System.Drawing.Size(137, 22);
			this.cmsWindowsPrintScreen.Text = "Print &Screen";
			// 
			// tabAutomate
			// 
			this.tabAutomate.Controls.Add(this.pgAutomation);
			this.tabAutomate.Controls.Add(this.cbAutomationPattern);
			this.tabAutomate.Location = new System.Drawing.Point(4, 22);
			this.tabAutomate.Name = "tabAutomate";
			this.tabAutomate.Padding = new System.Windows.Forms.Padding(3);
			this.tabAutomate.Size = new System.Drawing.Size(322, 319);
			this.tabAutomate.TabIndex = 1;
			this.tabAutomate.Text = "Automate";
			this.tabAutomate.UseVisualStyleBackColor = true;
			// 
			// pgAutomation
			// 
			this.pgAutomation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgAutomation.Location = new System.Drawing.Point(3, 24);
			this.pgAutomation.Name = "pgAutomation";
			this.pgAutomation.Size = new System.Drawing.Size(316, 292);
			this.pgAutomation.TabIndex = 0;
			// 
			// cbAutomationPattern
			// 
			this.cbAutomationPattern.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbAutomationPattern.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbAutomationPattern.FormattingEnabled = true;
			this.cbAutomationPattern.Location = new System.Drawing.Point(3, 3);
			this.cbAutomationPattern.Name = "cbAutomationPattern";
			this.cbAutomationPattern.Size = new System.Drawing.Size(316, 21);
			this.cbAutomationPattern.TabIndex = 1;
			this.cbAutomationPattern.SelectedIndexChanged += new System.EventHandler(this.cbAutomationPattern_SelectedIndexChanged);
			// 
			// txtModuleName
			// 
			this.txtModuleName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtModuleName.Location = new System.Drawing.Point(59, 80);
			this.txtModuleName.Name = "txtModuleName";
			this.txtModuleName.ReadOnly = true;
			this.txtModuleName.Size = new System.Drawing.Size(257, 20);
			this.txtModuleName.TabIndex = 18;
			// 
			// lblModuleName
			// 
			lblModuleName.AutoSize = true;
			lblModuleName.Location = new System.Drawing.Point(3, 83);
			lblModuleName.Name = "lblModuleName";
			lblModuleName.Size = new System.Drawing.Size(45, 13);
			lblModuleName.TabIndex = 19;
			lblModuleName.Text = "Module:";
			// 
			// gridSearch
			// 
			this.gridSearch.DataGrid = null;
			this.gridSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.gridSearch.EnableFindCase = true;
			this.gridSearch.EnableFindHilight = true;
			this.gridSearch.EnableFindPrevNext = true;
			this.gridSearch.EnableSearchHilight = false;
			this.gridSearch.ListView = null;
			this.gridSearch.Location = new System.Drawing.Point(3, 155);
			this.gridSearch.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.gridSearch.Name = "gridSearch";
			this.gridSearch.Size = new System.Drawing.Size(440, 29);
			this.gridSearch.TabIndex = 1;
			this.gridSearch.TreeView = null;
			this.gridSearch.Visible = false;
			// 
			// ctlWindowFinder
			// 
			this.ctlWindowFinder.Cursor = System.Windows.Forms.Cursors.Hand;
			this.ctlWindowFinder.Image = ((System.Drawing.Image)(resources.GetObject("ctlWindowFinder.Image")));
			this.ctlWindowFinder.Location = new System.Drawing.Point(11, 13);
			this.ctlWindowFinder.Name = "ctlWindowFinder";
			this.ctlWindowFinder.Size = new System.Drawing.Size(31, 28);
			this.ctlWindowFinder.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.ctlWindowFinder.TabIndex = 0;
			this.ctlWindowFinder.TabStop = false;
			this.toolTip.SetToolTip(this.ctlWindowFinder, "Start dragging ...");
			// 
			// lblClass
			// 
			lblClass.Location = new System.Drawing.Point(51, 33);
			lblClass.Name = "lblClass";
			lblClass.Size = new System.Drawing.Size(40, 16);
			lblClass.TabIndex = 13;
			lblClass.Text = "Class:";
			// 
			// PanelWindowFinder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tabMain);
			this.Controls.Add(this.ssMain);
			this.Name = "PanelWindowFinder";
			this.Size = new System.Drawing.Size(330, 367);
			this.ssMain.ResumeLayout(false);
			this.ssMain.PerformLayout();
			this.tabMain.ResumeLayout(false);
			this.tabFinder.ResumeLayout(false);
			this.tabFinder.PerformLayout();
			this.cmsWindows.ResumeLayout(false);
			this.tabAutomate.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.ctlWindowFinder)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox txtClassName;
		private System.Windows.Forms.TextBox txtRect;
		private System.Windows.Forms.TextBox txtCaption;
		private System.Windows.Forms.StatusStrip ssMain;
		private System.Windows.Forms.ToolStripStatusLabel tsCursor;
		private System.Windows.Forms.TabControl tabMain;
		private System.Windows.Forms.TabPage tabFinder;
		private System.Windows.Forms.TabPage tabAutomate;
		private System.Windows.Forms.TreeView tvWindows;
		private System.Windows.Forms.ContextMenuStrip cmsWindows;
		private System.Windows.Forms.PropertyGrid pgAutomation;
		private System.Windows.Forms.ComboBox cbAutomationPattern;
		private System.Windows.Forms.ToolStripMenuItem cmsWindowsRefresh;
		private System.Windows.Forms.ToolStripMenuItem cmsWindowsPrintScreen;
		private AlphaOmega.Windows.Forms.SearchGrid gridSearch;
		private System.Windows.Forms.TextBox txtModuleName;
	}
}
