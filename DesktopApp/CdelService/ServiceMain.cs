using CdelService.Local;
using CdelService.Model;
using CdelService.Remote;
using CdelService.Utility;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace CdelService
{
	public partial class ServiceMain : ServiceBase
	{
		public ServiceMain()
		{
			InitializeComponent();

			_timerMain = new System.Timers.Timer
			{
				Interval = 60000,
			};

			_timerMain.Elapsed += DoTimer;
		}

		private readonly System.Timers.Timer _timerMain;

        private int timeCount = 0;

		private void DoTimer(object sender, System.Timers.ElapsedEventArgs e)
		{
            timeCount++;
			//if (timeCount % 5 == 1)//30分钟执行一次
			//{
				InitData();
			//}
            //每分钟要执行的任务
            Log.RecordLog("DoTimer");
		}

		protected override void OnStart(string[] args)
		{
			Trace.WriteLine("Service Started");
			Trace.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
			_timerMain.Start();
		}

		protected override void OnStop()
		{
			Trace.WriteLine("Service Stop");
			_timerMain.Stop();
		}

		protected override void OnPause()
		{
			Trace.WriteLine("Service Pause");
			_timerMain.Enabled = false;
		}

		protected override void OnContinue()
		{
			Trace.WriteLine("Service Continue");
			_timerMain.Enabled = true;
		}

        /// <summary>
        /// 加载数据
        /// </summary>
        private void InitData()
        {
            //启动是否在线的测试
	        Log.RecordLog("测试网络");
            SystemInfo.ThreadTestOnline();
            if(Util.IsOnline)
            {
                OperData();
            }
            
        }

        /// <summary>
        /// 数据操作
        /// </summary>
        private void OperData()
        {
            //所有网校下载课堂的注册表信息
            string[] strArys = new string[] { "{B137F1CD-63BD-4BC6-B298-D98763D8F26C}", "{3EF81618-65C0-4008-8F1E-D2CC8A7F9857}", "{F2B170CC-1EB2-46AD-9687-31EDF0FE897B}", "{F3E0E3EE-6BD5-471C-A637-923B76AF605A}", "{C8F82721-261F-4AD5-949B-9D0E7853A410}", "{5F6893B9-71AB-4CB6-9306-2BF84172CB56}", "{4025CE03-3966-4411-8BD8-0E0A8D484644}", "{F97D6EA8-1893-4921-9128-930E968A0879}", "{2C60CCF7-6676-4B2C-A398-55934D32693E}", "{90948DED-AE6E-486A-A685-9E8477FFF1B3}" };
            YXType type = YXType.CHINAACC;
            string path = "";
            string rootUurl = "";
            foreach (string str in strArys)
            {
	            Log.RecordLog("Try Find App " + str);
                RegistryKey subKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\" + str + "_is1");
                if (subKey != null)
                {
                    path = subKey.GetValue("InstallLocation").ToString();
                    rootUurl = subKey.GetValue("URLInfoAbout").ToString();
                    #region 网校类型
                    if (rootUurl.Contains("chinaacc"))
                    {
                        type = YXType.CHINAACC;//会计网
                    }
                    else if (rootUurl.Contains("chengkao365"))
                    {
                        type = YXType.CHENGKAO;//成考
                    }
                    else if (rootUurl.Contains("chinatat"))
                    {
                        type = YXType.CHINATAT;//职教网
                    }
                    else if (rootUurl.Contains("for68"))
                    {
                        type = YXType.FOR68;//外语
                    }
                    else if (rootUurl.Contains("g12e"))
                    {
                        type = YXType.G12E;//中小学
                    }
                    else if (rootUurl.Contains("jianshe99"))
                    {
                        type = YXType.JIANSHE;//建设
                    }
                    else if (rootUurl.Contains("cnedu"))
                    {
                        type = YXType.KAOYAN;//考研
                    }
                    else if (rootUurl.Contains("chinalawedu"))
                    {
                        type = YXType.LAW;//法律
                    }
                    else if (rootUurl.Contains("med66"))
                    {
                        type = YXType.MED;//医学
                    }
                    else if (rootUurl.Contains("zikao365"))
                    {
                        type = YXType.ZIKAO;//自考
                    }
                    #endregion

	                Log.RecordLog(string.Format("Get App {0} at {1}", type, path));
                    Common.IniData(type, path);
					Log.RecordLog("GetLoginedStudent " + type);
                    var student = new StudentData();
                    StudentInfo studentInfo = student.GetLoginedStudent();
					var re = new StudentRemote();
                    if (studentInfo != null)
                    {
                        Util.UserName = studentInfo.UserName;
                        Util.Password = studentInfo.Password;
                        Util.SsoUid = studentInfo.Ssouid;
						Log.RecordLog("CheckUserFrozen " + type);
                        //如果该用户冻结则需写个文本标识
                        ReturnItem rt = re.CheckUserFrozen();
                        string fpath = path + "冻结标识.txt";
                        if (rt.Code == "1")
                        {
                            if (!File.Exists(fpath))
                            {
                                File.Create(fpath);
                            }
                        }
                        else
                        {
                            File.Delete(fpath);
                        }
                    }
					Log.RecordLog("UploadBigDataLog " + type);
					re.UploadBigDataLog();//上传大数据
                }
            }
        }
	}
}
