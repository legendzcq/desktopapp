using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Framework.Local;
using Framework.Model;
using Framework.NewModel;
using Framework.Remote;
using Framework.Utility;

namespace DesktopApp.Logic
{
    internal static class StudentWareLogic
    {
        /// <summary>
        /// 获取学员课件信息
        /// </summary>
        /// <returns></returns>
        public static void GetStudentWareFromRemote(Action<ReturnItem> callBack)
        {
            var local = new StudentWareData();
            local.ClearCourse();
            AutoGetDetailList.Clear();
            SystemInfo.StartBackGroundThread("异步获取学员课件", () =>
            {
                var ware = new StudentWareRemote();
                ReturnItem item = ware.GetUserCourseList();
                if (callBack != null) callBack(item);
            });
        }

        /// <summary>
        /// 增加列表，保证运行期间，每个班次只更新一遍
        /// </summary>
        private static readonly List<string> AutoGetDetailList = new List<string>();

        /// <summary>
        /// 获取某课件的明细信息
        /// </summary>
        /// <returns></returns>
        public static void GetWareDetailFromRemote(string cwId, string type, Action<ReturnItem> callBack)
        {
            if (AutoGetDetailList.Contains(cwId)) return;
            AutoGetDetailList.Add(cwId);
            SystemInfo.StartBackGroundThread("异步获取学员课件明细", () =>
            {
                var web = new StudentWareRemote();
                ReturnItem item = web.GetStudentCourseDetail(Util.SessionId, cwId, type);
                if (callBack != null) callBack(item);
            });
        }

        /// <summary>
        /// 获取学员所有的班次列表
        /// </summary>
        /// <param name="cwIds"></param>
        public static void GetWareDetailFromRemoteAll(IEnumerable<string> cwIds)
        {
            SystemInfo.StartBackGroundThread("异步获取学员课件明细", () =>
            {
                foreach (var cwId in cwIds)
                {
                    if (AutoGetDetailList.Contains(cwId)) continue;
                    AutoGetDetailList.Add(cwId);
                    var web = new StudentWareRemote();
                    web.GetStudentCourseDetail(Util.SessionId, cwId, "1");
                }
            });
        }

