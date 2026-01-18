namespace BBMath.Application
{
	partial class FormPause
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
			this.components = new System.ComponentModel.Container();
			this.button1 = new System.Windows.Forms.Button();
			this.lbRemainTime = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.timerPause = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("微软雅黑", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.button1.ForeColor = System.Drawing.Color.MediumBlue;
			this.button1.Location = new System.Drawing.Point(251, 135);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(141, 54);
			this.button1.TabIndex = 0;
			this.button1.Text = "继续";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// lbRemainTime
			// 
			this.lbRemainTime.AutoSize = true;
			this.lbRemainTime.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lbRemainTime.Location = new System.Drawing.Point(93, 149);
			this.lbRemainTime.Name = "lbRemainTime";
			this.lbRemainTime.Size = new System.Drawing.Size(152, 27);
			this.lbRemainTime.TabIndex = 1;
			this.lbRemainTime.Text = "剩余暂停时间：";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label2.Location = new System.Drawing.Point(145, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(212, 27);
			this.label2.TabIndex = 2;
			this.label2.Text = "暂停中，点击按钮继续";
			// 
			// timerPause
			// 
			this.timerPause.Interval = 1000;
			this.timerPause.Tick += new System.EventHandler(this.timerPause_Tick);
			// 
			// FormPause
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(517, 257);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.lbRemainTime);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPause";
			this.Text = "暂停中";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label lbRemainTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Timer timerPause;
	}
}