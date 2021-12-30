using Framework.Local;
using Framework.Model;
using Framework.NewModel;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Framework.Remote
{
	public class StudentFaqRemote : RemoteBase
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
        /// <summary>
        /// 课堂提问：保存答疑中提问的问题信息 dgh 2015.06.29
        /// </summary>
        /// <param name="siteCourseId">对外课程ID</param>
        /// <param name="qNo">讲义号</param>
        /// <param name="title">答疑标题</param>
        /// <param name="content">问题正文</param>
        /// <param name="lecFromStr">讲义片</param>
        /// <returns></returns>
        public ReturnItem GetSaveFaqLecture(string siteCourseId, string qNo, string title, string content, string lecFromStr)
        {
            var re = new ReturnItem() { State = true };
            var time = Util.GetNowString();
            const string categoryId = "16";
            var pkey = Crypt.Md5(Util.SsoUid.ToString(), siteCourseId, qNo, categoryId, time, NewCourseSalt);
            string yjUrl = "http://elearning.chinaacc.com/cware/lecture/faqAsk/getJy4Faq.shtm?nodeid=" + qNo;
            var values = new NameValueCollection
			{
                {"pkey", pkey.ToLower()},
                {"ptime", time},
                {"uid", Util.SsoUid.ToString()},
                {"categoryID", categoryId},
                {"userName", Util.UserName},
                {"siteCourseID", siteCourseId},
                {"title", title},
                {"content", content},
                {"QNo", qNo},
                {"Jy_url", yjUrl},
                {"lecFromStr",lecFromStr},
                {"chapterNum", ""},
                {"platformSource",Interface.PlatformSource}
			};
            var web = new WebProxyClient();
            try
            {
                var data = web.UploadValues(Interface.SaveFaqLectureUrl, values);
                var str = Encoding.UTF8.GetString(data);
                Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentFaqLecture>(data);
                if (obj != null)
                {
                    var stu = new StudentFaqQues() { BoardId = obj.BoardId, CategoryId = categoryId, CreatePtime = obj.CreateTime, Content = content, FaqId = obj.FaqId, Title = title, TopicId = obj.TopicId };
                    re.State = new StudentFaqLocal().AddFaqInfo(stu);
                    if (re.State) re.Message = "提交成功";
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = ex.Message;
            }
            return re;
        }

        /// <summary>
        ///题库提问：保存答疑中提问的问题信息 dgh 2015.06.29
        /// </summary>
        /// <param name="siteCourseId">课程ID</param>
        /// <param name="qNo">问题ID</param>
        /// <param name="title">答疑标题</param>
        /// <param name="content">问题正文</param>
        /// <returns></returns>
        public ReturnItem GetSaveQuestionFaq(string siteCourseId, string qNo, string title, string content)
        {
            var re = new ReturnItem() { State = true };
            var time = Util.GetNowString();
            const string categoryId = "16";
            var pkey = Crypt.Md5(Util.SsoUid.ToString(), siteCourseId, qNo, categoryId, time, NewCourseSalt);
            var values = new NameValueCollection
			{
                {"pkey", pkey.ToLower()},
                {"ptime", time},
                {"uid", Util.SsoUid.ToString()},
                {"categoryID", categoryId},
                {"userName", Util.UserName},
                {"boardID", siteCourseId},
                {"title", title},
                {"content", content},
                {"chapterNum", ""},
                {"QNo", qNo},
                {"isFree",""},
                {"isvalid",""},
                {"platformSource",Interface.PlatformSource}

			};
            var web = new WebProxyClient();
            try
            {
                var data = web.UploadValues(Interface.SaveQuestionFaq, values);
                var str = Encoding.UTF8.GetString(data);
                Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<StudentFaqLecture>(data);
                if (obj != null)
                {
                    var stu = new StudentFaqQues() { BoardId = obj.BoardId, CategoryId = categoryId, CreatePtime = obj.CreateTime, Content = content, FaqId = obj.FaqId, Title = title, TopicId = obj.TopicId };
                    re.State = new StudentFaqLocal().AddFaqInfo(stu);
                    if (re.State) re.Message = "提交成功";
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                re.Message = ex.Message;
            }
            return re;
        }
        /// <summary>
        /// 获取题目的提问 dgh 2015.06.30
        /// </summary>
        /// <param name="siteCourseId">课程ID</param>
        /// <param name="qNo">问题ID</param>
        /// <returns></returns>
        public ReturnItem GetQueListByQuesId(string siteCourseId, string qNo)
        {
            var re = new ReturnItem() { State = true };
            var time = Util.GetNowString();
            var pkey = Crypt.Md5(siteCourseId, qNo, time, NewCourseSalt);
            var values = new NameValueCollection
			{
                {"pkey", pkey.ToLower()},
                {"ptime", time},
                {"siteCourseID", siteCourseId},
                {"QNo", qNo},
                {"count", "10"},
                {"updateptime", ""},
                {"faqIDs", ""},
                {"noAnswerfaqIDs", ""},
                {"platformSource",Interface.PlatformSource},
                {"updateFaqID", ""}
			};
            var web = new WebProxyClient();
            try
            {
                var data = web.UploadValues(Interface.GetQueListByQuesID, values);
                var str = Encoding.UTF8.GetString(data);
                Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<RemoteReturnItem>(data);
                //re.State = obj.Code!="";
                re.Message = obj.Message;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            return re;
        }
	}
}
