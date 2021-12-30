using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

using Microsoft.Win32;

using MessageBox = System.Windows.Forms.MessageBox;
using WebBrowser = System.Windows.Controls.WebBrowser;

namespace Framework.Utility
{
    public static class SystemInfo
    {
#if CHINAACC
        private const string ProductName = "ChinaaccDownClass";
#endif
#if MED
        private const string ProductName = "Med66DownClass";
#endif
#if JIANSHE
        private const string ProductName = "Jianshe99DownClass";
#endif
#if LAW
        private const string ProductName = "LawDownClass";
#endif
#if CHINATAT
        private const string ProductName = "ChinatatDownClass";
#endif
#if G12E
        private const string ProductName = "G12eDownClass";
#endif
#if ZIKAO
        private const string ProductName = "ZikaoDownClass";
#endif
#if CHENGKAO
        private const string ProductName = "ChengkaoDownClass";
#endif
#if KAOYAN
        private const string ProductName = "KaoyanDownClass";
#endif
#if FOR68
        private const string ProductName = "For68DownClass";
#endif
#if CK100
        private const string ProductName = "Ck100DownClass";
#endif

        public static readonly string AppDataPath;

        static SystemInfo()
        {
#if !NOUAC
            AppDataPath = AppDomain.CurrentDomain.BaseDirectory;
#else
            AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\CDEL\\" + ProductName + "\\";
            if (!Directory.Exists(_appDataPath)) Directory.CreateDirectory(_appDataPath);
#endif
        }

        #region Window全屏扩展方法

        private static Window _fullWindow;
        private static WindowState _windowState;
        private static WindowStyle _windowStyle;
        private static bool _windowTopMost;
        private static ResizeMode _windowResizeMode;
        private static Rect _windowRect;

        /// <summary>
        /// 进入全屏
        /// </summary>
        /// <param name="window"></param>
        public static void GoFullscreen(this Window window)
        {
            //已经是全屏
            if (window.IsFullscreen()) return;

            //存储窗体信息
            _windowState = window.WindowState;
            _windowStyle = window.WindowStyle;
            _windowTopMost = window.Topmost;
            _windowResizeMode = window.ResizeMode;
            _windowRect.X = window.Left;
            _windowRect.Y = window.Top;
            _windowRect.Width = window.Width;
            _windowRect.Height = window.Height;


            //变成无边窗体
            window.WindowState = WindowState.Normal;//假如已经是Maximized，就不能进入全屏，所以这里先调整状态
            window.WindowStyle = WindowStyle.None;
            window.ResizeMode = ResizeMode.NoResize;
            window.Topmost = true;//最大化后总是在最上面

            //获取窗口句柄
            IntPtr handle = new WindowInteropHelper(window).Handle;
            //获取当前显示器屏幕
            var screen = Screen.FromHandle(handle);

            //调整窗口最大化,全屏的关键代码就是下面3句
            window.MaxWidth = screen.Bounds.Width;
            window.MaxHeight = screen.Bounds.Height;
            window.WindowState = WindowState.Maximized;

            //解决切换应用程序的问题
            window.Activated += window_Activated;
            window.Deactivated += window_Deactivated;
            //记住成功最大化的窗体
            _fullWindow = window;
        }

        private static void window_Deactivated(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window != null) window.Topmost = false;
        }

        private static void window_Activated(object sender, EventArgs e)
        {
            var window = sender as Window;
            if (window != null) window.Topmost = true;
        }

        /// <summary>
        /// 退出全屏
        /// </summary>
        /// <param name="window"></param>
        public static void ExitFullscreen(this Window window)
        {
            //已经不是全屏无操作
            if (!window.IsFullscreen()) return;
            //恢复窗口先前信息，这样就退出了全屏
            window.Topmost = _windowTopMost;
            window.WindowStyle = _windowStyle;
            window.ResizeMode = ResizeMode.CanResize;//设置为可调整窗体大小
            window.Left = _windowRect.Left;
            window.Width = _windowRect.Width;
            window.Top = _windowRect.Top;
            window.Height = _windowRect.Height;
            window.WindowState = _windowState;//恢复窗口状态信息
            window.ResizeMode = _windowResizeMode;//恢复窗口可调整信息
            //移除不需要的事件
            window.Activated -= window_Activated;
            window.Deactivated -= window_Deactivated;
            _fullWindow = null;
        }

