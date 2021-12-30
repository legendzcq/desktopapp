using CdelService.Local;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace CdelService.Utility
{
	internal static class Crypt
	{

		private static byte[] _machineKey;

		private static readonly Random Rnd = new Random();

		static Crypt()
		{
			GetMachineKey();
		}

		private static void GetMachineKey()
		{
			var da = new Baseinfo();
			var key = da.GetSecurityKey();
			if (key == null || key.Length == 0)
			{
				_machineKey = GenKey(key == null);
				if (key != null) da.InsertKey(_machineKey);
			}
			else
			{
				_machineKey = key;
			}
			//MachineKey = GenKey(true);
			SetKey(_machineKey);
			//Log.RecordLog("LoadSID");
		}

		private delegate bool SetKeyDelegate(byte[] key);

		private static void SetKey(byte[] key)
		{
			var commonPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
			var commonPathx86 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);

			string appPath = AppDomain.CurrentDomain.BaseDirectory;
			string cmPath = commonPath + "\\cdel\\cm.dll";
			if (!File.Exists(cmPath))
			{
				cmPath = commonPathx86 + "\\cdel\\cm.dll";
				if (!File.Exists(cmPath))
				{
					cmPath = appPath + "cm.dll";
					if (!File.Exists(cmPath))
					{
						Log.RecordLog(@"系统文件检测失败cm.dll，请重新安装");
						return;
					}
				}
			}

			IntPtr cmLib = NativeMethod.LoadLibrary(cmPath);

			IntPtr api = NativeMethod.GetProcAddress(cmLib, "SetKey");
			var setKey = (SetKeyDelegate)Marshal.GetDelegateForFunctionPointer(api, typeof(SetKeyDelegate));
			var value = setKey(key);
			//Log.RecordLog("SetKeyResult:" + value);
		}

		/// <summary>
		/// 获取机器Key的字符串
		/// </summary>
		/// <returns></returns>
		internal static string GetMachineKeyString()
		{
			return BitConverter.ToString(_machineKey).Replace("-", string.Empty);
		}
        /// <summary>
        /// 获取加密Key
        /// </summary>
        /// <param name="forOld"></param>
        /// <returns></returns>
        private static byte[] GenKey(bool forOld)
        {
            //Log.RecordLog("ComputeSID");
            var sb = new StringBuilder();
            sb.Append("CDEL");
            ManagementClass mc;
            ManagementObjectCollection moc;
            try
            {
                mc = new ManagementClass("Win32_BIOS");
                moc = mc.GetInstances();
                foreach (var str in from ManagementObject mo in moc select mo.GetPropertyValue("SerialNumber") as string
                    )
                {
                    sb.Append(str);
                }
            }
            catch
            {
                Log.RecordLog("Win32_BIOS Error");
            }
            try
            {
                mc = new ManagementClass("Win32_BaseBoard");
                moc = mc.GetInstances();
                foreach (var str in from ManagementObject mo in moc select mo.GetPropertyValue("SerialNumber") as string)
                {
                    sb.Append(str);
                }
            }
            catch
            {
                Log.RecordLog("Win32_BaseBoard Error");
            }
            if (!forOld)
            {
                try
                {
                    mc = new ManagementClass("Win32_Processor");
                    moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        var str = mo.GetPropertyValue("ProcessorId") as string;
                        sb.Append(str);
                    }
                }
                catch
                {
                    Log.RecordLog("Win32_Processor Error");
                }
                try
                {
                    mc = new ManagementClass("Win32_DiskDrive");
                    moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        var str = mo.GetPropertyValue("SerialNumber") as string;
                        sb.Append(str);
                    }
                }
                catch
                {
                    Log.RecordLog("Win32_DiskDrive Error");
                }
                try
                {
                    mc = new ManagementClass("Win32_PhysicalMemory");
                    moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        var str = mo.GetPropertyValue("SerialNumber") as string;
                        sb.Append(str);
                    }
                }
                catch
                {
                    Log.RecordLog("Win32_PhysicalMemory Error");
                }
            }
            var info = sb.ToString();
            var buffer = Encoding.Default.GetBytes(info);

            var md5 = new MD5CryptoServiceProvider();
            buffer = md5.ComputeHash(buffer);
            Log.RecordLog("ComputeSID OK");
            return buffer;
        }

		/// <summary>
		/// MD5
		/// </summary>
		/// <param name="contents"></param>
		/// <returns></returns>
		internal static string Md5(params string[] contents)
		{
			string content = string.Join(string.Empty, contents);
			byte[] buffer = Encoding.UTF8.GetBytes(content);
			using (var md5 = new MD5CryptoServiceProvider())
			{
				return BitConverter.ToString(md5.ComputeHash(buffer)).Replace("-", "");
			}
		}


		/// <summary>
		/// RC4解密字符串
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		internal static string Rc4DecryptString(string source)
		{
			if (string.IsNullOrEmpty(source)) return string.Empty;
			var buffer = Enumerable.Range(0, source.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(source.Substring(x, 2), 16)).ToArray();
			Rc4(buffer, _machineKey);
			return Encoding.UTF8.GetString(buffer).Substring(8);
		}

		/// <summary>
		/// Rc4原生算法
		/// </summary>
		/// <param name="bytes"></param>
		/// <param name="key"></param>
		private static void Rc4(byte[] bytes, byte[] key)
		{
			var s = new Byte[256];
			var k = new Byte[256];
			Byte temp;
			int i;

			for (i = 0; i < 256; i++)
			{
				s[i] = (Byte)i;
				k[i] = key[i % key.GetLength(0)];
			}

			int j = 0;
			for (i = 0; i < 256; i++)
			{
				j = (j + s[i] + k[i]) % 256;
				temp = s[i];
				s[i] = s[j];
				s[j] = temp;
			}

			i = j = 0;
			for (int x = 0; x < bytes.GetLength(0); x++)
			{
				i = (i + 1) % 256;
				j = (j + s[i]) % 256;
				temp = s[i];
				s[i] = s[j];
				s[j] = temp;
				int t = (s[i] + s[j]) % 256;
				bytes[x] ^= s[t];
			}
		}
	}
}