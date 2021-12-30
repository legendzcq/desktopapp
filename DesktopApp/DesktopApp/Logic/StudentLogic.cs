using System;
using System.Collections;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Framework.Local;
using Framework.Model;
using Framework.NewModel;
using Framework.Remote;
using Framework.Utility;


namespace DesktopApp.Logic
{
    /// <summary>
    /// 学员所涉及的业务逻辑
    /// </summary>
    internal static class StudentLogic
    {

        private static readonly object Lockobj = new object();

        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public static void ExecuteLogin(string userName, string password, Action<ReturnItem> callback, Action<ReturnItem, RemoteLogin/*List<RemoteLogin.LoginedDevice>*/> relogincallback = null)
        {
            SystemInfo.StartBackGroundThread("异步执行登录", () =>
            {
                lock (Lockobj)
                {
                    if (!string.IsNullOrEmpty(Util.SessionId))
                    {
                        callback(new ReturnItem { State = true });
                        return;
                    }
                    var re = new ReturnItem();
                    var stu = new StudentRemote();
                    TakenRemote.GetToken();
                    RemoteLogin item = stu.Login(userName, password);
                    if (item == null)
                    {
                        re.Message = "登录失败:请检查网络！";
                        re.State = false;
                        callback(re);
                        return;
                    }
                    if (item.Code == "0")
                    {
                        Util.UserName = userName;
                        Util.SsoUid = item.Ssouid;
                        Util.Password = password;
                        Util.SessionId = item.Sid;
                        var localstu = new StudentData();
                        if (localstu.AddLoginInfo(new StudentInfo
                        {
                            UserName = userName,
                            Password = password,
                            Ssouid = item.Ssouid,
                            LastLogin = DateTime.Now
                        }))
                        {
                            re.State = true;
                            callback(re);
                        }
                        else
                        {
                            re.Message = "登录失败:本地信息保存失败";
                            re.State = false;
                            callback(re);
                        }
                    }
                    else if (item.Code == "-4")
                    {
                        re.Message = "学员代码或密码错误";
                        re.State = false;
                        callback(re);
                    }
                    else if (item.Code == "-5")
                    {
                        re.Message = "学员代码或密码错误";
                        re.State = false;
                        callback(re);
                    }
                    else if (item.Code == "-12")
                    {
                        Log.RecordData("LoginTooMany", userName, "true");
                        re.Message ="您在太多电脑上登录了该帐号";
                        re.State = false;
                        //callback(re);
                        if (relogincallback != null)
                        {
                            Util.SsoUid = item.Ssouid;
                            Util.UserName = userName;
                            relogincallback(re, item/*.LoginedDeviceList*/);
                            return;
                        } 
                    }
                    else if (item.Code == "-18")
                    {
                        re.Message = "该设备已经被注销，不能再次使用";
                        re.State = false;
                        //callback(re);
                        if (relogincallback != null)
                        {
                            Util.SsoUid = item.Ssouid;
                            Util.UserName = userName;
                            relogincallback(re, item/*.LoginedDeviceList*/);
                            return;
                        }
                    }
                    else
                    {
                        re.Message = "登录失败:请检查您的学员代码和密码是否正确";
                        re.State = false;
                        callback(re);
                    }
                }
            });
        }

        public static void KickDeviceAndRelogin(string userName, string password, string deviceId, Action<ReturnItem> callback)
        {
            SystemInfo.StartBackGroundThread("异步执行登录", () =>
            {
                lock (Lockobj)
                {
                    var re = new ReturnItem();
                    var stu = new StudentRemote();
                    if (Util.SsoUid != 0) stu.KickDevice(deviceId);
                    RemoteLogin item = stu.Login(userName, password);
                    if (item == null)
                    {
                        re.Message = "登录失败:请检查网络！";
                        re.State = false;
                        callback(re);
                        return;
                    }
                    if (item.Code == "0")
                    {
                        Util.UserName = userName;
                        Util.SsoUid = item.Ssouid;
                        Util.Password = password;
                        Util.SessionId = item.Sid;
                        var localstu = new StudentData();
                        if (localstu.AddLoginInfo(new StudentInfo
                        {
                            UserName = userName,
                            Password = password,
                            Ssouid = item.Ssouid,
                            LastLogin = DateTime.Now
                        }))
                        {
                            re.State = true;
                            callback(re);
                        }
                        else
                        {
                            re.Message = "登录失败:本地信息保存失败";
                            re.State = false;
                            callback(re);
                        }
                    }
                    else if (item.Code == "-4")
                    {
                        re.Message = "学员代码或密码错误";
                        re.State = false;
                        callback(re);
                    }
                    else if (item.Code == "-5")
                    {
                        re.Message = "学员代码或密码错误";
                        re.State = false;
                        callback(re);
                    }
                    else if (item.Code == "-12")
                    {
                        Log.RecordData("LoginTooMany", userName, "true");
                        re.Message = "您在太多电脑上登录了该帐号";
                        re.State = false;
                        //Util.SsoUid = item.Ssouid;
                        callback(re);
                    }
                    else if (item.Code == "-18")
                    {
                        re.Message = "该设备已经被注销，不能再次使用";
                        re.State = false;
                        callback(re);
                    }
                    else
                    {
                        re.Message = "登录失败:请检查您的学员代码和密码是否正确";
                        re.State = false;
                        callback(re);
                    }
                }
            });
        }

