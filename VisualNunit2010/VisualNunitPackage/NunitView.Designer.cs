namespace BubbleCloudorg.VisualNunit
{
    partial class NuniView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }


        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.Debug = new System.Windows.Forms.DataGridViewImageColumn();
            this.Success = new System.Windows.Forms.DataGridViewImageColumn();
            this.Namespace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Case = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Test = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Stacktrace = new System.Windows.Forms.DataGridViewButtonColumn();
            this.testRunWorker = new System.ComponentModel.BackgroundWorker();
            this.testListWorker = new System.ComponentModel.BackgroundWorker();
            this.runTestsButton = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.homeButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.projectComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.namespaceComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.caseComboBox = new System.Windows.Forms.ComboBox();
            this.statusButton = new System.Windows.Forms.PictureBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusButton)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Debug,
            this.Success,
            this.Namespace,
            this.Case,
            this.Test,
            this.Time,
            this.Message,
            this.Stacktrace});
            this.dataGridView1.GridColor = System.Drawing.Color.White;
            this.dataGridView1.Location = new System.Drawing.Point(6, 144);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 20;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(546, 313);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            this.dataGridView1.VisibleChanged += new System.EventHandler(this.dataGridView1_VisibleChanged);
            // 
            // Debug
            // 
            this.Debug.HeaderText = "";
            this.Debug.Name = "Debug";
            this.Debug.ReadOnly = true;
            this.Debug.Width = 22;
            // 
            // Success
            // 
            this.Success.HeaderText = "";
            this.Success.Name = "Success";
            this.Success.ReadOnly = true;
            this.Success.Width = 22;
            // 
            // Namespace
            // 
            this.Namespace.DataPropertyName = "Namespace";
            this.Namespace.HeaderText = "Namespace";
            this.Namespace.Name = "Namespace";
            this.Namespace.ReadOnly = true;
            this.Namespace.Width = 80;
            // 
            // Case
            // 
            this.Case.DataPropertyName = "Case";
            this.Case.HeaderText = "Case";
            this.Case.Name = "Case";
            this.Case.ReadOnly = true;
            this.Case.Width = 80;
            // 
            // Test
            // 
            this.Test.DataPropertyName = "Test";
            this.Test.HeaderText = "Test";
            this.Test.Name = "Test";
            this.Test.ReadOnly = true;
            this.Test.Width = 160;
            // 
            // Time
            // 
            this.Time.DataPropertyName = "Time";
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.ReadOnly = true;
            this.Time.Width = 50;
            // 
            // Message
            // 
            this.Message.DataPropertyName = "Message";
            this.Message.HeaderText = "Message";
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            // 
            // Stacktrace
            // 
            this.Stacktrace.DataPropertyName = "TestInformation";
            this.Stacktrace.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Stacktrace.HeaderText = "";
            this.Stacktrace.Name = "Stacktrace";
            this.Stacktrace.ReadOnly = true;
            this.Stacktrace.Text = "...";
            this.Stacktrace.ToolTipText = "Stacktrace";
            this.Stacktrace.UseColumnTextForButtonValue = true;
            this.Stacktrace.Width = 30;
            // 
            // testRunWorker
            // 
            this.testRunWorker.WorkerReportsProgress = true;
            this.testRunWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.testRunWorker_DoWork);
            this.testRunWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.testRunWorker_ProgressChanged);
            this.testRunWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.testRunWorker_RunWorkerCompleted);
            // 
            // testListWorker
            // 
            this.testListWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.testListWorker_DoWork);
            this.testListWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.testListWorker_RunWorkerCompleted);
            // 
            // runTestsButton
            // 
            this.runTestsButton.Location = new System.Drawing.Point(6, 5);
            this.runTestsButton.Name = "runTestsButton";
            this.runTestsButton.Size = new System.Drawing.Size(25, 25);
            this.runTestsButton.TabIndex = 3;
            this.runTestsButton.UseVisualStyleBackColor = true;
            this.runTestsButton.Click += new System.EventHandler(this.runTests_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(36, 5);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(489, 20);
            this.progressBar1.TabIndex = 4;
            // 
            // homeButton
            // 
            this.homeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.homeButton.Location = new System.Drawing.Point(531, 5);
            this.homeButton.Name = "homeButton";
            this.homeButton.Size = new System.Drawing.Size(21, 21);
            this.homeButton.TabIndex = 5;
            this.homeButton.UseVisualStyleBackColor = true;
            this.homeButton.Click += new System.EventHandler(this.homeButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Project";
            // 
            // projectComboBox
            // 
            this.projectComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.projectComboBox.FormattingEnabled = true;
            this.projectComboBox.Location = new System.Drawing.Point(73, 34);
            this.projectComboBox.Name = "projectComboBox";
            this.projectComboBox.Size = new System.Drawing.Size(479, 21);
            this.projectComboBox.TabIndex = 7;
            this.projectComboBox.SelectedIndexChanged += new System.EventHandler(this.projectComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Namespace";
            // 
            // namespaceComboBox
            // 
            this.namespaceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.namespaceComboBox.FormattingEnabled = true;
            this.namespaceComboBox.Location = new System.Drawing.Point(73, 61);
            this.namespaceComboBox.Name = "namespaceComboBox";
            this.namespaceComboBox.Size = new System.Drawing.Size(479, 21);
            this.namespaceComboBox.TabIndex = 9;
            this.namespaceComboBox.SelectedIndexChanged += new System.EventHandler(this.namespaceComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Fixture";
            // 
            // caseComboBox
            // 
            this.caseComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.caseComboBox.FormattingEnabled = true;
            this.caseComboBox.Location = new System.Drawing.Point(73, 88);
            this.caseComboBox.Name = "caseComboBox";
            this.caseComboBox.Size = new System.Drawing.Size(479, 21);
            this.caseComboBox.TabIndex = 11;
            this.caseComboBox.SelectedIndexChanged += new System.EventHandler(this.caseComboBox_SelectedIndexChanged);
            // 
            // statusButton
            // 
            this.statusButton.Location = new System.Drawing.Point(73, 115);
            this.statusButton.Name = "statusButton";
            this.statusButton.Size = new System.Drawing.Size(20, 20);
            this.statusButton.TabIndex = 12;
            this.statusButton.TabStop = false;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(99, 119);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "Result";
            // 
            // NuniView
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.label4);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.statusButton);
            this.Controls.Add(this.caseComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.namespaceComboBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.projectComboBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.homeButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.runTestsButton);
            this.Controls.Add(this.dataGridView1);
            this.Name = "NuniView";
            this.Size = new System.Drawing.Size(557, 460);
            this.Load += new System.EventHandler(this.NuniView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.ComponentModel.BackgroundWorker testRunWorker;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.ComponentModel.BackgroundWorker testListWorker;
        private System.Windows.Forms.DataGridViewImageColumn Debug;
        private System.Windows.Forms.DataGridViewImageColumn Success;
        private System.Windows.Forms.DataGridViewTextBoxColumn Namespace;
        private System.Windows.Forms.DataGridViewTextBoxColumn Case;
        private System.Windows.Forms.DataGridViewTextBoxColumn Test;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewButtonColumn Stacktrace;
        private System.Windows.Forms.Button runTestsButton;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button homeButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox projectComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox namespaceComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox caseComboBox;
        private System.Windows.Forms.PictureBox statusButton;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label4;

    }
}
