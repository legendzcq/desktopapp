using System;
using System.Diagnostics;

namespace Framework.Local
{
	public class DatabaseUpdate : DataAccessBase
	{
		public void UpdateDatabase()
		{
			int ver = GetDbVer();
			if (ver < 1)
			{
				DoUpdate("Alter Table StudentCourseWare Add ClassOrder int");
				DoUpdate("Alter Table StudentCourseWare Add CwareClassID int");
				DoUpdate("Update StudentCourseWare Set CwareClassID = 0, ClassOrder = 0");
				DoUpdate("Update DbVer Set Ver = 1");
			}
			if (ver < 2)
			{
				DoUpdate("Alter Table StudentQuestion Add Sequence int");
				DoUpdate("Update StudentQuestion Set Sequence = 0");
				DoUpdate("Update DbVer Set Ver = 2");
			}
			if (ver < 3)
			{
				DoUpdate("Alter Table StudentCourseWare Add CwareTitle NVARCHAR");
				DoUpdate("Update StudentCourseWare Set CwareTitle=''");
				DoUpdate("Update DbVer Set Ver = 3");
			}
			if (ver < 4)
			{
				DoUpdate("CREATE TABLE [StudentSubjectClassify] ([SubjectClassifyId] INT,[SubjectName] NVARCHAR, [TreeType] INT, [ParentId] INT, CONSTRAINT [] PRIMARY KEY ([SubjectClassifyId]));");
				DoUpdate("CREATE TABLE [StudentCwareKcjyDown] ([SmallListId] INT, [SmallListName] NVARCHAR, [CwareId] INT, [JiangyiFile] NVARCHAR, [SmallOrder] INT, CONSTRAINT [] PRIMARY KEY ([SmallListId]));");
				DoUpdate("Delete From [StudentCourseWare]");
				DoUpdate("Update DbVer Set Ver = 4");
			}
			if (ver < 5)
			{
				DoUpdate("Delete From [StudentCourseDetail]");
				DoUpdate("Alter Table StudentCourseDetail Add VideoType INT");
				DoUpdate("Alter Table StudentCourseDetail Add PointId INT");
				DoUpdate("Alter Table StudentCourseDetail Add PointName NVARCHAR");
				DoUpdate("Update DbVer Set Ver = 5");
			}
			if (ver < 6)
			{
				DoUpdate("CREATE TABLE [PushMessage] ([Id] INT, [Type] INT, [Content] NVARCHAR, [PushTime] DATETIME, CONSTRAINT [] PRIMARY KEY ([Id]));");
				DoUpdate("Update DbVer Set Ver = 6");
			}
			if (ver < 7)
			{
				DoUpdate("Delete From StudentCourse;");
				DoUpdate("Alter Table StudentCourse Add CourseEduId INT;");
				DoUpdate("Alter Table StudentCourse Add EduSubjectId INT;");
				DoUpdate("Alter Table StudentCourse Add Disporder INT;");
				DoUpdate("Update DbVer Set Ver = 7");
			}
			if (ver < 8)
			{
				DoUpdate("CREATE TABLE [StudentCwareDownload] ([DownID] INTEGER PRIMARY KEY AUTOINCREMENT, [CwareID] INT, [VideoID] NVARCHAR, [DownUrl] NVARCHAR, [LocalFile] NVARCHAR, [DownState] INT, [DownType] INT, [DownOrder] INT, [ForImport] INT);");
				DoUpdate("Alter Table StudentCourseWare Add CwareKey NVARCHAR");
				DoUpdate("CREATE TABLE [PointTestStartTime] ([CwareId] INT, [VideoId] NVARCHAR, [PointName] NVARCHAR, [TestId] INT, [PointTestStartTime] INT, [PointOpenType] NVARCHAR);");
				DoUpdate("CREATE INDEX [IdxCwareIdVideoId] ON [PointTestStartTime] ([CwareId], [VideoId]);");
				DoUpdate("CREATE TABLE [PointTestQuestion] ([QuestionId] INT, [ParentId] INT, [PointTestId] INT, [QuesViewType] INT, [Content] NVARCHAR, [Score] DOUBLE, [RightAnswer] NVARCHAR, [Analysis] NVARCHAR, [SplitScore] DOUBLE, [PointId] INT, [QuesType] INT);");
				DoUpdate("CREATE INDEX [Idx_PointTestQuestionTestId] ON [PointTestQuestion] ([PointTestId]);");
				DoUpdate("CREATE TABLE [PointTestQuestionOption] ([QuestionId] INT, [QuesValue] NVARCHAR, [QuesOption] NVARCHAR, [Sequence] INT);");
				DoUpdate("CREATE INDEX [IdxQuestionId] ON [PointTestQuestionOption] ([QuestionId]);");
				DoUpdate("Delete From [StudentCourseDetail];");
				DoUpdate("Alter Table [StudentCourseDetail] Add AudioHDZipUrl NVARCHAR;");
				DoUpdate("Alter Table [StudentCourseDetail] Add OrderBy INT;");
				DoUpdate("Drop Table [StudentCenter]");
				DoUpdate("CREATE TABLE [StudentCenter] ([siteCourseID] INT, [centerID] INT NOT NULL, [centerName] NVARCHAR, [centerYear] NVARCHAR, [centerType] INT, [openStatus] NVARCHAR, [centerParam] NVARCHAR, UNIQUE([siteCourseID], [centerID]));");
				DoUpdate("CREATE TABLE [StudentEduSubject] ([EduSubjectId] INT, [EduSubjectName] NVARCHAR, [BoardId] INT, [OrderNo] INT, [CourseEduId] INT, [Disporder] INT, [CourseId] NVARCHAR, CONSTRAINT [] PRIMARY KEY ([EduSubjectId]));");
				DoUpdate("CREATE TABLE [StudentSubjectCourseRelation] ([EduSubjectId] INT, [CourseId] NVARCHAR);");
				DoUpdate("CREATE TABLE [StudentCware] ([CwareId] INT, [CwId] NVARCHAR, [EduSubjectId] INT, [CwareTitle] NVARCHAR, [CYearName] NVARCHAR, [CwareName] NVARCHAR, [TeacherName] NVARCHAR, [DateEnd] DATETIME, [CwareClassId] INT, [CwareClassName] NVARCHAR, [ClassOrder] INT, [MobileCourseOpen] INT, [Download] NVARCHAR, [VideoType] INT, [CwareUrl] NVARCHAR, [UpdateTime] DATETIME, [Rownum] INT, [CwareImg] NVARCHAR, [UseFul] INT, [BoardId] INT, CONSTRAINT [] PRIMARY KEY ([CwareId], [EduSubjectId]));");
				DoUpdate("Update DbVer Set Ver = 8");
			}
			if (ver < 9)
			{
				DoUpdate("Delete From [PointTestStartTime];");
				DoUpdate("Alter Table [PointTestStartTime] Add BackTime INT;");
				DoUpdate("Update DbVer Set Ver = 9");
			}
			if (ver < 10)
            {
				//// 新增UpdateTime字段，用于在本地显示CenterId对应的题库的更新时间，@author ChW，@date 2021 - 05 - 17
				//DoUpdate("CREATE TABLE [StudentCenterSubject] ([EduSubjectId] INT, [CenterId] INT, [CenterType] INT, [Creator] INT, [CenterYear] INT, [OpenStatus] INT, [CenterName] NVARCHAR, [Description] NVARCHAR, [Sequence] INT, [SiteCourseId] INT, [CenterParam] NVARCHAR,[CreateTime] NVARCHAR,[UpdateTime] NVARCHAR, CONSTRAINT [] PRIMARY KEY ([EduSubjectId], [CenterId]));");
				DoUpdate("CREATE TABLE [StudentCenterSubject] ([EduSubjectId] INT, [CenterId] INT, [CenterType] INT, [Creator] INT, [CenterYear] INT, [OpenStatus] INT, [CenterName] NVARCHAR, [Description] NVARCHAR, [Sequence] INT, [SiteCourseId] INT, [CenterParam] NVARCHAR,[CreateTime] NVARCHAR, CONSTRAINT [] PRIMARY KEY ([EduSubjectId], [CenterId]));");
				DoUpdate("Update DbVer Set Ver = 10");
			}
			if (ver < 11)
			{
				DoUpdate("Drop Table [StudentQuestion]");
				DoUpdate("CREATE TABLE [StudentQuestion] (  [paperViewID] INT, [questionID] INT NOT NULL, [parentID] INT, [quesTypeID] INT, [quesViewType] INT, [content] NVARCHAR, [answer] NVARCHAR, [analysis] NVARCHAR, [limitMinute] INT, [score] DOUBLE, [splitScore] DOUBLE, [status] INT, [lecture] NVARCHAR, [creator] NVARCHAR, [createTime] DATETIME, [modifyStatus] INT, [wrongRate] NVARCHAR, [Sequence] int, CONSTRAINT [] PRIMARY KEY ([questionID], [paperViewID]));");
				DoUpdate("Delete From [StudentPaperView];");
				DoUpdate("Update DbVer Set Ver = 11");
			}
			if (ver < 12)
			{
				DoUpdate("CREATE TABLE [StudentCwareSetting] ( [Uid] INT NOT NULL, [EduSubjectId] INT NOT NULL, [CwareId] INT NOT NULL, [UserHide] INT NOT NULL, [UserShowOrder] INT NOT NULL, CONSTRAINT [] PRIMARY KEY ([Uid], [EduSubjectId], [CwareId]));");
				DoUpdate("Update DbVer Set Ver = 12");
			}
            if (ver < 13)
            {
                DoUpdate("Alter Table [StudentCourseDetail] Add modTime NVARCHAR");
                DoUpdate("Alter Table [StudentCWareDown] Add modTime NVARCHAR");
                DoUpdate("Alter Table [StudentCwareDownload] Add modTime NVARCHAR");
                DoUpdate("Update DbVer Set Ver = 13");
            }
            if (ver < 14)
            {
                DoUpdate("CREATE TABLE [StudentPaperScores] ([SSOUID] INT, [PaperScoreID] INT, [PaperViewID] INT, [SiteCourseID] NVARCHAR, [AutoScore] NVARCHAR, [LastScore] NVARCHAR, [PaperScore] NVARCHAR, [SpendTime] NVARCHAR, [CenterID] INT, [CreateTime] DATETIME, CONSTRAINT [sqlite_autoindex_StudentPaperScores_1] PRIMARY KEY ([SSOUID], [PaperScoreID]));");
                DoUpdate("CREATE TABLE [StudentPaperScoresAnswer] ([SSOUID] INT, [PaperScoreID] NVARCHAR, [QuestionID] INT, [UserAnswer] NVARCHAR, [UserScore] NVARCHAR, CONSTRAINT [sqlite_autoindex_StudentPaperScoresAnswer_1] PRIMARY KEY ([PaperScoreID], [QuestionID]));");
                DoUpdate("Update DbVer Set Ver = 14");
            }
            if (ver < 15)
            {
                DoUpdate("CREATE TABLE [StudentPaperScores1] ([SSOUID] INT, [PaperScoreID] NVARCHAR, [PaperViewID] INT, [SiteCourseID] NVARCHAR, [AutoScore] NVARCHAR, [LastScore] NVARCHAR, [PaperScore] NVARCHAR, [SpendTime] NVARCHAR, [CenterID] INT, [CreateTime] DATETIME, CONSTRAINT [sqlite_autoindex_StudentPaperScores_1] PRIMARY KEY ([SSOUID], [PaperScoreID]))");
                DoUpdate("insert into StudentPaperScores1 select * from StudentPaperScores");
                DoUpdate("drop table StudentPaperScores");
                DoUpdate("alter table StudentPaperScores1 rename to StudentPaperScores");
                DoUpdate("Update DbVer Set Ver = 15");
            }

            if (ver < 16)
            {
                //添加字段isFinish：1表示该课程已经听完过，0未听完过
                DoUpdate("Alter Table [StudentVideoRecord] Add MaxLastPosition INT");
                DoUpdate("update StudentVideoRecord set MaxLastPosition=LastPosition");
                DoUpdate("Update DbVer Set Ver = 16");
            }
            if (ver < 17)
            {
                DoUpdate("CREATE TABLE [StudentVideoTimebase] ([SSOUID] INT, [CwareID] INT, [VideoID] NVARCHAR, [videoStartTime] INT, [videoEndTime] INT,speed NVARCHAR,studyTimeEnd NVARCHAR,studyTimeStart NVARCHAR,[LastTime] DATETIME);");
                DoUpdate("CREATE INDEX [pk_timebase] ON [StudentVideoTimebase] ([SSOUID], [CwareID], [VideoID], [LastTime]);");
                DoUpdate("Update DbVer Set Ver = 17");
            }
            if (ver < 18)
            {
                // 调整字段的新增方式，2021-07-20（新增UpdateTime字段，用于在本地显示CenterId对应的题库的更新时间，@author ChW，@date 2021 - 05 - 17）
                DoUpdate("ALTER TABLE StudentCenterSubject ADD COLUMN UpdateTime NVARCHAR");
                DoUpdate("Update DbVer Set Ver = 18");
            }
			if (ver < 19)
            {
				// 新增IsFree字段，用于在课程列表中区分“已购课程”和“赠送课程”
				DoUpdate("Alter Table StudentCware Add COLUMN IsFree INT");
				DoUpdate("Update DbVer Set Ver = 19");
			}
			if (ver < 20)
			{
				// 新增Version字段，用于修复题库提交到web线上答案不显示的Bug
				DoUpdate("Alter Table StudentPaperView Add COLUMN Version INT DEFAULT 1");
				DoUpdate("Update DbVer Set Ver = 20");
			}
		}

		private void DoUpdate(string sql)
		{
			try
			{
				ExecuteNonQuery(sql);
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
		}

		private int GetDbVer()
		{
			int ver = 0;
			try
			{
				var verobj = ExecuteScalar("Select Ver From DbVer");
				if (verobj != null && verobj != DBNull.Value)
				{
					ver = (int)verobj;
				}
			}
			catch (Exception ex)
			{
				Trace.WriteLine(ex.ToString());
			}
			return ver;
		}
	}
}
