using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace upforthis
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length == 0)
            {
                Application.Run(new From_Update());
            }
            else
            {
                if (args[0] == "restart")
                {
                    Thread.Sleep(2000);
                    if (CheckProcess())
                    {
                        Thread.Sleep(2000);
                        if (CheckProcess())
                        {
                            KillProcess();
                        }
                    }
                    ShellExecute(Application.StartupPath + "\\CdelCourse.exe", string.Empty);
                }
            }
        }

        private static bool CheckProcess()
        {
            var thispath = Application.ExecutablePath.ToLower();
            var path = Application.StartupPath.ToLower() + "\\";
            var procs = Process.GetProcesses();
            foreach (Process proc in procs)
            {
                try
                {
                    string procPath = proc.MainModule.FileName.ToLower();
                    if (procPath.Contains(path) && procPath != thispath)
                    {
                        return true;
                    }
                }
                catch { }
            }
            return false;
        }

        private static void KillProcess()
        {
            var thispath = Application.ExecutablePath.ToLower();
            var path = Application.StartupPath.ToLower() + "\\";
            var procs = Process.GetProcesses();
            foreach (Process proc in procs)
            {
                try
                {
                    string procPath = proc.MainModule.FileName.ToLower();
                    if (procPath.Contains(path) && procPath != thispath)
                    {
                        proc.Kill();
                    }
                }
                catch { }
            }
        }

        private static void ShellExecute(string command, string args)
        {
            var proc = new Process
            {
                StartInfo = { FileName = command, Arguments = args, UseShellExecute = true }
            };
            proc.Start();
        }
    }
}
