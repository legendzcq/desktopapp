using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;

namespace RestorTool
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnRestor_Click(object sender, EventArgs e)
        {
            RestorFiles();
            RestorPlayer();
        }
        /// <summary>
        /// 修复文件 操作方法
        /// </summary>
        private void RestorFiles()
        {
            rchFile.Text = "";
            string xmlPath = AppDomain.CurrentDomain.BaseDirectory + "\\Common.xml";
            string commonPath, systemPath;
            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string tip = "";
            try
            {
                if (File.Exists(xmlPath))
                {
                    rchFile.Text = "正在检测........";
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
                    XmlDocument doc = new XmlDocument();
                    doc.Load(xmlPath);
                    XmlElement elRoot = doc.DocumentElement;
                    XmlNodeList nodeList = elRoot.SelectNodes("files");

                    foreach (XmlNode node in nodeList)
                    {
                        string forwin = node.Attributes["forwin"].Value;
                        string type = node.Attributes["type"].Value;
                        string local = node.Attributes["local"].Value;
                        string downpath = node.Attributes["downpath"].Value;
                        //文件目录
                        string sourceDirectory = "";
                        //源文件位置
                        if (type.ToLower() == "app")
                        {
                            sourceDirectory = appPath + local;

                        }
                        else if (type.ToLower() == "commonfile")
                        {
                            sourceDirectory = commonPath + local;
                        }
                        else if (type.ToLower() == "system32")
                        {
                            sourceDirectory = systemPath + local;
                        }
                        //先判断根目录是否存在
                        if (!Directory.Exists(sourceDirectory))
                        {
                            Directory.CreateDirectory(sourceDirectory);
                        }
                        if (forwin == "")
                        {
                            foreach (XmlNode chnode in node.ChildNodes)
                            {
                                string hash = chnode.Attributes["hash"].Value;
                                string name = chnode.Attributes["name"].Value;
                                //源文件位置
                                string localFile = Path.Combine(sourceDirectory, name);
                                string[] strDll = Directory.GetFiles(sourceDirectory, name);
                                string url = downpath + name;//网址文件
                                if (strDll.Length > 0)//存在文件
                                {
                                    if (hash != "")//该文件的哈希值存在
                                    {
                                        //判断哈希值是否相等
                                        bool bol = Restor.FileHashEqual(strDll[0], hash);
                                        if (!bol)
                                        {
                                            string downFile = localFile + ".old";//先重命名下载文件的名称
                                            DownLoadFile(url, downFile);//下载文件
                                            //判断下载后的文件的哈希值是否相等
                                            bool bolDown = Restor.FileHashEqual(downFile, hash);
                                            if (bolDown)
                                            {
                                                //重命名源文件
                                                File.Move(localFile, localFile + "1.old");
                                                //将下载的文件重命名为原来的名称
                                                File.Move(downFile, localFile);
                                                if (name == "ffdshow.ax")
                                                {
                                                    RunDllReg(localFile);//注册文件
                                                }
                                                tip += "更新文件:" + name + "已修复\r\n";
                                            }
                                        }
                                    }
                                }

                                else
                                {
                                    DownLoadFile(url, localFile);
                                    if (name == "ffdshow.ax")
                                    {
                                        RunDllReg(localFile);//注册文件
                                    }
                                    tip += "丢失文件：" + name + "已修复\r\n";
                                }
                            }

                        }
                        else
                        {
                            var ver = Environment.OSVersion.Version;//判断当前系统版本
                            string version = ver.Major.ToString() + "." + ver.Minor.ToString();
                            if (ver.Major.ToString() == "5" || forwin.Contains(version))
                            {
                                foreach (XmlNode chnode in node.ChildNodes)
                                {
                                    string name = chnode.Attributes["name"].Value;
                                    //源文件位置
                                    string localFile = Path.Combine(sourceDirectory, name);
                                    string[] strDll = Directory.GetFiles(sourceDirectory, name);
                                    string url = downpath + name;//网址文件
                                    if (strDll.Length <= 0)//不存在
                                    {
                                        DownLoadFile(url, localFile);
                                        //string command = chnode.Attributes["command"].Value;
                                        RunDllReg(localFile);//注册文件
                                        tip += "丢失文件：" + name + "已修复\r\n";
                                    }
                                }
                                break;
                            }
                        }
                    }
                    if (tip == "")
                    {
                        rchFile.Text = "该程序不需要修复";
                    }
                    else
                    {
                        Application.DoEvents();
                        Thread.Sleep(2000);
                        rchFile.Text = tip + "修复完毕";
                    }
                }
                else
                {
                    MessageBox.Show("缺少配置文件");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        }
        /// <summary>
        /// 修复播放器
        /// </summary>
        private void RestorPlayer()
        {
            var key = Registry.CurrentUser.CreateSubKey(@"Software\GNU\ffdshow");
            if (key != null)
            {
                key.SetValue("trayIcon", 0, RegistryValueKind.DWord);
                key.SetValue("trayIconExt", 0, RegistryValueKind.DWord);
                key.SetValue("h264", 1, RegistryValueKind.DWord);
                key.SetValue("isWhitelist", 0, RegistryValueKind.DWord);
            }
            key = Registry.CurrentUser.CreateSubKey(@"Software\GNU\ffdshow_audio");
            if (key != null)
            {
                key.SetValue("trayIcon", 0, RegistryValueKind.DWord);
                key.SetValue("trayIconExt", 0, RegistryValueKind.DWord);
                key.SetValue("aac", 1, RegistryValueKind.DWord);
                key.SetValue("isWhitelist", 0, RegistryValueKind.DWord);
            }
        }
        /// <summary>
        /// 注册文件
        /// </summary>
        /// <param name="path"></param>
        private static void RunDllReg(string path)
        {
            string regPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\system32\\regsvr32.exe";
            var p = new Process
            {
                StartInfo =
                {
                    FileName = regPath,
                    Arguments = "/s \"" + path + "\""
                    //Arguments = " \"" + path + "\""//提示信息(可做测试使用)
                }
            };
            p.Start();
            p.WaitForExit();
        }
        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="url">http网址</param>
        /// <param name="localFile">要存的本地文件目录</param>
        private void DownLoadFile(string url, string localFile)
        {
            try
            {
                var myrq = (HttpWebRequest)WebRequest.Create(url);
                var myrp = (HttpWebResponse)myrq.GetResponse();
                long totalBytes = myrp.ContentLength;
                if (totalBytes < 0) return;
                using (var ms = new FileStream(localFile, FileMode.Create, FileAccess.Write))
                {
                    Stream st = myrp.GetResponseStream();
                    var by = new byte[1024];
                    int osize = st.Read(@by, 0, @by.Length);
                    while (osize > 0)
                    {
                        ms.Write(@by, 0, osize);
                        osize = st.Read(@by, 0, @by.Length);
                    }
                    st.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
