using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Framework.Local;
using Framework.Model;
using Framework.NewModel;
using Framework.Remote;
using System.Linq;
using Framework.Utility;

namespace DesktopApp.Logic
{
	internal static class StudentQuestionLogic
	{
		/// <summary>
		/// 获取题库基础信息
		/// </summary>
		/// <returns></returns>
		public static void GetQuestionBaseInfoFromRemote(Action<ReturnItem> callback)
		{
			SystemInfo.StartBackGroundThread("异步更新题库信息", () =>
			{
				
				var web = new StudentQuestionRemote();
                var localstu = new StudentWareData();
				var subjectList = localstu.GetStudentEduSubjectList();
                //写在这里的目的：只调用一次
                TakenRemote.GetToken();
				foreach (var subject in subjectList)
				{
					web.GetCentersBySubjectId(subject);
				}
				var qitem = web.GetQuestionType();
#if CHINAACC
				var citem = web.GetSubjectClassify();
#endif
				//item.State = item.State && qitem.State;
				//item.Message = item.Message + "\r\n" + qitem.Message;
				if (callback != null) callback(qitem);
			});
		}

		/// <summary>
		/// 获取某试卷中心的数据
		/// </summary>
		/// <param name="centerId"></param>
		/// <param name="callback"></param>
		public static void GetCenterInfo(int centerId, Action<DateTime> callback)
		{
			SystemInfo.StartBackGroundThread("异步更新试卷信息", () =>
			{
				// 获取服务器时间
				var remote = new StudentRemote();
				DateTime remoteTime = remote.GetCurrentTime();

				//写在这里的目的：只调用一次
				TakenRemote.GetToken();
				var web = new StudentQuestionRemote();
				web.GetCenterPapers(centerId, string.Empty);
				web.GetCenterPaperParts(centerId, string.Empty);
				web.GetCenterPaperViews(centerId, string.Empty);
				// 将更新时间写入数据库，@author ChW，@date 2021-05-14
				AddTimeStampToCenterInfo(centerId, remoteTime.ToString());
				//if (callback != null) callback(remoteTime);
				callback?.Invoke(remoteTime);
			});
		}

		/**
         * 将CenterId对应的题库的更新时间写入数据库
         * @param centerId 练习集id
         * @param updateTime 练习集的更新时间
         * @author ChW
         * @date 2021-05-14
         */
		public static bool AddTimeStampToCenterInfo(int centerId, string updateTime)
		{
			var db = new StudentQuestionData();
			return db.UpdateCenterSubjectUpdateTime(centerId, updateTime);
		}

		/// <summary>
		/// 获取试卷明细
		/// </summary>
		/// <param name="paperViewId"></param>
		/// <param name="callBack"></param>
		/// <returns></returns>
		public static void GetPaperDetailFromRemote(int paperViewId, Action<ReturnItem> callBack)
		{
			SystemInfo.StartBackGroundThread("异步更新试卷明细", () =>
			{
                TakenRemote.GetToken();
				var web = new StudentQuestionRemote();
				web.GetPaperQuestionInfo(paperViewId, string.Empty, string.Empty, string.Empty);
				web.GetPaperQuestionOptions(paperViewId, string.Empty, string.Empty, string.Empty);
				if (callBack != null) callBack(new ReturnItem { State = true });
			});
		}

		/// <summary>
		/// 获取本地的试卷列表
		/// </summary>
		/// <param name="courseId"></param>
		/// <returns></returns>
		public static List<ViewStudentPaper> GetCenterPaperListFromLocal(int courseId)
		{
			var local = new StudentQuestionData();
			var list = local.GetCenterPaperList(courseId);
			return list;
		}

		/// <summary>
		/// 获取本地的试卷明细
		/// </summary>
		/// <param name="paperViewId"></param>
		/// <returns></returns>
		public static IEnumerable<ViewStudentQuestion> GetPaperDetail(int paperViewId)
		{
			var local = new StudentQuestionData();
			var list = local.GetPaperDetail(paperViewId, Util.SsoUid);
			return list;
		}

