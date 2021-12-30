using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

using Framework.Local;
using Framework.Model;
using Framework.NewModel;
using Framework.Utility;

namespace Framework.Remote
{
    public class StudentWareRemote : RemoteBase
    {
        #region "接口加密Key定义"

        private const string StudentSalt = "eiiskdui";
#if CHINAACC
        private const string CourseSalt = "eiiskdui";
        private const string NewCourseSalt = "fJ3UjIFyTu";
#endif
#if MED
        private const string CourseSalt = "tFdfJdfRys";
        private const string NewCourseSalt = "tFdfJdfRys";
#endif
#if JIANSHE
        private const string CourseSalt = "fJ3UjIFyTu";
        private const string NewCourseSalt = "fJ3UjIFyTu";
#endif
#if LAW
        private const string CourseSalt = "Yu3hUifOvJ";
        private const string NewCourseSalt = "Yu3hUifOvJ";
#endif
#if CHINATAT
        private const string CourseSalt = "It1UjIJyYu";
        private const string NewCourseSalt = "It1UjIJyYu";
#endif
#if G12E
        private const string CourseSalt = "L3iyA1nHui";
        private const string NewCourseSalt = "L3iyA1nHui";
#endif
#if ZIKAO
        private const string CourseSalt = "wY2Y1FMs9n";
        private const string NewCourseSalt = "wY2Y1FMs9n";
#endif
#if CHENGKAO
        private const string CourseSalt = "fJ3UjIFyTu";
        private const string NewCourseSalt = "fJ3UjIFyTu";
#endif
#if KAOYAN
        private const string CourseSalt = "hgDfgYghKj";
        private const string NewCourseSalt = "hgDfgYghKj";
#endif
#if FOR68
        private const string CourseSalt = "LyBsw3Ai1b";
        private const string NewCourseSalt = "LyBsw3Ai1b";
#endif
#if CK100
        private const string CourseSalt = "fJ3UjIFyTu";
        private const string NewCourseSalt = "fJ3UjIFyTu";
#endif

        #endregion

        #region 通过科目获取学员课件方法

        public ReturnItem GetUserCourseList()
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-15
             */
            TakenRemote.GetToken();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var pkey = Crypt.Md5(Util.SessionId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString).ToLower();

            var paramData = new Dictionary<string, string>
            {
                {"appkey", Interface.AppKey},
                {"ltime", Util.TokenLongTime.ToString()},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"sid", Util.SessionId.ToString(CultureInfo.InvariantCulture)},
                {"time", time},
                {"etime", etime},
                {"version", Util.AppVersion}
            };

            var byte_valueData = MixParamData(paramData, Interface.GetUserCourseListUrl_2021);

