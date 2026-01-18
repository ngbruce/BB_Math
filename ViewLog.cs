using System;
using System.IO;
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
    public partial class formViewLog : Form
    {
        public formViewLog()
        {
            InitializeComponent();
        }

        private void formViewLog_Load(object sender, EventArgs e)
        {
            try
            {
                //textBox1.Text = "hahha";
                string path = AppDomain.CurrentDomain.BaseDirectory + GameStateManager.fileName;
                //textBox1.Text = path;
                if (!File.Exists(path))
                {
                    textBox1.Multiline = false;
					textBox1.BackColor = Color.SkyBlue;//Color.Tomato;

					textBox1.Location = new Point(12, 135);
                    textBox1.Text = "目前没有记录文件，初次记录时将自动生成。";
                    return;
                }
                else
                {
                    string line;
                    using (StreamReader sr = new StreamReader(path))
                    {
						// 从文件读取并显示行，直到文件的末尾 
						//while ((line = sr.ReadLine()) != null)
						//{
						//	textBox1.AppendText(line);
						//	textBox1.AppendText("\r\n");    //win10一定要\r\n，否则不换行
						//}

						line = sr.ReadToEnd();		//用这个更简洁
						textBox1.AppendText(line);

					}
				}
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Exception of formViewLog_Load()");
            }
        }

		private void formViewLog_Shown(object sender, EventArgs e)
		{
			textBox1.Focus();
			textBox1.Select(textBox1.TextLength, 0);
			textBox1.ScrollToCaret();
		}
	}
}
