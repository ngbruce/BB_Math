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
	public partial class FormMsg : Form
	{
		public int timeLeft;
		public FormMsg()
		{
			InitializeComponent();
			timeLeft = GameStateManager.errorMsgShowTime;
			//设置位置在屏幕中间
			int x = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Size.Width) / 2;
			int y = (System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Size.Height) / 2;
			this.StartPosition = FormStartPosition.Manual; //窗体的位置由Location属性决定
			this.Location = (Point)new Size(x, y);         //窗体的起始位置为(x,y)
		}

		private void timerShow_Tick(object sender, EventArgs e)
		{
			timeLeft--;
			if (timeLeft < 0)
			{
				this.Close();
			}
			else
			{
				btnClose.Text = timeLeft.ToString();
			}
			
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void FormMsg_Load(object sender, EventArgs e)
		{
			btnClose.Text = timeLeft.ToString();
		}
	}
}
