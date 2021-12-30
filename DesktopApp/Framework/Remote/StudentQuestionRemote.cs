using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Framework.Download;
using Framework.Local;
using Framework.Model;
using Framework.NewModel;
using Framework.Utility;
using System.Collections.Generic;
using System.Net;

namespace Framework.Remote
{
    public class StudentQuestionRemote : RemoteBase
    {
#if CHINAACC
        private const string StudentSalt = "eiiskdui";
        private const string QuestionSalt = "fJ3UjIFyTu";
#endif
#if MED
        private const string StudentSalt = "tFdfJdfRys";
#endif
#if JIANSHE
        private const string StudentSalt = "fJ3UjIFyTu";
#endif
#if LAW
        private const string StudentSalt = "Yu3hUifOvJ";
#endif
#if CHINATAT
        private const string StudentSalt = "It1UjIJyYu";
#endif
#if G12E
        private const string StudentSalt = "L3iyA1nHui";
#endif
#if ZIKAO
        private const string StudentSalt = "wY2Y1FMs9n";
#endif
#if CHENGKAO
        private const string StudentSalt = "fJ3UjIFyTu";
#endif
#if KAOYAN
        private const string StudentSalt = "hgDfgYghKj";
#endif
#if FOR68
        private const string StudentSalt = "LyBsw3Ai1b";
#endif
#if CK100
        private const string StudentSalt = "fJ3UjIFyTu";
#endif
        public ReturnItem GetCentersBySubjectId(int eduSubjectId)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-08
             */
            var re = new ReturnItem();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            string etime = time;

            #if CHINAACC
                var pkey = Crypt.Md5(eduSubjectId.ToString(CultureInfo.InvariantCulture), Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
            #else
                var pkey = Crypt.Md5(eduSubjectId.ToString(CultureInfo.InvariantCulture), Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource,Util.AppVersion, time, StudentSalt,Util.TokenString);
            #endif

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"eduSubjectID", eduSubjectId.ToString(CultureInfo.InvariantCulture)},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"userID",Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version", Util.AppVersion}
            };

            byte[] byte_valueData = MixParamData(paramData, Interface.GetPaperCentersBySubjectIdUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<StudentPaperCenterReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.CenterList != null)
                {
                    var local = new StudentQuestionData();
                    local.AddCenterSubjectList(eduSubjectId, obj.Result.CenterList);
                }
                re.State = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.State = false;
            }
            return re;

            /*
            #region 新接口
            var re = new ReturnItem();
            string time = Util.GetNowString();
#if CHINAACC
            var pkey = Crypt.Md5(eduSubjectId.ToString(CultureInfo.InvariantCulture), Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
#else
            var pkey = Crypt.Md5(eduSubjectId.ToString(CultureInfo.InvariantCulture), Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource,Util.AppVersion, time, StudentSalt,Util.TokenString);
#endif
            var values = new NameValueCollection
                    {
                        {"eduSubjectID", eduSubjectId.ToString(CultureInfo.InvariantCulture)},
                        {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                        {"pkey", pkey.ToLower()},
                        {"platformSource", Interface.PlatformSource},
                        {"time", time},
                        {"userID",Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                        {"version", Util.AppVersion}
                        
                    };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetPaperCentersBySubjectIdUrlNew, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentPaperCenterReturnItem>(data);
                if (obj != null && obj.CenterList != null)
                {
                    var local = new StudentQuestionData();
                    local.AddCenterSubjectList(eduSubjectId, obj.CenterList);
                }
                re.State = true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.State = false;
            }
            return re;
            #endregion
            */
        }

        /// <summary>
        /// 获取题型
        /// </summary>
        /// <returns></returns>
        public ReturnItem GetQuestionType()
        {
            #region 新接口 dgh 2018.01.16
            var re = new ReturnItem();
            string time = Util.GetNowString();
#if CHINAACC
            var pkey = Crypt.Md5("",Interface.PlatformSource, Util.AppVersion, time,Util.TokenString,QuestionSalt);
#else
            var pkey = Crypt.Md5("",Interface.PlatformSource,Util.AppVersion, time,Util.TokenString, StudentSalt);
#endif
            var values = new NameValueCollection 
            {
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()}, 
                {"platformSource", Interface.PlatformSource},
                {"quesTypeID", ""},
                {"time", time},
                {"version", Util.AppVersion}
            };
            var web = new WebProxyClient();
            var local = new StudentQuestionData();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetQzQuestionTypeUrlNew, values);
                var str = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentQuestionTypeReturnItem>(buffer);

                if (obj != null && obj.Code == "1")
                {
                    if (obj.QuestionTypeList == null)
                    {
                        re.Message = "获取题型信息数据为空";
                        return re;
                    }
                    else
                    {
                        re.State = local.AddQuestionTypeList(obj.QuestionTypeList);
                        if (!re.State) re.Message = "保存题型信息失败";
                    }

                }
                else
                {
                    re.Message = "获取题型信息失败";
                    return re;
                }
                return re;
                
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取题型信息失败";
                return re;
            }
            #endregion
        }

        /// <summary>
        /// 获取对内试卷
        /// </summary>
        /// <param name="centerId"></param>
        /// <param name="updateTime"></param>
        /// <returns></returns>
        public ReturnItem GetCenterPapers(int centerId, string updateTime)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-08
             */
            var re = new ReturnItem();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;

