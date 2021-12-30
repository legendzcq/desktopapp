using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestVersion
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var sb = new StringBuilder();
			sb.AppendLine(Environment.Version.ToString());
			sb.AppendLine(Environment.OSVersion.ToString());
			sb.AppendLine(Environment.OSVersion.Version.ToString());
			sb.AppendLine(Environment.OSVersion.VersionString);
			sb.AppendLine(Environment.OSVersion.Platform.ToString());
			MessageBox.Show(sb.ToString());
		}
	}
}