		public static List<ViewStudentQuestion> GetPaperDetailFav(int paperViewId)
		{
			var list = GetPaperDetail(paperViewId);
			return list.Where(x => x.IsFav).ToList();
		}

		public static List<ViewStudentQuestion> GetPaperDetailWrong(int paperViewId)
		{
			var list = GetPaperDetail(paperViewId);
			return list.Where(x => x.IsWrong).ToList();
		}

		public static List<ViewStudentQuestion> GetPaperDetailUnDone(int paperViewId)
		{
			var list = GetPaperDetail(paperViewId);
			return list.Where(x => !x.IsDone).ToList();
		}

		public static bool CheckPaperDetailExists(int paperViewId)
		{
			var local = new StudentQuestionData();
			return local.CheckPaperDetailExists(paperViewId);
		}

		public static List<ViewStudentCenter> GetCenterList()
		{
			var local = new StudentQuestionData();
			return local.GetCenterList();
		}

		public static List<ViewStudentPaper> GetCenterPaperList(int centerId)
		{
			var local = new StudentQuestionData();
			return local.GetCenterPaperList(centerId);
		}

		public static bool SaveUserResult(IEnumerable<ViewStudentQuestionResult> list)
		{
			var local = new StudentQuestionData();
			return local.SaveUserResult(Util.SsoUid, list);
		}

		public static bool SetQuestionResolved(int paperViewId, int questionId)
		{
			var local = new StudentQuestionData();
			return local.SetQuestionResolved(Util.SsoUid, paperViewId, questionId);
		}

		public static List<ViewStudentQuestionRecord> GetStudentQuestionRecord(int paperViewId, int questionId)
		{
			var local = new StudentQuestionData();
			return local.GetStudentQuestionRecord(Util.SsoUid, paperViewId, questionId);
		}
        public static IEnumerable<ViewStudentQuestion> GetPaperScoreDetail(int paperViewId, string paperScoreID)
        {
            var local = new StudentQuestionData();
            var list = local.GetPaperScoreDetail(paperViewId, Util.SsoUid, paperScoreID);
            return list;
        }
        /// <summary>
        /// 上传做题记录 dgh 2017.05.03
        /// </summary>
        /// <param name="StudentPaperScore"></param>
        public static void BatchPaperScoreSubmit(StudentPaperScore stuPaper)
        {
            var web = new StudentQuestionRemote();
            web.BatchPaperScoreSubmit(stuPaper);
        }
        /// <summary>
        /// 从线上获取做题记录
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="callBack"></param>
        public static void GetPaperSocresFromWeb(Action callBack)
        {
            SystemInfo.StartBackGroundThread("异步获取做题记录", () =>
            {
                var local = new StudentQuestionRemote();
                var item = local.GetPaperScoreBatch();
                if (callBack != null) callBack();
            });
        }
        /// <summary>
        /// 获取记录总数 dgh 2017.08.11
        /// </summary>
        /// <param name="ssoUid">用户ID</param>
        /// <returns></returns>
        public static int GetPaperScoreCount(int ssoUid)
        {
            var local = new StudentQuestionData();
            return local.GetPaperScoreCount(ssoUid);
        }
        /// <summary>
        /// 获取已提交次数 dgh 2017.08.11
        /// </summary>
        /// <param name="paperViewId"></param>
        /// <param name="submitTimes"></param>
        public static void GetPaperSubmitCnt(int paperViewId, ref int submitTimes)
        {
            var web = new StudentQuestionRemote();
            web.GetPaperSubmitCnt(paperViewId, ref submitTimes);
        }
#if CHINAACC
		public static List<SubjectClassify> GetSubjectClassify()
		{
			return new StudentQuestionData().GetSubjectClassify();
		}
#endif
	}
}