        private static readonly Regex Re = new Regex("<div(?:.*?)><a(?:.*?)>(.*?)</a></div>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        /// <summary>
        /// 做讲义的内容替换
        /// </summary>
        /// <param name="jyHtml"></param>
        /// <returns></returns>
        private static string DoKcjyReplace(string jyHtml) => Re.Replace(jyHtml, "$1").Trim();

        /// <summary>
        /// 从本地获取课程班次列表
        /// </summary>
        /// <returns></returns>
        public static List<ViewStudentCourseWare> GetStudentCourseWareList()
        {
            var local = new StudentWareData();
            return local.GetStudentSubjectCourseWareList();
        }

        /// <summary>
        /// 从本地获取班次明细
        /// </summary>
        /// <param name="cWareId"></param>
        /// <returns></returns>
        public static List<ViewStudentWareDetail> GetStudentWareDetail(int cWareId)
        {
            var local = new StudentWareData();
            return local.GetStudentCWareDetail(cWareId);
        }

        public static IEnumerable<ViewStudentWareDetail> GetMobileDownloadDetail(int cwareId, int downloadType) => new StudentWareData().GetMobileDownloadDetail(cwareId, downloadType);

        /// <summary>
        /// 从本地获取某个课件的讲义明细
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public static List<StudentCwareKcjy> GetStudentWareKcjyList(int cwareId, string videoId)
        {
            var local = new StudentWareData();
            return local.GetStudentWareKcjy(cwareId, videoId);
        }

        public static bool UpdateTime(int cwareId, string videoId, int lastPosition, int duration)
        {
            if (lastPosition != 0) Log.RecordData("PlayVideoProcess", cwareId, videoId, lastPosition, duration);
            var local = new StudentWareData();
            return local.UpdateTime(Util.SsoUid, cwareId, videoId, lastPosition, duration);
        }
        public static ViewStudentWareDetail GetViewStudentCwareDetailItem(int cwareId, string videoId)
        {
            var local = new StudentWareData();
            return local.GetViewStudentCwareDetailItem(cwareId, videoId);
        }
        /// <summary>
        /// 上一个视频
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public static ViewStudentWareDetail GetStudentCwareDetailNextItem(int cwareId, string videoId)
        {
            var local = new StudentWareData();
            return local.GetStudentCwareDetailNextItem(cwareId, videoId);
        }

        /// <summary>
        /// 下一个视频
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public static ViewStudentWareDetail GetStudentCwareDetailPreItem(int cwareId, string videoId)
        {
            var local = new StudentWareData();
            return local.GetStudentCwareDetailPreItem(cwareId, videoId);
        }

        public static int GetVideoPosition(int cwareId, string videoId)
        {
            var local = new StudentWareData();
            return local.GetVideoPosition(Util.SsoUid, cwareId, videoId);
        }

        public static IEnumerable<ViewStudentWareKcjyDown> GetStudentCwareKcjyDown(int cwareId)
        {
            var local = new StudentWareData();
            return local.GetCwareKcjyDown(cwareId);
        }

        public static IEnumerable<PointTestStartTimeItem> GetPointTestStartTimeList(int cwareId, string videoId)
        {
            if (Util.IsOnline)
            {
                new StudentWareRemote().GetPointTestStartTime(cwareId, videoId);

            }
            return new StudentWareData().GetPointTestStartTimeList(cwareId, videoId);
        }
        /// <summary>
        /// 播放中的随堂提问 dgh 2016.09.14
        /// </summary>
        /// <param name="cwareId"></param>
        /// <returns></returns>
        public static void GotoLoginedWebSite(int cwareId, int bordId, string QNo, string jy_url) => new StudentRemote().GotoLoginedWebSite(cwareId, bordId, QNo, jy_url);
        /// <summary>
        /// 获取听课记录信息  dgh 2017.03.21
        /// </summary>
        /// <returns></returns>
        public static List<ViewCourseRecord> GetCourseRecord()
        {
            var local = new StudentWareData();
            return local.GetCourseRecord();
        }
        /// <summary>
        /// 获取指定的课程信息  dgh 2017.03.22
        /// </summary>
        /// <param name="eduSubjectId">科目ID</param>
        /// <param name="cwareId">班次ID</param>
        /// <returns></returns>
        public static ViewStudentCourseWare GetStudentSubjectCourseWareItem(int eduSubjectId, int cwareId) => new StudentWareData().GetStudentSubjectCourseWareItem(eduSubjectId, cwareId);
        /// <summary>
        /// 删除听课记录信息 dgh 2017.03.23
        /// </summary>
        /// <param name="cwareId">课程ID</param>
        /// <param name="videoId">视频ID</param>
        /// <returns></returns>
        public static bool DeleteStudentVideoRecord(int cwareId, string videoId) => new StudentWareData().DeleteStudentVideoRecord(cwareId, videoId);
        /// <summary>
        /// 获取听课记录的相关信息  主要使用于时长的百分比 dgh 2018.03.07
        /// </summary>
        /// <param name="cwareId"></param>
        /// <returns></returns>
        public static IEnumerable<CourseRecordOtherInfo> GetCourseRecordByCwareId(int cwareId) => new StudentWareData().GetCourseRecordByCwareId(cwareId);
        /// <summary>
        /// 同步视频记录 dgh 2017.12.01
        /// </summary>
        /// <param name="svr"></param>

        public static void SaveNextBeginTime(StudentVideoRecord svr) => new StudentWareRemote().SaveNextBeginTime(svr);
        /// <summary>
        /// 同步学习记录 dgh 2018.06.01
        /// </summary>
        public static void SaveCwareRecord() => new StudentWareRemote().SaveCwareRecord();

    }
}
