using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32;

namespace CdelService.Utility
{
	public static class SystemInfo
    {
        /// <summary>
		/// 测试是否在线的线程
		/// </summary>
        //public static void ThreadTestOnline()
        //{
        //    StartBackGroundThread("在线测试", () =>
        //    {
        //        while (true)
        //        {
        //            try
        //            {
        //                int flag;
        //                var check = NativeMethod.IsNetworkAlive(out flag);
        //                if (!check)
        //                {
        //                    check = NativeMethod.InternetGetConnectedState(ref flag, 0);
        //                }
        //                Util.IsOnline = check;
        //            }
        //            catch (Exception ex)
        //            {
        //                Log.RecordLog(ex.ToString());
        //                Util.IsOnline = false;
        //            }
        //            Thread.Sleep(10000);
        //        }
        //    });
        //}
        public static void ThreadTestOnline()
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
                //var key = Registry.LocalMachine.CreateSubKey("Software\\CDEL\\" + ProductName);
                var key = Registry.LocalMachine.CreateSubKey("Software\\CDEL\\" +Common.ProductName);
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
                var key = Registry.LocalMachine.OpenSubKey("Software\\CDEL\\" + Common.ProductName);
                return key != null ? key.GetValue(name) : null;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                return null;
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
                var moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    var str = mo.GetPropertyValue("ProcessorId") as string;
                    sb.Append(str + "|");
                }
                return sb.ToString();
            }
            catch
            {
                Log.RecordLog("Win32_Processor Error");
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
                var moc = mc.GetInstances();
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
                Log.RecordLog("Win32_DiskDrive Error");
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
                var moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    var size = Convert.ToInt64(mo.GetPropertyValue("Capacity"));
                    totalsize += size;
                }
                return totalsize.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                Log.RecordLog("Win32_PhysicalMemory Error");
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
                var moc = mc.GetInstances();
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
                Log.RecordLog("Win32_NetworkAdapter Error");
                return string.Empty;
            }
        }

        ///// <summary>
        ///// 启动一个后台线程
        ///// </summary>
        ///// <param name="threadName"></param>
        ///// <param name="action"></param>
        //[DebuggerStepThrough]
        //public static void StartBackGroundThread(string threadName, ThreadStart action)
        //{
        //    var th = new Thread(action)
        //    {
        //        Name = threadName,
        //        IsBackground = true
        //    };
        //    th.Start();
        //}
	}
}
