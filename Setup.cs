using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BBMath.Core;

namespace BBMath.Application
{
    public partial class frmSetup : Form
    {
        private void readData()     //读取bbmath的设置
        {
            tbCoinSetVal.Text = GameStateManager.coinTtl.ToString();

        }
        public frmSetup()
        {
            InitializeComponent();
            readData();
        }
        public frmSetup(bool allowChange)
        {
            InitializeComponent();
            readData();
            if (!allowChange)       //冻结设置控件
            {
                MessageBox.Show("由于没有正确输入权限密码，不允许修改设置", "设置界面");
                btnSave.Enabled = false;
				cbChangePsw.Enabled = false;
            }
            
        }

        private void trackBar0_ValueChanged(object sender, EventArgs e)
        {
            tb0a.Text = trackBar0.Value.ToString();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            tb1a.Text = trackBar1.Value.ToString();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
            trackBar0.Value = 800;
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 80;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int pocket = 0;
            //int box = 0;
            if(int.TryParse(tbCoinSetVal.Text, out pocket) )
            {
                GameStateManager.coinTtl=pocket;
                if((cbChangePsw.Checked)&&(tbNewPsw.TextLength>0))
				{
					GameStateManager.PSW = tbNewPsw.Text;
					LoggerHelper.Print($"密码已修改为: {tbNewPsw.Text}\r\n");
				}
                
                // 保存配置到文件
                GameStateManager.saveProgSettings();
                LoggerHelper.Print($"金币已修改为: {pocket}\r\n");
                
                this.Close();
            }
            else
			{
                MessageBox.Show("请输入一个整数", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
            
        }

		private void cbChangePsw_CheckedChanged(object sender, EventArgs e)
		{
			if (cbChangePsw.Checked)
			{
				lbNewPsw.Visible = true;
				tbNewPsw.Visible = true;
			}
			else
			{
				lbNewPsw.Visible = false;
				tbNewPsw.Visible = false;
			}
		}
	}
}