        /// <summary>
        /// 窗体是否在全屏状态
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool IsFullscreen(this Window window)
        {
            if (window == null)
                throw new ArgumentNullException("window");
            return _fullWindow == window;
        }

        #endregion

        #region 浏览器控件扩展方法

        /// <summary>
        /// WebBrowser禁用脚本错误提示
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="hide"></param>
        public static void SuppressScriptErrors(this WebBrowser webBrowser, bool hide)
        {
            FieldInfo fiComWebBrowser = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fiComWebBrowser == null) return;

            var objComWebBrowser = fiComWebBrowser.GetValue(webBrowser);
            if (objComWebBrowser == null) return;

            objComWebBrowser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, objComWebBrowser, new object[] { hide });
        }
        #endregion

        /// <summary>
        /// 测试是否在线的线程
        /// </summary>
        public static void ThreadTestOnline(Action failCheckTime, Action failOfflineTime)
        {
            StartBackGroundThread("在线测试", () =>
            {
                while (true)
                {
                    if (Util.DisableOnlineCheck)
                    {
                        Util.IsOnline = true;
                    }
                    else
                    {
                        try
                        {
                            int flag;
                            var check = NativeMethod.IsNetworkAlive(out flag);
                            if (!check)
                            {
                                check = NativeMethod.InternetGetConnectedState(ref flag, 0);
                            }
                            Util.IsOnline = check;
                        }
                        catch (Exception ex)
                        {
                            Log.RecordLog(ex.ToString());
                            Util.IsOnline = false;
                        }
                    }
                    Thread.Sleep(10000);
                    if (!CheckLastTime())
                    {
                        if (failCheckTime != null) failCheckTime();
                        MessageBox.Show("请勿修改系统时间，谢谢！");
                        Process.GetCurrentProcess().Kill();
                    }
                    /*去掉离线14小时功能*/
                    //if (!Util.IsOnline && Util.SsoUid > 0)
                    //{
                    //	//学员未联网，检查剩余时间，要求学员联网
                    //	Util.LastOffLineTime -= 10;
                    //	if (Util.LastOffLineTime <= 0)
                    //	{
                    //		if (failOfflineTime != null) failOfflineTime();
                    //	}
                    //}
                }
            });
        }

        /// <summary>
        /// 获取某个文件的文件大小
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static long GetFileSize(string fileName)
        {
            if (File.Exists(fileName))
            {
                var f = new FileInfo(fileName);
                return f.Length;
            }
            return 0;
        }

        /// <summary>
        /// 保存设置
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SaveSetting(string name, object value)
        {
            //todo :NOUAC 注册表必须迁移到其他位置
            try
            {
                RegistryKey key = Registry.LocalMachine.CreateSubKey("Software\\CDEL\\" + ProductName);
                if (key != null) key.SetValue(name, value);
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
            }
        }

        /// <summary>
        /// 获取设置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static object GetSetting(string name)
        {
            //todo :NOUAC 注册表必须迁移到其他位置
            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\CDEL\\" + ProductName);
                return key != null ? key.GetValue(name) : null;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                return null;
            }
        }

        private static bool CheckLastTime()
        {
            var nowtick = Util.GetNow().Ticks;
            var lasttimestr = GetSetting("LastTime") as string;
            if (!string.IsNullOrEmpty(lasttimestr))
            {
                long lasttime = long.TryParse(lasttimestr, out lasttime) ? lasttime : 0L;
                if (lasttime > Util.GetNow().Ticks)
                {
                    if (lasttime > Util.GetNow().AddHours(1).Ticks)
                    {
                        return false;
                    }
                    return true;
                }
            }
            SaveSetting("LastTime", nowtick.ToString(CultureInfo.InvariantCulture));
            return true;
        }

        /// <summary>
        /// 修复文件注册
        /// </summary>
        /// <returns></returns>
        public static bool FixReg()
        {
            //todo ： 这个功能需要移到外面去，因为这个需要UAC权限，得先搞修复工具
            string commonPath, systemPath;
            var apppath = AppDomain.CurrentDomain.BaseDirectory;
            if (Environment.Is64BitOperatingSystem)
            {
                //针对64位机器
                commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
                systemPath = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
            }
            else
            {
                //针对32位机器
                commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                systemPath = Environment.GetFolderPath(Environment.SpecialFolder.System);
            }
            try
            {
                var quartzPath = systemPath + "\\quartz.dll";
                var devenumpath = systemPath + "\\devenum.dll";
                if (!File.Exists(quartzPath))
                {
                    Log.RecordLog(@"系统文件检测失败quartz.dll，请重新安装");
                    return false;
                }
                if (!File.Exists(devenumpath))
                {
                    Log.RecordLog(@"系统文件检测失败devenum.dll，请重新安装");
                    return false;
                }

                var cmPath = commonPath + "\\cdel\\cm.dll";
                var mapPath = commonPath + "\\cdel\\map.dll";
                if (!File.Exists(cmPath))
                {
                    cmPath = apppath + "cm.dll";
                    if (!File.Exists(cmPath))
                    {
                        Log.RecordLog(@"系统文件检测失败cm.dll，请重新安装");
                        return false;
                    }
                }
                if (!File.Exists(mapPath))
                {
                    mapPath = apppath + "map.dll";
                    if (!File.Exists(mapPath))
                    {
                        Log.RecordLog(@"系统文件检测失败map.dll，请重新安装");
                        return false;
                    }
                }
                RunDllReg(quartzPath);
                RunDllReg(devenumpath);
                RunDllReg(cmPath);
                RunDllReg(mapPath);
                Version ver = Environment.OSVersion.Version;
                if (ver.Major == 5 || (ver.Major == 6 && ver.Minor == 0) || Util.IsUseffDshow)
                {
                    var ffdshowPath = commonPath + "\\cdel\\ffdshow\\ffdshow.ax";
                    if (!File.Exists(ffdshowPath))
                    {
                        ffdshowPath = apppath + "ffdshow\\ffdshow.ax";
                        if (!File.Exists(ffdshowPath))
                        {
                            Log.RecordLog(@"系统文件检测失败ffdshow，请重新安装");
                            return false;
                        }
                    }
                    RunDllReg(ffdshowPath);
                    RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\GNU\ffdshow");
                    if (key != null)
                    {
                        key.SetValue("trayIcon", 0, RegistryValueKind.DWord);
                        key.SetValue("trayIconExt", 0, RegistryValueKind.DWord);
                        key.SetValue("h264", 1, RegistryValueKind.DWord);
                        key.SetValue("isWhitelist", 0, RegistryValueKind.DWord);
                    }
                    else
                    {
                        return false;
                    }
                    key = Registry.CurrentUser.CreateSubKey(@"Software\GNU\ffdshow_audio");
                    if (key != null)
                    {
                        key.SetValue("trayIcon", 0, RegistryValueKind.DWord);
                        key.SetValue("trayIconExt", 0, RegistryValueKind.DWord);
                        key.SetValue("aac", 1, RegistryValueKind.DWord);
                        key.SetValue("isWhitelist", 0, RegistryValueKind.DWord);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    var vdecpath = systemPath + "\\msmpeg2vdec.dll";
                    var adecpath = systemPath + "\\msmpeg2adec.dll";
                    if (!File.Exists(vdecpath))
                    {
                        Log.RecordLog(@"系统文件检测失败msmpeg2vdec.dll，请重新安装");
                        return false;
                    }
                    if (!File.Exists(adecpath))
                    {
                        Log.RecordLog(@"系统文件检测失败msmpeg2adec.dll，请重新安装");
                        return false;
                    }
                    RunDllReg(vdecpath);
                    RunDllReg(adecpath);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        private static void RunDllReg(string path)
        {
            var regPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\system32\\regsvr32.exe";
            var p = new Process
            {
                StartInfo =
                {
                    FileName = regPath,
                    Arguments = "/s \"" + path + "\""
                }
            };
            p.Start();
            p.WaitForExit();
        }

        /// <summary>
        /// 获取文件夹可用空间
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static double GetFolderFreeSpaceInMb(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                #region 有些电脑对GetDiskFreeSpaceEx方法识别不了 暂时启用 dgh 2016.08.26
                //ulong freeBytesAvailable;
                //ulong totalNumberOfBytes;
                //ulong totalNumberOfFreeBytes;
                //bool success = NativeMethod.GetDiskFreeSpaceEx(path, out freeBytesAvailable, out totalNumberOfBytes,
                //          out totalNumberOfFreeBytes);
                //if (!success)
                //    throw new Win32Exception();
                //return (double)freeBytesAvailable / (1024 * 1024);
                #endregion
                var dri = new DriveInfo(path);
                var free = (double)dri.AvailableFreeSpace / (1024 * 1024);
                return free;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return 0.0;
            }
        }

        public static bool CheckVideoControllerDriver()
        {
            try
            {
                var mc = new ManagementClass("Win32_VideoController");
                ManagementObjectCollection moc = mc.GetInstances();

                if (moc.Count == 0)
                {
                    //xp下显卡数量为0，表示没有安装驱动
                    return false;
                }
                foreach (ManagementBaseObject mo in moc)
                {
                    var status = mo.Properties["Status"].Value.ToString();
                    if (status != "OK")
                    {
                        return false;
                    }
                    var name = mo.Properties["Name"].Value.ToString();
                    if (name == "标准 VGA 图形适配器")
                    {
                        //win 7 未安装驱动
                        return false;
                    }
                }
            }
            catch
            {
                return true;
            }
            return true;
        }

        public static string[] GetNetworkBoardCastIpArr()
        {
            //string arr = "255.255.255.255";
            var arr = string.Empty;
            try
            {
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementBaseObject mo in moc)
                {
                    var isEnable = (bool)mo.Properties["IPEnabled"].Value;
                    if (isEnable)
                    {
                        var ips = (string[])mo.Properties["IPAddress"].Value;
                        var sub = (string[])mo.Properties["IPSubnet"].Value;
                        for (var i = 0; i < ips.Length; i++)
                        {
                            //Trace.WriteLine(ips[i] + ":" + sub[i]);
                            arr += ";" + ComputeBoardCastIp(ips[i], sub[i]);
                        }
                    }
                }
            }
            catch
            {
                Trace.WriteLine("Win32_NetworkAdapterConfiguration Error");
            }
            return arr.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// 由IP和子网掩码，获取广播地址
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="mask"></param>
        /// <returns></returns>
        private static string ComputeBoardCastIp(string ip, string mask)
        {
            var ipa = GetIpNum(ip);
            var ips = GetIpNum(mask);
            if (ipa == 0 || ips == 0)
            {
                return string.Empty;
            }
            var ipd = ipa | (~ips);
            var by = BitConverter.GetBytes(ipd);
            return string.Join(".", @by.Reverse().ToArray());
        }

        /// <summary>
        /// 将IP转换为Uint值
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static uint GetIpNum(string ip)
        {
            var ipa = IPAddress.Parse(ip);
            if (ipa.AddressFamily == AddressFamily.InterNetwork)
            {
                var ipb = ipa.GetAddressBytes().Reverse().ToArray();
                return BitConverter.ToUInt32(ipb, 0);
            }
            return 0;
        }

        /// <summary>
        /// 查询最大的盘符
        /// </summary>
        /// <returns></returns>
        internal static string GetMaxDrive()
        {
            DriveInfo[] drs = DriveInfo.GetDrives();
            var maxDrive = string.Empty;
            long maxSize = 0;
            foreach (DriveInfo dr in drs)
            {
                try
                {
                    if (dr.DriveType != DriveType.Fixed || dr.AvailableFreeSpace <= maxSize) continue;
                    maxDrive = dr.Name;
                    maxSize = dr.AvailableFreeSpace;
                }
                catch {; }
            }
            return maxDrive;
        }

        /// <summary>
        /// 判断路径是否在移动磁盘上
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CheckPathOnFixedDevice(string path)
        {
            try
            {
                var drName = Path.GetPathRoot(path);
                if (drName != null)
                {
                    var dr = new DriveInfo(drName);
                    return dr.DriveType == DriveType.Fixed;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试删除文件
        /// </summary>
        /// <param name="filePath"></param>
        public static void TryDeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 强制删除文件，否则改名后重启删除
        /// </summary>
        /// <param name="filePath"></param>
        public static void ForceDeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Log.RecordLog(ex.Message + "\r\n" + ex.StackTrace);
                    File.Move(filePath, filePath + ".err");
                    NativeMethod.MoveFileEx(filePath + ".err", string.Empty, NativeMethod.MoveFileFlags.DelayUntilReboot | NativeMethod.MoveFileFlags.ReplaceExisting);
                }
            }
        }

        /// <summary>
        /// 尝试删除文件夹
        /// </summary>
        /// <param name="dirPath"></param>
        public static void TryDeleteDirectory(string dirPath)
        {
            try
            {
                if (Directory.Exists(dirPath))
                {
                    Directory.Delete(dirPath, true);
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.Message + "\r\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 获取物理CPU序列号
        /// </summary>
        /// <returns></returns>
        public static string GetCpuInfo()
        {
            try
            {
                var sb = new StringBuilder();
                var mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    var str = mo.GetPropertyValue("ProcessorId") as string;
                    sb.Append(str + "|");
                }
                return sb.ToString();
            }
            catch
            {
                Trace.WriteLine("Win32_Processor Error");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取硬盘序列号
        /// </summary>
        /// <returns></returns>
        public static string GetDiskDriveInfo()
        {
            try
            {
                var sb = new StringBuilder();
                var mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    try
                    {
                        var str = mo.GetPropertyValue("SerialNumber") as string;
                        sb.Append(str + "|");
                    }
                    catch
                    {
                        var str = mo.GetPropertyValue("Model") as string;
                        sb.Append(str + "|");
                    }
                }
                return sb.ToString();
            }
            catch
            {
                Trace.WriteLine("Win32_DiskDrive Error");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取系统内存大小
        /// </summary>
        /// <returns></returns>
        public static string GetMemorySize()
        {
            try
            {
                long totalsize = 0;
                var mc = new ManagementClass("Win32_PhysicalMemory");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    var size = Convert.ToInt64(mo.GetPropertyValue("Capacity"));
                    totalsize += size;
                }
                return totalsize.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                Trace.WriteLine("Win32_PhysicalMemory Error");
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取网卡Mac
        /// </summary>
        /// <returns></returns>
        public static string GetNetworkAdapterMac()
        {
            try
            {
                var sb = new StringBuilder();
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if (Convert.ToBoolean(mo.GetPropertyValue("IPEnabled")))
                    {
                        var str = mo.GetPropertyValue("MACAddress") as string;
                        sb.Append(str + "|");
                    }
                }
                return sb.ToString();
            }
            catch
            {
                Trace.WriteLine("Win32_NetworkAdapter Error");
                return string.Empty;
            }
        }

        /// <summary>
        /// 启动一个后台线程
        /// </summary>
        /// <param name="threadName"></param>
        /// <param name="action"></param>
        [DebuggerStepThrough]
        public static void StartBackGroundThread(string threadName, ThreadStart action)
        {
            var th = new Thread(action)
            {
                Name = threadName,
                IsBackground = true
            };
            th.Start();
        }

        /// <summary>
        /// 获取当前计算机名称
        /// </summary>
        /// <returns></returns>
        public static string GetDeviceName() => Dns.GetHostName();

        /// <summary>
        /// 关闭显卡的硬件加速
        /// </summary>
        public static void DisableHardWareAccel()
        {
            var dv = new NativeMethod.DISPLAY_DEVICE
            {
                cb = Marshal.SizeOf(typeof(NativeMethod.DISPLAY_DEVICE))
            };
            var res = NativeMethod.EnumDisplayDevices(null, 0, ref dv, 0);
            if (!res) return;
            try
            {
                var keystr = dv.DeviceKey.Substring(18);
                RegistryKey key = Registry.LocalMachine.CreateSubKey(keystr);
                if (key != null) key.SetValue("Acceleration.Level", 0x5, RegistryValueKind.DWord);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            //ChangeDisplaySettings(IntPtr.Zero, CDS_RESET);
        }

        /// <summary>
        /// 打开显卡的硬件加速
        /// </summary>
        public static void EnableHardWareAccel()
        {
            var dv = new NativeMethod.DISPLAY_DEVICE
            {
                cb = Marshal.SizeOf(typeof(NativeMethod.DISPLAY_DEVICE))
            };
            var res = NativeMethod.EnumDisplayDevices(null, 0, ref dv, 0);
            if (!res) return;
            try
            {
                var keystr = dv.DeviceKey.Substring(18);
                RegistryKey key = Registry.LocalMachine.CreateSubKey(keystr);
                if (key != null) key.DeleteValue("Acceleration.Level");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
            //ChangeDisplaySettings(IntPtr.Zero, CDS_RESET);
        }

        /// <summary>
        /// 检查Zip文件是否正常
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool CheckZipFile(string fileName)
        {
            try
            {
                Trace.WriteLine("CheckZipFileStart : " + DateTime.Now.ToString("HH:mm:ss.fff"));
                //添加配置是否启用文件检测：有的电脑用不适用与这个检测方法 dgh 2016.06.13
                if (!Util.IsCheckFile)
                {
                    using (ZipArchive zip = ZipFile.OpenRead(fileName))
                    {
                        ReadOnlyCollection<ZipArchiveEntry> entries = zip.Entries;
                        return true;
                    }
                }
                else
                {
                    return true;
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                Trace.WriteLine("CheckZipFileOver : " + DateTime.Now.ToString("HH:mm:ss.fff"));
            }
        }

        public static void CheckAndInstallBackGroundService()
        {
            var windowsPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            //string commonPath = Environment.GetFolderPath(Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.CommonProgramFilesX86 : Environment.SpecialFolder.CommonProgramFiles);
            var commonPath = Environment.GetFolderPath(Environment.Is64BitOperatingSystem ? Environment.SpecialFolder.ProgramFilesX86 : Environment.SpecialFolder.ProgramFiles);
            //检查文件存在与否，如果不存在则复制过去
            //var serviceExePath = commonPath + "\\cdel\\CdelService.exe";
            var serviceExePath = commonPath + "\\cdelservice\\CdelService.exe";
            //服务中需要这两个文件
            //var sqlitedll = commonPath + "\\cdel\\System.Data.SQLite.dll";
            //var sqliteInteropdll = commonPath + "\\cdel\\x86\\SQLite.Interop.dll";
            var sqlitedll = commonPath + "\\cdelservice\\System.Data.SQLite.dll";
            var sqliteInteropdll = commonPath + "\\cdelservice\\x86\\SQLite.Interop.dll";
            try
            {
                var localSqlitedll = AppDomain.CurrentDomain.BaseDirectory + "\\System.Data.SQLite.dll";
                var localSqliteInteropdll = AppDomain.CurrentDomain.BaseDirectory + "\\x86\\SQLite.Interop.dll";
                var localExePath = AppDomain.CurrentDomain.BaseDirectory + "\\cdelservice.exe";
                //如果安装程序目录下不存在该文件则不执行
                if (!File.Exists(localExePath)) return;
                if (!File.Exists(serviceExePath))
                {
                    //if (!Directory.Exists(commonPath + "\\cdel"))
                    //{
                    //    Directory.CreateDirectory(commonPath + "\\cdel");
                    //}
                    if (!Directory.Exists(commonPath + "\\cdelservice"))
                    {
                        Directory.CreateDirectory(commonPath + "\\cdelservice");
                    }
                    File.Copy(localExePath, serviceExePath);
                }
                if (!File.Exists(sqlitedll))
                {
                    File.Copy(localSqlitedll, sqlitedll);
                }
                if (!File.Exists(sqliteInteropdll))
                {
                    //if (!Directory.Exists(commonPath + "\\cdel\\x86"))
                    //{
                    //    Directory.CreateDirectory(commonPath + "\\cdel\\x86");
                    //}
                    if (!Directory.Exists(commonPath + "\\cdelservice\\x86"))
                    {
                        Directory.CreateDirectory(commonPath + "\\cdelservice\\x86");
                    }
                    File.Copy(localSqliteInteropdll, sqliteInteropdll);
                }

            }
            catch
            {
                ;
            }
            //判断服务是否已经存在，如果已存在则检查路径是否一致
            var hasService = ServiceController.GetServices().Any(x => x.ServiceName.ToLower() == "cdelservice");
            if (hasService)
            {
                RegistryKey regKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\services\\CdelService");
                if (regKey != null)
                {
                    var path = regKey.GetValue("ImagePath") as string;
                    if (path != null && (serviceExePath.ToLower() == path.ToLower() || "\"" + serviceExePath.ToLower() + "\"" == path.ToLower()))
                    {
                        goto StartService;
                    }
                }
                var serv = new ServiceController("cdelservice");
                if (serv.CanStop) serv.Stop();
                ShellExecute("sc.exe", "delete cdelservice");
            }
            ShellExecute(windowsPath + "\\Microsoft.NET\\Framework\\v4.0.30319\\InstallUtil.exe", "\"" + serviceExePath + "\"");
        StartService:
            var service = new ServiceController("cdelservice");
            if (service.Status == ServiceControllerStatus.Stopped) service.Start();
            if (service.Status == ServiceControllerStatus.Paused) service.Continue();
        }
        public static void ShellExecute(string command, bool isShow = false, bool isExist = true) => ShellExecute(command, string.Empty, isShow, isExist);

        public static void ShellExecute(string command, string args, bool isShow = false, bool isExist = true)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = command,
                    Arguments = args,
                    UseShellExecute = true,
                    CreateNoWindow = !isShow,
                    WindowStyle = ProcessWindowStyle.Hidden
                }
            };
            proc.Start();
            if (isExist)//播放过程中的随堂提问不用WaitForExit dgh 2016.09.14
            {
                proc.WaitForExit();
            }

        }
    }
}
