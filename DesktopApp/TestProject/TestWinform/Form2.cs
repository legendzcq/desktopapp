using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace TestWinform
{
	public partial class Form2 : Form
	{
		private byte[] _key = { 0x0d, 0x01, 0x7d, 0x94, 0x89, 0x7f, 0xfa, 0x11, 0x1a, 0x91, 0x71, 0x72, 0x2e, 0x0e, 0xad, 0x92 };

		public Form2()
		{
			InitializeComponent();
			SetKey(_key);
		}

		[DllImport("kernel32.dll")]
		private extern static IntPtr LoadLibrary(String path);
		[DllImport("kernel32.dll")]
		private extern static IntPtr GetProcAddress(IntPtr lib, String funcName);

		private delegate bool SetKeyDelegate(byte[] key);

		private delegate bool GetKeyDelegate(byte[] key);

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
						//Log.RecordLog(@"系统文件检测失败cm.dll，请重新安装");
						return;
					}
				}
			}

			IntPtr cmLib = LoadLibrary(cmPath);

			IntPtr api = GetProcAddress(cmLib, "SetKey");
			var setKey = (SetKeyDelegate)Marshal.GetDelegateForFunctionPointer(api, typeof(SetKeyDelegate));
			setKey(key);

			api = GetProcAddress(cmLib, "GetKey");
			var getKey = (GetKeyDelegate)Marshal.GetDelegateForFunctionPointer(api, typeof(GetKeyDelegate));
			var buffer = new byte[16];
			getKey(buffer);
			Trace.WriteLine("GetKeyResult:" + BitConverter.ToString(buffer).Replace("-", ""));
		}

		private void Form2_Load(object sender, EventArgs e)
		{
		}

		private void button1_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				PlayerMain.URL = ofd.FileName;
			}
		}

		private void button2_Click(object sender, EventArgs e)
		{
			var obj = PlayerMain.settings.isAvailable["rate"];
			if (obj)
			{
				PlayerMain.settings.rate = 2.0;
				this.Text = @"支持变速";
			}
			else
			{
				this.Text = @"不支持变速";
			}
		}

		private void button3_Click(object sender, EventArgs e)
		{
			var ofd = new OpenFileDialog();
			if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				var file = ofd.FileName;
				var newFile = file + ".mp4";
				//using (FileStream ws = File.Create(newFile))
				//{
				//	using (FileStream rs = File.OpenRead(file))
				//	{
				//		ws.Position = 1;
				//		var buffer = new byte[4096];
				//		var readLen = 0;
				//		while ((readLen = rs.Read(buffer, 0, 4096)) > 0)
				//		{
				//			ws.Write(buffer, 0, readLen);
				//		}
				//	}
				//}
				var fs = File.OpenRead(file);
				var buffer = new byte[1024];
				fs.Read(buffer, 0, 1024);
				fs.Close();
				fs.Dispose();
				TransformVideo(buffer, file, newFile);
				this.Text = "转换完成";
			}
		}



		private static readonly byte[] MachineKey = new byte[16];
		private static readonly byte[] FilePrefix = { 0x43, 0x44, 0x45, 0x4c };

		internal static bool TransformVideo(byte[] videoHeader, string fileBody, string fileSave)
		{
			var rnd = new Random();
			var cryptLenKb = rnd.Next(960) + 64;
			string savePath = Path.GetDirectoryName(fileSave);
			if (savePath != null && !Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}

			FileStream saveStream = File.Create(fileSave);
			FileStream bodyStream = File.OpenRead(fileBody);
			var finfo = new FileInfo(fileBody);
			var sizeB = BitConverter.GetBytes(finfo.Length);
			var aes = new AesCryptoServiceProvider
			{
				Padding = PaddingMode.None,
				KeySize = 128,
				Key = MachineKey
			};
			var iv = aes.IV;
			bodyStream.Seek(videoHeader.Length, SeekOrigin.Current);
			saveStream.Write(FilePrefix, 0, 4);
			saveStream.Write(BitConverter.GetBytes((cryptLenKb << 16) | rnd.Next(65535)), 0, 4);
			saveStream.Write(iv, 0, 16);
			saveStream.Write(sizeB, 0, sizeB.Length);
			var buffer = new byte[1024];
			using (var cs = new CryptoStream(saveStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
			{
				cs.Write(videoHeader, 0, videoHeader.Length);
				int offset = videoHeader.Length;
				int byRead;
				while ((byRead = bodyStream.Read(buffer, 0, 1024)) > 0)
				{
					cs.Write(buffer, 0, byRead);
					offset += byRead;
					if (offset >= 1024 * cryptLenKb) break;
				}
				cs.Flush();
				saveStream.Flush();
				while ((byRead = bodyStream.Read(buffer, 0, 1024)) > 0)
				{
					saveStream.Write(buffer, 0, byRead);
				}
			}
			saveStream.Close();
			bodyStream.Close();
			return true;
		}
	}
}