            var re = new ReturnItem();
            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                StudentCourseReturn obj = WebProxyClient.JsonDeserialize<StudentCourseReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.MyCourseInfo != null)
                {
                    new StudentWareData().AddEduSubjectList(obj.Result.MyCourseInfo);
                    new StudentWareData().AddSubjectCourseRelation(obj.Result.CourseRelation);
                    foreach (StudentCourseLIst.StudentEduSubjectItem item in obj.Result.MyCourseInfo)
                    {
                        GetStudentWareBySubjectId(item.EduSubjectId);
                    }
                }
                re.State = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取学员课程信息失败";
            }
            return re;

            /*
            TakenRemote.GetToken();
            var time = Util.GetNowString();
            var pkey = Crypt.Md5(Util.SessionId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString).ToLower();
            var values = new NameValueCollection
            {
                {"appkey", Interface.AppKey},
                {"ltime", Util.TokenLongTime.ToString()},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"sid", Util.SessionId.ToString(CultureInfo.InvariantCulture)},
                {"time", time},
                {"version", Util.AppVersion}
            };
            var re = new ReturnItem();
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetUserCourseListUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentCourseLIst>(data);
                if (obj != null && obj.MyCourseInfo != null)
                {
                    new StudentWareData().AddEduSubjectList(obj.MyCourseInfo);
                    new StudentWareData().AddSubjectCourseRelation(obj.CourseRelation);
                    foreach (var item in obj.MyCourseInfo)
                    {
                        GetStudentWareBySubjectId(item.EduSubjectId);
                    }
                }
                re.State = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取学员课程信息失败";
            }
            return re;
            */
        }
        public void GetStudentWareBySubjectId(int eduSubjectId)
        {
            /**获取班次列表
             * @author ChW
             * @date 2021-06-15
             */
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var pkey = Crypt.Md5(Util.SessionId, eduSubjectId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, NewCourseSalt, Util.TokenString, time);

            var paramData = new Dictionary<string, string>
            {
                {"eduSubjectID", eduSubjectId.ToString(CultureInfo.InvariantCulture)},
                {"ltime", Util.TokenLongTime.ToString()},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"sid", Util.SessionId},
                {"time", time},
                {"etime", etime},
                {"uid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion},
                {"vflag",""}
            };
            paramData.Add("fordown", "1");

            var byte_valueData = MixParamData(paramData, Interface.GetUserWareBySubjectIdUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                StudentCwareReturn obj = WebProxyClient.JsonDeserialize<StudentCwareReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.CwareList != null)
                {
                    new StudentWareData().AddCwareList(eduSubjectId, obj.Result.CwareList);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            /*
            var time = Util.GetNowString();
            var pkey = Crypt.Md5(Util.SessionId, eduSubjectId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, NewCourseSalt, Util.TokenString, time);
            var values = new NameValueCollection
            {
                {"eduSubjectID", eduSubjectId.ToString(CultureInfo.InvariantCulture)},
                {"ltime", Util.TokenLongTime.ToString()},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"sid", Util.SessionId},
                {"time", time},
                {"uid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion},
                {"vflag",""}
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetUserWareBySubjectIdUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentCwareList>(data);
                if (obj != null && obj.CwareList != null)
                {
                    new StudentWareData().AddCwareList(eduSubjectId, obj.CwareList);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            */
        }
        #endregion

        /// <summary>
        /// 获取课程明细
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="classid"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ReturnItem GetStudentCourseDetail(string sid, string classid, string type)
        {
            var local = new StudentWareData();
            StudentCourseWare wareItem = local.GetCourseWareItem(classid);
            var re = new ReturnItem();
            if (wareItem.IsOpen == 0 || wareItem.PathUrl == null || wareItem.PathUrl.ToLower() == "/confirm/yetopen.asp")
            {
                re.Message = "课程尚未开放";
                return re;
            }

            /**
             * 新接口的替换
             * @author ChW
             * @date 2021-06-16
             */
            TakenRemote.GetToken();
            const string getType = "2";
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var pkey = Crypt.Md5(getType, Util.UserName, classid, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt);

            var paramData = new Dictionary<string, string>
            {
                {"cwID", classid},
                {"freeOpenVersion", classid},
                {"getType", getType},
                {"innerCwareID", ""},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"username", Util.UserName},
                {"version", Util.AppVersion},
                {"cdn", "1"},
                {"videoType", "1"},
            };

            var byte_valueData = MixParamData(paramData, Interface.GetStudentCourseDetailMemberUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                StudentCourseDetailReturn obj = WebProxyClient.JsonDeserialize<StudentCourseDetailReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.code == "1")
                {
                    obj.Result.Ret = obj.Result.Ret.Replace(".", "+").Replace("-", "/").Replace("_", "=");
                    var strbuf = Convert.FromBase64String(obj.Result.Ret);
                    strbuf = Crypt.DesDecrypt(strbuf);
                    var strBuff = Encoding.UTF8.GetString(strbuf);
                    StudentIEnumerableList list = WebProxyClient.JsonDeserialize<StudentIEnumerableList>(strbuf);
                    if (list.CourseWareList == null)
                    {
                        re.Message = "获取班次明细信息失败";
                        return re;
                    }

                    re.State = local.AddCourseDetailList(wareItem.CWareId, list.CourseWareList) && local.AddChapterList(wareItem.CWareId, list.ChapterList);
                    GetCwareKeyNew(classid);

                    if (!re.State) re.Message = "保存到数据库失败";
                    return re;
                }
                else
                {
                    re.Message = "获取班次明细信息失败";
                    return re;
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取班次明细信息失败";
                return re;
            }

            /*
            #region 新接口 dgh
            TakenRemote.GetToken();
            const string getType = "2";
            string time = Util.GetNowString();
            string pkey = Crypt.Md5(getType, Util.UserName, classid, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt);
            var values = new NameValueCollection
            {
                {"cwID", classid},
                {"freeOpenVersion", classid},
                {"getType", getType},
                {"innerCwareID", ""},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"username", Util.UserName},
                {"version", Util.AppVersion},
                {"cdn", "1"},
                {"videoType", "1"},
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetStudentCourseDetailMemberUrlNew, values);
                var str = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentCourseDetailNew>(buffer);
                if (obj.code == "1")
                {
                    obj.Ret = obj.Ret.Replace(".", "+").Replace("-", "/").Replace("_", "=");
                    var strbuf = Convert.FromBase64String(obj.Ret);
                    strbuf = Crypt.DesDecrypt(strbuf);
                    var strBuff = Encoding.UTF8.GetString(strbuf);
                    //Trace.WriteLine(strBuff);
                    var list = WebProxyClient.JsonDeserialize<StudentIEnumerableList>(strbuf);
                    if (list.CourseWareList == null)
                    {
                        re.Message = "获取班次明细信息失败";
                        return re;
                    }

                    re.State = local.AddCourseDetailList(wareItem.CWareId, list.CourseWareList) && local.AddChapterList(wareItem.CWareId, list.ChapterList);

                    GetCwareKeyNew(classid);

                    if (!re.State) re.Message = "保存到数据库失败";
                    return re;
                }
                else
                {
                    re.Message = "获取班次明细信息失败";
                    return re;
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取班次明细信息失败";
                return re;
            }
            #endregion
            */
        }
        #region 获取下载服务器IP相关

        /// <summary>
        /// 获取下载的IP地址
        /// </summary>
        /// <returns></returns>
        internal string[] GetDownloadIp()
        {
            var time = Util.GetNowString();
            var key = Crypt.Md5(Interface.PlatformSource, time, StudentSalt).ToLower();
            var values = new NameValueCollection
            {
                { "platformSource", Interface.PlatformSource },
                { "time", time },
                { "pkey", key }
            };
            try
            {
                var web = new WebProxyClient();
                var data = web.UploadValues(Interface.GetDownloadIpUrl, values);
                var str = Encoding.Default.GetString(data);
                ReturnIp obj = WebProxyClient.JsonDeserialize<ReturnIp>(str);
                return obj.Code == "1" ? obj.Ip.Split('#') : new string[0];
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return new string[0];
            }
        }

        [DataContract]
        private class ReturnIp
        {
            [DataMember(Name = "code")]
            public string Code { get; set; }
            [DataMember(Name = "ip")]
            public string Ip { get; set; }
        }
        #endregion

        #region 弹出知识点相关

        /// <summary>
        /// 获取课件视频对应的知识点测试及弹出时间
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public bool GetPointTestStartTime(int cwareId, string videoId)
        {
            /**
             * 新接口替换
             * @author ChW
             * @date 2021-06-07
             */
            TakenRemote.GetToken();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var key = Crypt.Md5(cwareId.ToString(CultureInfo.InvariantCulture), Convert.ToInt16(videoId).ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString).ToLower();

            var paramData = new Dictionary<string, string>
            {
                {"cwareID", cwareId.ToString(CultureInfo.InvariantCulture)},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", key},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"etime", etime },
                {"version", Util.AppVersion},
                {"videoID", Convert.ToInt16(videoId).ToString(CultureInfo.InvariantCulture)},
                //1为移动班只去知识点测试 2为下载课堂取全部的 不传的话按照旧逻辑查询
                {"isMobileClass", "2"}
            };

            var byte_valueData = MixParamData(paramData, Interface.GetPointTestStartTimeUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                PointTestStartTimeReturn obj = WebProxyClient.JsonDeserialize<PointTestStartTimeReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.ItemList != null)
                {
                    new Local.StudentWareData().AddPointTestStartTime(cwareId, videoId, obj.Result.ItemList);
                    foreach (PointTestStartTimeItem item in obj.Result.ItemList)
                    {
                        GetQuestionByPointTestId(item.TestId, item.PointOpenType);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return false;
            }

            /*
            #region 新接口 dgh 2018.01.17
            TakenRemote.GetToken();
            var time = Util.GetNowString();
            string key = Crypt.Md5(cwareId.ToString(CultureInfo.InvariantCulture), Convert.ToInt16(videoId).ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString).ToLower();
            var values = new NameValueCollection
            {
                {"cwareID", cwareId.ToString(CultureInfo.InvariantCulture)},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", key},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"version", Util.AppVersion},
                {"videoID", Convert.ToInt16(videoId).ToString(CultureInfo.InvariantCulture)},
                //1为移动班只去知识点测试 2为下载课堂取全部的 不传的话按照旧逻辑查询
                {"isMobileClass", "2"}
            };
            try
            {
                var web = new WebProxyClient();
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetPointTestStartTimeUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<PointTestStartTimeReturn>(data);
                if (obj.ItemList != null)
                {
                    new Local.StudentWareData().AddPointTestStartTime(cwareId, videoId, obj.ItemList);
                    foreach (var item in obj.ItemList)
                    {
                        GetQuestionByPointTestId(item.TestId, item.PointOpenType);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return false;
            }
            #endregion
            */
        }

        /// <summary>
        /// 获取知识点测（练习）试题目信息
        /// </summary>
        /// <param name="testId"></param>
        /// <param name="pointOpenType"></param>
        /// <returns></returns>
        public bool GetQuestionByPointTestId(int testId, string pointOpenType)
        {
            /**新接口替换
             * @author ChW
             * @date 2021-06-08
             */
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var key = Crypt.Md5(testId.ToString(CultureInfo.InvariantCulture), pointOpenType, Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString).ToLower();

            var paramData = new Dictionary<string, string>
            {
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", key},
                {"platformSource", Interface.PlatformSource},
                {"pointOpenType", pointOpenType},
                {"testID", testId.ToString(CultureInfo.InvariantCulture)},
                {"time", time},
                {"etime", etime},
                {"version", Util.AppVersion}
            };

            var byte_valueData = MixParamData(paramData, Interface.GetQuestionByPointTestIdUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                PointTestQuestionReturn obj = WebProxyClient.JsonDeserialize<PointTestQuestionReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.QuestionList != null && obj.Result.QuestionList.Any())
                {
                    new StudentWareData().AddPointTestQuestion(testId, obj.Result.QuestionList);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return false;
            }

            /*
            #region 新接口 dgh 2018.01.17
            var time = Util.GetNowString();
            string key = Crypt.Md5(testId.ToString(CultureInfo.InvariantCulture), pointOpenType, Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString).ToLower();
            var values = new NameValueCollection
            {
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", key},
                {"platformSource", Interface.PlatformSource},
                {"pointOpenType", pointOpenType},
                {"testID", testId.ToString(CultureInfo.InvariantCulture)},
                {"time", time},
                {"version", Util.AppVersion}
            };
            try
            {
                var web = new WebProxyClient();
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetQuestionByPointTestIdUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<PointTestQuestionReturn>(data);
                if (obj.QuestionList != null && obj.QuestionList.Any())
                {
                    new StudentWareData().AddPointTestQuestion(testId, obj.QuestionList);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return false;
            }
            #endregion
            */
        }

        public void SavePointTestResult(int testId, string cwId, int siteCourseId, string questionsInfo, string pointOpenType)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-08
             */
            TakenRemote.GetToken();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), testId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString);

            var paramData = new Dictionary<string, string>
            {
                {"cwID", cwId},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", key.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"pointOpenType",pointOpenType},
                {"questionsInfo",questionsInfo},
                {"siteCourseID",siteCourseId.ToString(CultureInfo.InvariantCulture)},
                {"testID",testId.ToString(CultureInfo.InvariantCulture)},
                {"time", time},
                {"etime", etime},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion}

            };

            var byte_valueData = MixParamData(paramData, Interface.SavePointTestQzResultUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            /*
            #region 新接口 dgh 2018.01.18
            TakenRemote.GetToken();
            var time = Util.GetNowString();
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), testId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, NewCourseSalt, Util.TokenString);
            var values = new NameValueCollection
            {
                {"cwID", cwId},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", key.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"pointOpenType",pointOpenType},
                {"questionsInfo",questionsInfo},
                {"siteCourseID",siteCourseId.ToString(CultureInfo.InvariantCulture)},
                {"testID",testId.ToString(CultureInfo.InvariantCulture)},
                {"time", time},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion}
                
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.UploadValues(Interface.SavePointTestQzResultUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            #endregion
            */
        }
        #endregion

        #region word讲义

        /// <summary>
        /// 获取讲义
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="cwId"></param>
        public void GetSmallListKcjy(int cwareId, string cwId)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-16
             */
            TakenRemote.GetToken();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), cwId, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt).ToLower();

            var paramData = new Dictionary<string, string>
            {
                {"cwID", cwId},
                {"ltime",Util.TokenLongTime.ToString()},
                {"pkey", key.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion}
            };

            var byte_valueData = MixParamData(paramData, Interface.GetSmallListKcjyUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                StudentWareKcjyDownReturn obj = WebProxyClient.JsonDeserialize<StudentWareKcjyDownReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1" && obj.Result.KcjyList != null)
                {
                    new StudentWareData().AddKcjyDown(cwareId, obj.Result.KcjyList);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            /*
            #region 新接口 dgh 2018.01.16
            TakenRemote.GetToken();
            var time = Util.GetNowString();
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), cwId, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt).ToLower();
            var values = new NameValueCollection
            {
                {"cwID", cwId},
                {"ltime",Util.TokenLongTime.ToString()},
                {"pkey", key.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion}
            };
            try
            {
                var web = new WebProxyClient();
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetSmallListKcjyUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentWareKcjyDownItem>(data);
                if (obj != null && obj.Code == "1" && obj.KcjyList != null)
                {
                    new StudentWareData().AddKcjyDown(cwareId, obj.KcjyList);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            #endregion
            */
        }

        #endregion

        #region 解密视频相关

        internal byte[] GetVideoHead(int cwareId, string videoId, int videoType, long timeStamp, string userName, string videoHeadhash)
        {
            var web = new WebProxyClient();
            string hashString;
            try
            {
                hashString = Encoding.UTF8.GetString(web.UploadValues(Interface.GetGenKeyUrl, null));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                return new byte[0];
            }
            var hashBy = Enumerable.Range(0, hashString.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(hashString.Substring(x, 2), 16))
                    .ToArray();
            var rng = new RNGCryptoServiceProvider();
            var pKeyBy = new byte[64];
            rng.GetBytes(pKeyBy);
            var keyBy = hashBy.Concat(pKeyBy).ToArray();
            using (var md5 = new Md5Impl())
            {
                keyBy = md5.ComputeHash(keyBy);
            }
            var pkeystr = Convert.ToBase64String(pKeyBy);
            //去除特殊字符，否则传给接口会报错 dgh 2016.11.02
            //pkeystr = pkeystr.Replace("+", "%2B");
            var values = new NameValueCollection
            {
                {"userName", userName},
                {"cwareid", cwareId.ToString(CultureInfo.InvariantCulture)},
                {"videoid", videoId},
                {"videoType", videoType.ToString(CultureInfo.InvariantCulture)},
                {"hash", hashString},
                {"priKey", pkeystr},
                {"time", timeStamp.ToString(CultureInfo.InvariantCulture)},
                {"videoHeadhash", videoHeadhash}
            };
            try
            {
                var headby = web.UploadValues(Interface.GetVideoHeadUrl, values);
                var restr = Encoding.UTF8.GetString(headby);
                if (restr.ToLower().Contains("error"))
                {
                    Trace.WriteLine(restr);
                    return new byte[0];
                }
                var index = restr.IndexOf('|');
                if (index >= 0)
                {
                    restr = restr.Substring(index + 1);
                }
                headby = Enumerable.Range(0, restr.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(restr.Substring(x, 2), 16))
                    .ToArray();
                var iv = new byte[16];
                var headerby = new byte[headby.Length - 16];
                Buffer.BlockCopy(headby, 0, iv, 0, 16);
                Buffer.BlockCopy(headby, 16, headerby, 0, headerby.Length);
                using (var aes = new AesCryptoServiceProvider())
                {
                    aes.Key = keyBy;
                    aes.Padding = PaddingMode.None;
                    aes.IV = iv;
                    using (var ms = new MemoryStream(headerby))
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            var buffer = new byte[headerby.Length];
                            cs.Read(buffer, 0, buffer.Length);
                            return buffer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                return new byte[0];
            }
        }

        internal byte[] GetVideoHeader(int cwareId, string videoId, int videoType, long timeStamp, string userName, string videoHeadhash)
        {
            var md5 = new Md5Impl();
            var hash = "v8eds9287d" + cwareId + videoId + videoType + userName;
            var keyBy = Encoding.Default.GetBytes(hash);
            keyBy = md5.ComputeHash(keyBy);
            hash = BitConverter.ToString(keyBy).Replace("-", string.Empty);
            var values = new NameValueCollection
            {
                {"userName", userName},
                {"cwareid", cwareId.ToString(CultureInfo.InvariantCulture)},
                {"videoid", videoId},
                {"videoType", videoType.ToString(CultureInfo.InvariantCulture)},
                {"hash", hash},
                {"time", timeStamp.ToString(CultureInfo.InvariantCulture)},
                {"videoHeadhash", videoHeadhash}
            };
            try
            {
                var web = new WebProxyClient();
                var headby = web.UploadValues(Interface.GetVideoHeaderUrl, values);
                var restr = Encoding.UTF8.GetString(headby);
                if (restr.ToLower().Contains("error"))
                {
                    Trace.WriteLine(restr);
                    return new byte[1];
                }
                var index = restr.IndexOf('|');
                if (index >= 0)
                {
                    restr = restr.Substring(index + 1);
                }
                headby = Enumerable.Range(0, restr.Length)
                    .Where(x => x % 2 == 0)
                    .Select(x => Convert.ToByte(restr.Substring(x, 2), 16))
                    .ToArray();
                var iv = new byte[16];
                var aesKey = new byte[16];
                var headerby = new byte[headby.Length - 32];
                Buffer.BlockCopy(headby, 0, iv, 0, 16);
                Buffer.BlockCopy(headby, headby.Length - 16, aesKey, 0, 16);
                Buffer.BlockCopy(headby, 16, headerby, 0, headerby.Length);
                using (var aes = new AesCryptoServiceProvider())
                {
                    aes.Key = aesKey;
                    aes.Padding = PaddingMode.None;
                    aes.IV = iv;
                    using (var ms = new MemoryStream(headerby))
                    {
                        using (var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            var buffer = new byte[headerby.Length];
                            cs.Read(buffer, 0, buffer.Length);
                            return buffer;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                return new byte[0];
            }
        }

        /// <summary>
        /// 获取视频加密密钥
        /// </summary>
        /// <param name="cwId"></param>
        /// <returns></returns>
        public string GetCwareKey(string cwId)
        {
            var key = Crypt.Md5(cwId, "md5CwareKey");
            var web = new WebProxyClient();
            try
            {
                var buffer = web.DownloadData(Interface.GetStudentCwareKeyUrl + "?cwid=" + cwId + "&Mkey=" + key.ToLower());
                var ret = Encoding.UTF8.GetString(buffer);
                if (ret.Length == 8)
                {
                    new StudentWareData().UpdateCwareKey(cwId, ret);
                }
                return ret;
            }
            catch
            {
                return string.Empty;
            }
        }

        public string GetCwareKeyNew(string cwId)
        {
            /**
             * 新接口的替换
             * @author ChW
             * @date 2021-05-26
             */
            TakenRemote.GetToken();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var key = Crypt.Md5(cwId, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt);

            var paramData = new Dictionary<string, string> // 原有参数
            {
                { "cwID", cwId },
                { "cwareKey", "" },
                { "ltime", Util.TokenLongTime.ToString() },
                { "pkey", key.ToLower() },
                { "platformSource", Interface.PlatformSource },
                { "time", time },
                { "etime", etime },
                { "version", Util.AppVersion }
            };

            var byte_valueData = MixParamData(paramData, Interface.GetStudentCwareKeyUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                CwareKeyReturn obj = WebProxyClient.JsonDeserialize<CwareKeyReturn>(responseData);
                if (obj != null && obj.Result != null)
                {
                    if (obj.Result.CwareKey != null && obj.Result.CwareKey.Length == 8)
                    {
                        new StudentWareData().UpdateCwareKey(cwId, obj.Result.CwareKey);
                    }
                    return obj.Result.CwareKey ?? string.Empty;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return string.Empty;
            }

            /*
            #region 新接口 dgh 20118.01.15
            TakenRemote.GetToken();
            var time = Util.GetNowString();
            var key = Crypt.Md5(cwId, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt);
            var values = new NameValueCollection
            {
                {"cwID", cwId},
                {"cwareKey",""},
                {"ltime", Util.TokenLongTime.ToString()},
                {"pkey", key.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"version", Util.AppVersion}
                
            };
            try
            {
                var web = new WebProxyClient();
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetStudentCwareKeyUrlNew, values);
                var obj = WebProxyClient.JsonDeserialize<CwareKeyItem>(data);
                if (obj != null)
                {
                    if (obj.CwareKey != null && obj.CwareKey.Length == 8)
                    {
                        new StudentWareData().UpdateCwareKey(cwId, obj.CwareKey);
                    }
                    return obj.CwareKey ?? string.Empty;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return string.Empty;
            }
            #endregion
            */
        }

        #endregion

        /// <summary>
        /// 下载协议学员开通课程信息 dgh 2015.06.23
        /// </summary>
        /// <returns></returns>
        public ReturnItem GetOpenCourseDownloadMsg()
        {
            #region 新接口 dgh 2018.01.04
            TakenRemote.GetToken();
            var re = new ReturnItem() { State = true };
            var time = Util.GetNowString();
            var pkey = Crypt.Md5(Util.SsoUid.ToString(), time, Interface.PlatformSource, Util.AppVersion, Util.TokenString, NewCourseSalt);
            var values = new NameValueCollection
            {
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"version", Util.AppVersion},
                {"userID", Util.SsoUid.ToString()}

            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.StudentGetOpenCourseDownloadUrl, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                RemoteReturnItem obj = WebProxyClient.JsonDeserialize<RemoteReturnItem>(data);
                re.State = obj.Code == 1;
                re.Message = obj.Message;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return re;
            #endregion
        }

        /// <summary>
        /// 同步视频记录 dgh 2017.12.01
        /// </summary>
        /// <param name="svr"></param>
        public void SaveNextBeginTime(StudentVideoRecord svr)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-08
             */
            var history = WebProxyClient.ConventToJson(svr);
            history = "{\"history\":[" + history + "]}";
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var pkey = Crypt.Md5(history, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt);

            var paramData = new Dictionary<string, string>
            {
                {"history", history},
                {"ltime",Util.TokenLongTime.ToString()},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                { "etime", etime },
                {"version", Util.AppVersion}
            };

            var byte_valueData = MixParamData(paramData, Interface.SaveNextBeginTime_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                Trace.WriteLine(ex);
            }

            /*
            #region 新接口 dgh
            string history = WebProxyClient.ConventToJson(svr);
            history = "{\"history\":[" + history + "]}";
            string time = Util.GetNowString();
            string pkey = Crypt.Md5(history, Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, NewCourseSalt);
            var values = new NameValueCollection
            {
                {"history", history},
                {"ltime",Util.TokenLongTime.ToString()},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"version", Util.AppVersion}
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.UploadValues(Interface.SaveNextBeginTime, values);
                var str = Encoding.UTF8.GetString(buffer);
                Trace.WriteLine(str);
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                Trace.WriteLine(ex);
            }
            #endregion
            */
        }

        /// <summary>
        /// 同步学习记录
        /// </summary>
        /// <param name="svr"></param>
        public void SaveCwareRecord()
        {
            /**
             * 新接口的替换
             * @author ChW
             * @date 2021-06-07
             */
            TakenRemote.GetToken();
            var time = StudentRemote.GetServerTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var type = "cwareNew";
            var guid = Guid.NewGuid().ToString();
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, type, guid, NewCourseSalt, Util.TokenString);

            var sw = new StudentWareData();
            var list = sw.StudyVideoStrList().ToList();
            var studyJson = new StudyVideoJson() { StudyVideo = list };
            var studyRecord = WebProxyClient.ConventToJson(studyJson);

            var paramData = new Dictionary<string, string> // 原有参数
            {
                {"appkey",Interface.AppKey},
                {"guid",guid},
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"online","1"},
                {"pkey", key.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"studyVideoJson", studyRecord},
                {"time", time},
                {"etime", etime },
                {"type", type},
                {"uid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version",Util.AppVersion}
            };

            var byte_valueData = MixParamData(paramData, Interface.SaveBatchMessageUrl_2021);

            try
            {
                var wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var str = Encoding.UTF8.GetString(responseData);
                Trace.WriteLine("同步学习记录：" + str);
                RemoteReturn obj = WebProxyClient.JsonDeserialize<RemoteReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == 1)
                {
                    foreach (VideoStr item in list)
                    {
                        foreach (TimebaseStr tm in item.Timebase)
                        {
                            sw.DeleteTimebase(item.CwareId, item.VideoID, tm.StudyTimeEnd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            /*
            TakenRemote.GetToken();
            var time = Util.GetNowString();
            string type = "cwareNew";
            string guid = Guid.NewGuid().ToString();
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture),
                Interface.PlatformSource, Util.AppVersion, time, type, guid, NewCourseSalt, Util.TokenString);
            StudentWareData sw = new StudentWareData();
            var list = sw.StudyVideoStrList().ToList();
            StudyVideoJson studyJson = new StudyVideoJson() { StudyVideo = list };
            string studyRecord = WebProxyClient.ConventToJson(studyJson);

            var values = new NameValueCollection
            {
                {"appkey",Interface.AppKey},
                {"guid",guid},
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"online","1"},
                {"pkey", key.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"studyVideoJson", studyRecord},
                {"time", time},
                {"type", type},
                {"uid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version",Util.AppVersion}
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.UploadValues(Interface.SaveBatchMessageUrl, values);
                var str = Encoding.UTF8.GetString(data);
                Trace.WriteLine("同步学习记录：" + str);
                var obj = WebProxyClient.JsonDeserialize<RemoteReturnItem>(data);
                if (obj.Code == 1)
                {
                    foreach (var item in list)
                    {
                        foreach (var tm in item.Timebase)
                        {
                            sw.DeleteTimebase(item.CwareId, item.VideoID, tm.StudyTimeEnd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            */
        }
    }

}
