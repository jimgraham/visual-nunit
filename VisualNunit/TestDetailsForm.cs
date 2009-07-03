using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VisualNunitLogic;

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
            this.textBox1.Text = testInformation.FailureStackTrace;
        }
    }
}
