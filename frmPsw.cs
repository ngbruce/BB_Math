using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BBMath.Application
{
    public partial class frmPsw : Form
    {
        public string OutValue;     //记录输入的密码
        public frmPsw()
        {
            InitializeComponent();
            textBox1.Select();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            OutValue = textBox1.Text;
            this.Close();
        }

		private void frmPsw_Load(object sender, EventArgs e)
		{
			//MessageBox.Show("Load!");

		}
	}
}
