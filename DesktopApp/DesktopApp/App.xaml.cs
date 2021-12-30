using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using DesktopApp.Controls;
using DesktopApp.Logic;
using DesktopApp.ViewModel;

using Framework.Local;
using Framework.Model;
using Framework.Utility;

namespace DesktopApp
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        private readonly Stopwatch sw = new Stopwatch();
        /// <summary>
        /// 当前的主窗体
        /// </summary>
        public static MainWindow CurrentMainWindow { get; set; }

        public static CustomWindow CurrentCustomWindow { get; set; }

        /// <summary>
        /// ViewModel管理类
        /// </summary>
        public static ViewModelLocator Loc { get; private set; }

        /// <summary>
        /// 应用程序构造函数
        /// </summary>
        public App()
        {
            sw.Start();
            System.Windows.Forms.Application.EnableVisualStyles();
            ShutdownMode = ShutdownMode.OnLastWindowClose;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            //防止Task中抛错导致的异常退出
            TaskScheduler.UnobservedTaskException += (s, e) => e.SetObserved();
        }

        /// <summary>
        /// 处理应用程序域未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Trace.WriteLine("CurrentDomain_UnhandledException");
            Log.RecordLog(e.ExceptionObject.ToString());
        }

        /// <summary>
        /// 处理程序未捕获的异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Trace.WriteLine("App_DispatcherUnhandledException");
            Log.RecordLog(e.Exception.ToString());
            e.Handled = true;
        }

        /// <summary>
        /// 互斥量
        /// </summary>
        private static Mutex _mutex;

        /// <summary>
        /// 程序启动
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;

            //恢复备份数据库
            //ReplaceDBData();
            Loc = (ViewModelLocator)(Resources["Locator"]);
            _mutex = new Mutex(true, Util.AppName, out var isNew);
            if (!isNew)
            {
                ActivateOtherWindow();
                Shutdown();
                return;
            }
            if (File.Exists(SystemInfo.AppDataPath + "trace.log"))
            {
                Trace.Listeners.Add(new TextWriterTraceListener(SystemInfo.AppDataPath + "trace.log"));
                Trace.AutoFlush = true;
            }
            Trace.WriteLine(string.Format("Start at {0} on {1}", Util.GetNow(), Environment.OSVersion));
            InitApp();
            Trace.WriteLine("LoadLoginedStudent");
            var student = new StudentData();
            StudentInfo studentInfo = student.GetLoginedStudent();
            if (studentInfo == null)
            {
                //如果学员未登录那么显示登录窗体
                Trace.WriteLine("ExecuteLogin");
                Log.RecordData("ShowLogin");
                var loginWin = new LoginWindow();
                loginWin.ShowDialog();
                return;
            }
            //如果学员已经登录，那么显示主窗体
            Util.UserName = studentInfo.UserName;
            Util.Password = studentInfo.Password;
            Util.SsoUid = studentInfo.Ssouid;
            Trace.WriteLine("SaveLoginTime");
            studentInfo.LastLogin = Util.GetNow();
            student.AddLoginInfo(studentInfo);
            Trace.WriteLine("ShowMain");
            Log.RecordData("ShowExistUser", Util.SsoUid);
            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            sw.Stop();
            Trace.WriteLine("启动程序用时" + sw.ElapsedMilliseconds);
        }

        /// <summary>
        /// 初始化应用程序
        /// </summary>
        private static void InitApp()
        {
            Current.Resources.Remove("BaseFont");
            var baseFont = new FontFamily(Util.BaseFont);
            Current.Resources.Add("BaseFont", baseFont);
            //升级数据库
            var dbup = new DatabaseUpdate();
            dbup.UpdateDatabase();
            //启动大数据收集线程
            SystemInfo.StartBackGroundThread("大数据收集", () =>
            {
                Thread.Sleep(10000);
                Log.RecordData("SystemInfo", "OS", Environment.OSVersion, Environment.Is64BitOperatingSystem ? "X64" : "X86");
                System.Windows.Forms.Screen sc = System.Windows.Forms.Screen.PrimaryScreen;
                Log.RecordData("SystemInfo", "Screen", sc.Bounds.Width, sc.Bounds.Height, sc.BitsPerPixel);
                Log.RecordData("SystemInfo", "CPUID", SystemInfo.GetCpuInfo());
                Log.RecordData("SystemInfo", "HDID", SystemInfo.GetDiskDriveInfo());
                Log.RecordData("SystemInfo", "MemorySize", SystemInfo.GetMemorySize());
                Log.RecordData("SystemInfo", "NetMac", SystemInfo.GetNetworkAdapterMac());
                while (true)
                {
                    if (Util.IsOnline)
                    {
                        var re = new Framework.Remote.StudentRemote();
                        re.UploadBigDataLog();
                        for (var i = 1; i < 10; i++)
                        {
                            Thread.Sleep(60000);
                        }
                    }
                    Thread.Sleep(60000);
                }
            });
            Trace.WriteLine("StartOnlineTest");
            //在线状态更改为在线时要自动继续下载
            Util.OnlineStateChanged += (o, e) =>
            {
                try
                {
                    if (Util.IsOnline)
                    {
                        Loc.DownloadCenter.StartNext();
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            };
            //启动是否在线的测试
            SystemInfo.ThreadTestOnline(() =>
            {
                try
                {
                    Current.Dispatcher.Invoke(new Action(() =>
                    {
                        //foreach (Window win in Current.Windows)
                        //{
                        //	win.Visibility = Visibility.Collapsed;
                        //}
                        if (CurrentMainWindow != null)
                        {
                            CurrentMainWindow.Visibility = Visibility.Collapsed;
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }, () =>
            {
                try
                {
                    Current.Dispatcher.Invoke(new Action(() =>
                    {
                        //foreach (Window win in Current.Windows)
                        //{
                        //	win.Visibility = Visibility.Collapsed;
                        //}
                        if (CurrentMainWindow != null)
                        {
                            CurrentMainWindow.Visibility = Visibility.Collapsed;
                        }
                        Log.RecordData("StudentOffLineTimeOut");
                        System.Windows.Forms.MessageBox.Show(@"您的离线时长已用完，请联网后重新认证！");
                        StudentLogic.ExecuteLogout();
                    }));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            });

            SystemInfo.TryDeleteDirectory(Path.GetTempPath() + "\\CDELTemp\\");
            //启动小web服务，该web服务用于浏览讲义
            Trace.WriteLine("StartWebServer:" + Util.HttpPort);
            var server = new SimpleHttpServer(Util.HttpPort);
            server.Start();
        }

        /// <summary>
        /// 新实例启动时要显示之前实例的窗体
        /// </summary>
        private static void ActivateOtherWindow()
        {
            IntPtr other = NativeMethod.FindWindow(null, Util.AppName);
            if (other != IntPtr.Zero)
            {
                NativeMethod.SetForegroundWindow(other);
                if (NativeMethod.IsIconic(other))
                    NativeMethod.OpenIcon(other);
            }
            else
            {
                //找不到之前的实例,结束旧的进程，开启新的进程
                Process[] plist = Process.GetProcesses();
                var cp = Process.GetCurrentProcess();
                var cName = cp.ProcessName;
                var cId = cp.Id;
                foreach (Process p in plist)
                {
                    Trace.WriteLine(p.MainWindowTitle);
                    if (p.ProcessName == cName && p.Id != cId)
                    {
                        try
                        {
                            if (string.IsNullOrEmpty(p.MainWindowTitle))
                            {
                                //找到了当前的进程,杀掉进程，并且重启当前进程
                                p.Kill();
                                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "upforthis.exe", "restart");
                                Process.GetCurrentProcess().Kill();
                            }
                        }
                        catch (Exception)
                        {
                            ;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 应用程序退出时要执行的操作
        /// </summary>
        /// <param name="e"></param>
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                Loc.DownloadCenter.StopDownloading();
                Log.RecordData("CloseApp");
                var dirTemp = SystemInfo.AppDataPath + "web";
                SystemInfo.TryDeleteDirectory(dirTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            if (_mutex != null)
                _mutex.Close();
            base.OnExit(e);
        }

        /// <summary>
        /// 恢复数据
        /// </summary>
        private static void ReplaceDBData()
        {
            var origRoot = SystemInfo.AppDataPath + "\\db";
            var path = origRoot + "\\存在备份数据文件.txt";
            if (File.Exists(path))
            {
                try
                {
                    var orignFile = origRoot + "\\db.db";
                    var newFile = origRoot + "\\db.bak";
                    if (File.Exists(orignFile) && File.Exists(newFile))
                    {
                        File.Move(orignFile, orignFile + ".old");
                        File.Move(newFile, orignFile);
                        File.Move(path, path + ".old");
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex);
                }
            }
        }
    }
}