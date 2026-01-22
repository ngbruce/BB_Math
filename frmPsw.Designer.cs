namespace BBMath.Application
{
    partial class frmPsw
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
			this.button1 = new System.Windows.Forms.Button();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(91, 75);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(106, 62);
			this.button1.TabIndex = 0;
			this.button1.Text = "OK";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.btnStart_Click);
			// 
			// textBox1
			// 
			this.textBox1.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.textBox1.Location = new System.Drawing.Point(44, 37);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(207, 32);
			this.textBox1.TabIndex = 1;
			this.textBox1.UseSystemPasswordChar = true;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label1.Location = new System.Drawing.Point(2, 218);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(184, 63);
			this.label1.TabIndex = 3;
			this.label1.Text = "如果确实不会用\r\n在主界面的\"原作说明\"处\r\n查看作者的社交媒体";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.Location = new System.Drawing.Point(2, 157);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(279, 20);
			this.label3.TabIndex = 5;
			this.label3.Text = "软件初次启动会生成默认配置文件和密码";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("微软雅黑", 10.5F);
			this.label5.Location = new System.Drawing.Point(2, 177);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(217, 20);
			this.label5.TabIndex = 7;
			this.label5.Text = "初始密码就在《使用说明.pdf》里";
			// 
			// frmPsw
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.ClientSize = new System.Drawing.Size(293, 301);
			this.ControlBox = false;
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.button1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "frmPsw";
			this.Text = "请输入设置权限密码";
			this.TopMost = true;
			this.Load += new System.EventHandler(this.frmPsw_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
	}
}