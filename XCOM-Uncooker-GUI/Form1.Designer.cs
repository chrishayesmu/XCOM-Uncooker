namespace XCOM_Uncooker_GUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            xcomEwArchiveManager?.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            txtSourcePath = new TextBox();
            label1 = new Label();
            btnOpenSourceFolder = new Button();
            dlgOpenSourceFolder = new FolderBrowserDialog();
            btnSelectAllInputArchives = new Button();
            treeViewExportObjects = new TreeView();
            lstInputArchives = new CheckedListBox();
            txtInputArchivesFilter = new TextBox();
            btnSelectNoInputArchives = new Button();
            btnFullyLoadArchives = new Button();
            splitContainer1 = new SplitContainer();
            flowLayoutPanel1 = new FlowLayoutPanel();
            btnUncookArchives = new Button();
            statusStrip = new StatusStrip();
            toolStripProgressBar = new ToolStripProgressBar();
            toolStripStatusLabel = new ToolStripStatusLabel();
            toolStripTimer = new ToolStripStatusLabel();
            dlgChooseUncookDestination = new FolderBrowserDialog();
            timerEachSecond = new System.Windows.Forms.Timer(components);
            txtWotcSdkPath = new TextBox();
            btnOpenWotcSdkPath = new Button();
            label2 = new Label();
            dlgOpenWotcSdkFolder = new FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // txtSourcePath
            // 
            txtSourcePath.Location = new Point(126, 6);
            txtSourcePath.Name = "txtSourcePath";
            txtSourcePath.ReadOnly = true;
            txtSourcePath.Size = new Size(376, 23);
            txtSourcePath.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(108, 15);
            label1.TabIndex = 1;
            label1.Text = "Load archives from";
            // 
            // btnOpenSourceFolder
            // 
            btnOpenSourceFolder.Location = new Point(508, 6);
            btnOpenSourceFolder.Name = "btnOpenSourceFolder";
            btnOpenSourceFolder.Size = new Size(75, 23);
            btnOpenSourceFolder.TabIndex = 2;
            btnOpenSourceFolder.Text = "Browse..";
            btnOpenSourceFolder.UseVisualStyleBackColor = true;
            btnOpenSourceFolder.Click += btnOpenSourceFolder_Click;
            // 
            // dlgOpenSourceFolder
            // 
            dlgOpenSourceFolder.Description = "Choose the XEW folder within your XCOM install";
            dlgOpenSourceFolder.ShowNewFolderButton = false;
            dlgOpenSourceFolder.UseDescriptionForTitle = true;
            // 
            // btnSelectAllInputArchives
            // 
            btnSelectAllInputArchives.Enabled = false;
            btnSelectAllInputArchives.Location = new Point(3, 407);
            btnSelectAllInputArchives.Name = "btnSelectAllInputArchives";
            btnSelectAllInputArchives.Size = new Size(131, 23);
            btnSelectAllInputArchives.TabIndex = 6;
            btnSelectAllInputArchives.Text = "Select All";
            btnSelectAllInputArchives.UseVisualStyleBackColor = true;
            btnSelectAllInputArchives.Click += btnSelectAllInputArchives_Click;
            // 
            // treeViewExportObjects
            // 
            treeViewExportObjects.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeViewExportObjects.Location = new Point(3, 3);
            treeViewExportObjects.Name = "treeViewExportObjects";
            treeViewExportObjects.Size = new Size(535, 515);
            treeViewExportObjects.TabIndex = 0;
            // 
            // lstInputArchives
            // 
            lstInputArchives.FormattingEnabled = true;
            lstInputArchives.IntegralHeight = false;
            lstInputArchives.Location = new Point(3, 3);
            lstInputArchives.Name = "lstInputArchives";
            lstInputArchives.Size = new Size(269, 369);
            lstInputArchives.TabIndex = 0;
            lstInputArchives.ItemCheck += lstInputArchives_ItemCheck;
            lstInputArchives.SelectedIndexChanged += lstInputArchives_SelectedIndexChanged;
            // 
            // txtInputArchivesFilter
            // 
            txtInputArchivesFilter.Location = new Point(3, 378);
            txtInputArchivesFilter.Name = "txtInputArchivesFilter";
            txtInputArchivesFilter.PlaceholderText = "Filter archives..";
            txtInputArchivesFilter.Size = new Size(269, 23);
            txtInputArchivesFilter.TabIndex = 1;
            txtInputArchivesFilter.TextChanged += txtInputArchivesFilter_TextChanged;
            // 
            // btnSelectNoInputArchives
            // 
            btnSelectNoInputArchives.Enabled = false;
            btnSelectNoInputArchives.Location = new Point(140, 407);
            btnSelectNoInputArchives.Name = "btnSelectNoInputArchives";
            btnSelectNoInputArchives.Size = new Size(132, 23);
            btnSelectNoInputArchives.TabIndex = 7;
            btnSelectNoInputArchives.Text = "Select None";
            btnSelectNoInputArchives.UseVisualStyleBackColor = true;
            btnSelectNoInputArchives.Click += btnSelectNoInputArchives_Click;
            // 
            // btnFullyLoadArchives
            // 
            btnFullyLoadArchives.Enabled = false;
            btnFullyLoadArchives.Location = new Point(3, 436);
            btnFullyLoadArchives.Name = "btnFullyLoadArchives";
            btnFullyLoadArchives.Size = new Size(269, 23);
            btnFullyLoadArchives.TabIndex = 5;
            btnFullyLoadArchives.Text = "Fully Load Selected Archives";
            btnFullyLoadArchives.UseVisualStyleBackColor = true;
            btnFullyLoadArchives.Click += btnFullyLoadArchives_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.FixedPanel = FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new Point(12, 65);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(flowLayoutPanel1);
            splitContainer1.Panel1MinSize = 275;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(treeViewExportObjects);
            splitContainer1.Size = new Size(820, 521);
            splitContainer1.SplitterDistance = 275;
            splitContainer1.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flowLayoutPanel1.Controls.Add(lstInputArchives);
            flowLayoutPanel1.Controls.Add(txtInputArchivesFilter);
            flowLayoutPanel1.Controls.Add(btnSelectAllInputArchives);
            flowLayoutPanel1.Controls.Add(btnSelectNoInputArchives);
            flowLayoutPanel1.Controls.Add(btnFullyLoadArchives);
            flowLayoutPanel1.Controls.Add(btnUncookArchives);
            flowLayoutPanel1.Location = new Point(0, 0);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(275, 518);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // btnUncookArchives
            // 
            btnUncookArchives.Location = new Point(3, 465);
            btnUncookArchives.Name = "btnUncookArchives";
            btnUncookArchives.Size = new Size(269, 23);
            btnUncookArchives.TabIndex = 8;
            btnUncookArchives.Text = "Uncook Loaded Archives..";
            btnUncookArchives.UseVisualStyleBackColor = true;
            btnUncookArchives.Click += btnUncookArchives_Click;
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { toolStripProgressBar, toolStripStatusLabel, toolStripTimer });
            statusStrip.Location = new Point(0, 589);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(844, 22);
            statusStrip.SizingGrip = false;
            statusStrip.TabIndex = 9;
            statusStrip.Text = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.Size = new Size(100, 16);
            toolStripProgressBar.Step = 1;
            toolStripProgressBar.Style = ProgressBarStyle.Continuous;
            // 
            // toolStripStatusLabel
            // 
            toolStripStatusLabel.Name = "toolStripStatusLabel";
            toolStripStatusLabel.Size = new Size(0, 17);
            // 
            // toolStripTimer
            // 
            toolStripTimer.Name = "toolStripTimer";
            toolStripTimer.Size = new Size(0, 17);
            // 
            // timerEachSecond
            // 
            timerEachSecond.Enabled = true;
            timerEachSecond.Interval = 1000;
            // 
            // txtWotcSdkPath
            // 
            txtWotcSdkPath.Location = new Point(126, 35);
            txtWotcSdkPath.Name = "txtWotcSdkPath";
            txtWotcSdkPath.ReadOnly = true;
            txtWotcSdkPath.Size = new Size(376, 23);
            txtWotcSdkPath.TabIndex = 10;
            // 
            // btnOpenWotcSdkPath
            // 
            btnOpenWotcSdkPath.Location = new Point(508, 35);
            btnOpenWotcSdkPath.Name = "btnOpenWotcSdkPath";
            btnOpenWotcSdkPath.Size = new Size(75, 23);
            btnOpenWotcSdkPath.TabIndex = 11;
            btnOpenWotcSdkPath.Text = "Browse..";
            btnOpenWotcSdkPath.UseVisualStyleBackColor = true;
            btnOpenWotcSdkPath.Click += btnOpenWotcSdkPath_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 39);
            label2.Name = "label2";
            label2.Size = new Size(108, 15);
            label2.TabIndex = 12;
            label2.Text = "XCOM 2 WotC SDK";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(844, 611);
            Controls.Add(label2);
            Controls.Add(btnOpenWotcSdkPath);
            Controls.Add(txtWotcSdkPath);
            Controls.Add(statusStrip);
            Controls.Add(splitContainer1);
            Controls.Add(btnOpenSourceFolder);
            Controls.Add(label1);
            Controls.Add(txtSourcePath);
            MinimumSize = new Size(620, 595);
            Name = "Form1";
            Text = "XCOM Uncooker";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSourcePath;
        private Label label1;
        private Button btnOpenSourceFolder;
        private FolderBrowserDialog dlgOpenSourceFolder;
        private Button btnSelectAllInputArchives;
        private TreeView treeViewExportObjects;
        private CheckedListBox lstInputArchives;
        private TextBox txtInputArchivesFilter;
        private Button btnSelectNoInputArchives;
        private Button btnFullyLoadArchives;
        private SplitContainer splitContainer1;
        private FlowLayoutPanel flowLayoutPanel1;
        private StatusStrip statusStrip;
        private ToolStripProgressBar toolStripProgressBar;
        private ToolStripStatusLabel toolStripStatusLabel;
        private Button btnUncookArchives;
        private FolderBrowserDialog dlgChooseUncookDestination;
        private ToolStripStatusLabel toolStripTimer;
        private System.Windows.Forms.Timer timerEachSecond;
        private TextBox txtWotcSdkPath;
        private Button btnOpenWotcSdkPath;
        private Label label2;
        private FolderBrowserDialog dlgOpenWotcSdkFolder;
    }
}
