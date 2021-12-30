using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWinform
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var buffer = new Remote().GetStudentCourseNew(this.textBox1.Text);
			var strBy = BitConverter.ToString(buffer).Replace("-", string.Empty);
			var str = Encoding.UTF8.GetString(buffer);
			this.textBox4.Text = strBy;
			this.textBox5.Text = str;
			try
			{
				var obj = WebProxyClient.JsonDeserialize<StudentCourse>(str, Encoding.UTF8);
				MessageBox.Show(obj.Uid);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			//trackBar1.
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			this.webBrowser1.Navigate("http://www.useragentstring.com/");
		}
	}
}