#if CHINAACC
            var pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
            #else
                string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, StudentSalt, Util.TokenString);
            #endif

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"centerID", centerId.ToString(CultureInfo.InvariantCulture)},
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                { "etime", etime },
                {"version", Util.AppVersion}
            };
            paramData.Add("fordown", "1");

            byte[] byte_valueData = MixParamData(paramData, Interface.GetQzCenterPapersUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<StudentPaperReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1")
                {
                    if (obj.Result.CenterPaperList != null)
                    {
                        var db = new StudentQuestionData();
                        re.State = db.AddPaperList(obj.Result.CenterPaperList);
                        if (!re.State) re.Message = "保存对内试卷信息失败";
                    }
                    else
                    {
                        re.State = false;
                        re.Message = "没有对内试卷信息";
                    }
                }
                else
                {
                    re.State = false;
                    re.Message = "获取对内试卷失败";
                }
                return re;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取对内试卷失败";
                return re;
            }

            /*
            #region 新接口 dgh 2018.01.16
            var re = new ReturnItem();
            string time = Util.GetNowString();

#if CHINAACC
            var pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
#else

            string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, StudentSalt, Util.TokenString);
#endif
            var values = new NameValueCollection
            {
                {"centerID", centerId.ToString(CultureInfo.InvariantCulture)},
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"version", Util.AppVersion}
            };
            var web = new WebProxyClient();
            var db = new StudentQuestionData();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetQzCenterPapersUrlNew, values);
                string str = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine("获取对内试卷：" + str);
                var obj = WebProxyClient.JsonDeserialize<StudentPaperList>(buffer);
                if (obj.Code == "1")
                {
                    if (obj.CenterPaperList != null)
                    {
                        re.State = db.AddPaperList(obj.CenterPaperList);
                        if (!re.State) re.Message = "保存对内试卷信息失败";
                    }
                    else
                    {
                        re.State = false;
                        re.Message = "没有对内试卷信息";
                    }
                }
                else
                {
                    re.State = false;
                    re.Message = "获取对内试卷失败";
                }
                return re;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取对内试卷失败";
                return re;
            }
            #endregion
            */
        }

        /// <summary>
        /// 获取对外试卷
        /// </summary>
        /// <param name="centerId"></param>
        /// <param name="updateTime"></param>
        /// <returns></returns>
        public ReturnItem GetCenterPaperViews(int centerId, string updateTime)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-09
             */
            var re = new ReturnItem();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;

            #if CHINAACC
                string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, QuestionSalt);
            #else
                string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture),Interface.PlatformSource,Util.AppVersion, time,Util.TokenString, StudentSalt);
            #endif

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"centerID", centerId.ToString(CultureInfo.InvariantCulture)},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"updateTime", updateTime},
                {"version", Util.AppVersion}
            };
            paramData.Add("paperType", "1");
            paramData.Add("fordown", "1");

            byte[] byte_valueData = MixParamData(paramData, Interface.GetQzCenterPaperViewsUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<StudentPaperViewReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1")
                {
                    if (obj.Result.QuestionTypeList == null)
                    {
                        re.Message = "获取对外试卷数据为空";
                        return re;
                    }
                    else
                    {
                        var list = obj.Result.QuestionTypeList.ToList();

                        // 先保存试卷
                        var db = new StudentQuestionData();
                        re.State = db.AddPaperViewList(list, centerId);
                        if (!re.State) re.Message = "保存对外试卷失败";

                        list.ForEach(x =>
                        {
                            x.CenterId = centerId;
                            // 再保存试卷内题目，题目附带试卷的Version字段，需要Update到试卷内
                            GetPaperQuestionInfo(x.PaperViewId, string.Empty, string.Empty, string.Empty);
                            GetPaperQuestionOptions(x.PaperViewId, string.Empty, string.Empty, string.Empty);
                        });
                        return re;
                    }
                }
                else
                {
                    re.Message = "获取对外试卷失败";
                    return re;
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取对外试卷失败";
                return re;
            }

            /*
            #region 新接口 dgh 2018.01.16
            var re = new ReturnItem();
            string time = Util.GetNowString();
#if CHINAACC
            string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, Util.TokenString, QuestionSalt);
#else
        string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture),Interface.PlatformSource,Util.AppVersion, time,Util.TokenString, StudentSalt);
#endif
            var values = new NameValueCollection
            {
                {"centerID", centerId.ToString(CultureInfo.InvariantCulture)},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"updateTime", updateTime},
                {"version", Util.AppVersion}
            };

            values.Add("fordown", "1");
            //values.Add("downver", Util.AppVersion);
            //values.Add("downuid", Util.SsoUid.ToString(CultureInfo.InvariantCulture));
            //values.Add("_r", Util.GetNow().Ticks.ToString(CultureInfo.InvariantCulture));
            //values.Add("_t", Util.GetNow().Ticks.ToString(CultureInfo.InvariantCulture));

            try
            {
                var web = new WebProxyClient();
                var db = new StudentQuestionData();
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetQzCenterPaperViewsUrlNew, values);
                string data = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine(data);
                var obj = WebProxyClient.JsonDeserialize<StudentPaperViewResult>(buffer);
                if (obj.Code == "1")
                {
                    if (obj.QuestionTypeList == null)
                    {
                        re.Message = "获取对外试卷数据为空";
                        return re;
                    }
                    else
                    {
                        var list = obj.QuestionTypeList.ToList();
                        list.ForEach(x =>
                        {
                            x.CenterId = centerId;
                            GetPaperQuestionInfo(x.PaperViewId, string.Empty, string.Empty, string.Empty);
                            GetPaperQuestionOptions(x.PaperViewId, string.Empty, string.Empty, string.Empty);
                        });
                        re.State = db.AddPaperViewList(list, centerId);
                        if (!re.State) re.Message = "保存对外试卷失败";
                        return re;

                    }
                }
                else
                {
                    re.Message = "获取对外试卷失败";
                    return re;
                }
            }

            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取对外试卷失败";
                return re;
            }
            #endregion
            */

        }

        /// <summary>
        /// 获取试卷大类
        /// </summary>
        /// <param name="centerId"></param>
        /// <param name="updateTime"></param>
        /// <returns></returns>
        public ReturnItem GetCenterPaperParts(int centerId, string updateTime)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-08
             */
            var re = new ReturnItem();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;

            #if CHINAACC
                string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
            #else
                string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, StudentSalt,Util.TokenString);
            #endif

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"centerID", centerId.ToString(CultureInfo.InvariantCulture)},
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                { "etime", etime },
                {"version", Util.AppVersion}
            };
            paramData.Add("fordown", "1");

            byte[] byte_valueData = MixParamData(paramData, Interface.GetQzCenterPaperPartsUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<PaperPartListReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1")
                {
                    if (obj.Result.PaperPartsList == null)
                    {
                        re.Message = "获取试卷大类数据为空";
                        return re;
                    }
                    else
                    {
                        var db = new StudentQuestionData();
                        re.State = db.AddPaperPartList(obj.Result.PaperPartsList);
                        if (!re.State) re.Message = "保存试卷大类失败";
                    }

                }
                else
                {
                    re.Message = "获取试卷大类失败";
                    return re;
                }
                return re;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取试卷大类失败";
                return re;
            }

            /*
            #region 新接口 dgh 2018.01.16
            var re = new ReturnItem();
            string time = Util.GetNowString();
#if CHINAACC
            string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
#else
            string pkey = Crypt.Md5(centerId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, StudentSalt,Util.TokenString);
#endif
            var values = new NameValueCollection
            {
                {"centerID", centerId.ToString(CultureInfo.InvariantCulture)},
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"version", Util.AppVersion}
            };
            var web = new WebProxyClient();
            var db = new StudentQuestionData();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetQzCenterPaperPartsUrlNew, values);
                var str = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<PaperPartListReturnItem>(buffer);

                if (obj.Code == "1")
                {
                    if (obj.PaperPartsList == null)
                    {
                        re.Message = "获取试卷大类数据为空";
                        return re;
                    }
                    else
                    {
                        re.State = db.AddPaperPartList(obj.PaperPartsList);
                        if (!re.State) re.Message = "保存试卷大类失败";
                    }

                }
                else
                {
                    re.Message = "获取试卷大类失败";
                    return re;
                }
                return re;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                re.Message = "获取试卷大类失败";
                return re;
            }
            #endregion
            */
        }

        /// <summary>
        /// 获取题目
        /// </summary>
        /// <param name="paperViewId"></param>
        /// <param name="updateTime"></param>
        /// <param name="rowNumStart"></param>
        /// <param name="rowNumEnd"></param>
        /// <returns></returns>
        public ReturnItem GetPaperQuestionInfo(int paperViewId, string updateTime, string rowNumStart, string rowNumEnd)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-10
             */
            var re = new ReturnItem();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;

            #if CHINAACC
                string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
            #else
                string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture),Interface.PlatformSource,Util.AppVersion, time, StudentSalt,Util.TokenString);
            #endif

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"paperViewID", paperViewId.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"rowNumEnd", rowNumEnd},
                {"rowNumStart", rowNumStart},
                {"time", time},
                {"etime", etime},
                {"updateTime", updateTime},
                {"version", Util.AppVersion}
            };
            paramData.Add("fordown", "1");

            byte[] byte_valueData = MixParamData(paramData, Interface.GetQzPaperQuestionInfoUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<StudentQuestionReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1")
                {
                    if (obj.Result.QuestionList == null)
                    {
                        re.Message = "获取题目数据为空";
                        return re;
                    }
                    else
                    {
                        var seq = 1;
                        var list = obj.Result.QuestionList.ToList();
                        list.ForEach(x =>
                        {
                            var index = seq++;
                            x.PaperViewId = paperViewId;
                            x.Sequence = index;
                        });
                        var db = new StudentQuestionData();
                        re.State = db.AddQuestionList(list);
                        if (!re.State)
                        {
                            re.Message = "保存题目失败";
                        }
                        else
                        {
                            // 题目保存成功后，添加该试卷的公共信息，该公共信息用于修复用户提交到线上的答案未显示的Bug
                            db.AddPaperCommonInfo(paperViewId, obj.Result.CommonInfo);
                        }
                        return re;
                    }
                }
                else
                {
                    re.Message = "获取题目失败";
                    return re;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取题目失败";
                return re;
            }

            /*
            #region 新接口 dgh 2018.01.16
            var re = new ReturnItem();
            string time = Util.GetNowString();
#if CHINAACC
            string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture),Interface.PlatformSource,Util.AppVersion, time, QuestionSalt,Util.TokenString);
#else
                    string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture),Interface.PlatformSource,Util.AppVersion, time, StudentSalt,Util.TokenString);
#endif
            var values = new NameValueCollection
                    {
                        {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                        {"paperViewID", paperViewId.ToString(CultureInfo.InvariantCulture)},
                        {"pkey", pkey.ToLower()},
                        {"platformSource",Interface.PlatformSource},
                        {"rowNumEnd", rowNumEnd},
                        {"rowNumStart", rowNumStart},
                        {"time", time},
                        {"updateTime", updateTime},
                        {"version", Util.AppVersion}
                        
                    };
            var web = new WebProxyClient();
            var db = new StudentQuestionData();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetQzPaperQuestionInfoUrlNew, values);
                string data = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine(data);
                var obj = WebProxyClient.JsonDeserialize<StudentQuestionReturnItem>(buffer);
                if (obj.Code == "1")
                {
                    if (obj.QuestionList == null)
                    {
                        re.Message = "获取题目数据为空";
                        return re;
                    }
                    else
                    {
                        var seq = 1;
                        var list = obj.QuestionList.ToList();
                        list.ForEach(x => {
                            var index = seq++;
                            x.PaperViewId = paperViewId;
                            x.Sequence = index;
                        });
                        re.State = db.AddQuestionList(list);
                        if (!re.State) re.Message = "保存题目失败";
                        return re;
                    }
                }
                else
                {
                    re.Message = "获取题目失败";
                    return re;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取题目失败";
                return re;
            }
            #endregion
            */
        }

        /// <summary>
        /// 获取题目选项
        /// </summary>
        /// <param name="paperViewId"></param>
        /// <param name="updateTime"></param>
        /// <param name="rowNumStart"></param>
        /// <param name="rowNumEnd"></param>
        /// <returns></returns>
        public ReturnItem GetPaperQuestionOptions(int paperViewId, string updateTime, string rowNumStart, string rowNumEnd)
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-10
             */
            var re = new ReturnItem();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;

            #if CHINAACC
                string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
            #else
                string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource,Util.AppVersion, time, StudentSalt,Util.TokenString);
            #endif

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"paperViewID", paperViewId.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"rowNumEnd", rowNumEnd},
                {"rowNumStart", rowNumStart},
                {"time", time},
                {"etime", etime},
                {"updateTime", updateTime},
                {"version", Util.AppVersion},
            };
            paramData.Add("fordown", "1");

            byte[] byte_valueData = MixParamData(paramData, Interface.GetQzPaperQuestionOptionsUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<StudentQuestionOptionReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1")
                {
                    if (obj.Result.QuestionOptionList == null)
                    {
                        re.Message = "获取题目选项数据为空";
                        return re;
                    }
                    else
                    {
                        var db = new StudentQuestionData();
                        re.State = db.AddQuestionOptionList(obj.Result.QuestionOptionList);
                    }
                }
                else
                {
                    re.Message = "获取题目选项失败";
                    return re;
                }
                return re;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取题目选项失败";
                return re;
            }

            /*
            #region 新接口 dgh 2018.01.16
            var re = new ReturnItem();
            string time = Util.GetNowString();
#if CHINAACC
            string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
#else
            string pkey = Crypt.Md5(paperViewId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource,Util.AppVersion, time, StudentSalt,Util.TokenString);
#endif
            var values = new NameValueCollection
            {
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"paperViewID", paperViewId.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"rowNumEnd", rowNumEnd},
                {"rowNumStart", rowNumStart},
                {"time", time},
                {"updateTime", updateTime},
                {"version", Util.AppVersion},
            };
            var web = new WebProxyClient();
            var db = new StudentQuestionData();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetQzPaperQuestionOptionsUrlNew, values);

                var data = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine(data);
                var obj = WebProxyClient.JsonDeserialize<StudentQuestionOptionReturnItem>(buffer);
                if (obj.Code == "1")
                {
                    if (obj.QuestionOptionList == null)
                    {
                        re.Message = "获取题目选项数据为空";
                        return re;
                    }
                    else
                    {
                        re.State = db.AddQuestionOptionList(obj.QuestionOptionList);
                    }
                }
                else
                {
                    re.Message = "获取题目选项失败";
                    return re;
                }
                return re;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取题目选项失败";
                return re;
            }
            #endregion
            */
        }

        #region 做题记录
        /// <summary>
        /// 上传做题记录 dgh 2017.05.03
        /// </summary>
        /// <param name="StudentPaperScore"></param>
        public void BatchPaperScoreSubmit(StudentPaperScore stuPaper)
        {
            /**
             * 新接口的替换
             * @author ChW
             * @date 2021-06-07
             */
            TakenRemote.GetToken();
            var stuQues = new StudentQuestionData();
            var time = StudentRemote.GetServerTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            string type = "qz";
            string guid = Guid.NewGuid().ToString();

#if CHINAACC
            string personkey = QuestionSalt;
#else
            string personkey = StudentSalt;
#endif

            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, type, guid, personkey, Util.TokenString);
            string paperScores = WebProxyClient.ConventToJson(stuPaper);
            paperScores = "{\"paperScores\":[" + paperScores + "]}";

            Dictionary<string, string> paramData = new Dictionary<string, string> // 原有参数
            {
                {"appkey",Interface.AppKey},
                {"guid",guid},
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"online",""},
                {"paperScores", paperScores},
                {"pkey", key.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"type", type},
                {"uid", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version",Util.AppVersion}
            };
            paramData.Add("fordown", "1");

            byte[] byte_valueData = MixParamData(paramData, Interface.SaveBatchMessageUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<StudentPaperScoresReturn>(responseData);
                Trace.WriteLine(obj);
                if (obj != null && obj.Result != null && obj.Result.Code == "1" && obj.Result.FinalList != null && obj.Result.FinalList.Count > 0)
                {
                    stuPaper.PaperScoreID = obj.Result.FinalList[0].PaperResult[0].PaperNewScoreID.ToString();
                    stuQues.AddPaperSocres(stuPaper);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }

            /*
            TakenRemote.GetToken();
            var stuQues = new StudentQuestionData();
            var time = Util.GetNowString();
            string type = "qz";
            string guid = Guid.NewGuid().ToString();
#if CHINAACC
            string personkey = QuestionSalt;
#else
        string personkey = StudentSalt;
#endif
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture),
                Interface.PlatformSource, Util.AppVersion, time, type, guid, personkey, Util.TokenString);
            string paperScores = WebProxyClient.ConventToJson(stuPaper);
            paperScores = "{\"paperScores\":[" + paperScores + "]}";

            var values = new NameValueCollection
        {
            {"appkey",Interface.AppKey},
            {"guid",guid},
            {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
            {"online",""},
            {"paperScores", paperScores},
            {"pkey", key.ToLower()},
            {"platformSource",Interface.PlatformSource},
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
                Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentPaperScores>(data);
                Trace.WriteLine(obj);
                if (obj.Code == "1" && obj.FinalList != null && obj.FinalList.Count > 0)
                {
                    stuPaper.PaperScoreID = obj.FinalList[0].PaperResult[0].PaperNewScoreID.ToString();
                    stuQues.AddPaperSocres(stuPaper);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            */
        }
        /// <summary>
        /// 从线上获取做题记录信息
        /// </summary>
        /// <param name="courseId">课程ID</param>
        /// <param name="paperScoreIDs">客户端将7天内已同步的做题记录ID提交给服务器，返回客户端不存在的做题记录信息</param>
        public ReturnItem GetPaperScoreBatch()
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-11
             */
            var re = new ReturnItem();
            var stuQues = new StudentQuestionData();
            TakenRemote.GetToken();
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var paperScoreIDs = stuQues.GetPaperScoreIds(Util.SsoUid);
#if CHINAACC
            string personkey = QuestionSalt;
