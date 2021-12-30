using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using CdelService.Utility;

namespace CdelService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "trace.log"))
			{
				Trace.Listeners.Add(new TextWriterTraceListener(AppDomain.CurrentDomain.BaseDirectory + "trace.log"));
				Trace.AutoFlush = true;
			}
			var servicesToRun = new ServiceBase[] 
			{ 
				new ServiceMain() 
			};
			ServiceBase.Run(servicesToRun);
		}
	}
}
