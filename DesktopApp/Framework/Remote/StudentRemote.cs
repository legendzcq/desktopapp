using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Xml.Linq;

using Framework.Model;
using Framework.NewModel;
using Framework.Utility;

namespace Framework.Remote
{
    public class StudentRemote : RemoteBase
    {
        private const string StudentSalt = "eiiskdui";
        private const string MemberLevel = "pc";
        private const string MemberKey = "84772CDB";

        public enum VerificationCodeType
        {
            NOCHECK_REGISTER, // 0：不校验是否注册
            CHECK_REGISTER, // 1：校验是否注册
            CHECK_BINDED, // 2：校验手机号是否已被绑定
            VIP_CONTRACT// 3：vip签约获取验证码
        }

#if CHINAACC
        private const string MemberSalt = "fJ3UjIFyTu";
        private const string AdvSiteID = "1";
#endif
#if MED
        private const string MemberSalt = "tFdfJdfRys";
        private const string AdvSiteID = "5";
#endif
#if JIANSHE
        private const string MemberSalt = "fJ3UjIFyTu";
         private const string AdvSiteID = "4";
#endif
#if LAW
        private const string MemberSalt = "Yu3hUifOvJ";
#endif
#if CHINATAT
        private const string MemberSalt = "It1UjIJyYu";
#endif
#if G12E
        private const string MemberSalt = "L3iyA1nHui";
#endif
#if ZIKAO
        private const string MemberSalt = "wY2Y1FMs9n";
#endif
#if CHENGKAO
        private const string MemberSalt = "fJ3UjIFyTu";
#endif
#if KAOYAN
        private const string MemberSalt = "hgDfgYghKj";
#endif
#if FOR68
        private const string MemberSalt = "LyBsw3Ai1b";
#endif
#if CK100
        private const string MemberSalt = "fJ3UjIFyTu";
#endif
        public RemoteLogin Login(string userName, string passwd)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-17
             */
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();

            var paramData = new Dictionary<string, string>
            {
                {"username", userName},
                {"passwd", passwd},
                {"memberlevel", MemberLevel},
                {"memberkey", MemberKey},
                {"mid", Crypt.GetMachineKeyString()},
                {"mname", SystemInfo.GetDeviceName()},
                {"muname", ""},
                {"appname", Util.AppNameForLogin},
                {"isNeedReplaceMid", ""},
                {"platformSource", Interface.PlatformSource},
                {"domain", Interface.Domain},
                {"time", time}
            };

            var doormanParam = new Dictionary<string, string>
            {
                {"appType", "winpc"},
                //{"domain", ""},
                //{"publicKey", ""},
                //{"aesKey", ""},
                //{"resourcePath", ""},
                //{"sid", Interface.AppKey},
                {"appKey", Interface.AppKey},
                {"platform", Interface.PlatformSource},
                {"appVersion", Util.AppVersion},
                {"af", Interface.PlatformSource},
                {"fs", "300"},
                {"ve", Util.AppVersion},
                {"lt", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"ap", Interface.AppKey}
            };

            var byte_valueData = MixParamData(paramData, Interface.StudentLoginUrl_2021, doormanParam);

            //var paramData = new Dictionary<string, string>
            //{
            //    // app端
            //    {"username", userName},
            //    {"passwd", passwd},
            //    {"memberlevel", MemberLevel},
            //    {"memberkey", MemberKey},
            //    {"mid", Crypt.GetMachineKeyString()},
            //    {"mname", SystemInfo.GetDeviceName()},
            //    {"muname", ""},
            //    {"appname", Util.AppNameForLogin},
            //    {"isNeedReplaceMid", ""},
            //    {"platformSource", Interface.PlatformSource},
            //    {"domain", Interface.Domain},
            //    {"time", time}
            //};

            //var doormanParam = new Dictionary<string, string>
            //{
            //    // app端
            //    {"appType", "mobile"},
            //    //{"domain", ""},
            //    //{"publicKey", ""},
            //    //{"aesKey", ""},
            //    //{"resourcePath", ""},
            //    //{"sid", Interface.AppKey},
            //    {"appKey", Interface.AppKey},
            //    {"platform", "1"},
            //    {"appVersion", Util.AppVersion},
            //    {"af", "1"},
            //    {"fs", "101"},
            //    {"ve", Util.AppVersion},
            //    {"lt", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
            //    {"ap", Interface.AppKey}
            //};

