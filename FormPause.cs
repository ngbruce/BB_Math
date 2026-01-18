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
	public partial class FormPause : Form
	{
		public int startSec ;		//记录窗体生成时的暂停剩余时间
		private DateTime pauseStartTime;	//记录暂停开始时间
		public FormPause()
		{
			InitializeComponent();
			startSec = GameStateManager.pauseSecLeft;     //记录窗体生成时的暂停剩余时间
			pauseStartTime = DateTime.Now;                  //记录暂停开始时间
			//设置位置在屏幕中间
			int x = (System.Windows.Forms.SystemInformation.WorkingArea.Width - this.Size.Width) / 2;
			int y = (System.Windows.Forms.SystemInformation.WorkingArea.Height - this.Size.Height) / 2;
			this.StartPosition = FormStartPosition.Manual;	//窗体的位置由Location属性决定
			this.Location = (Point)new Size(x, y);			//窗体的起始位置为(x,y)

			if(GameStateManager.pauseType==1)							//显示剩余暂停时间
			{
				button1.Text= GameStateManager.pauseSecLeft.ToString();
				timerPause.Enabled = true;
			}
			else if (GameStateManager.pauseType==0)
			{
				button1.Text = GameStateManager.allowPause.ToString();
				lbRemainTime.Text = "剩余暂停次数：";
			}
		}

		private void btnStart_Click(object sender, EventArgs e)
		{
			if (GameStateManager.pauseType == 1)    //限时间暂停模式
			{
				//计算实际暂停时长（秒），向上取整
				TimeSpan actualPauseDuration = DateTime.Now - pauseStartTime;
				int pauseSeconds = (int)Math.Ceiling(actualPauseDuration.TotalSeconds);

				//确保至少扣除1秒（防止用户快速关闭窗口）
				pauseSeconds = Math.Max(1, pauseSeconds);

				//直接设置最终值：开始时间 - 实际暂停时长
				//这样可以避免 timerPause_Tick 和手动扣减的重复计算问题
				int finalPauseSecLeft = startSec - pauseSeconds;
				GameStateManager.pauseSecLeft = Math.Max(0, finalPauseSecLeft);
			}

			this.Close();
		}

		private void timerPause_Tick(object sender, EventArgs e)
		{
			GameStateManager.pauseSecLeft -= 1;
			button1.Text = GameStateManager.pauseSecLeft.ToString();
			if (GameStateManager.pauseSecLeft<=0)
			{
				this.Close();
			}
		}
	}
}
