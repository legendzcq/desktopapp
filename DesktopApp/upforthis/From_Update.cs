using System;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace upforthis
{
    public partial class From_Update : Form
    {
        [Flags]
        public enum MoveFileFlags
        {
            None = 0,
            ReplaceExisting = 1,
            CopyAllowed = 2,
            DelayUntilReboot = 4,
            WriteThrough = 8,
            CreateHardlink = 16,
            FailIfNotTrackable = 32,
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool ExitWindowsEx(int DoFlag, int rea);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCurrentProcess();


        private const int SE_PRIVILEGE_ENABLED = 0x00000002;
        private const int TOKEN_QUERY = 0x00000008;
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        private const int EWX_LOGOFF = 0x00000000;
        private const int EWX_SHUTDOWN = 0x00000001;
        private const int EWX_REBOOT = 0x00000002;
        private const int EWX_FORCE = 0x00000004;
        private const int EWX_POWEROFF = 0x00000008;
        private const int EWX_FORCEIFHUNG = 0x00000010;

        public From_Update()
        {
            InitializeComponent();

            Icon = Properties.Resources.DesktopApp;

            Load += (o, e) =>
            {
                var th = new Thread(() =>
                {
                    Thread.Sleep(2000);
                    if (!File.Exists(Application.StartupPath + "\\needupdate.bin"))
                    {
                        Application.Exit();
                        return;
                    }
                    if (CheckProcess())
                    {
                        Thread.Sleep(2000);
                        CheckProcess();
                    }
                    KillProcess();
                    string files = File.ReadAllText(Application.StartupPath + "\\needupdate.bin");
                    string[] fileArr = files.Split('|');
                    bool haserror = false;
                    foreach (string file in fileArr)
                    {
                        if (string.IsNullOrEmpty(file)) continue;
                        try
                        {
                            File.Copy(Application.StartupPath + @"\update" + file, Application.StartupPath + file, true);
                            File.Delete(Application.StartupPath + @"\update" + file);
                            //File.AppendAllText(Application.StartupPath + "\\log.txt", "Move " + file + "\r\n");
                        }
                        catch (Exception)
                        {
                            //File.AppendAllText(Application.StartupPath + "\\log.txt", ex.ToString() + "\r\n");
                            haserror = true;
                            MoveFileEx(Application.StartupPath + @"\update" + file, Application.StartupPath + file, MoveFileFlags.DelayUntilReboot | MoveFileFlags.ReplaceExisting);
                        }
                    }
                    try
                    {
                        File.Delete(Application.StartupPath + "\\needupdate.bin");
                    }
                    catch (Exception)
                    { }
                    if (haserror)
                    {
                        if (MessageBox.Show(@"有文件当前正在使用中，是否重启计算机", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            //执行重启电脑动作
                            Reboot();
                            return;
                        }
                        MessageBox.Show(@"更新将在计算机重启后生效");
                        Application.Exit();
                        return;
                    }
                    MessageBox.Show(@"更新成功");
                    ShellExecute(Application.StartupPath + "\\CdelCourse.exe", string.Empty);
                    Application.Exit();
                }) { Name = "更新线程" };
                th.Start();
            };
        }

        private void KillProcess()
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

        private bool CheckProcess()
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

        private static void DoExitWin(int doFlag)
        {
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
            ExitWindowsEx(doFlag, 0);
        }

        /**/

        /// <summary>
        /// 重新启动
        /// </summary>
        private static void Reboot()
        {
            DoExitWin(EWX_FORCE | EWX_REBOOT);
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
