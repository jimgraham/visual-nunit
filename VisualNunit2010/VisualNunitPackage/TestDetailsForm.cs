using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VisualNunitLogic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace BubbleCloudorg.VisualNunit
{
    public partial class TestDetailsForm : Form
    {
        public TestDetailsForm()
        {
            InitializeComponent();
        }

        public void SetTestInformation(TestInformation testInformation)
        {
            this.textBox1.Text = testInformation.FailureMessage;

            DataTable table = new DataTable();
            table.Columns.Add("File", typeof(String));
            table.Columns.Add("Method", typeof(String));
            table.Columns.Add("Row", typeof(String));

            if (testInformation.FailureStackTrace != null)
            {
                StringReader reader = new StringReader(testInformation.FailureStackTrace);

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith("at "))
                    {
                        if (line.Contains(" in "))
                        {
                            int methodStartIndex = 3;
                            int methodEndIndex = line.LastIndexOf(" in ");
                            int fileStartIndex = line.LastIndexOf(" in ") + 4;
                            int fileEndIndex = line.LastIndexOf(":line ");
                            int rowStartIndex = line.LastIndexOf(":line ") + 5;
                            int rowEndIndex = line.Length;
                            String method = line.Substring(methodStartIndex, methodEndIndex - methodStartIndex);
                            String file = line.Substring(fileStartIndex, fileEndIndex - fileStartIndex);
                            String row = line.Substring(rowStartIndex, rowEndIndex - rowStartIndex);
                            table.Rows.Add(file, method, row);
                        }
                        else
                        {
                            int methodStartIndex = 3;
                            int methodEndIndex = line.Length;
                            String method = line.Substring(methodStartIndex, methodEndIndex - methodStartIndex);
                            table.Rows.Add(null, method, null);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            dataGridView1.DataSource = table;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void TestDetailsForm_Shown(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
            this.textBox1.SelectionLength = 0;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataRow dataRow = ((DataRowView)dataGridView1.CurrentRow.DataBoundItem).Row;
            if (dataRow["File"] != DBNull.Value)
            {
                Close();
                DTE dte = (DTE)Package.GetGlobalService(typeof(DTE));
                String fileName = (String)dataRow["File"];
                dte.ItemOperations.OpenFile(fileName);
                TextSelection selection = (TextSelection)dte.ActiveDocument.Selection;
                selection.GotoLine(Convert.ToInt16(dataRow["Row"]), true);
            }
        }
    }
}
