namespace XCOM_Uncooker_GUI
{
    partial class UncookForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lstSourceArchives = new ListBox();
            label1 = new Label();
            groupBox1 = new GroupBox();
            lstOutputArchivesSelected = new ListBox();
            label2 = new Label();
            btnDeselectOutputArchives = new Button();
            btnSelectOutputArchives = new Button();
            lstOutputArchivesUnselected = new ListBox();
            label3 = new Label();
            btnStartUncookingProcess = new Button();
            dlgChooseUncookDestination = new FolderBrowserDialog();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // lstSourceArchives
            // 
            lstSourceArchives.FormattingEnabled = true;
            lstSourceArchives.IntegralHeight = false;
            lstSourceArchives.ItemHeight = 15;
            lstSourceArchives.Location = new Point(12, 27);
            lstSourceArchives.Name = "lstSourceArchives";
            lstSourceArchives.SelectionMode = SelectionMode.None;
            lstSourceArchives.Size = new Size(224, 491);
            lstSourceArchives.Sorted = true;
            lstSourceArchives.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(89, 15);
            label1.TabIndex = 1;
            label1.Text = "Source archives";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(lstOutputArchivesSelected);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(btnDeselectOutputArchives);
            groupBox1.Controls.Add(btnSelectOutputArchives);
            groupBox1.Controls.Add(lstOutputArchivesUnselected);
            groupBox1.Location = new Point(260, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(520, 510);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Output archives";
            // 
            // lstOutputArchivesSelected
            // 
            lstOutputArchivesSelected.FormattingEnabled = true;
            lstOutputArchivesSelected.IntegralHeight = false;
            lstOutputArchivesSelected.ItemHeight = 15;
            lstOutputArchivesSelected.Location = new Point(284, 35);
            lstOutputArchivesSelected.Name = "lstOutputArchivesSelected";
            lstOutputArchivesSelected.SelectionMode = SelectionMode.MultiExtended;
            lstOutputArchivesSelected.Size = new Size(228, 469);
            lstOutputArchivesSelected.Sorted = true;
            lstOutputArchivesSelected.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(88, 19);
            label2.Name = "label2";
            label2.Size = new Size(55, 15);
            label2.TabIndex = 3;
            label2.Text = "Available";
            // 
            // btnDeselectOutputArchives
            // 
            btnDeselectOutputArchives.Location = new Point(238, 258);
            btnDeselectOutputArchives.Name = "btnDeselectOutputArchives";
            btnDeselectOutputArchives.Size = new Size(40, 23);
            btnDeselectOutputArchives.TabIndex = 2;
            btnDeselectOutputArchives.Text = "<<";
            btnDeselectOutputArchives.UseVisualStyleBackColor = true;
            btnDeselectOutputArchives.Click += btnDeselectOutputArchives_Click;
            // 
            // btnSelectOutputArchives
            // 
            btnSelectOutputArchives.Location = new Point(238, 229);
            btnSelectOutputArchives.Name = "btnSelectOutputArchives";
            btnSelectOutputArchives.Size = new Size(40, 23);
            btnSelectOutputArchives.TabIndex = 1;
            btnSelectOutputArchives.Text = ">>";
            btnSelectOutputArchives.UseVisualStyleBackColor = true;
            btnSelectOutputArchives.Click += btnSelectOutputArchives_Click;
            // 
            // lstOutputArchivesUnselected
            // 
            lstOutputArchivesUnselected.FormattingEnabled = true;
            lstOutputArchivesUnselected.IntegralHeight = false;
            lstOutputArchivesUnselected.ItemHeight = 15;
            lstOutputArchivesUnselected.Location = new Point(6, 37);
            lstOutputArchivesUnselected.Name = "lstOutputArchivesUnselected";
            lstOutputArchivesUnselected.SelectionMode = SelectionMode.MultiExtended;
            lstOutputArchivesUnselected.Size = new Size(228, 469);
            lstOutputArchivesUnselected.Sorted = true;
            lstOutputArchivesUnselected.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(335, 19);
            label3.Name = "label3";
            label3.Size = new Size(129, 15);
            label3.TabIndex = 5;
            label3.Text = "Selected for uncooking";
            // 
            // btnStartUncookingProcess
            // 
            btnStartUncookingProcess.Location = new Point(325, 539);
            btnStartUncookingProcess.Name = "btnStartUncookingProcess";
            btnStartUncookingProcess.Size = new Size(122, 23);
            btnStartUncookingProcess.TabIndex = 3;
            btnStartUncookingProcess.Text = "Start Uncooking..";
            btnStartUncookingProcess.UseVisualStyleBackColor = true;
            btnStartUncookingProcess.Click += btnStartUncookingProcess_Click;
            // 
            // dlgChooseUncookDestination
            // 
            dlgChooseUncookDestination.Description = "Choose directory for uncooker output";
            dlgChooseUncookDestination.UseDescriptionForTitle = true;
            // 
            // UncookForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(798, 574);
            Controls.Add(btnStartUncookingProcess);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Controls.Add(lstSourceArchives);
            Name = "UncookForm";
            Text = "Uncooking Options";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox lstSourceArchives;
        private Label label1;
        private GroupBox groupBox1;
        private ListBox lstOutputArchivesSelected;
        private Label label2;
        private Button btnDeselectOutputArchives;
        private Button btnSelectOutputArchives;
        private ListBox lstOutputArchivesUnselected;
        private Label label3;
        private Button btnStartUncookingProcess;
        private FolderBrowserDialog dlgChooseUncookDestination;
    }
}