#else
            string personkey = StudentSalt;
#endif
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture),
                Interface.PlatformSource, Util.AppVersion, time, personkey, Util.TokenString);

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"courseID",""},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"paperScoreIDs", paperScoreIDs},
                {"pkey", key.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version",  Util.AppVersion}

            };

            byte[] byte_valueData = MixParamData(paramData, Interface.GetPaperScoreBatchUrl_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<StudentPaperScoresReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1")
                {
                    if (obj.Result.PaperScores != null && obj.Result.PaperScores.Count > 0)
                    {
                        foreach (var item in obj.Result.PaperScores)
                        {
                            stuQues.AddPaperSocres(item);
                        }
                        re.State = true;
                    }
                }
                else
                {
                    re.State = false;
                }
                return re;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取做题记录失败";
                re.State = false;
                return re;
            }

            /*
            var re = new ReturnItem();
            var stuQues = new StudentQuestionData();
            TakenRemote.GetToken();
            var time = Util.GetNowString();
            var paperScoreIDs = stuQues.GetPaperScoreIds(Util.SsoUid);
#if CHINAACC
            string personkey = QuestionSalt;
#else
            string personkey = StudentSalt;
#endif
            var key = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture),
                Interface.PlatformSource, Util.AppVersion, time, personkey, Util.TokenString);
            var values = new NameValueCollection
            {
                {"courseID",""},
                {"ltime",Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"paperScoreIDs", paperScoreIDs},
                {"pkey", key.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"userID", Util.SsoUid.ToString(CultureInfo.InvariantCulture)},
                {"version",  Util.AppVersion}
				
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var data = web.DownloadData(Interface.GetPaperScoreBatchUrl, values);
                var str = Encoding.UTF8.GetString(data);
                Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentPaperScores>(data);
                if (obj.Code == "1")
                {
                    if (obj.PaperScores != null && obj.PaperScores.Count > 0)
                    {
                        foreach (var item in obj.PaperScores)
                        {
                            stuQues.AddPaperSocres(item);
                        }
                        re.State = true;
                    }
                }
                else
                {
                    re.State = false;
                }
                return re;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取做题记录失败";
                re.State = false;
                return re;
            }
            */
        }
        /// <summary>
        /// 获取试卷提交次数
        /// </summary>
        /// <param name="paperViewId"></param>
        /// <returns></returns>
        public ReturnItem GetPaperSubmitCnt(int paperViewId, ref int submitTimes)
        {
            TakenRemote.GetToken();
            var re = new ReturnItem();
            string time = Util.GetNowString();
#if CHINAACC
            string pkey = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), paperViewId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, QuestionSalt, Util.TokenString);
