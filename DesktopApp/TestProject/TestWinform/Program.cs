using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestWinform
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			bool isNew;
			_mutex = new Mutex(true, "会计下载课堂", out isNew);

			Application.Run(new Form2());
		}

		private static Mutex _mutex;
	}
}
