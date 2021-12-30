using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Framework.Model;
using Framework.NewModel;
using Framework.Utility;
using System.Globalization;

namespace Framework.Local
{
	public class StudentQuestionData : DataAccessBase
	{
		#region 从线上获取到的数据，保存到本地
		private const string AddSiteCourseSql = "INSERT or REPLACE INTO StudentSiteCourse(SiteCourseId,CourseId,SiteCourseName,courseChapter,status,CreateTime,BoardId) values($SiteCourseId,$CourseId,$SiteCourseName,$courseChapter,$status,$CreateTime,$BoardId)";
		private const string AddQuestionTypeSql = "INSERT or REPLACE INTO StudentQuestionType(quesViewType, viewTypeName, quesTypeId, description, status , creator, createTime ,paperTypeName) values($quesViewType, $viewTypeName, $quesTypeId, $description, $status , $creator, $createTime ,$paperTypeName)";
		private const string AddChapterListSql = "INSERT or REPLACE INTO StudentChapterList ([chapterListId],[chapterListName],[sequence],[status],[creator],[createTime],[courseId],[chapterNum]) VALUES ($chapterListId,$chapterListName,$sequence,$status,$creator,$createTime,$courseId,$chapterNum);";
		private const string AddChapterSql = "INSERT or REPLACE INTO StudentChapter ([chapterId],[chapterListId],[chapterName],[sequence],[ShowStatus],[status],[creator],[createTime],[courseId],[chapterNum]) VALUES ($chapterId,$chapterListId,$chapterName,$sequence,$ShowStatus,$status,$creator,$createTime,$courseId,$chapterNum);";
		private const string AddCenterSql = "INSERT or REPLACE INTO StudentCenter ([siteCourseId],[centerId],[centerName],[centerYear],[centerType],[openStatus],[centerParam]) VALUES ($siteCourseId,$centerId,$centerName,$centerYear,$centerType,$openStatus,$centerParam);";
		private const string AddPaperSql = "INSERT or REPLACE INTO StudentPaper ([centerId],[paperId],[paperCatId],[courseId],[paperYear],[chapter],[suitNum],[paperName],[totalScore],[status],[creator],[createTime],[chapterId]) VALUES ($centerId,$paperId,$paperCatId,$courseId,$paperYear,$chapter,$suitNum,$paperName,$totalScore,$status,$creator,$createTime,$chapterId);";

        private const string AddPaperViewSql = "INSERT or REPLACE INTO StudentPaperView ([centerId],[paperViewId],[paperViewName],[paperId],[paperParam],[openStatus],[explainURL],[isContest],[contestTimes],[contestStartTime],[contestEndTime],[contestTimeLimit],[dayiId],[doneCount],[avgScore],[status],[creator],[createTime],[description],[paperViewCatId],[modifyStatus],[paperType],[sequence]) VALUES ($centerId,$paperViewId,$paperViewName,$paperId,$paperParam,$openStatus,$explainURL,$isContest,$contestTimes,$contestStartTime,$contestEndTime,$contestTimeLimit,$dayiId,$doneCount,$avgScore,$status,$creator,$createTime,$description,$paperViewCatId,$modifyStatus,$paperType,$sequence);";
        // 向数据库存入时带上Version字段
        private const string AddPaperViewSql_withVersion = "INSERT or REPLACE INTO StudentPaperView ([centerId],[paperViewId],[paperViewName],[paperId],[paperParam],[openStatus],[explainURL],[isContest],[contestTimes],[contestStartTime],[contestEndTime],[contestTimeLimit],[dayiId],[doneCount],[avgScore],[status],[creator],[createTime],[description],[paperViewCatId],[modifyStatus],[paperType],[sequence],[Version]) VALUES ($centerId,$paperViewId,$paperViewName,$paperId,$paperParam,$openStatus,$explainURL,$isContest,$contestTimes,$contestStartTime,$contestEndTime,$contestTimeLimit,$dayiId,$doneCount,$avgScore,$status,$creator,$createTime,$description,$paperViewCatId,$modifyStatus,$paperType,$sequence,$Version);";

        private const string AddPaperPartSql = "INSERT or REPLACE INTO StudentPaperPart ([partId],[paperId],[partName],[sequence],[creator],[createTime],[quesViewType],[randomNum]) VALUES ($partId,$paperId,$partName,$sequence,$creator,$createTime,$quesViewType,$randomNum);";
		private const string AddQuestionSql = "INSERT or REPLACE INTO StudentQuestion ([paperViewId],[questionId],[parentId],[quesTypeId],[quesViewType],[content],[answer],[analysis],[limitMinute],[score],[splitScore],[status],[lecture],[creator],[createTime],[modifyStatus],[wrongRate],[Sequence]) VALUES ($paperViewId,$questionId,$parentId,$quesTypeId,$quesViewType,$content,$answer,$analysis,$limitMinute,$score,$splitScore,$status,$lecture,$creator,$createTime,$modifyStatus,$wrongRate,$Sequence);";
		private const string AddQuestionOptionSql = "INSERT or REPLACE INTO StudentQuestionOption ([questionId],[quesOption],[quesValue],[sequence]) VALUES ($questionId,$quesOption,$quesValue,$sequence);";
		private const string AddSubjectClassifySql = "Insert Or Replace Into StudentSubjectClassify([SubjectClassifyId],[SubjectName],[TreeType],[ParentId]) Values($SubjectClassifyId,$SubjectName,$TreeType,$ParentId);";

