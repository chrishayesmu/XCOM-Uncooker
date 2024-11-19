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
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtSourcePath = new TextBox();
            label1 = new Label();
            btnOpenSourceFolder = new Button();
            groupBox1 = new GroupBox();
            btnFullyLoadArchives = new Button();
            btnSelectNoInputArchives = new Button();
            splitContainer1 = new SplitContainer();
            lstInputArchives = new CheckedListBox();
            treeViewExportObjects = new TreeView();
            btnSelectAllInputArchives = new Button();
            dlgOpenSourceFolder = new FolderBrowserDialog();
            groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
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
            // groupBox1
            // 
            groupBox1.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(btnFullyLoadArchives);
            groupBox1.Controls.Add(btnSelectNoInputArchives);
            groupBox1.Controls.Add(splitContainer1);
            groupBox1.Controls.Add(btnSelectAllInputArchives);
            groupBox1.Location = new Point(12, 45);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(776, 433);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Cooked assets";
            // 
            // btnFullyLoadArchives
            // 
            btnFullyLoadArchives.Enabled = false;
            btnFullyLoadArchives.Location = new Point(6, 404);
            btnFullyLoadArchives.Name = "btnFullyLoadArchives";
            btnFullyLoadArchives.Size = new Size(168, 23);
            btnFullyLoadArchives.TabIndex = 5;
            btnFullyLoadArchives.Text = "Fully Load Selected Archives";
            btnFullyLoadArchives.UseVisualStyleBackColor = true;
            btnFullyLoadArchives.Click += btnFullyLoadArchives_Click;
            // 
            // btnSelectNoInputArchives
            // 
            btnSelectNoInputArchives.Enabled = false;
            btnSelectNoInputArchives.Location = new Point(93, 375);
            btnSelectNoInputArchives.Name = "btnSelectNoInputArchives";
            btnSelectNoInputArchives.Size = new Size(81, 23);
            btnSelectNoInputArchives.TabIndex = 7;
            btnSelectNoInputArchives.Text = "Select None";
            btnSelectNoInputArchives.UseVisualStyleBackColor = true;
            btnSelectNoInputArchives.Click += btnSelectNoInputArchives_Click;
            // 
            // splitContainer1
            // 
            splitContainer1.Location = new Point(6, 22);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lstInputArchives);
            splitContainer1.Panel1MinSize = 275;
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(treeViewExportObjects);
            splitContainer1.Size = new Size(764, 347);
            splitContainer1.SplitterDistance = 275;
            splitContainer1.TabIndex = 5;
            // 
            // lstInputArchives
            // 
            lstInputArchives.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstInputArchives.CheckOnClick = true;
            lstInputArchives.FormattingEnabled = true;
            lstInputArchives.IntegralHeight = false;
            lstInputArchives.Location = new Point(0, 0);
            lstInputArchives.Name = "lstInputArchives";
            lstInputArchives.Size = new Size(275, 347);
            lstInputArchives.TabIndex = 0;
            lstInputArchives.SelectedIndexChanged += lstInputArchives_SelectedIndexChanged;
            // 
            // treeViewExportObjects
            // 
            treeViewExportObjects.Anchor =  AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            treeViewExportObjects.Location = new Point(0, 0);
            treeViewExportObjects.Name = "treeViewExportObjects";
            treeViewExportObjects.Size = new Size(485, 347);
            treeViewExportObjects.TabIndex = 0;
            // 
            // btnSelectAllInputArchives
            // 
            btnSelectAllInputArchives.Enabled = false;
            btnSelectAllInputArchives.Location = new Point(6, 375);
            btnSelectAllInputArchives.Name = "btnSelectAllInputArchives";
            btnSelectAllInputArchives.Size = new Size(81, 23);
            btnSelectAllInputArchives.TabIndex = 6;
            btnSelectAllInputArchives.Text = "Select All";
            btnSelectAllInputArchives.UseVisualStyleBackColor = true;
            btnSelectAllInputArchives.Click += btnSelectAllInputArchives_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 531);
            Controls.Add(groupBox1);
            Controls.Add(btnOpenSourceFolder);
            Controls.Add(label1);
            Controls.Add(txtSourcePath);
            Name = "Form1";
            Text = "Form1";
            groupBox1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSourcePath;
        private Label label1;
        private Button btnOpenSourceFolder;
        private GroupBox groupBox1;
        private SplitContainer splitContainer1;
        private TreeView treeViewExportObjects;
        private FolderBrowserDialog dlgOpenSourceFolder;
        private Button btnFullyLoadArchives;
        private CheckedListBox lstInputArchives;
        private Button btnSelectNoInputArchives;
        private Button btnSelectAllInputArchives;
    }
}
