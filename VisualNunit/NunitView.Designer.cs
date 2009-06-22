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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.caseComboBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.namespaceComboBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.projectComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.runTests = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.Success = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Namespace = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Case = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Test = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StackTraceButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Debug = new System.Windows.Forms.DataGridViewButtonColumn();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.progressBar1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(643, 422);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar1.Location = new System.Drawing.Point(3, 385);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(637, 14);
            this.progressBar1.TabIndex = 0;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Success,
            this.Namespace,
            this.Case,
            this.Test,
            this.Time,
            this.Message,
            this.StackTraceButton,
            this.Debug});
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 32);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(637, 347);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.caseComboBox);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.namespaceComboBox);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.projectComboBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.runTests);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(643, 29);
            this.panel1.TabIndex = 4;
            // 
            // caseComboBox
            // 
            this.caseComboBox.FormattingEnabled = true;
            this.caseComboBox.Location = new System.Drawing.Point(493, 5);
            this.caseComboBox.Name = "caseComboBox";
            this.caseComboBox.Size = new System.Drawing.Size(121, 21);
            this.caseComboBox.TabIndex = 9;
            this.caseComboBox.SelectedIndexChanged += new System.EventHandler(this.caseComboBox_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(456, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Case";
            // 
            // namespaceComboBox
            // 
            this.namespaceComboBox.FormattingEnabled = true;
            this.namespaceComboBox.Location = new System.Drawing.Point(327, 5);
            this.namespaceComboBox.Name = "namespaceComboBox";
            this.namespaceComboBox.Size = new System.Drawing.Size(121, 21);
            this.namespaceComboBox.TabIndex = 7;
            this.namespaceComboBox.SelectedIndexChanged += new System.EventHandler(this.namespaceComboBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(257, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Namespace";
            // 
            // projectComboBox
            // 
            this.projectComboBox.FormattingEnabled = true;
            this.projectComboBox.Location = new System.Drawing.Point(130, 5);
            this.projectComboBox.MaxDropDownItems = 20;
            this.projectComboBox.Name = "projectComboBox";
            this.projectComboBox.Size = new System.Drawing.Size(121, 21);
            this.projectComboBox.TabIndex = 5;
            this.projectComboBox.SelectedIndexChanged += new System.EventHandler(this.projectComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(84, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Project";
            // 
            // runTests
            // 
            this.runTests.AutoSize = true;
            this.runTests.Location = new System.Drawing.Point(3, 3);
            this.runTests.Name = "runTests";
            this.runTests.Size = new System.Drawing.Size(75, 23);
            this.runTests.TabIndex = 3;
            this.runTests.Text = "Run";
            this.runTests.UseVisualStyleBackColor = true;
            this.runTests.Click += new System.EventHandler(this.runTests_Click);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerReportsProgress = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
            // 
            // Success
            // 
            this.Success.DataPropertyName = "Success";
            this.Success.HeaderText = "Ok";
            this.Success.MinimumWidth = 30;
            this.Success.Name = "Success";
            this.Success.ReadOnly = true;
            this.Success.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Success.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Success.ToolTipText = "Success";
            this.Success.Width = 30;
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
            // StackTraceButton
            // 
            this.StackTraceButton.DataPropertyName = "TestInformation";
            this.StackTraceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.StackTraceButton.HeaderText = "";
            this.StackTraceButton.Name = "StackTraceButton";
            this.StackTraceButton.ReadOnly = true;
            this.StackTraceButton.Text = "...";
            this.StackTraceButton.UseColumnTextForButtonValue = true;
            this.StackTraceButton.Width = 30;
            // 
            // Debug
            // 
            this.Debug.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.Debug.HeaderText = "";
            this.Debug.Name = "Debug";
            this.Debug.ReadOnly = true;
            this.Debug.Text = "Debug";
            this.Debug.UseColumnTextForButtonValue = true;
            this.Debug.Width = 80;
            // 
            // NuniView
            // 
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "NuniView";
            this.Size = new System.Drawing.Size(643, 422);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button runTests;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox namespaceComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox projectComboBox;
        private System.Windows.Forms.ComboBox caseComboBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Success;
        private System.Windows.Forms.DataGridViewTextBoxColumn Namespace;
        private System.Windows.Forms.DataGridViewTextBoxColumn Case;
        private System.Windows.Forms.DataGridViewTextBoxColumn Test;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewButtonColumn StackTraceButton;
        private System.Windows.Forms.DataGridViewButtonColumn Debug;

    }
}