		private const string AddCenterSubjectSql = "Insert Or Replace Into StudentCenterSubject(EduSubjectId,CenterId,CenterType,Creator,CenterYear,OpenStatus,CenterName,Description,Sequence,SiteCourseId,CenterParam,CreateTime) Values($EduSubjectId,$CenterId,$CenterType,$Creator,$CenterYear,$OpenStatus,$CenterName,$Description,$Sequence,$SiteCourseId,$CenterParam,$CreateTime)";
        // 向数据库存入时带上UpdateTime属性，@author ChW，@date 2021-05-14
        private const string AddCenterSubjectSql_withUpdateTime =
			"Insert Or Replace Into StudentCenterSubject(" +
			"EduSubjectId,CenterId,CenterType,Creator,CenterYear,OpenStatus,CenterName,Description,Sequence,SiteCourseId,CenterParam,CreateTime,UpdateTime" +
			")" +
			"Values(" +
			"$EduSubjectId,$CenterId,$CenterType,$Creator,$CenterYear,$OpenStatus,$CenterName,$Description,$Sequence,$SiteCourseId,$CenterParam,$CreateTime,$UpdateTime" +
			")";
		internal bool AddSiteCourseList(IEnumerable<StudentSiteCourse> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
					var cmd = new SQLiteCommand(AddSiteCourseSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$boardId") { Value = item.BoardId });
					cmd.Parameters.Add(new SQLiteParameter("$courseChapter") { Value = item.CourseChapter });
					cmd.Parameters.Add(new SQLiteParameter("$courseId") { Value = item.CourseId });
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$siteCourseId") { Value = item.SiteCourseId });
					cmd.Parameters.Add(new SQLiteParameter("$siteCourseName") { Value = item.SiteCourseName });
					cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddChapterListList(IEnumerable<StudentChapterList> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
					var cmd = new SQLiteCommand(AddChapterListSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$chapterListId") { Value = item.ChapterListId });
					cmd.Parameters.Add(new SQLiteParameter("$chapterListName") { Value = item.ChapterListName });
					cmd.Parameters.Add(new SQLiteParameter("$chapterNum") { Value = item.ChapterNum });
					cmd.Parameters.Add(new SQLiteParameter("$courseId") { Value = item.CourseId });
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$sequence") { Value = item.Sequence });
					cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddChapterList(IEnumerable<StudentChapter> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
					var cmd = new SQLiteCommand(AddChapterSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$chapterId") { Value = item.ChapterId, DbType = DbType.Int32 });
					cmd.Parameters.Add(new SQLiteParameter("$chapterListId") { Value = item.ChapterListId });
					cmd.Parameters.Add(new SQLiteParameter("$chapterName") { Value = item.ChapterName });
					cmd.Parameters.Add(new SQLiteParameter("$chapterNum") { Value = item.ChapterNum });
					cmd.Parameters.Add(new SQLiteParameter("$courseId") { Value = item.CourseId });
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$sequence") { Value = item.Sequence });
					cmd.Parameters.Add(new SQLiteParameter("$ShowStatus") { Value = item.ShowStatus });
					cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddQuestionTypeList(IEnumerable<StudentQuestionType> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
					var cmd = new SQLiteCommand(AddQuestionTypeSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$description") { Value = item.Description });
					cmd.Parameters.Add(new SQLiteParameter("$paperTypeName") { Value = item.PaperTypeName });
					cmd.Parameters.Add(new SQLiteParameter("$quesTypeId") { Value = item.QuesTypeId });
					cmd.Parameters.Add(new SQLiteParameter("$quesViewType") { Value = item.QuesViewType });
					cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
					cmd.Parameters.Add(new SQLiteParameter("$viewTypeName") { Value = item.ViewTypeName });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddCenterList(IEnumerable<StudentCenter> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
					var cmd = new SQLiteCommand(AddCenterSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$centerId") { Value = item.CenterId });
					cmd.Parameters.Add(new SQLiteParameter("$centerName") { Value = item.CenterName });
					cmd.Parameters.Add(new SQLiteParameter("$centerParam") { Value = item.CenterParam });
					cmd.Parameters.Add(new SQLiteParameter("$centerType") { Value = item.CenterType });
					cmd.Parameters.Add(new SQLiteParameter("$centerYear") { Value = item.CenterYear });
					cmd.Parameters.Add(new SQLiteParameter("$openStatus") { Value = item.OpenStatus });
					cmd.Parameters.Add(new SQLiteParameter("$siteCourseId") { Value = item.SiteCourseId });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddPaperList(IEnumerable<StudentPaper> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
                    var cmd = new SQLiteCommand(AddPaperSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$centerId") { Value = item.CenterId });
					cmd.Parameters.Add(new SQLiteParameter("$chapter") { Value = item.Chapter });
					cmd.Parameters.Add(new SQLiteParameter("$chapterId") { Value = item.ChapterId });
					cmd.Parameters.Add(new SQLiteParameter("$courseId") { Value = item.CourseId });
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$paperCatId") { Value = item.PaperCatId });
					cmd.Parameters.Add(new SQLiteParameter("$paperId") { Value = item.PaperId });
					cmd.Parameters.Add(new SQLiteParameter("$paperName") { Value = item.PaperName });
					cmd.Parameters.Add(new SQLiteParameter("$paperYear") { Value = item.PaperYear });
					cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
					cmd.Parameters.Add(new SQLiteParameter("$suitNum") { Value = item.SuitNum });
					cmd.Parameters.Add(new SQLiteParameter("$totalScore") { Value = item.TotalScore });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddPaperPartList(IEnumerable<StudentPaperPart> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
                    var cmd = new SQLiteCommand(AddPaperPartSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$paperId") { Value = item.PaperId });
					cmd.Parameters.Add(new SQLiteParameter("$partId") { Value = item.PartId });
					cmd.Parameters.Add(new SQLiteParameter("$partName") { Value = item.PartName });
					cmd.Parameters.Add(new SQLiteParameter("$quesViewType") { Value = item.QuesViewType });
					cmd.Parameters.Add(new SQLiteParameter("$randomNum") { Value = item.RandomNum });
					cmd.Parameters.Add(new SQLiteParameter("$sequence") { Value = item.Sequence });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddPaperViewList(IEnumerable<StudentPaperView> list)
		{
			OpenConn();
			SQLiteTransaction tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
                    var cmd = new SQLiteCommand(AddPaperViewSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$avgScore") { Value = item.AvgScore });
					cmd.Parameters.Add(new SQLiteParameter("$centerId") { Value = item.CenterId });
					cmd.Parameters.Add(new SQLiteParameter("$contestEndTime") { Value = item.ContestEndTime });
					cmd.Parameters.Add(new SQLiteParameter("$contestStartTime") { Value = item.ContestStartTime });
					cmd.Parameters.Add(new SQLiteParameter("$contestTimeLimit") { Value = item.ContestTimeLimit });
					cmd.Parameters.Add(new SQLiteParameter("$contestTimes") { Value = item.ContestTimes });
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$dayiId") { Value = item.DayiId });
					cmd.Parameters.Add(new SQLiteParameter("$description") { Value = item.Description });
					cmd.Parameters.Add(new SQLiteParameter("$doneCount") { Value = item.DoneCount });
					cmd.Parameters.Add(new SQLiteParameter("$explainURL") { Value = item.ExplainUrl });
					cmd.Parameters.Add(new SQLiteParameter("$isContest") { Value = item.IsContest });
					cmd.Parameters.Add(new SQLiteParameter("$modifyStatus") { Value = item.ModifyStatus });
					cmd.Parameters.Add(new SQLiteParameter("$openStatus") { Value = item.OpenStatus });
					cmd.Parameters.Add(new SQLiteParameter("$paperId") { Value = item.PaperId });
					cmd.Parameters.Add(new SQLiteParameter("$paperParam") { Value = item.PaperParam });
					cmd.Parameters.Add(new SQLiteParameter("$paperType") { Value = item.PaperType });
					cmd.Parameters.Add(new SQLiteParameter("$paperViewCatId") { Value = item.PaperViewCatId });
					cmd.Parameters.Add(new SQLiteParameter("$paperViewId") { Value = item.PaperViewId });
					cmd.Parameters.Add(new SQLiteParameter("$paperViewName") { Value = item.PaperViewName });
					cmd.Parameters.Add(new SQLiteParameter("$sequence") { Value = item.Sequence });
					cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

        /// <summary>
        ///添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="centerId"></param>
        /// <returns></returns>
        internal bool AddPaperViewList(IEnumerable<StudentPaperView> list,int centerId)
        {
            OpenConn();

            // 在事务开始之前获取Version字段，用于更新列表时不覆盖Version
            Dictionary<int, int> paperViewID_Version = new Dictionary<int, int>();
            foreach (var item in list)
            {
                // 读取Version字段
                var cmd = new SQLiteCommand("Select Version,paperViewID From StudentPaperView", Conn);
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int paperViewID = Convert.ToInt32(reader["paperViewID"].ToString());
                    int Version = Convert.ToInt32(reader["Version"].ToString());
                    try
                    {
                        paperViewID_Version.Add(paperViewID, Version);
                    }
                    catch (ArgumentException)
                    {
                        // 忽略重复的key值
                        continue;
                    }
                }
            }

            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (var item in list)
                {
                    SQLiteCommand cmd = null;
                    // Version字段无值，则跳过该字段
                    if (paperViewID_Version.ContainsKey(Convert.ToInt32(item.PaperViewId)))
                    {
                        cmd = new SQLiteCommand(AddPaperViewSql_withVersion, Conn) { Transaction = tran };
                    }
                    else
                    {
                        cmd = new SQLiteCommand(AddPaperViewSql, Conn) { Transaction = tran };
                    }
                    //var cmd = new SQLiteCommand(AddPaperViewSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$avgScore") { Value = item.AvgScore });
                    cmd.Parameters.Add(new SQLiteParameter("$centerId") { Value = centerId });
                    cmd.Parameters.Add(new SQLiteParameter("$contestEndTime") { Value = item.ContestEndTime });
                    cmd.Parameters.Add(new SQLiteParameter("$contestStartTime") { Value = item.ContestStartTime });
                    cmd.Parameters.Add(new SQLiteParameter("$contestTimeLimit") { Value = item.ContestTimeLimit });
                    cmd.Parameters.Add(new SQLiteParameter("$contestTimes") { Value = item.ContestTimes });
                    cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
                    cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
                    cmd.Parameters.Add(new SQLiteParameter("$dayiId") { Value = item.DayiId });
                    cmd.Parameters.Add(new SQLiteParameter("$description") { Value = item.Description });
                    cmd.Parameters.Add(new SQLiteParameter("$doneCount") { Value = item.DoneCount });
                    cmd.Parameters.Add(new SQLiteParameter("$explainURL") { Value = item.ExplainUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$isContest") { Value = item.IsContest });
                    cmd.Parameters.Add(new SQLiteParameter("$modifyStatus") { Value = item.ModifyStatus });
                    cmd.Parameters.Add(new SQLiteParameter("$openStatus") { Value = item.OpenStatus });
                    cmd.Parameters.Add(new SQLiteParameter("$paperId") { Value = item.PaperId });
                    cmd.Parameters.Add(new SQLiteParameter("$paperParam") { Value = item.PaperParam });
                    cmd.Parameters.Add(new SQLiteParameter("$paperType") { Value = item.PaperType });
                    cmd.Parameters.Add(new SQLiteParameter("$paperViewCatId") { Value = item.PaperViewCatId });
                    cmd.Parameters.Add(new SQLiteParameter("$paperViewId") { Value = item.PaperViewId });
                    cmd.Parameters.Add(new SQLiteParameter("$paperViewName") { Value = item.PaperViewName });
                    cmd.Parameters.Add(new SQLiteParameter("$sequence") { Value = item.Sequence });
                    cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
                    // 添加对“试卷公用信息版本”字段的刷新
                    if (paperViewID_Version.ContainsKey(Convert.ToInt32(item.PaperViewId)))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("$Version") { Value = paperViewID_Version[Convert.ToInt32(item.PaperViewId)] });
                    }
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                return true;
            }
            catch
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                CloseConn();
            }
        }

		internal bool AddQuestionList(IEnumerable<StudentQuestion> list)
		{
			OpenConn();
			SQLiteTransaction tran = Conn.BeginTransaction();
			try
			{
				var fitem = list.FirstOrDefault();
				if (fitem != null)
				{
					var delcmd = new SQLiteCommand("Delete From StudentQuestion Where paperViewId = $paperViewId", Conn) { Transaction = tran };
					delcmd.Parameters.Add(new SQLiteParameter("$paperViewId") { Value = fitem.PaperViewId });
					delcmd.ExecuteNonQuery();
				}
				foreach (var item in list)
				{
					var cmd = new SQLiteCommand(AddQuestionSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$analysis") { Value = Crypt.Rc4EncryptString(item.Analysis) });
					cmd.Parameters.Add(new SQLiteParameter("$answer") { Value = Crypt.Rc4EncryptString(item.Answer) });
					cmd.Parameters.Add(new SQLiteParameter("$content") { Value = Crypt.Rc4EncryptString(item.Content) });
					cmd.Parameters.Add(new SQLiteParameter("$createTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$lecture") { Value = item.Lecture });
					cmd.Parameters.Add(new SQLiteParameter("$limitMinute") { Value = item.LimitMinute });
					cmd.Parameters.Add(new SQLiteParameter("$modifyStatus") { Value = item.ModifyStatus });
					cmd.Parameters.Add(new SQLiteParameter("$paperViewId") { Value = item.PaperViewId });
					cmd.Parameters.Add(new SQLiteParameter("$parentId") { Value = item.ParentId });
					cmd.Parameters.Add(new SQLiteParameter("$questionId") { Value = item.QuestionId });
					cmd.Parameters.Add(new SQLiteParameter("$quesTypeId") { Value = item.QuesTypeId });
					cmd.Parameters.Add(new SQLiteParameter("$quesViewType") { Value = item.QuesViewType });
					cmd.Parameters.Add(new SQLiteParameter("$score") { Value = item.Score });
					cmd.Parameters.Add(new SQLiteParameter("$splitScore") { Value = item.SplitScore });
					cmd.Parameters.Add(new SQLiteParameter("$status") { Value = item.Status });
					cmd.Parameters.Add(new SQLiteParameter("$wrongRate") { Value = item.WrongRate });
					cmd.Parameters.Add(new SQLiteParameter("$Sequence") { Value = item.Sequence });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddPaperCommonInfo(int paperViewID, PaperCommonInfo comInfo)
		{
			if (comInfo == null)
				return false;

			OpenConn();
			SQLiteTransaction tran = Conn.BeginTransaction();
			try
			{
				const string AddPaperVersionSql = "UPDATE StudentPaperView SET Version = $Version WHERE paperViewID = $paperViewID;";

				var cmd = new SQLiteCommand(AddPaperVersionSql, Conn) { Transaction = tran };
				cmd.Parameters.Add(new SQLiteParameter("$Version") { Value = comInfo.Version });
				cmd.Parameters.Add(new SQLiteParameter("$paperViewID") { Value = paperViewID });
				cmd.ExecuteNonQuery();

				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		internal bool AddQuestionOptionList(IEnumerable<StudentQuestionOption> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
                    var cmd = new SQLiteCommand(AddQuestionOptionSql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$quesOption") { Value = Crypt.Rc4EncryptString(item.QuesOption) });
					cmd.Parameters.Add(new SQLiteParameter("$questionId") { Value = item.QuestionId });
					cmd.Parameters.Add(new SQLiteParameter("$quesValue") { Value = item.QuesValue });
					cmd.Parameters.Add(new SQLiteParameter("$sequence") { Value = item.Sequence });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

        internal bool AddCenterSubjectList(int eduSubjectId, IEnumerable<StudentPaperCenter> list)
		{
			OpenConn();

            /**
			 * 在事务开始之前获取updatetime，用于更新列表时不覆盖updatetime
			 * @author ChW
			 * @date 2021-05-17
			 */
            Dictionary<int, string> centerId_updateTime = new Dictionary<int, string>();
			foreach (var item in list)
            {
                // 读取UpdateTime字段，可以放在另一个流程里，填充好list再进来刷新数据库~~~~~~~~~~~~~~~~~~~~~~~
                var cmd = new SQLiteCommand("Select UpdateTime From StudentCenterSubject Where CenterId = $CenterId", Conn);
                cmd.Parameters.Add(new SQLiteParameter("$CenterId") { Value = item.CenterId });
                SQLiteDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string updateTime = reader["UpdateTime"].ToString();
                    centerId_updateTime.Add(Convert.ToInt32(item.CenterId), updateTime);
                }
            }

			var tran = Conn.BeginTransaction();

            try
			{
				var cmd = new SQLiteCommand("Delete From StudentCenterSubject Where EduSubjectId = $EduSubjectId", Conn)
				{
					Transaction = tran
				};
				cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = eduSubjectId });
				cmd.ExecuteNonQuery();

                foreach (var item in list)
				{
                    // UpdateTime字段无值，则跳过该字段，@author ChW，@date 2021-05-17
                    if (centerId_updateTime.ContainsKey(Convert.ToInt32(item.CenterId)))
                    {
                        cmd = new SQLiteCommand(AddCenterSubjectSql_withUpdateTime, Conn) { Transaction = tran };
                    }
                    else
                    {
                        cmd = new SQLiteCommand(AddCenterSubjectSql, Conn) { Transaction = tran };
                    }
					cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = eduSubjectId });
					cmd.Parameters.Add(new SQLiteParameter("$CenterId") { Value = item.CenterId });
					cmd.Parameters.Add(new SQLiteParameter("$CenterName") { Value = item.CenterName });
					cmd.Parameters.Add(new SQLiteParameter("$CenterParam") { Value = item.CenterParam });
					cmd.Parameters.Add(new SQLiteParameter("$CenterType") { Value = item.CenterType });
					cmd.Parameters.Add(new SQLiteParameter("$CenterYear") { Value = item.CenterYear });
					cmd.Parameters.Add(new SQLiteParameter("$CreateTime") { Value = item.CreateTime });
					cmd.Parameters.Add(new SQLiteParameter("$Creator") { Value = item.Creator });
					cmd.Parameters.Add(new SQLiteParameter("$Description") { Value = item.Description });
					cmd.Parameters.Add(new SQLiteParameter("$OpenStatus") { Value = item.OpenStatus });
					cmd.Parameters.Add(new SQLiteParameter("$Sequence") { Value = item.Sequence });
                    cmd.Parameters.Add(new SQLiteParameter("$SiteCourseId") { Value = item.SiteCourseId });
                    // 添加对“更新时间”字段的刷新，@author ChW，@date 2021-05-17
                    if (centerId_updateTime.ContainsKey(Convert.ToInt32(item.CenterId)))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("$UpdateTime") { Value = centerId_updateTime[Convert.ToInt32(item.CenterId)] });
                    }
                    cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch (Exception ex)
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}

		}

        /// <summary>
        /// 添加做题记录信息
        /// </summary>
        /// <param name="paperScore"></param>
        /// <returns></returns>
        public bool AddPaperSocres(StudentPaperScore paperScore)
        {
            OpenConn();
            var tran = Conn.BeginTransaction();
            try
            {
                string soreSql = "Insert Or Replace Into StudentPaperScores(SSOUID,PaperScoreID,PaperViewID,SiteCourseID,AutoScore,LastScore,PaperScore,SpendTime,CenterID,CreateTime) Values($SSOUID,$PaperScoreID,$PaperViewID,$SiteCourseID,$AutoScore,$LastScore,$PaperScore,$SpendTime,$CenterID,$CreateTime)";
                var cmd = new SQLiteCommand(soreSql, Conn) { Transaction = tran };
                cmd.Parameters.Add(new SQLiteParameter("$SSOUID") { Value = paperScore.UserID });
                cmd.Parameters.Add(new SQLiteParameter("$PaperScoreID") { Value = paperScore.PaperScoreID });
                cmd.Parameters.Add(new SQLiteParameter("$PaperViewID") { Value = paperScore.PaperViewID });
                cmd.Parameters.Add(new SQLiteParameter("$SiteCourseID") { Value = paperScore.SiteCourseID });
                cmd.Parameters.Add(new SQLiteParameter("$AutoScore") { Value = paperScore.AutoScore });
                cmd.Parameters.Add(new SQLiteParameter("$LastScore") { Value = paperScore.LastScore });
                cmd.Parameters.Add(new SQLiteParameter("$PaperScore") { Value = paperScore.PaperScore });
                cmd.Parameters.Add(new SQLiteParameter("$SpendTime") { Value = paperScore.SpendTime });
                cmd.Parameters.Add(new SQLiteParameter("$CenterID") { Value = paperScore.CenterID });
                cmd.Parameters.Add(new SQLiteParameter("$CreateTime") { Value = paperScore.CreateTime });
                cmd.ExecuteNonQuery();
                if (paperScore.Answers != null && paperScore.Answers.Count > 0)
                {
                    string answerSql = "Insert Or Replace Into StudentPaperScoresAnswer(SSOUID,PaperScoreID,QuestionID,UserAnswer,UserScore) Values($SSOUID,$PaperScoreID,$QuestionID,$UserAnswer,$UserScore)";
                    foreach (var item in paperScore.Answers)
                    {
                        cmd = new SQLiteCommand(answerSql, Conn) { Transaction = tran };
                        cmd.Parameters.Add(new SQLiteParameter("$SSOUID") { Value = paperScore.UserID });
                        cmd.Parameters.Add(new SQLiteParameter("$PaperScoreID") { Value = paperScore.PaperScoreID });
                        cmd.Parameters.Add(new SQLiteParameter("$QuestionID") { Value = item.QuestionID });
                        cmd.Parameters.Add(new SQLiteParameter("$UserAnswer") { Value = item.UserAnswer });
                        cmd.Parameters.Add(new SQLiteParameter("$UserScore") { Value = item.UserScore });
                        cmd.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                CloseConn();
            }

        }

#if CHINAACC
		internal bool AddSubjectClassify(IEnumerable<SubjectClassify> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				foreach (var item in list)
				{
					var cmd = new SQLiteCommand(AddSubjectClassifySql, Conn) { Transaction = tran };
					cmd.Parameters.Add(new SQLiteParameter("$SubjectClassifyId") { Value = item.SubjectClassifyId });
					cmd.Parameters.Add(new SQLiteParameter("$SubjectName") { Value = item.SubjectName });
					cmd.Parameters.Add(new SQLiteParameter("$TreeType") { Value = item.TreeType });
					cmd.Parameters.Add(new SQLiteParameter("$ParentId") { Value = item.ParentId });
					cmd.ExecuteNonQuery();
				}
				tran.Commit();
				return true;
			}
			catch
			{
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}
#endif
		#endregion

		//private const string GetCenterPaperListSql = "Select C.CenterName,V.PaperViewId,V.PaperViewName,V.[Sequence] from StudentCenter C Inner Join StudentPaperView V On C.CenterId=V.CenterId Where C.SiteCourseId=$SiteCourseId";
        //因缺少quesViewType排序 导致题号重复 dgh 2017.04.28
        private const string GetQuestionDetailSql = "Select PP.partName,PP.Sequence As PSeq,T.paperTypeName,Q.QuestionId,Q.ParentId,Q.QuesTypeId,Q.quesViewType,Q.Content,Q.Answer,Q.Analysis,Q.Score,Q.Sequence,QE.IsDone,QE.IsWrong,QE.IsFav,O.quesValue,O.quesOption,O.Sequence As OpSequence from StudentQuestion Q Inner Join StudentPaperView PV On Q.PaperViewId =PV.PaperViewId Left Join StudentPaperPart PP On PP.PaperId=Pv.PaperId And PP.quesViewType = Q.quesViewType Left Join StudentQuestionExtend QE On Q.QuestionId=QE.QuestionId And QE.SSOUId = $SSOUId And QE.PaperViewID=Q.paperViewID Left Join StudentQuestionOption O On Q.QuestionId=O.QuestionId Left Join StudentQuestionType T On Q.quesViewType=T.quesViewType where Q.PaperViewId=$PaperViewId order by Q.ParentId,Q.quesViewType,PP.Sequence,Q.Sequence,O.sequence";
 
        private const string sqlType = "select distinct PP.partName,PP.Sequence As PSeq from StudentQuestion Q Inner Join StudentPaperView PV On Q.PaperViewId =PV.PaperViewId inner Join StudentPaperPart PP On PP.PaperId=Pv.PaperId And PP.quesViewType = Q.quesViewType inner Join StudentQuestionType T On Q.quesViewType=T.quesViewType where Q.PaperViewId=$paperViewId order by PP.Sequence";

        private const string GetCenterListSql = "Select C.SiteCourseId,S.EduSubjectName As SiteCourseName,C.CenterId,C.CenterName,CC.AllCnt,CC.PCnt,CC.FavCnt,CC.WrongCnt,CC.DoCnt from StudentCenterSubject C Inner Join StudentEduSubject S On C.EduSubjectId = S.EduSubjectId Left Join (Select V.CenterId,Count(*) AllCnt,Count(distinct Q.ParentId)-1 PCnt,Sum(QE.IsFav) FavCnt,Sum(QE.IsWrong) WrongCnt,Sum(QE.ISDone) DoCnt From StudentPaperView V Inner Join StudentQuestion Q On V.paperViewId=Q.paperViewId Left Join StudentQuestionExtend QE On Q.QuestionId=QE.QuestionId group by V.CenterId) CC On CC.CenterId = C.CenterId Order By S.[OrderNo] Desc,C.[Sequence] Asc";
        // 从数据库取CenterList信息的时候带上UpdateTime属性，用于在界面上显示，@author ChW，@date 2021-05-17
        private const string GetCenterListSql_withUpdateTime = "Select C.SiteCourseId,S.EduSubjectName As SiteCourseName,C.CenterId,C.CenterName,C.UpdateTime,CC.AllCnt,CC.PCnt,CC.FavCnt,CC.WrongCnt,CC.DoCnt from StudentCenterSubject C Inner Join StudentEduSubject S On C.EduSubjectId = S.EduSubjectId Left Join (Select V.CenterId,Count(*) AllCnt,Count(distinct Q.ParentId)-1 PCnt,Sum(QE.IsFav) FavCnt,Sum(QE.IsWrong) WrongCnt,Sum(QE.ISDone) DoCnt From StudentPaperView V Inner Join StudentQuestion Q On V.paperViewId=Q.paperViewId Left Join StudentQuestionExtend QE On Q.QuestionId=QE.QuestionId group by V.CenterId) CC On CC.CenterId = C.CenterId Order By S.[OrderNo] Desc,C.[Sequence] Asc";

        private const string GetCenterPaperListSql = "Select V.Version,V.PaperViewId,V.PaperViewName,V.ContestTimes,Count(Q.questionId) AllCnt,Count(Distinct Q.ParentId) -1 PCnt,Sum(QE.IsDone) DoCnt,Sum(QE.IsFav) FavCnt,Sum(QE.IsWrong) WrongCnt from StudentPaperView V  Left Join StudentQuestion Q On V.paperViewId = Q.paperViewId Left Join StudentQuestionExtend QE On Q.QuestionId=QE.QuestionId and QE.paperViewId = Q.paperViewId Where V.centerId=$centerId Group By V.PaperViewId Order By V.Sequence";

		public List<ViewStudentQuestion> GetPaperDetail(int paperViewId, int ssoUid)
		{
			var dt = ExecuteTable(GetQuestionDetailSql,
				new SQLiteParameter("$paperViewId") { Value = paperViewId },
				new SQLiteParameter("$SSOUId") { Value = ssoUid });
			if (dt == null) return null;
			var lastQuestionId = 0;
			var lastQuestionType = string.Empty;
			var mainId = 1;
			var lst = new List<ViewStudentQuestion>();
			foreach (DataRow dr in dt.Rows)
			{
				var qid = dr.Field<int>("QuestionId");
				var qtype = dr.Field<string>(dr["partName"] != DBNull.Value ? "partName" : "paperTypeName");
				//var qtype = dr.Field<int>("quesViewType");
				if (qtype != lastQuestionType)
				{
					lastQuestionType = qtype;
					mainId = 1;
				}
				//Trace.WriteLine(dr.Field<int>("QuestionId") + ":" + dr.Field<int?>("IsFav") + ":" + dr.Field<string>("quesValue"));
				if (lastQuestionId != qid)
				{
					lastQuestionId = qid;
					var item = new ViewStudentQuestion
					{
						QuestionTypeName = dr.Field<string>("paperTypeName"),
						QuestionId = dr.Field<int>("QuestionId"),
						Analysis = Crypt.Rc4DecryptString(dr.Field<string>("Analysis")),
						Answer = Crypt.Rc4DecryptString(dr.Field<string>("Answer")),
						Content = Crypt.Rc4DecryptString(dr.Field<string>("Content")),
						ParentId = dr.Field<int>("ParentId"),
						QuesTypeId = dr.Field<int>("quesTypeId"),
						QuesViewType = dr.Field<int>("quesViewType"),
						Score = dr.Field<double>("score"),
						UserAnswer = string.Empty,
						Sequence = dr.Field<int>("Sequence")
					};

					if (item.ParentId > 0)
					{
						item.Sequence = item.ParentId * 100 + item.Sequence;
					}
					if (dr["partName"] != DBNull.Value)
					{
						item.PartName = dr.Field<string>("partName");
						item.PartSequence = dr.Field<int>("PSeq");
					}
					else
					{
                        item.PartName = item.QuestionTypeName;
                        ////item.PartSequence = 1000000 + item.QuesTypeId;
                        //防止出现题号错乱的问题 dgh 2017.08.22
                        var dtType = ExecuteTable(sqlType, new SQLiteParameter("$paperViewId") { Value = paperViewId });
                        var dtf = dtType.DefaultView;
                        if (dtf == null || dtf.Count == 0)
                        {
                            item.PartSequence = 1000000 + item.QuesTypeId;
                        }
                        else
                        {
                            dtf.RowFilter = "partName='" + item.QuestionTypeName + "'";
                            if (dtf.Count!= 0)
                            {
                                item.PartSequence = Int32.Parse(dtf[0]["PSeq"].ToString());
                            }
                            else
                            {
                                item.PartSequence = 1000000 + item.QuesTypeId;
                            }
                            
                        }
                        
					}
					var va = dr.Field<int?>("IsDone");
					item.IsDone = va.HasValue && va.Value == 1;
					va = dr.Field<int?>("IsWrong");
					item.IsWrong = va.HasValue && va.Value == 1;
					va = dr.Field<int?>("IsFav");
					item.IsFav = va.HasValue && va.Value == 1;

					if (item.ParentId != 0)
					{
						item.Parent = lst.FirstOrDefault(x => x.QuestionId == item.ParentId);
						if (item.Parent != null)
						{
							item.MainId = item.Parent.MainId;
							item.QuestionTypeName = item.Parent.QuestionTypeName;
							item.QuesViewType = item.Parent.QuesViewType;
							item.Parent.SubCnt++;
							item.SubId = item.Parent.SubId + item.Parent.SubCnt;
							item.PartName = item.Parent.PartName;
							item.PartSequence = item.Parent.PartSequence;
						}
					}
					else
					{
						item.MainId = mainId++;
						item.SubId = 0;
						item.SubCnt = 0;
					}
					if (dr["OpSequence"] != DBNull.Value)
					{
						item.OptionList.Add(new StudentQuestionOption
						{
							QuesOption = Crypt.Rc4DecryptString(dr.Field<string>("quesOption")),
							QuestionId = dr.Field<int>("questionId"),
							QuesValue = dr.Field<string>("quesValue"),
							Sequence = dr.Field<int>("OpSequence")
						});
					}
					lst.Add(item);
				}
				else
				{
					var item = lst.First(x => x.QuestionId == qid);
					item.OptionList.Add(new StudentQuestionOption
					{
						QuesOption = Crypt.Rc4DecryptString(dr.Field<string>("quesOption")),
						QuestionId = dr.Field<int>("questionId"),
						QuesValue = dr.Field<string>("quesValue"),
						Sequence = dr["OpSequence"] == DBNull.Value ? 0 : dr.Field<int>("OpSequence")
					});
				}
			}
            return lst.Where(x => !lst.Exists(y => y.ParentId == x.QuestionId))
                .OrderBy(x => x.PartSequence)
                //.ThenBy(x => x.ParentId)
                //.ThenBy(x => x.Sequence)
                .ThenBy(x => x.MainId)
                .ThenBy(x => x.SubId)
                .ToList();
		}
        /// <summary>
        /// 做题记录详细信息
        /// </summary>
        /// <param name="paperViewId"></param>
        /// <param name="ssoUid"></param>
        /// <param name="paperScoreID"></param>
        /// <returns></returns>
        public List<ViewStudentQuestion> GetPaperScoreDetail(int paperViewId, int ssoUid, string paperScoreID)
        {
            const string Sql = "Select PP.partName,PP.Sequence As PSeq,T.paperTypeName,Q.QuestionId,Q.ParentId,Q.QuesTypeId,Q.quesViewType,Q.Content,Q.Answer,Q.Analysis,Q.Score,SS.[UserAnswer],ifnull(SS.[UserScore],'0') as UserScore,Q.Sequence,QE.IsDone,QE.IsWrong,QE.IsFav,O.quesValue,O.quesOption,O.Sequence As OpSequence from StudentQuestion Q Inner Join StudentPaperView PV On Q.PaperViewId =PV.PaperViewId Left Join StudentPaperPart PP On PP.PaperId=Pv.PaperId And PP.quesViewType = Q.quesViewType Left Join StudentQuestionExtend QE On Q.QuestionId=QE.QuestionId And QE.SSOUId = $SSOUId And QE.PaperViewID=Q.paperViewID Left Join StudentQuestionOption O On Q.QuestionId=O.QuestionId Left Join StudentQuestionType T On Q.quesViewType=T.quesViewType left join StudentPaperScores SC on SC.[PaperViewID]=PV.[paperViewID] and SC.PaperScoreID=$PaperScoreID left join StudentPaperScoresAnswer SS On SS.PaperScoreID=SC.PaperScoreID And SS.QuestionID=Q.QuestionID And SS.SSOUId=SC.SSOUId where Q.PaperViewId=$PaperViewId order by Q.ParentId,Q.quesViewType,PP.Sequence,Q.Sequence,O.sequence";
            var dt = ExecuteTable(Sql,
                 new SQLiteParameter("$paperViewId") { Value = paperViewId },
                 new SQLiteParameter("$PaperScoreID") { Value = paperScoreID },
                 new SQLiteParameter("$SSOUId") { Value = ssoUid });
            if (dt == null) return new List<ViewStudentQuestion>();
            var lastQuestionId = 0;
            var lastQuestionType = string.Empty;
            var mainId = 1;
            var lst = new List<ViewStudentQuestion>();
            foreach (DataRow dr in dt.Rows)
            {
                var qid = dr.Field<int>("QuestionId");
                var qtype = dr.Field<string>(dr["partName"] != DBNull.Value ? "partName" : "paperTypeName");
                if (qtype != lastQuestionType)
                {
                    lastQuestionType = qtype;
                    mainId = 1;
                }
                if (lastQuestionId != qid)
                {
                    lastQuestionId = qid;
                    var item = new ViewStudentQuestion
                    {
                        QuestionTypeName = dr.Field<string>("paperTypeName"),
                        QuestionId = dr.Field<int>("QuestionId"),
                        Analysis = Crypt.Rc4DecryptString(dr.Field<string>("Analysis")),
                        Answer = Crypt.Rc4DecryptString(dr.Field<string>("Answer")),
                        Content = Crypt.Rc4DecryptString(dr.Field<string>("Content")),
                        ParentId = dr.Field<int>("ParentId"),
                        QuesTypeId = dr.Field<int>("quesTypeId"),
                        QuesViewType = dr.Field<int>("quesViewType"),
                        Score = dr.Field<double>("score"),
                        UserAnswer = dr.Field<string>("UserAnswer"),
                        UserScore = double.Parse(dr.Field<string>("UserScore")),
                        Sequence = dr.Field<int>("Sequence")
                    };

                    if (item.ParentId > 0)
                    {
                        item.Sequence = item.ParentId * 100 + item.Sequence;
                    }
                    if (dr["partName"] != DBNull.Value)
                    {
                        item.PartName = dr.Field<string>("partName");
                        item.PartSequence = dr.Field<int>("PSeq");
                    }
                    else
                    {
                        item.PartName = item.QuestionTypeName;
                        //item.PartSequence = 1000000 + item.QuesTypeId;
                        //防止出现题号错乱的问题 dgh 2017.08.22
                        var dtType = ExecuteTable(sqlType, new SQLiteParameter("$paperViewId") { Value = paperViewId });
                        var dtf = dtType.DefaultView;
                        if (dtf==null||dtf.ToTable().Rows.Count == 0)
                        {
                            item.PartSequence = 1000000 + item.QuesTypeId;
                        }
                        else
                        {
                            dtf.RowFilter = "partName='" + item.QuestionTypeName + "'";
                            if (dtf.Count != 0)
                            {
                                item.PartSequence = Int32.Parse(dtf[0]["PSeq"].ToString());
                            }
                            else
                            {
                                item.PartSequence = 1000000 + item.QuesTypeId;
                            }
                        }
                       
                    }
                    var va = dr.Field<int?>("IsDone");
                    item.IsDone = va.HasValue && va.Value == 1;
                    va = dr.Field<int?>("IsWrong");
                    item.IsWrong = va.HasValue && va.Value == 1;
                    va = dr.Field<int?>("IsFav");
                    item.IsFav = va.HasValue && va.Value == 1;

                    if (item.ParentId != 0)
                    {
                        item.Parent = lst.FirstOrDefault(x => x.QuestionId == item.ParentId);
                        if (item.Parent != null)
                        {
                            item.MainId = item.Parent.MainId;
                            item.QuestionTypeName = item.Parent.QuestionTypeName;
                            item.QuesViewType = item.Parent.QuesViewType;
                            item.Parent.SubCnt++;
                            item.SubId = item.Parent.SubId + item.Parent.SubCnt;
                            item.PartName = item.Parent.PartName;
                            item.PartSequence = item.Parent.PartSequence;
                        }
                    }
                    else
                    {
                        item.MainId = mainId++;
                        item.SubId = 0;
                        item.SubCnt = 0;
                    }
                    if (dr["OpSequence"] != DBNull.Value)
                    {
                        item.OptionList.Add(new StudentQuestionOption
                        {
                            QuesOption = Crypt.Rc4DecryptString(dr.Field<string>("quesOption")),
                            QuestionId = dr.Field<int>("questionId"),
                            QuesValue = dr.Field<string>("quesValue"),
                            Sequence = dr.Field<int>("OpSequence")
                        });
                    }
                    lst.Add(item);
                }
                else
                {
                    var item = lst.First(x => x.QuestionId == qid);
                    item.OptionList.Add(new StudentQuestionOption
                    {
                        QuesOption = Crypt.Rc4DecryptString(dr.Field<string>("quesOption")),
                        QuestionId = dr.Field<int>("questionId"),
                        QuesValue = dr.Field<string>("quesValue"),
                        Sequence = dr["OpSequence"] == DBNull.Value ? 0 : dr.Field<int>("OpSequence")
                    });
                }
            }
            return lst.Where(x => !lst.Exists(y => y.ParentId == x.QuestionId))
                .OrderBy(x => x.PartSequence)
                .ThenBy(x => x.MainId)
                .ThenBy(x => x.SubId)
                .ToList();
        }


		public bool CheckPaperDetailExists(int paperViewId)
		{
			var obj = ExecuteScalar("Select Count(*) From StudentQuestion Where PaperViewId=$PaperViewId", new SQLiteParameter("$PaperViewId") { Value = paperViewId });
			if (obj == null) return false;
			return (long)obj > 0;
		}

		public List<ViewStudentCenter> GetCenterList()
		{
			var dt = ExecuteTable(GetCenterListSql_withUpdateTime);
			if (dt == null || dt.Rows.Count == 0) return new List<ViewStudentCenter>();
			return dt.AsEnumerable().Select(x =>
			{
				var item = new ViewStudentCenter
				{
					SiteCourseId = x.Field<int>("SiteCourseId"),
					SiteCourseName = x.Field<string>("SiteCourseName"),
					CenterId = x.Field<int>("CenterId"),
                    CenterName = x.Field<string>("CenterName"),
                    // 取CenterList值的时候，带上UpdateTime字段，用于在界面上显示，@author ChW，@date 2021-05-17
                    UpdateTime = x.Field<string>("UpdateTime")
                };
				var va = x.Field<long?>("AllCnt");
				item.AllCnt = va.HasValue ? (int)va.Value : 0;
				va = x.Field<long?>("PCnt");
				if (va.HasValue) item.AllCnt -= (int)va.Value;
				va = x.Field<long?>("FavCnt");
				item.FavCnt = va.HasValue ? (int)va.Value : 0;
				va = x.Field<long?>("DoCnt");
				item.DoCnt = va.HasValue ? (int)va.Value : 0;
				va = x.Field<long?>("WrongCnt");
				item.WrongCnt = va.HasValue ? (int)va.Value : 0;
				return item;
			}).ToList();
		}

		public List<ViewStudentPaper> GetCenterPaperList(int centerId)
		{
			var dt = ExecuteTable(GetCenterPaperListSql, new SQLiteParameter("$centerid") { Value = centerId });
			if (dt == null || dt.Rows.Count == 0) return new List<ViewStudentPaper>();
			return dt.AsEnumerable().Select(x =>
			{
                var item = new ViewStudentPaper
                {
                    Version = x.Field<int>("Version"),
					PaperViewId = x.Field<int>("PaperViewId"),
					PaperViewName = x.Field<string>("PaperViewName"),
                    ContestTimes = string.IsNullOrWhiteSpace(x.Field<string>("ContestTimes")) ? "0" : x.Field<string>("ContestTimes")
				};
				var va = x.Field<long?>("AllCnt");
				item.AllCnt = va.HasValue ? (int)va.Value : 0;
				va = x.Field<long?>("PCnt");
				if (va.HasValue && item.AllCnt > 0) item.AllCnt -= (int)va.Value;
				va = x.Field<long?>("FavCnt");
				item.FavCnt = va.HasValue ? (int)va.Value : 0;
				va = x.Field<long?>("DoCnt");
				item.DoCnt = item.AllCnt - (va.HasValue ? (int)va.Value : 0);
				va = x.Field<long?>("WrongCnt");
				item.WrongCnt = va.HasValue ? (int)va.Value : 0;
				return item;
			}).ToList();
		}

		public bool SaveUserResult(int ssoUid, IEnumerable<ViewStudentQuestionResult> list)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				var cmd = new SQLiteCommand
				{
					Connection = Conn,
					Transaction = tran
				};
				foreach (var item in list)
				{
					cmd.Parameters.Clear();
					bool isDone, isWrong;
                    #region 屏蔽原来的
                    //switch (item.IsRight)
                    //{
                    //    case 0:
                    //        isDone = false;
                    //        isWrong = false;
                    //        break;
                    //    case 1:
                    //        isDone = true;
                    //        isWrong = false;
                    //        break;
                    //    default:
                    //        isDone = true;
                    //        isWrong = true;
                    //        break;
                    //}
                    #endregion
                    
                    switch (item.IsRight)
                    {
                        case 0:
                            isWrong = false;
                            break;
                        case 1:
                            isWrong = false;
                            break;
                        default:
                            isWrong = true;
                            break;
                    }
                    //不管什么题型，只要用户写了答案就算做过 dgh 2016.06.20
                    isDone = !string.IsNullOrEmpty(item.UserAnswer);
					cmd.CommandText =
						"Select IsFav From StudentQuestionExtend Where SSOUId = $SSOUId And PaperVIewId=$PaperVIewId And QuestionId = $QuestionId";
					cmd.Parameters.Add(new SQLiteParameter("$SSOUId") { Value = ssoUid });
					cmd.Parameters.Add(new SQLiteParameter("$PaperVIewId") { Value = item.PaperViewId });
					cmd.Parameters.Add(new SQLiteParameter("$QuestionId") { Value = item.QuestionId });
					var obj = cmd.ExecuteScalar();
					if (obj != null && obj != DBNull.Value)
					{
						cmd.Parameters.Clear();
						cmd.Parameters.Add(new SQLiteParameter("$SSOUId") { Value = ssoUid });
						cmd.Parameters.Add(new SQLiteParameter("$PaperVIewId") { Value = item.PaperViewId });
						cmd.Parameters.Add(new SQLiteParameter("$QuestionId") { Value = item.QuestionId });
						cmd.Parameters.Add(new SQLiteParameter("$IsFav") { Value = item.IsFav ? 1 : 0 });
						string upParam = string.Empty;
						if (isDone)
						{
							upParam = ",IsDone = $IsDone";
							cmd.Parameters.Add(new SQLiteParameter("$IsDone") { Value = 1 });
						}
						if (isWrong)
						{
							upParam += ",IsWrong = $IsWrong";
							cmd.Parameters.Add(new SQLiteParameter("$IsWrong") { Value = 1 });
						}
						cmd.CommandText = "Update StudentQuestionExtend set IsFav=$IsFav" + upParam +
										  " Where SSOUId = $SSOUId And PaperVIewId=$PaperVIewId And QuestionId = $QuestionId";
						cmd.ExecuteNonQuery();
					}
					else
					{
						cmd.Parameters.Clear();
						cmd.CommandText =
								"Insert Into StudentQuestionExtend(SSOUId,PaperVIewId,QuestionId,IsDone,IsWrong,IsFav) Values($SSOUId,$PaperVIewId,$QuestionId,$IsDone,$IsWrong,$IsFav)";
						cmd.Parameters.Add(new SQLiteParameter("$SSOUId") { Value = ssoUid });
						cmd.Parameters.Add(new SQLiteParameter("$PaperVIewId") { Value = item.PaperViewId });
						cmd.Parameters.Add(new SQLiteParameter("$QuestionId") { Value = item.QuestionId });
						cmd.Parameters.Add(new SQLiteParameter("$IsDone") { Value = isDone ? 1 : 0 });
						cmd.Parameters.Add(new SQLiteParameter("$IsWrong") { Value = isWrong ? 1 : 0 });
						cmd.Parameters.Add(new SQLiteParameter("$IsFav") { Value = item.IsFav ? 1 : 0 });
						cmd.ExecuteNonQuery();
					}
					if (isWrong)
					{
						cmd.Parameters.Clear();
						cmd.CommandText =
							"Insert into StudentQuestionRecord(SSOUId,PaperVIewId,QuestionId,UserAnswer,SaveTime) Values($SSOUId,$PaperVIewId,$QuestionId,$UserAnswer,$SaveTime)";
						cmd.Parameters.Add(new SQLiteParameter("$SSOUId") { Value = ssoUid });
						cmd.Parameters.Add(new SQLiteParameter("$PaperVIewId") { Value = item.PaperViewId });
						cmd.Parameters.Add(new SQLiteParameter("$QuestionId") { Value = item.QuestionId });
						cmd.Parameters.Add(new SQLiteParameter("$UserAnswer") { Value = item.UserAnswer });
						cmd.Parameters.Add(new SQLiteParameter("$SaveTime") { Value = Util.GetNow() });
						cmd.ExecuteNonQuery();
					}
				}
				tran.Commit();
				return true;
			}
			catch (Exception ex)
			{
				Log.RecordLog(ex.ToString());
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

		public bool SetQuestionResolved(int ssoUid, int paperViewId, int questionId)
		{
			ExecuteNonQuery(
				"Update StudentQuestionExtend set IsWrong = 0 Where SSOUId = $SSOUId And PaperVIewId=$PaperVIewId And QuestionId = $QuestionId",
				new SQLiteParameter("$SSOUId") { Value = ssoUid },
				new SQLiteParameter("$PaperVIewId") { Value = paperViewId },
				new SQLiteParameter("$QuestionId") { Value = questionId });
			return ExecuteNonQuery(
				"Delete From StudentQuestionRecord Where SSOUId = $SSOUId And PaperVIewId=$PaperVIewId And QuestionId = $QuestionId",
				new SQLiteParameter("$SSOUId") { Value = ssoUid },
				new SQLiteParameter("$PaperVIewId") { Value = paperViewId },
				new SQLiteParameter("$QuestionId") { Value = questionId }) >= 0;
		}

		public List<ViewStudentQuestionRecord> GetStudentQuestionRecord(int ssoUid, int paperViewId, int questionId)
		{
			var dt =
				ExecuteTable(
					"Select UserAnswer,SaveTime From StudentQuestionRecord Where SSOUId = $SSOUId And PaperVIewId=$PaperVIewId And QuestionId = $QuestionId",
					new SQLiteParameter("$SSOUId") { Value = ssoUid },
					new SQLiteParameter("$PaperVIewId") { Value = paperViewId },
					new SQLiteParameter("$QuestionId") { Value = questionId });
			if (dt == null) return new List<ViewStudentQuestionRecord>();
			return dt.AsEnumerable().Select(x => new ViewStudentQuestionRecord
			{
				SaveTime = x.Field<DateTime>("SaveTime"),
				UserAnswer = x.Field<string>("UserAnswer")
			}).ToList();
		}

		public void ClearCenter()
		{
			ExecuteNonQuery("Delete From StudentSiteCourse");
			ExecuteNonQuery("Delete From StudentCenter");
			//ExecuteNonQuery("Delete From StudentChapter");
			//ExecuteNonQuery("Delete From StudentChapterList");
		}

		/**
		 * 向数据库刷新当前CenterId对应的题库的更新时间
		 * @param CenterId 练习集id
		 * @author ChW
		 * @date 2021-05-14
		 */
		public bool UpdateCenterSubjectUpdateTime(int CenterId, string updateTime /*int eduSubjectId, IEnumerable<StudentPaperCenter> list*/)
		{
			OpenConn();
			var tran = Conn.BeginTransaction();
			try
			{
				string sql = "Update StudentCenterSubject Set UpdateTime = $UpdateTime Where CenterId = $CenterId";
				var cmd = new SQLiteCommand(sql, Conn) { Transaction = tran };
				cmd.Parameters.Add(new SQLiteParameter("$CenterId") { Value = CenterId });
				cmd.Parameters.Add(new SQLiteParameter("$UpdateTime") { Value = updateTime });
				cmd.ExecuteNonQuery();
				tran.Commit();
				return true;
			}
			catch (Exception ex)
			{
				Log.RecordLog(ex.ToString());
				tran.Rollback();
				return false;
			}
			finally
			{
				CloseConn();
			}
		}

#if CHINAACC
		public List<SubjectClassify> GetSubjectClassify()
		{
			var dt = ExecuteTable("Select SubjectClassifyId,SubjectName,TreeType,ParentId From StudentSubjectClassify");
			if (dt == null) return new List<SubjectClassify>();
			return dt.AsEnumerable().Select(x => new SubjectClassify
			{
				ParentId = x.Field<int>("ParentId"),
				TreeType = x.Field<int>("TreeType"),
				SubjectName = x.Field<string>("SubjectName"),
				SubjectClassifyId = x.Field<int>("SubjectClassifyId")
			}).ToList();
		}
#endif
        /// <summary>
        /// 获取做题记录信息
        /// </summary>
        /// <param name="ssoUid">用户ID</param>
        /// <param name="paperViewId">对外试卷ID</param>
        /// <returns></returns>
        internal List<StudentPaperScore> GetPaperSocres(int ssoUid, int paperViewId)
        {
            const string sql = "select PaperScoreID,PaperViewID,ifnull(AutoScore,'0') as AutoScore,ifnull(LastScore,'0') as LastScore,ifnull(SpendTime,'0') as SpendTime,CreateTime from StudentPaperScores where SSOUID=$SSOUID and PaperViewID=$PaperViewID order by CreateTime desc";
            var dt = ExecuteTable(sql, new SQLiteParameter("$SSOUId") { Value = ssoUid },
                    new SQLiteParameter("$PaperViewID") { Value = paperViewId });
            if (dt == null) return new List<StudentPaperScore>();
            return dt.AsEnumerable().Select(x => new StudentPaperScore
            {
                PaperScoreID = x.Field<string>("PaperScoreID"),
                PaperViewID = x.Field<int>("PaperViewID"),
                AutoScore = x.Field<string>("AutoScore"),
                LastScore = x.Field<string>("LastScore"),
                SpendTime = x.Field<string>("SpendTime"),
                CreateTime = x.Field<DateTime>("CreateTime").ToString("yyyy-MM-dd HH:mm")
            }).ToList();
        }
        /// <summary>
        /// 获取7天内的做题记录ID
        /// </summary>
        /// <param name="ssoUid">用户ID</param>
        /// <returns></returns>
        internal string GetPaperScoreIds(int ssoUid)
        {
            const string sql = "select PaperScoreID from StudentPaperScores where date(CreateTime)>=date('now', '-7 day') and SSOUID=$SSOUID";
            var dt = ExecuteTable(sql, new SQLiteParameter("$SSOUId") { Value = ssoUid });
            if (dt == null || dt.Rows.Count == 0) return "";
            IEnumerable<string> paperScoreIds = dt.AsEnumerable().Select(x => x.Field<string>("PaperScoreID")).ToArray();
            string paperScoreIdsStr = string.Join(",", paperScoreIds);
            return paperScoreIdsStr;
        }
        /// <summary>
        /// 获取记录总数 dgh 2017.08.11
        /// </summary>
        /// <param name="ssoUid">用户ID</param>
        /// <returns></returns>
        public int GetPaperScoreCount(int ssoUid)
        {
            const string sql = "select count(PaperScoreID)+1 from StudentPaperScores where SSOUID=$SSOUID";
            var obj = ExecuteScalar(sql, new SQLiteParameter("$SSOUId") { Value = ssoUid });
            return Convert.ToInt32(obj);
        }
	}
}