        /// <summary>
        /// 检查学员能否继续使用客户端
        /// </summary>
        public static void CheckUserCanUseClient(Action<ReturnItem> callback, bool callCheckFrozen)
        {
            SystemInfo.StartBackGroundThread("检查最后登录Sid", () =>
            {
                var re = new ReturnItem() { State = true };
                var stu = new StudentRemote();
                /*不再检查离线14小时 20150703*/
                //stu.GetOffLineTime();
                if (string.IsNullOrEmpty(Util.SessionId))
                {
                    lock (Lockobj)
                    {
                        if (string.IsNullOrEmpty(Util.SessionId))
                        {
                            RemoteLogin item = stu.Login(Util.UserName, Util.Password);
                            if (item == null)
                            {
                                //网络异常，不做处理;
                                re.State = true;
                                callback(re);
                                return;
                            }
                            if (item.Code == "0")
                            {
                                Util.SsoUid = item.Ssouid;
                                Util.SessionId = item.Sid;
                                var localstu = new StudentData();
                                localstu.AddLoginInfo(new StudentInfo
                                {
                                    UserName = Util.UserName,
                                    Password = Util.Password,
                                    Ssouid = item.Ssouid,
                                    LastLogin = DateTime.Now
                                });
                            }
                            else if (item.Code == "-4")
                            {
                                re.Message = "学员代码或密码错误";
                                re.State = false;
                            }
                            else if (item.Code == "-5")
                            {
                                re.Message = "学员代码或密码错误";
                                re.State = false;
                            }
                            else if (item.Code == "-12")
                            {
                                Log.RecordData("LoginTooMany", Util.UserName, "true");
                                re.Message = "您在太多电脑上登录了该帐号";
                                re.State = false;
                            }
                            else if (item.Code == "-18")
                            {
                                re.Message = "该设备已经被注销，不能再次使用";
                                re.State = false;
                            }
                            else
                            {
                                re.Message = "登录失败:请检查您的学员代码和密码是否正确";
                                re.State = false;
                            }
                            if (!re.State)
                            {
                                callback(re);
                                return;
                            }
                        }
                    }
                }
                /**去掉在授权设备登录时的检测踢人操作
                 * @author ChW
                 * @date 2021-06-16
                 */
                //if (!string.IsNullOrEmpty(Util.SessionId))
                //{
                //    var item = stu.CheckLastSId();
                //    if (item != null && item.Code == "-7")
                //    {
                //        re.State = false;
                //        re.Message = "您的帐号在另一地点登录，您被迫下线\r\n\r\n如果这不是您本人的操作，那么您的密码很可能已经泄漏\r\n建议您修改密码";
                //        callback(re);
                //    }
                //}
                //#if CHINAACC || MED || JIANSHE || KAOYAN || CHINATAT|| G12E||ZIKAO||FOR68
                if (callCheckFrozen)
                {
                    var rer = stu.CheckUserFrozen();
                    if (!rer.State)
                    {
                        re.State = false;
                        re.Message = rer.Message;
                        callback(re);
                        return;
                    }
                }
//#endif
                //if (!stu.ChecUserAble())
                //{
                //	re.State = false;
                //	re.Message = "该用户已经被禁用";
                //	callback(re);
                //}
            });
        }

        public static void GetMarqueeInfo(Action<IEnumerable<MarqueeInfoListItem>> callback)
        {
            SystemInfo.StartBackGroundThread("获取跑马灯", () =>
            {
                var mlist = new StudentRemote().GetMarqueeInfoList();
                if (callback != null) callback(mlist);
            });
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public static void ExecuteLogout()
        {
            var local = new StudentData();
            local.Logout();

            /**
             * 添加登出操作附加任务的异常处理
             * @author ChW
             * @date 2021-05-14
             */
            try
            {
                Process.Start(AppDomain.CurrentDomain.BaseDirectory + "upforthis.exe", "restart");
            }
            catch (System.ComponentModel.Win32Exception e)
            {
                Trace.WriteLine(e.Message);
                return;
                //throw e;
            }
            finally
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// 记录用户
        /// </summary>
        public static void RecordUser()
        {
            var stu = new StudentRemote();
            stu.RecordUser();
        }
        /// <summary>
        /// 恢复设备 dgh 2016.11.23
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <returns></returns>
        public static string KickDeviceInfo(string userName)
        {
            return new StudentRemote().KickDeviceInfo(userName);
        }
    }
}