            //// app端
            //var byte_valueData = MixParamData(paramData, "+/appLogin/userLogin", doormanParam);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                RemoteLoginReturn obj = WebProxyClient.JsonDeserialize<RemoteLoginReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code != "0" && obj.Result.Code != "-4" && obj.Result.Code != "-5" && obj.Result.Code != "-12" && obj.Result.Code != "-18")
                {
                    Trace.WriteLine(obj.Result.Msg);
                }
                return obj.Result;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }

            /*
            var pkey = Crypt.Md5(userName, Interface.Domain, passwd, MemberLevel, StudentSalt);
            var deviceName = SystemInfo.GetDeviceName();
            var values = new NameValueCollection
            {
                {"username", userName},
                {"domain", Interface.Domain},
                {"passwd", passwd},
                {"memberlevel", MemberLevel},
                {"memberkey", MemberKey},
                {"pkey", pkey},
                {"mid", Crypt.GetMachineKeyString()},
                // 恢复成原来的登录方式 20150703
                {"mname", deviceName},
                {"appname", Util.AppNameForLogin},
                {"version", Util.AppVersion},
                // 新添加的两个参数 dgh 2017.11.08
                {"from", "member_app_passwd"},
                {"desc", Util.AppNameForLogin}
            };
            var web = new WebProxyClient { TimeOut = 30000 };
            try
            {
                var buffer = web.UploadValues(Interface.StudentLoginUrl, values);
                var xml = Encoding.UTF8.GetString(buffer);
                xml = FixXmlHead(xml);
                var model = new RemoteLogin();
                var doc = XDocument.Parse(xml);
                var ret = doc.Element("ret");
                if (ret != null)
                {
                    var element = ret.Element("code");
                    if (element != null) model.Code = element.Value;
                    element = ret.Element("msg");
                    if (element != null) model.Msg = element.Value;
                    element = ret.Element("sid");
                    if (element != null) model.Sid = element.Value;
                    element = ret.Element("ssouid");
                    if (element != null)
                    {
                        int uid = int.TryParse(element.Value, out uid) ? uid : 0;
                        model.Ssouid = uid;
                    }
                    element = ret.Element("username");
                    if (element != null) model.Username = element.Value;
                    element = ret.Element("mlist");
                    if (element != null)
                    {
                        model.LoginedDeviceList = new List<RemoteLogin.LoginedDevice>();
                        var mc = element.Elements("m");
                        foreach (var mitem in mc)
                        {
                            var device = new RemoteLogin.LoginedDevice();
                            var mElement = mitem.Element("mid");
                            if (mElement != null) device.DeviceId = mElement.Value;
                            mElement = mitem.Element("mname");
                            if (mElement != null) device.DeviceName = mElement.Value;
                            model.LoginedDeviceList.Add(device);
                        }
                    }
                }
                if (model.Code != "0" && model.Code != "-4" && model.Code != "-5" && model.Code != "-12" && model.Code != "-18")
                {
                    Trace.WriteLine(xml);
                }
                return model;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
            */
        }

        /**获取当前账户的绑定设备列表
         * @author ChW
         * @date 2021-06-22
         */
        public ObservableCollection<RemoteLogin.LoginedDevice> GetBindedEquipment(string userName)
        {
            var time = StudentRemote.GetCurrentTimeStamp13().ToString();

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"username", userName},
                {"appname", Util.AppNameForLogin},
                {"memberlevel", MemberLevel},
                {"memberkey", MemberKey},
                {"time", time}
            };

            byte[] byte_valueData = MixParamData(paramData, Interface.GetBindedDeviceListUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<DeviceListModel>(responseData);

                return obj.Result.BindedDeviceList;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return new ObservableCollection<RemoteLogin.LoginedDevice>();
            }
        }

        /**发送验证码
         * @param mobilePhone 手机号
         * @param smsType 验证类别（0：不校验是否注册，1：校验是否注册,2：校验手机号是否已被绑定,3：vip签约获取验证码）
         * @author ChW
         * @date 2021-06-24
         */
        public SendVerificationResult SendVerificationCode(string mobilePhone, VerificationCodeType smsType)
        {
            var time = StudentRemote.GetCurrentTimeStamp13().ToString();

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"mobilePhone", mobilePhone},
                {"smstype", smsType.ToString()},
                {"time", time}
            };

            byte[] byte_valueData = MixParamData(paramData, Interface.SendVerificationCodeUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<SendVerificationCodeReturn>(responseData);
                if (obj != null)
                {
                    return obj.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        /**使用验证码效验身份
         * @author ChW
         * @date 2021-06-25
         */
        public CheckUserIdentityByVerificationCodeResult CheckUserIdentityByVerificationCode(string mobilePhone, string messageAuthCode)
        {
            var time = StudentRemote.GetCurrentTimeStamp13().ToString();

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"mobilePhone", mobilePhone},
                {"messageAuthCode", messageAuthCode},
                {"time", time}
            };

            byte[] byte_valueData = MixParamData(paramData, Interface.CheckUserIdentityByVerificationCodeUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<CheckUserIdentityByVerificationCodeReturn>(responseData);
                if (obj != null)
                {
                    return obj.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        /**解绑设备
         * @author ChW
         * @date 2021-06-25
         */
        public UnbindDeviceResult UnbindDevice(string selectedMid, string selectedMname)
        {
            var time = StudentRemote.GetCurrentTimeStamp13().ToString();

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"userMid", selectedMid},
                {"userMUname", selectedMname},
                {"appname", Util.AppNameForLogin},
                {"memberlevel", MemberLevel},
                {"memberkey", MemberKey},
                {"ssouid", Util.SsoUid.ToString()},
                {"username", Util.UserName},
                {"mid", Crypt.GetMachineKeyString()},
                {"mname", SystemInfo.GetDeviceName()},
                {"time", time}
            };

            byte[] byte_valueData = MixParamData(paramData, Interface.UnbindDeviceUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<UnbindDeviceReturn>(responseData);
                if (obj != null)
                {
                    return obj.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        /**绑定手机号
         * @author ChW
         * @date 2021-07-01
         */
        public BindPhoneResult BindPhone(string mobilePhone, string messageAuthCode)
        {
            var time = StudentRemote.GetCurrentTimeStamp13().ToString();

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"mobilePhone", mobilePhone},
                {"messageAuthCode", messageAuthCode},
                {"ssouid", Util.SsoUid.ToString()},
                {"time", time}
            };

            byte[] byte_valueData = MixParamData(paramData, Interface.BindPhoneUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<BindPhoneReturn>(responseData);
                if (obj != null)
                {
                    return obj.Result;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return null;
            }
        }

        /// <summary>
        /// 恢复设备 dgh
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public string KickDeviceInfo(string userName)
        {
            var time = Util.GetNowString();
            var mid = Crypt.GetMachineKeyString();
            var pkey = Crypt.Md5(userName, Interface.PlatformSource, Util.AppVersion, time, MemberSalt);
            pkey = pkey.ToLower();
            try
            {
                var url = Interface.DeviceUrl + "?appname=" + Util.AppNameForLogin + "&memberkey=" + MemberKey + "&memberlevel=" + MemberLevel + "&mid=" + mid + "&pkey=" + pkey + "&platformSource=" + Interface.PlatformSource + "&time=" + time + "&username=" + userName + "&version=" + Util.AppVersion;
                Trace.WriteLine("设备接口:" + url);
                return url;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return "";
            }
        }

        public bool KickDevice(string deviceId)
        {
            var pkey = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), deviceId, Util.AppNameForLogin, MemberLevel, StudentSalt);
            var values = new NameValueCollection
            {
                {"ssouid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"memberlevel", MemberLevel},
                {"memberkey", MemberKey},
                {"pkey", pkey.ToLower()},
                {"mid", deviceId},
                {"appname", Util.AppNameForLogin},
                {"version", Util.AppVersion}
            };
            var web = new WebProxyClient { TimeOut = 30000 };
            try
            {
                var buffer = web.UploadValues(Interface.StudentKickDevice, values);
                var xml = Encoding.UTF8.GetString(buffer);
                xml = FixXmlHead(xml);
                var doc = XDocument.Parse(xml);
                XElement ret = doc.Element("ret");
                if (ret != null)
                {
                    XElement element = ret.Element("code");
                    if (element != null)
                    {
                        return element.Value == "0";
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 记录用户登录
        /// </summary>
        public void RecordUser()
        {
            try
            {
                var values = new NameValueCollection
                {
                    {"userName", Util.UserName},
                    {"ssoUid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                    {"osVersion", Environment.OSVersion.VersionString},
                    {"ssid", Crypt.GetMachineKeyString()}
                };
                var web = new WebProxyClient();
                web.UploadValues(Interface.RecordUserUrl, values);
                //var buffer = web.UploadValues(Interface.RecordUserUrl, values);
                //var str = Encoding.Default.GetString(buffer);
                //Trace.WriteLine(str);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        #region 数据采集站点定义
#if CHINAACC
        private const int SiteId = 3;
#endif
#if MED
        private const int SiteId = 2;
#endif
#if JIANSHE
        private const int SiteId = 1;
#endif
#if LAW
        private const int SiteId = 4;
#endif
#if CHINATAT
        private const int SiteId = 5;
#endif
#if G12E
        private const int SiteId = 6;
#endif
#if ZIKAO
        private const int SiteId = 7;
#endif
#if CHENGKAO
        private const int SiteId = 10;
#endif
#if KAOYAN
        private const int SiteId = 8;
#endif
#if FOR68
        private const int SiteId = 9;
#endif
#if CK100
        private const int SiteId = 11;
#endif
        #endregion

        /// <summary>
        /// 上传收集的用户操作记录
        /// </summary>
        public void UploadBigDataLog()
        {
            try
            {
#if BIGDATA
                if (File.Exists(Log.DataFile))
                {
                    var content = File.ReadAllText(Log.DataFile);
                    if (content.Length == 0)
                    {
                        return;
                    }
                    content = content.Trim();
                    content = "[" + content.Replace("\r\n", ",") + "]";
                    var time = Util.GetNowString();
                    var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), time, "cdelofflineClint").ToLower();
                    var values = new NameValueCollection
                    {
                        {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                        {"siteID", SiteId.ToString(CultureInfo.InvariantCulture)},
                        {"platformSource", Interface.PlatformSource},
                        {"deviceId", Util.DeviceId},
                        {"file", content},
                        {"submitTime", time},
                        {"pKey", key}
                    };
                    var web = new WebProxyClient();
                    var data = web.UploadValues(Interface.UploadBigDataLogUrl, values);
                    var str = Encoding.UTF8.GetString(data);
                    Trace.WriteLine(str);
                    File.WriteAllText(Log.DataFile, string.Empty);
                }
#endif
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// 提交用户接受推送的信息数据
        /// </summary>
        /// <param name="msgId"></param>
        public void SavePcPushInfo(int msgId)
        {
            var time = Util.GetNowString();
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), msgId.ToString(CultureInfo.InvariantCulture), Crypt.GetMachineKeyString(), Interface.AppId.ToString(CultureInfo.InvariantCulture), time, StudentSalt).ToLower();
            var values = new NameValueCollection
            {
                {"pkey", key},
                {"time", time},
                {"msgID", msgId.ToString(CultureInfo.InvariantCulture)},
                {"deviceID", Crypt.GetMachineKeyString()},
                {"appID", Interface.AppId.ToString(CultureInfo.InvariantCulture)},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"isOpen", "1"}
            };
            try
            {
                var web = new WebProxyClient();
                var data = web.UploadValues(Interface.SavePcPushInfoUrl, values);
                var info = Encoding.UTF8.GetString(data);
                Trace.WriteLine(info);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// 检查最后登录的SID
        /// </summary>
        /// <returns></returns>
        public RemoteLogin CheckLastSId()
        {
            var time = Util.GetNowString();
            var key = Crypt.Md5("fJ3UjIFyTu", time, "ucChkUserLogin", Util.SessionId, Util.SsoUid.ToString(CultureInfo.InvariantCulture)).ToLower();
            var values = new NameValueCollection
            {
                {"cmd", "ucChkUserLogin"},
                {"sid", Util.SessionId},
                {"ssouid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"selfsid", "0"},
                {"pkey", key},
                {"time", time}
            };
            try
            {
                var item = new RemoteLogin();
                var web = new WebProxyClient();
                var buffer = web.UploadValues(Interface.StudentCheckSid, values);
                var xml = Encoding.UTF8.GetString(buffer);
                xml = FixXmlHead(xml);
                var doc = XDocument.Parse(xml);
                XElement ret = doc.Element("ret");
                if (ret != null)
                {
                    XElement elem = ret.Element("code");
                    if (elem != null) item.Code = elem.Value;
                    elem = ret.Element("msg");
                    if (elem != null) item.Msg = elem.Value;
                }
                return item;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 检查用户是否被屏蔽
        /// </summary>
        /// <returns></returns>
        public bool ChecUserAble()
        {
            var userName = Util.UserName + Interface.Domain;
            var time = Util.GetNowString();
            var key = Crypt.Md5(userName, Interface.AppId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, time, StudentSalt).ToLower();
            var values = new NameValueCollection
            {
                {"pkey", key},
                {"username", userName},
                {"appID", Interface.AppId.ToString(CultureInfo.InvariantCulture)},
                {"platformSource",Interface.PlatformSource},
                {"time", time}
            };
            var web = new WebProxyClient();
            try
            {
                var data = web.UploadValues(Interface.StudentCheckAbleUrl, values);
                //var str = Encoding.UTF8.GetString(data);
                //var obj = WebProxyClient.JsonDeserialize<RemoteReturnItem>(str, Encoding.UTF8);
                RemoteReturnItem obj = WebProxyClient.JsonDeserialize<RemoteReturnItem>(data);
                return obj.Code == 1;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return true;
            }
        }

        /// <summary>
        /// 获取跑马灯提示信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MarqueeInfoListItem> GetMarqueeInfoList()
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-15
             */
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            //var appkey="d8682a14-dc35-4aa1-9969-d02aead00db0";// 该appkey程序下面有跑马灯信息
            var key = Crypt.Md5(Interface.AppKey.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, time, StudentSalt);
            //var key = Crypt.Md5(appkey, Interface.PlatformSource, time, StudentSalt);

            var paramData = new Dictionary<string, string>
            {
                {"pkey", key.ToLower()},
                //{"appID", Interface.AppId.ToString(CultureInfo.InvariantCulture)},
                {"appkey", Interface.AppKey.ToString(CultureInfo.InvariantCulture)},
                //{"appkey", appkey},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"etime", etime}
            };

            var byte_valueData = MixParamData(paramData, Interface.StudentGetMarqueeInfoListUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                MarqueeInfoReturn obj = WebProxyClient.JsonDeserialize<MarqueeInfoReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.MarqueeList != null)
                {
                    return obj.Result.MarqueeList;
                }
                return null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return null;
            }

            /*
            var time = Util.GetNowString();
            var key = Crypt.Md5(Interface.AppId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, time + StudentSalt);
            var values = new NameValueCollection
            {
                {"pkey", key.ToLower()},
                {"appID", Interface.AppId.ToString(CultureInfo.InvariantCulture)},
                {"platformSource",Interface.PlatformSource},
                {"time", time}
            };
            var web = new WebProxyClient();
            try
            {
                var data = web.UploadValues(Interface.StudentGetMarqueeInfoListUrl, values);
                //var str = Encoding.UTF8.GetString(data);
                //var obj = WebProxyClient.JsonDeserialize<MarqueeInfoItem>(str, Encoding.UTF8);
                var obj = WebProxyClient.JsonDeserialize<MarqueeInfoItem>(data);
                if (obj != null && obj.MarqueeList != null)
                {
                    return obj.MarqueeList;
                }
                return null;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return null;
            }
            */
        }

        /// <summary>
        /// 获取离线时间
        /// </summary>
        public void GetOffLineTime()
        {
            var time = Util.GetNowString();
            var pkey = Crypt.Md5("1", time, Util.AppVersion, Interface.PlatformSource, StudentSalt);
            var values = new NameValueCollection
            {
                {"pkey", pkey.ToLower()},
                {"type", "1"},
                {"version", Util.AppVersion},
                {"platformSource",Interface.PlatformSource},
                {"time", time}
            };
            var web = new WebProxyClient();
            try
            {
                var data = web.UploadValues(Interface.StudentGetOfflineUseTimeUrl, values);
                //var str = Encoding.UTF8.GetString(data);
                //var obj = WebProxyClient.JsonDeserialize<OffLineTimeReturnItem>(str, Encoding.UTF8);
                OffLineTimeReturnItem obj = WebProxyClient.JsonDeserialize<OffLineTimeReturnItem>(data);
                if (obj != null && obj.Result != null && obj.Code == 1)
                {
                    Util.LastOffLineTime = obj.Result;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// 检查用户是否被冻结
        /// </summary>
        /// <returns></returns>
        public ReturnItem CheckUserFrozen()
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-15
             */
            TakenRemote.GetToken();
            var re = new ReturnItem { State = true };
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var pkey = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, MemberSalt, Util.TokenString);

            var paramData = new Dictionary<string, string>
            {
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion}
            };

            var byte_valueData = MixParamData(paramData, Interface.CheckUserFrozenUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                RemoteReturn obj = WebProxyClient.JsonDeserialize<RemoteReturn>(responseData);
                re.State = obj.Result.Code != 1;
                re.Message = obj.Result.Message;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return re;

            /*
            #region 新接口
            TakenRemote.GetToken();
            var re = new ReturnItem { State = true };
            var time = Util.GetNowString();
            var pkey = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, MemberSalt,Util.TokenString);
            var values = new NameValueCollection
            {
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion}
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.CheckUserFrozenUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<RemoteReturnItem>(data);
                re.State = obj.Code != 1;
                re.Message = obj.Message;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return re;
            #endregion
            */
        }

        /// <summary>
        /// 获取答疑板地址
        /// </summary>
        /// <returns></returns>
        public string GetStudentFaqUrl()
        {
            //var time = GetCurrentTimeStamp();
            ////var timeStr = new DateTime(1970, 1, 1, 8, 0, 0).AddSeconds(time);
            //var key = Crypt.Md5(Util.SessionId, Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.StudentFaqUrl, MemberLevel, time.ToString(CultureInfo.InvariantCulture), StudentSalt);
            //var toUrl = string.Format(Interface.StudentGotoUrlLoginUrl, Util.SessionId, Util.SsoUid, key, Interface.StudentFaqUrl.Replace("%", "%25").Replace("&", "%26"), MemberLevel, MemberKey, time);
            //Trace.WriteLine(time + ":" + toUrl);
            var toUrl = string.Format(Interface.StudentFaqUrl, Util.SessionId);
            return toUrl;
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>
        /// <returns></returns>
        private long GetCurrentTimeStamp()
        {
            try
            {
                var web = new WebProxyClient();
                var buffer = web.UploadValues(Interface.GetCurrentTimeUrl, null);
                var xml = Encoding.UTF8.GetString(buffer);
                xml = FixXmlHead(xml);
                var doc = XDocument.Parse(xml);
                XElement ret = doc.Element("time");
                if (ret != null)
                {
                    long cTime = long.TryParse(ret.Value, out cTime) ? cTime : 0;
                    return cTime;
                }
                return Util.GetNowTimeStamp();
            }
            catch (Exception)
            {
                return Util.GetNowTimeStamp();
            }
        }

        /**获取服务器时间，新接口的替换
         * @author ChW
         * @date 2021-06-17
         */
        public static long GetCurrentTimeStamp13()
        {
            if (ServerTime != 0)
            {
                return ServerTime;
            }

            return GetServerTimeStamp13();
        }

        // 获取服务器精确时间
        public static long GetServerTimeStamp13()
        {
            var valueData = new Dictionary<string, object> // 新接口的参数集合
            {
                {"domain", "cdel"},
                {"resourcePath", Interface.GetCurrentTimeUrl_2021}
            };

            var json_valueData = JsonSerializer.Serialize(valueData, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            var byte_valueData = Encoding.UTF8.GetBytes(json_valueData);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                RemoteTime obj = WebProxyClient.JsonDeserialize<RemoteTime>(responseData);
                if (obj != null && obj.Result != null)
                {
                    long cTime = long.TryParse(obj.Result, out cTime) ? cTime : 0;
                    ServerTime = cTime;
                    return cTime;
                }
                return Util.GetNowTimeStamp13();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return Util.GetNowTimeStamp13();
            }
        }

        public DateTime GetCurrentTime()
        {
            try
            {
                var web = new WebProxyClient();
                var buffer = web.UploadValues(Interface.GetCurrentTimeUrl, null);
                var xml = Encoding.UTF8.GetString(buffer);
                xml = FixXmlHead(xml);
                var doc = XDocument.Parse(xml);
                XElement ret = doc.Element("time");
                if (ret != null)
                {
                    long cTime = long.TryParse(ret.Value, out cTime) ? cTime : 0;
                    if (cTime > 0)
                    {
                        DateTime time = new DateTime(1970, 1, 1, 8, 0, 0).AddSeconds(cTime);
                        return time;
                    }
                }
                return DateTime.Now;
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static long ServerTime
        {
            get
            {
                return _serverTime;
            }
            set
            {
                _serverTime = value;
            }
        }
        private static long _serverTime = 0;

#if CHINAACC
        /// <summary>
        /// 获取用户资料
        /// </summary>
        public void GetUserData()
        {
            var time = Util.GetNowString();
            var pkey = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, MemberSalt);
            var values = new NameValueCollection
            {
                {"pkey", pkey.ToLower()},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"platformSource", Interface.PlatformSource},
                {"version", Util.AppVersion},
                {"time", time}
            };
            var web = new WebProxyClient();

            try
            {
                var data = web.UploadValues(Interface.GetUserDataUrl, values);
                var str = Encoding.UTF8.GetString(data);
                Trace.WriteLine(str);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

#endif

#if CHINAACC || JIANSHE || MED
        public string GetAdvUrl() => Interface.AdvUrl;
        /// <summary>
        /// 我的服务
        /// </summary>
        public string GetServiceUrl()
        {
            var time = Util.GetNowString();
            var pkey = Crypt.Md5(Interface.PlatformSource, Util.AppVersion, time, MemberSalt);
            pkey = pkey.ToLower();
            var url = Interface.MyServiceUrl + "?pkey=" + pkey + "&platformSource=" + Interface.PlatformSource + "&siteID=" + AdvSiteID + "&version=" + Util.AppVersion + "&time=" + time;
            Trace.WriteLine("我的服务:" + url);
            return url;
        }
#endif
        /// <summary>
        /// 跳转网页 dgh 2016.09.14
        /// </summary>
        /// <param name="queryUrl">要跳转的网址</param>
        /// <returns></returns>
        public string GetStudentQueryUrl(string queryUrl)
        {
            var time = GetCurrentTimeStamp();
            var key = Crypt.Md5(Util.SessionId, Util.SsoUid.ToString(CultureInfo.InvariantCulture), queryUrl, MemberLevel, time.ToString(CultureInfo.InvariantCulture), StudentSalt);
            var toUrl = string.Format(Interface.StudentGotoUrlLoginUrl, Util.SessionId, Util.SsoUid, key, queryUrl.Replace("%", "%25").Replace("&", "%26"), MemberLevel, MemberKey, time);
            Trace.WriteLine(time + ":" + toUrl);
            return toUrl;
        }
        /// <summary>
        /// 随堂提问(播放中) dgh 2016.09.14
        /// </summary>
        /// <param name="cwareId">课程ID</param>
        /// <param name="bordId">答疑板ID</param>
        /// <param name="QNo">讲义号</param>
        /// <param name="jy_url">讲义</param>
        public void GotoLoginedWebSite(int cwareId, int bordId, string QNo, string jy_url)
        {
#if CHINAACC
            var qurl = string.Format(Interface.queryUrl, cwareId, QNo, jy_url);
#else
                     var qurl = string.Format(Interface.queryUrl, bordId, QNo, jy_url);
#endif
            var turl = string.Format(Interface.LoginQueryUrl, qurl.Replace("%", "%25").Replace("/", "%2F").Replace(":", "%3A").Replace("&", "%26"));
            var gotourl = GetStudentQueryUrl(turl);
            SystemInfo.ShellExecute(gotourl, true, false);
        }

        public void GoToUrl(string url)
        {
            var turl = string.Format(Interface.LoginQueryUrl, url.Replace("%", "%25").Replace("/", "%2F").Replace(":", "%3A").Replace("&", "%26"));
            var gotourl = GetStudentQueryUrl(turl);
            SystemInfo.ShellExecute(gotourl, true, false);
        }
        /// <summary>
        /// 网页中选课
        /// </summary>
        public void SelectCourse()
        {
            var gotourl = GetStudentQueryUrl(Interface.GetSelectCourseUrl);
            SystemInfo.ShellExecute(gotourl, true, false);
        }
    }

    [DataContract]
    public class ReturnObject
    {
        [DataMember(Name = "success")]
        public string Success { get; set; }
        [DataMember(Name = "retry")]
        public string Retry { get; set; }
        [DataMember(Name = "result")]
        public string Result { get; set; }
    }
}