#else
            string pkey = Crypt.Md5(Util.SsoUid.ToString(CultureInfo.InvariantCulture), paperViewId.ToString(CultureInfo.InvariantCulture), Interface.PlatformSource, Util.AppVersion, time, StudentSalt, Util.TokenString);
#endif
            var values = new NameValueCollection
            {
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"paperViewID", paperViewId.ToString(CultureInfo.InvariantCulture)},
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
                var data = web.DownloadData(Interface.GetPaperSubmitCnt, values);
                //Trace.WriteLine("提交次数：" + Encoding.UTF8.GetString(data));
                var obj = WebProxyClient.JsonDeserialize<StudentPaperSubmitCnt>(data);
                if (obj != null)
                {
                    submitTimes = obj.SubmitTimes;
                }
                re.State = true;
                return re;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取题目选项失败";
                return re;
            }
        }
        /// <summary>
        /// 本地获取做题记录信息
        /// </summary>
        /// <param name="ssoUid">用户ID</param>
        /// <param name="paperViewId">对外试卷id</param>
        /// <returns></returns>
        public List<StudentPaperScore> GetPaperSocres(int ssoUid, int paperViewId)
        {
            return new StudentQuestionData().GetPaperSocres(ssoUid, paperViewId);
        }
        #endregion

#if CHINAACC

        /// <summary>
        /// 获取分录题借贷关系数据
        /// </summary>
        /// <returns></returns>
        public ReturnItem GetSubjectClassify()
        {
            #region 新接口 dgh 2018.01.16
            var re = new ReturnItem();
            string time = Util.GetNowString();
            var pkey = Crypt.Md5(Interface.PlatformSource, Util.AppVersion, time,Util.TokenString, QuestionSalt);
            var values = new NameValueCollection
            {
                {"ltime", Util.TokenLongTime.ToString(CultureInfo.InvariantCulture)},
                {"pkey", pkey.ToLower()},
                {"platformSource", Interface.PlatformSource},
                {"time", time},
                {"version", Util.AppVersion}
            };
            var web = new WebProxyClient();
            try
            {
                web.Headers.Add("authorization", Interface.AppKey + "|4.0");
                var buffer = web.DownloadData(Interface.GetSubjectClassifyUrlNew, values);
                string data = Encoding.UTF8.GetString(buffer);
                //Trace.WriteLine(data);
                var obj = WebProxyClient.JsonDeserialize<NewModel.SubjectClassifyReturnItem>(buffer);
                if (obj != null && obj.Code == "1")
                {
                    var local = new Local.StudentQuestionData();
                    re.State = local.AddSubjectClassify(obj.ItemList);
                    if (!re.State)
                    {
                        re.Message = "保存分录题借贷关系数据失败";
                    }
                }
                else
                {
                    re.State = false;
                    re.Message = obj.Message;
                }
                return re;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = "获取分录题借贷关系数据失败";
                return re;
            }
            #endregion
        }

        public List<AccClassifyAnswer> GetClassifyAnswer(string json)
        {
            List<AccClassifyAnswer> answerList = new List<AccClassifyAnswer>();
            answerList = WebProxyClient.JsonDeserialize<List<AccClassifyAnswer>>(json, Encoding.UTF8);
            return answerList;
        }
#endif
    }
}
