using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using Framework.Model;
using Framework.NewModel;
using Framework.Utility;

using StudentCourse = Framework.Model.StudentCourse;

namespace Framework.Local
{
    public class StudentWareData : DataAccessBase
    {
        #region 从线上获取到的数据，保存到本地

        private const string AddCourseSql = "insert or replace into StudentCourse(CourseId,Name,EndDate,BoardId,CourseEduId,EduSubjectId,Disporder) values($CourseId,$Name,$EndDate,$BoardId,$CourseEduId,$EduSubjectId,$Disporder)";
        private const string AddCourseWareSql = "INSERT or REPLACE into StudentCourseWare(CourseId,CwareId,CwId,PathUrl,Name,CwareClassName,cYearName,CTeacherName,CwareImg,VideoType,IsOpen,ClassOrder,CwareClassId,CwareTitle) values($CourseId,$CwareId,$CwId,$PathUrl,$Name,$CwareClassName,$cYearName,$CTeacherName,$CwareImg,$VideoType,$IsOpen,$ClassOrder,$CwareClassId,$CwareTitle)";
        private const string AddCourseWareDetailSql = "INSERT or REPLACE INTO StudentCourseDetail ([CWareId],[VideoId],[ChapterId],[DemoType],[VideoUrl],[AudioUrl],[Length],[Title],[VideoName],[VideoZipUrl],[AudioZipUrl],[VideoHDZipUrl],[VideoHDUrl],VideoType,PointID,PointName,OrderBy,modTime) VALUES ($CWareId,$VideoId,$ChapterId,$DemoType,$VideoUrl,$AudioUrl,$Length,$Title,$VideoName,$VideoZIpUrl,$AudioZipUrl,$VideoHDZIpUrl,$VideoHDUrl,$VideoType,$PointID,$PointName,$OrderBy,$modTime);";
        private const string AddCourseWareChapterSql = "INSERT or REPLACE INTO StudentCwareChapter ([CWareId],[ChapterId],[ChapterName],[Order]) VALUES ($CWareId,$ChapterId,$ChapterName,$Order)";
        private const string AddTimeNodeSql = "INSERT or REPLACE INTO StudentTimeNode ([CWareId],[VideoId],[NodeId],[Timestart],[TimeEnd],[FlashUrl]) VALUES ($CWareId,$VideoId,$NodeId,$Timestart,$TimeEnd,$FlashUrl)";
        private const string AddCwareKcjySql = "INSERT or REPLACE INTO StudentCwareKcjy ([CWareId],[VideoId],[NodeId],[TimeStart],[VideoTime],[NodeText]) VALUES ($CWareId,$VideoId,$NodeId,$TimeStart,$VideoTime,$NodeText);";
        private const string AddKcjyDownSql = "INSERT or REPLACE INTO StudentCwareKcjyDown(SmallListId,SmallListName,CwareId,JiangyiFile,SmallOrder) Values($SmallListId,$SmallListName,$CwareId,$JiangyiFile,$SmallOrder)";

        /// <summary>
        /// 添加课程列表
        /// </summary>
        /// <returns></returns>
        internal bool AddCourseList(IEnumerable<StudentCourse> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentCourse item in list)
                {
                    var cmd = new SQLiteCommand(AddCourseSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$BoardId") { Value = item.BoardId });
                    cmd.Parameters.Add(new SQLiteParameter("$CourseId") { Value = item.CourseId });
                    cmd.Parameters.Add(new SQLiteParameter("$EndDate") { Value = item.EndDate });
                    cmd.Parameters.Add(new SQLiteParameter("$Name") { Value = item.Name });
                    cmd.Parameters.Add(new SQLiteParameter("$CourseEduId") { Value = item.CourseEduId });
                    cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = item.EduSubjectId });
                    cmd.Parameters.Add(new SQLiteParameter("$Disporder") { Value = item.Disporder });
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
        /// 添加课件列表
        /// </summary>
        /// <returns></returns>
        internal bool AddCourseWareList(IEnumerable<StudentCourseWare> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentCourseWare item in list)
                {
                    var cmd = new SQLiteCommand(AddCourseWareSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$CourseId") { Value = item.CourseId });
                    cmd.Parameters.Add(new SQLiteParameter("$CWareClassName") { Value = item.CWareClassName });
                    cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = item.CWareId });
                    cmd.Parameters.Add(new SQLiteParameter("$CWareImg") { Value = item.CWareImg });
                    cmd.Parameters.Add(new SQLiteParameter("$CwId") { Value = item.CwId });
                    cmd.Parameters.Add(new SQLiteParameter("$cYearName") { Value = item.CYearName });
                    cmd.Parameters.Add(new SQLiteParameter("$IsOpen") { Value = item.IsOpen });
                    cmd.Parameters.Add(new SQLiteParameter("$Name") { Value = item.Name });
                    cmd.Parameters.Add(new SQLiteParameter("$PathURL") { Value = item.PathUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoType") { Value = item.VideoType });
                    cmd.Parameters.Add(new SQLiteParameter("$CTeacherName") { Value = item.CTeacherName });
                    cmd.Parameters.Add(new SQLiteParameter("$ClassOrder") { Value = item.ClassOrder });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareClassId") { Value = item.CwareClassId });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareTitle") { Value = item.CwareTitle });
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

        internal bool AddCourseDetailList(IEnumerable<StudentCourseDetail> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentCourseDetail item in list)
                {
                    var cmd = new SQLiteCommand(AddCourseWareDetailSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$ChapterId") { Value = item.ChapterId });
                    cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = item.CWareId });
                    cmd.Parameters.Add(new SQLiteParameter("$DemoType") { Value = item.DemoType });
                    cmd.Parameters.Add(new SQLiteParameter("$Length") { Value = item.Length });
                    cmd.Parameters.Add(new SQLiteParameter("$Title") { Value = item.Title });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoId") { Value = item.VideoId });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoName") { Value = item.VideoName });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoUrl") { Value = Crypt.Rc4EncryptString(item.VideoUrl) });
                    cmd.Parameters.Add(new SQLiteParameter("$AudioUrl") { Value = Crypt.Rc4EncryptString(item.AudioUrl) });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoZipUrl") { Value = item.VideoZipUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$AudioZipUrl") { Value = item.AudioZipUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoHDZipUrl") { Value = item.VideoHdZipUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoHDUrl") { Value = Crypt.Rc4EncryptString(item.VideoHdUrl) });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoType") { Value = item.VideoType });
                    cmd.Parameters.Add(new SQLiteParameter("$PointId") { Value = string.IsNullOrWhiteSpace(item.StrPointId) ? item.PointId : int.Parse(item.StrPointId.Trim()) });
                    cmd.Parameters.Add(new SQLiteParameter("$PointName") { Value = item.PointName });
                    cmd.Parameters.Add(new SQLiteParameter("$OrderBy") { Value = item.OrderBy });
                    cmd.Parameters.Add(new SQLiteParameter("$modTime") { Value = item.ModTime });
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
        internal bool AddCourseDetailList(int cwareId, IEnumerable<StudentCourseDetail> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentCourseDetail item in list)
                {
                    var cmd = new SQLiteCommand(AddCourseWareDetailSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$ChapterId") { Value = item.ChapterId });
                    cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = cwareId });
                    cmd.Parameters.Add(new SQLiteParameter("$DemoType") { Value = item.DemoType });
                    cmd.Parameters.Add(new SQLiteParameter("$Length") { Value = item.Length });
                    cmd.Parameters.Add(new SQLiteParameter("$Title") { Value = item.Title });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoId") { Value = item.VideoId });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoName") { Value = item.VideoName });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoUrl") { Value = Crypt.Rc4EncryptString(item.VideoUrl) });
                    cmd.Parameters.Add(new SQLiteParameter("$AudioUrl") { Value = Crypt.Rc4EncryptString(item.AudioUrl) });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoZipUrl") { Value = item.VideoZipUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$AudioZipUrl") { Value = item.AudioZipUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoHDZipUrl") { Value = item.VideoHdZipUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoHDUrl") { Value = Crypt.Rc4EncryptString(item.VideoHdUrl) });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoType") { Value = item.VideoType });
                    cmd.Parameters.Add(new SQLiteParameter("$PointId") { Value = string.IsNullOrWhiteSpace(item.StrPointId) ? item.PointId : int.Parse(item.StrPointId.Trim()) });
                    cmd.Parameters.Add(new SQLiteParameter("$PointName") { Value = item.PointName });
                    cmd.Parameters.Add(new SQLiteParameter("$OrderBy") { Value = item.OrderBy });
                    cmd.Parameters.Add(new SQLiteParameter("$modTime") { Value = item.ModTime });
                    //更新课件做测试使用 dgh
                    //cmd.Parameters.Add(new SQLiteParameter("$modTime") { Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") });
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
        internal bool AddCourseDetailItem(StudentCourseDetail item)
        {
            OpenConn();
            try
            {
                var cmd = new SQLiteCommand(AddCourseWareDetailSql, Conn);
                cmd.Parameters.Add(new SQLiteParameter("$ChapterId") { Value = item.ChapterId });
                cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = item.CWareId });
                cmd.Parameters.Add(new SQLiteParameter("$DemoType") { Value = item.DemoType });
                cmd.Parameters.Add(new SQLiteParameter("$Length") { Value = item.Length });
                cmd.Parameters.Add(new SQLiteParameter("$Title") { Value = item.Title });
                cmd.Parameters.Add(new SQLiteParameter("$VideoId") { Value = item.VideoId });
                cmd.Parameters.Add(new SQLiteParameter("$VideoName") { Value = item.VideoName });
                cmd.Parameters.Add(new SQLiteParameter("$VideoUrl") { Value = item.VideoUrl });
                cmd.Parameters.Add(new SQLiteParameter("$AudioUrl") { Value = item.AudioUrl });
                cmd.Parameters.Add(new SQLiteParameter("$VideoZipUrl") { Value = item.VideoZipUrl });
                cmd.Parameters.Add(new SQLiteParameter("$AudioZipUrl") { Value = item.AudioZipUrl });
                cmd.Parameters.Add(new SQLiteParameter("$VideoHDZipUrl") { Value = item.VideoHdZipUrl });
                cmd.Parameters.Add(new SQLiteParameter("$VideoHDUrl") { Value = item.VideoHdUrl });
                cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                CloseConn();
            }
        }

        internal bool AddEduSubjectList(IEnumerable<StudentCourseLIst.StudentEduSubjectItem> list)
        {
            const string sqldelete = "Delete From StudentEduSubject";
            const string sqlAdd = "Insert Or Replace Into StudentEduSubject(EduSubjectId,EduSubjectName,BoardId,OrderNo,CourseEduId,Disporder,CourseId) values($EduSubjectId,$EduSubjectName,$BoardId,$OrderNo,$CourseEduId,$Disporder,$CourseId)";
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                var cmd = new SQLiteCommand(sqldelete, Conn) { Transaction = tran };
                cmd.ExecuteNonQuery();
                foreach (StudentCourseLIst.StudentEduSubjectItem item in list)
                {
                    cmd = new SQLiteCommand(sqlAdd, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$BoardId") { Value = item.BoardId ?? "0" });
                    cmd.Parameters.Add(new SQLiteParameter("$CourseEduId") { Value = item.CourseEduId });
                    cmd.Parameters.Add(new SQLiteParameter("$CourseId") { Value = item.CourseId });
                    cmd.Parameters.Add(new SQLiteParameter("$Disporder") { Value = item.Disporder });
                    cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = item.EduSubjectId });
                    cmd.Parameters.Add(new SQLiteParameter("$EduSubjectName") { Value = item.EduSubjectName });
                    cmd.Parameters.Add(new SQLiteParameter("$OrderNo") { Value = item.OrderNo });
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

        internal bool AddSubjectCourseRelation(IEnumerable<StudentCourseLIst.StudentSubjectCourseRelation> list)
        {
            const string sqldelete = "Delete From StudentSubjectCourseRelation";
            const string sqlAdd = "Insert Or Replace Into StudentSubjectCourseRelation(EduSubjectId,CourseId) Values($EduSubjectId,$CourseId)";
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                var cmd = new SQLiteCommand(sqldelete, Conn) { Transaction = tran };
                cmd.ExecuteNonQuery();
                foreach (StudentCourseLIst.StudentSubjectCourseRelation item in list)
                {
                    cmd = new SQLiteCommand(sqlAdd, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = item.EduSubjectId });
                    cmd.Parameters.Add(new SQLiteParameter("$CourseId") { Value = item.CourseId });
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

        internal bool AddCwareList(int eduSubjectId, IEnumerable<StudentCwareList.StudentCwareItem> list)
        {
            const string sqldelete = "Delete From StudentCware Where EduSubjectId = $EduSubjectId";
            // const string sqlAdd = "Insert Or Replace Into StudentCware(CwareId,CwId,EduSubjectId,CwareTitle,CYearName,CwareName,TeacherName,DateEnd,CwareClassId,CwareClassName,ClassOrder,MobileCourseOpen,Download,VideoType,CwareUrl,UpdateTime,Rownum,CwareImg,UseFul,BoardId) Values($CwareId,$CwId,$EduSubjectId,$CwareTitle,$CYearName,$CwareName,$TeacherName,$DateEnd,$CwareClassId,$CwareClassName,$ClassOrder,$MobileCourseOpen,$Download,$VideoType,$CwareUrl,$UpdateTime,$Rownum,$CwareImg,$UseFul,$BoardId)";
            // 新增了IsFree字段在StudentCware表
            const string sqlAdd = "Insert Or Replace Into StudentCware(CwareId,CwId,EduSubjectId,CwareTitle,CYearName,CwareName,TeacherName,DateEnd,CwareClassId,CwareClassName,ClassOrder,MobileCourseOpen,Download,VideoType,CwareUrl,UpdateTime,Rownum,CwareImg,UseFul,BoardId,IsFree) Values($CwareId,$CwId,$EduSubjectId,$CwareTitle,$CYearName,$CwareName,$TeacherName,$DateEnd,$CwareClassId,$CwareClassName,$ClassOrder,$MobileCourseOpen,$Download,$VideoType,$CwareUrl,$UpdateTime,$Rownum,$CwareImg,$UseFul,$BoardId,$IsFree)";
            
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                var cmd = new SQLiteCommand(sqldelete, Conn) { Transaction = tran };
                cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = eduSubjectId });
                cmd.ExecuteNonQuery();
                foreach (StudentCwareList.StudentCwareItem item in list)
                {
                    cmd = new SQLiteCommand(sqlAdd, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$BoardId") { Value = item.BoardId });
                    cmd.Parameters.Add(new SQLiteParameter("$ClassOrder") { Value = item.ClassOrder });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareClassId") { Value = item.CwareClassId });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareClassName") { Value = item.CwareClassName });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareId") { Value = item.CwareId });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareImg") { Value = item.CwareImg });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareName") { Value = item.CwareName });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareTitle") { Value = item.CwareTitle });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareUrl") { Value = item.CwareUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$CwId") { Value = item.CwId });
                    cmd.Parameters.Add(new SQLiteParameter("$CYearName") { Value = item.CYearName });
                    cmd.Parameters.Add(new SQLiteParameter("$DateEnd") { Value = item.DateEnd });
                    cmd.Parameters.Add(new SQLiteParameter("$Download") { Value = item.Download });
                    cmd.Parameters.Add(new SQLiteParameter("$MobileCourseOpen") { Value = item.MobileCourseOpen });
                    cmd.Parameters.Add(new SQLiteParameter("$Rownum") { Value = item.Rownum });
                    cmd.Parameters.Add(new SQLiteParameter("$TeacherName") { Value = item.TeacherName });
                    cmd.Parameters.Add(new SQLiteParameter("$UpdateTime") { Value = item.UpdateTime });
                    cmd.Parameters.Add(new SQLiteParameter("$UseFul") { Value = item.UseFul });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoType") { Value = item.VideoType });
                    cmd.Parameters.Add(new SQLiteParameter("$IsFree") { Value = item.IsFree });
                    cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = eduSubjectId });
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                return true;
            }
            catch
            {
                Trace.WriteLine(string.Join(",", list.Select(x => x.CwareId)));
                tran.Rollback();
                return false;
            }
            finally
            {
                CloseConn();
            }
        }

        internal bool AddChapterList(IEnumerable<StudentCwareChapter> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentCwareChapter item in list)
                {
                    var cmd = new SQLiteCommand(AddCourseWareChapterSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$ChapterId") { Value = item.ChapterId });
                    cmd.Parameters.Add(new SQLiteParameter("$ChapterName") { Value = item.ChapterName });
                    cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = item.CWareId });
                    cmd.Parameters.Add(new SQLiteParameter("$Order") { Value = item.Order });
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
        internal bool AddChapterList(int cwareId, IEnumerable<StudentCwareChapter> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentCwareChapter item in list)
                {
                    var cmd = new SQLiteCommand(AddCourseWareChapterSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$ChapterId") { Value = item.ChapterId });
                    cmd.Parameters.Add(new SQLiteParameter("$ChapterName") { Value = item.ChapterName });
                    cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = cwareId });
                    cmd.Parameters.Add(new SQLiteParameter("$Order") { Value = item.Order });
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
        internal bool AddCourseKcjyList(IEnumerable<StudentCwareKcjy> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentCwareKcjy item in list)
                {
                    var cmd = new SQLiteCommand(AddCwareKcjySql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = item.CWareId });
                    cmd.Parameters.Add(new SQLiteParameter("$NodeId") { Value = item.NodeId });
                    cmd.Parameters.Add(new SQLiteParameter("$NodeText") { Value = Crypt.Rc4EncryptString(item.NodeText) });
                    cmd.Parameters.Add(new SQLiteParameter("$TimeStart") { Value = item.TimeStart });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoId") { Value = item.VideoId });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoTime") { Value = item.VideoTime });
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

        internal bool AddCourseTimeNodeList(IEnumerable<StudentTimeNode> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (StudentTimeNode item in list)
                {
                    var cmd = new SQLiteCommand(AddTimeNodeSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$CWareId") { Value = item.CWareId });
                    cmd.Parameters.Add(new SQLiteParameter("$FlashUrl") { Value = item.FlashUrl });
                    cmd.Parameters.Add(new SQLiteParameter("$NodeId") { Value = item.NodeId });
                    cmd.Parameters.Add(new SQLiteParameter("$TimeEnd") { Value = item.TimeEnd });
                    cmd.Parameters.Add(new SQLiteParameter("$Timestart") { Value = item.Timestart });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoId") { Value = item.VideoId });
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

        internal bool AddClassName(IEnumerable<StudentClassName> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                var cmd = new SQLiteCommand("Delete From StudentClassName", Conn) { Transaction = tran };
                cmd.ExecuteNonQuery();
                foreach (StudentClassName item in list)
                {
                    cmd = new SQLiteCommand("Insert Into StudentClassName(ClassName,ClassOrder) Values($ClassName,$ClassOrder)", Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$ClassName") { Value = item.ClassName });
                    cmd.Parameters.Add(new SQLiteParameter("$ClassOrder") { Value = item.ClassOrder });
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

        internal bool AddKcjyDown(int cwareId, IEnumerable<StudentWareKcjyDown> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                var cmd = new SQLiteCommand("Delete From StudentCwareKcjyDown Where CwareId = $CwareId", Conn)
                {
                    Transaction = tran
                };
                cmd.Parameters.Add(new SQLiteParameter("$CwareId") { Value = cwareId });
                cmd.ExecuteNonQuery();
                foreach (StudentWareKcjyDown item in list)
                {
                    cmd = new SQLiteCommand(AddKcjyDownSql, Conn) { Transaction = tran };
                    cmd.Parameters.Add(new SQLiteParameter("$CwareId") { Value = cwareId });
                    cmd.Parameters.Add(new SQLiteParameter("$JiangyiFile") { Value = item.JiangyiFile });
                    cmd.Parameters.Add(new SQLiteParameter("$SmallListId") { Value = item.SmallListId });
                    cmd.Parameters.Add(new SQLiteParameter("$SmallListName") { Value = item.SmallListName });
                    cmd.Parameters.Add(new SQLiteParameter("$SmallOrder") { Value = item.SmallOrder });
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
        /// 保存弹出知识点信息
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        internal bool AddPointTestStartTime(int cwareId, string videoId, IEnumerable<PointTestStartTimeItem> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                var cmd = new SQLiteCommand("Delete From PointTestStartTime Where CwareId= $CwareId And VideoId = $VideoId", Conn, tran);
                cmd.Parameters.Add(new SQLiteParameter("$CwareId") { Value = cwareId });
                cmd.Parameters.Add(new SQLiteParameter("$VideoId") { Value = videoId });
                cmd.ExecuteNonQuery();
                const string sql = "Insert Or Replace Into PointTestStartTime(CwareId,VideoId,PointName,TestId,PointTestStartTime,PointOpenType,BackTime) Values($CwareId,$VideoId,$PointName,$TestId,$PointTestStartTime,$PointOpenType,$BackTime)";
                foreach (PointTestStartTimeItem item in list)
                {
                    cmd = new SQLiteCommand(sql, Conn, tran);
                    cmd.Parameters.Add(new SQLiteParameter("$CwareId") { Value = cwareId });
                    cmd.Parameters.Add(new SQLiteParameter("$VideoId") { Value = videoId });
                    cmd.Parameters.Add(new SQLiteParameter("$TestId") { Value = item.TestId });
                    cmd.Parameters.Add(new SQLiteParameter("$PointTestStartTime") { Value = item.PointTestStartTime });
                    cmd.Parameters.Add(new SQLiteParameter("$PointOpenType") { Value = item.PointOpenType });
                    cmd.Parameters.Add(new SQLiteParameter("$PointName") { Value = item.PointName });
                    cmd.Parameters.Add(new SQLiteParameter("$BackTime") { Value = item.BackTime });
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                return true;
            }
            catch (Exception)
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
        /// 保存弹出知识点的题目
        /// </summary>
        /// <param name="testId"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        internal bool AddPointTestQuestion(int testId, IEnumerable<PointTestQuestionItem> list)
        {
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                var cmd = new SQLiteCommand("Delete From PointTestQuestion Where PointTestId= $PointTestId", Conn, tran);
                cmd.Parameters.Add(new SQLiteParameter("$PointTestId") { Value = testId });
                cmd.ExecuteNonQuery();
                const string sql = "Insert Or Replace Into PointTestQuestion(QuestionId,ParentId,PointTestId,QuesViewType,Content,Score,RightAnswer,Analysis,SplitScore,PointId,QuesType) Values($QuestionId,$ParentId,$PointTestId,$QuesViewType,$Content,$Score,$RightAnswer,$Analysis,$SplitScore,$PointId,$QuesType)";
                const string sqlOption = "Insert Or Replace Into PointTestQuestionOption(QuestionId,QuesValue,QuesOption,Sequence) Values($QuestionId,$QuesValue,$QuesOption,$Sequence)";
                foreach (PointTestQuestionItem item in list)
                {
                    cmd = new SQLiteCommand(sql, Conn, tran);
                    cmd.Parameters.Add(new SQLiteParameter("$Analysis") { Value = item.Analysis });
                    cmd.Parameters.Add(new SQLiteParameter("$Content") { Value = item.Content });
                    cmd.Parameters.Add(new SQLiteParameter("$ParentId") { Value = item.ParentId });
                    cmd.Parameters.Add(new SQLiteParameter("$PointId") { Value = item.PointId });
                    cmd.Parameters.Add(new SQLiteParameter("$PointTestId") { Value = testId });
                    cmd.Parameters.Add(new SQLiteParameter("$QuestionId") { Value = item.QuestionId });
                    cmd.Parameters.Add(new SQLiteParameter("$QuesType") { Value = item.QuesType });
                    cmd.Parameters.Add(new SQLiteParameter("$QuesViewType") { Value = item.QuesViewType });
                    cmd.Parameters.Add(new SQLiteParameter("$RightAnswer") { Value = item.RightAnswer });
                    cmd.Parameters.Add(new SQLiteParameter("$Score") { Value = item.Score });
                    cmd.Parameters.Add(new SQLiteParameter("$SplitScore") { Value = item.SplitScore });
                    cmd.ExecuteNonQuery();
                    cmd = new SQLiteCommand("Delete From PointTestQuestionOption Where Questionid = $QuestionId", Conn, tran);
                    cmd.Parameters.Add(new SQLiteParameter("$QuestionId") { Value = item.QuestionId });
                    cmd.ExecuteNonQuery();
                    if (item.QuestionOptionList != null && item.QuestionOptionList.Any())
                    {
                        foreach (PointTestQuestionOptionItem opt in item.QuestionOptionList)
                        {
                            cmd = new SQLiteCommand(sqlOption, Conn, tran);
                            cmd.Parameters.Add(new SQLiteParameter("$QuesOption") { Value = opt.QuesOption });
                            cmd.Parameters.Add(new SQLiteParameter("$QuestionId") { Value = opt.QuestionId });
                            cmd.Parameters.Add(new SQLiteParameter("$QuesValue") { Value = opt.QuesValue });
                            cmd.Parameters.Add(new SQLiteParameter("$Sequence") { Value = opt.Sequence });
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                tran.Commit();
                return true;
            }
            catch (Exception)
            {
                tran.Rollback();
                return false;
            }
            finally
            {
                CloseConn();
            }
        }

        #endregion

        #region 下载

        private const string AddCwareDownSql = "Insert or Replace Into StudentCwareDown(CwareId,VideoId,Url,LocalFile,State,Rate,modTime) Values($CwareId,$VideoId,$Url,$LocalFile,$State,$Rate,$modTime);select last_insert_rowid();";

        private const string AddCwareDownloadSql = "Insert Into StudentCwareDownload(CwareID,VideoID,DownUrl,LocalFile,DownState,DownType,DownOrder,ForImport,modTime) Values($CwareID,$VideoID,$DownUrl,$LocalFile,$DownState,$DownType,$DownOrder,$ForImport,$modTime);select last_insert_rowid();";
        private const string BeginCwareDownLoadSql = "Update StudentCwareDownload Set LocalFile = $LocalFile Where DownId=$DownId";
        private const string UpdateCwareDownLoadStateSql = "Update StudentCwareDownload set DownState=$DownState where downId=$downId";

        private const string UpdateCwareDownLoadModTimeSql = "Update StudentCwareDownload set modTime=$modTime where downId=$downId";

        //获取已下载的文件信息
        private const string GetDownloadSql = "Select D.DownId,D.CwareId,D.VideoId,D.DownUrl,D.LocalFile,D.DownState,D.DownOrder,D.DownType,D.ForImport From StudentCwareDownload D Where D.CwareId= $CwareId And D.VideoId= $VideoId And D.DownType in('1','3')";

        private const string GetNeedDownloadListSql = "Select D.DownId,D.CwareId,D.VideoId,D.DownUrl,D.LocalFile,D.DownState,D.DownOrder,D.DownType,D.ForImport,C.VideoName,W.CwareName as Name,E.EduSubjectName as CourseName From StudentCwareDownload D Inner Join StudentCourseDetail C On D.CwareId=C.CwareId ANd D.VideoId=C.VideoId Inner Join StudentCware W On C.CWareId=W.CwareId Inner Join StudentEduSubject E On W.EduSubjectId=E.EduSubjectId Where D.DownState < 2 Order By DownOrder";
        private const string GetDownloadListSql = "Select Distinct D.DownId,D.CwareId,D.VideoId,D.DownUrl,D.LocalFile,D.DownState,D.DownOrder,D.DownType,D.ForImport,C.VideoName,W.CwareName as Name From StudentCwareDownload D Inner Join StudentCourseDetail C On D.CwareId=C.CwareId ANd D.VideoId=C.VideoId Inner Join StudentCware W On C.CWareId=W.CwareId Where D.DownState <> 3 Order By DownOrder";
        private const string GetDownloadItemByDownIdSql = "Select D.DownId,D.CwareId,D.VideoId,D.DownUrl,D.LocalFile,D.DownState,D.DownOrder,D.DownType,D.ForImport,C.VideoName,W.CwareName as Name,E.EduSubjectName as CourseName From StudentCwareDownload D Inner Join StudentCourseDetail C On D.CwareId=C.CwareId ANd D.VideoId=C.VideoId Inner Join StudentCware W On C.CWareId=W.CwareId Inner Join StudentEduSubject E On W.EduSubjectId=E.EduSubjectId Where D.DownId = $DownId";
        private const string GetDownloadItemSql = "Select D.DownId,D.CwareId,D.VideoId,D.DownUrl,D.LocalFile,D.DownState,D.DownOrder,D.DownType,D.ForImport,C.VideoName,W.CwareName as Name,E.EduSubjectName as CourseName,D.[modTime]  From StudentCwareDownload D Inner Join StudentCourseDetail C On D.CwareId=C.CwareId ANd D.VideoId=C.VideoId Inner Join StudentCware W On C.CWareId=W.CwareId Inner Join StudentEduSubject E On W.EduSubjectId=E.EduSubjectId Where D.CwareId= $CwareId And D.VideoId= $VideoId And D.DownType = $DownType";

        public long AddCwareDown(StudentCWareDown item)
        {
            var objrate = ExecuteScalar("Select Max(Rate) From StudentCwareDown");
            double rate = 1;
            if (objrate != null && objrate != DBNull.Value) rate = ((double)objrate) + 1;
            var obj = ExecuteScalar(AddCwareDownSql,
                new SQLiteParameter("$CwareId") { Value = item.CwareId },
                new SQLiteParameter("$LocalFile") { Value = item.LocalFile },
                new SQLiteParameter("$Rate") { Value = rate },
                new SQLiteParameter("$State") { Value = item.State },
                new SQLiteParameter("$Url") { Value = item.Url },
                new SQLiteParameter("$VideoId") { Value = item.VideoId },
                new SQLiteParameter("$modTime") { Value = item.ModTime });
            if (obj != null) return (long)obj;
            return 0;
        }

        public long AddCwareDownload(StudentCwareDownload item)
        {
            var objorder = ExecuteScalar("Select Max(DownOrder) From StudentCwareDownload");
            long order = 1;
            if (objorder != null && objorder != DBNull.Value) order = (long)objorder + 1;
            var obj = ExecuteScalar(AddCwareDownloadSql,
                new SQLiteParameter("$CwareId") { Value = item.CwareId },
                new SQLiteParameter("$DownState") { Value = item.DownState },
                new SQLiteParameter("$DownType") { Value = item.DownType },
                new SQLiteParameter("$DownUrl") { Value = item.DownUrl },
                new SQLiteParameter("$LocalFile") { Value = item.LocalFile },
                new SQLiteParameter("$VideoId") { Value = item.VideoId },
                new SQLiteParameter("$DownOrder") { Value = order },
                new SQLiteParameter("$ForImport") { Value = item.ForImport },
                new SQLiteParameter("$modTime") { Value = item.ModTime }
                );
            if (obj != null) return (long)obj;
            return 0;
        }

        public bool BeginCwareDownload(long downId, string localFile)
        {
            return ExecuteNonQuery(BeginCwareDownLoadSql,
                new SQLiteParameter("$downId") { Value = downId },
                new SQLiteParameter("$LocalFile") { Value = localFile }
                ) >= 0;
        }

        public bool UpdateCwareDownloadState(long downId, int state)
        {
            return ExecuteNonQuery(UpdateCwareDownLoadStateSql,
                new SQLiteParameter("$downId") { Value = downId },
                new SQLiteParameter("$DownState") { Value = state }) >= 0;
        }

        public IEnumerable<ViewStudentCwareDownLoad> GetCwareDownloadList()
        {
            DataTable dt = ExecuteTable(GetDownloadListSql);
            return dt.AsEnumerable().Select(x => new ViewStudentCwareDownLoad
            {
                CwareId = x.Field<int>("CwareId"),
                DownId = x.Field<long>("DownId"),
                DownOrder = x.Field<int>("DownOrder"),
                DownState = x.Field<int>("DownState"),
                DownType = x.Field<int>("DownType"),
                DownUrl = x.Field<string>("DownUrl"),
                ForImport = x.Field<int>("ForImport"),
                LocalFile = x.Field<string>("LocalFile"),
                VideoId = x.Field<string>("VideoId"),
                VideoName = x.Field<string>("Name") + "  " + x.Field<string>("VideoName")
            });
        }

        public IEnumerable<ViewStudentCwareDownLoad> GetCwareNeedDownloadList()
        {
            DataTable dt = ExecuteTable(GetNeedDownloadListSql);
            return dt.AsEnumerable().Select(x => new ViewStudentCwareDownLoad
            {
                CwareId = x.Field<int>("CwareId"),
                DownId = x.Field<long>("DownId"),
                DownOrder = x.Field<int>("DownOrder"),
                DownState = x.Field<int>("DownState"),
                DownType = x.Field<int>("DownType"),
                DownUrl = x.Field<string>("DownUrl"),
                ForImport = x.Field<int>("ForImport"),
                LocalFile = x.Field<string>("LocalFile"),
                VideoId = x.Field<string>("VideoId"),
                //带参数的下载路径 dgh
                ParmDownUrl = string.IsNullOrWhiteSpace(x.Field<string>("DownUrl")) ? string.Empty : GetDownUrl(x.Field<string>("DownUrl")),
                VideoName = x.Field<string>("CourseName") + " " + x.Field<string>("Name") + " " + x.Field<string>("VideoName")
            });
        }

        public ViewStudentCwareDownLoad GetCwareDownloadItem(int cwareId, string videoId, int downType)
        {
            DataTable dt = ExecuteTable(GetDownloadItemSql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$VideoId") { Value = videoId },
                new SQLiteParameter("$DownType") { Value = downType }
                );
            if (dt == null || dt.Rows.Count == 0) return null;
            DataRow x = dt.Rows[0];
            return new ViewStudentCwareDownLoad
            {
                CwareId = x.Field<int>("CwareId"),
                DownId = x.Field<long>("DownId"),
                DownOrder = x.Field<int>("DownOrder"),
                DownState = x.Field<int>("DownState"),
                DownType = x.Field<int>("DownType"),
                DownUrl = x.Field<string>("DownUrl"),
                ForImport = x.Field<int>("ForImport"),
                LocalFile = x.Field<string>("LocalFile"),
                VideoId = x.Field<string>("VideoId"),
                VideoName = x.Field<string>("CourseName") + " " + x.Field<string>("Name") + " " + x.Field<string>("VideoName"),
                ModeTime = x.Field<string>("modTime")
            };
        }
        /// <summary>
        /// 用于批量删除 dgh
        /// </summary>
        /// <param name="cwareId">章节编号</param>
        /// <param name="videoId">视频ID</param>
        /// <returns></returns>
        public IEnumerable<ViewStudentCwareDownLoad> GetCwareDownloadItemList(int cwareId, string videoId)
        {
            DataTable dt = ExecuteTable(GetDownloadSql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$VideoId") { Value = videoId }
                );
            if (dt == null || dt.Rows.Count == 0) return null;
            return dt.AsEnumerable().Select(x => new ViewStudentCwareDownLoad
            {
                CwareId = x.Field<int>("CwareId"),
                DownId = x.Field<long>("DownId"),
                DownOrder = x.Field<int>("DownOrder"),
                DownState = x.Field<int>("DownState"),
                DownType = x.Field<int>("DownType"),
                DownUrl = x.Field<string>("DownUrl"),
                ForImport = x.Field<int>("ForImport"),
                LocalFile = x.Field<string>("LocalFile")
            });
        }
        public bool SetDownloadForImport(long downId)
        {
            const string sql = "Update StudentCwareDownload Set Forimport = 1 Where DownState <>3 And DownId = $DownId";
            return ExecuteNonQuery(sql, new SQLiteParameter("$DownId") { Value = downId }) >= 0;
        }

        public ViewStudentCwareDownLoad GetCwareDownloadItem(long downId)
        {
            DataTable dt = ExecuteTable(GetDownloadItemByDownIdSql, new SQLiteParameter("$DownId") { Value = downId });
            if (dt == null || dt.Rows.Count == 0) return null;
            DataRow x = dt.Rows[0];
            return new ViewStudentCwareDownLoad
            {
                CwareId = x.Field<int>("CwareId"),
                DownId = x.Field<long>("DownId"),
                DownOrder = x.Field<int>("DownOrder"),
                DownState = x.Field<int>("DownState"),
                DownType = x.Field<int>("DownType"),
                DownUrl = x.Field<string>("DownUrl"),
                ForImport = x.Field<int>("ForImport"),
                LocalFile = x.Field<string>("LocalFile"),
                VideoId = x.Field<string>("VideoId"),
                VideoName = x.Field<string>("CourseName") + " " + x.Field<string>("Name") + " " + x.Field<string>("VideoName")
            };
        }

        public bool MoveDownloadToLast(long downId)
        {
            var objorder = ExecuteScalar("Select Max(DownOrder) From StudentCwareDownload");
            long order = 1;
            if (objorder != null && objorder != DBNull.Value) order = (long)objorder + 1;
            return ExecuteNonQuery("Update StudentCwareDownload Set DownOrder=$DownOrder Where DownId=$DownId",
                new SQLiteParameter("$DownOrder") { Value = order },
                new SQLiteParameter("$downId") { Value = downId }
                ) >= 0;
        }

        internal bool TestVideoExists(int cwareId, string videoId)
        {
            var obj = ExecuteScalar("Select Count(*) From StudentCwareDown Where State=3 And CwareId=$CwareId And VideoId=$VideoId",
                new SQLiteParameter("$CwareId", cwareId),
                new SQLiteParameter("$VideoId", videoId)
                );
            if (obj == null) return false;
            return (long)obj > 0;
        }

        public ViewStudentCwareDown GetCwareDownItem(int cwareId, string videoId)
        {
            const string sql =
                "select D.DownId,D.CwareId,D.VideoId,D.Url,D.LocalFile,D.State,D.Rate,C.VideoName,W.Name,I.Name as CourseName from StudentCwareDown D Inner Join StudentCourseDetail C On D.CwareId=C.CwareId ANd D.VideoId=C.VideoId Inner Join StudentCourseWare W On C.CWareId=W.CwareId Inner Join StudentCourse I On W.CourseId=I.CourseId Where D.State=3 and D.Cwareid==$CwareId And D.VideoId=$VideoId Order by Rate";
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$CwareId", cwareId),
                new SQLiteParameter("$VideoId", videoId)
                );
            if (dt == null || dt.Rows.Count == 0) return null;
            DataRow x = dt.Rows[0];
            return new ViewStudentCwareDown
            {
                CwareId = x.Field<int>("CwareId"),
                DownId = x.Field<long>("DownId"),
                LocalFile = x.Field<string>("LocalFile"),
                Rate = x.Field<double>("Rate"),
                State = x.Field<int>("State"),
                Url = x.Field<string>("Url"),
                VideoId = x.Field<string>("VideoId"),
                VideoName = x.Field<string>("CourseName") + " " + x.Field<string>("Name") + " " + x.Field<string>("VideoName")
            };
        }

        public long GetCwareDownId(int cwareId, string videoId)
        {
            const string sql = "Select DownId from StudentCwareDown Where Cwareid==$CwareId And VideoId=$VideoId";
            var obj = ExecuteScalar(sql,
                new SQLiteParameter("$CwareId", cwareId),
                new SQLiteParameter("$VideoId", videoId));
            if (obj != null && obj != DBNull.Value)
            {
                return (long)obj;
            }
            return 0;
        }
        /// <summary>
        /// 更改更新时间
        /// </summary>
        /// <param name="downId">主键</param>
        /// <param name="modTime">更新时间</param>
        /// <returns></returns>
        public bool UpdateCwareDownloadModTime(long downId, string modTime)
        {
            return ExecuteNonQuery(UpdateCwareDownLoadModTimeSql,
                new SQLiteParameter("$downId") { Value = downId },
                new SQLiteParameter("$modTime") { Value = modTime }) >= 0;
        }
        /// <summary>
        /// 获取单个章节详细内容
        /// </summary>
        /// <param name="cwareId">章节号</param>
        /// <param name="videoId">视频号</param>
        /// <returns></returns>
        public StudentCourseDetail GetCourseDetailItem(int cwareId, string videoId)
        {
            const string sql = "select D.CwareId,D.VideoId,D.Length,D.VideoName,D.Title,D.VideoType,D.modTime from StudentCourseDetail D where  D.CwareId=$CwareId and D.VideoId=$VideoId";
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$CwareId", cwareId),
                new SQLiteParameter("$VideoId", videoId)
                );
            if (dt == null || dt.Rows.Count == 0) return null;
            DataRow x = dt.Rows[0];
            return new StudentCourseDetail
            {
                CWareId = x.Field<int>("CwareId"),
                VideoId = x.Field<string>("VideoId"),
                Length = x.Field<string>("Length"),
                VideoName = x.Field<string>("VideoName"),
                Title = x.Field<string>("Title"),
                VideoType = x.Field<int>("VideoType"),
                //用于导入时如果数据库中已存在时间则无需更改反之则用章节更新时间
                ModTime = x.Field<string>("modTime")
            };
        }
        /// <summary>
        /// 删除下载的数据包括zip包 dgh 2016.04.05(更新章节功能)
        /// </summary>
        /// <param name="cwareId">章节编号</param>
        /// <param name="videoId">视频编号</param>
        /// <param name="downType">下载类型</param>
        /// <returns></returns>
        public bool DeleteDonwload(int cwareId, string videoId, int downType)
        {
            ViewStudentCwareDownLoad item = GetCwareDownloadItem(cwareId, videoId, downType);

            try
            {
                if (item == null)
                {
                    Log.RecordLog("该下载的文件不存在：cwareId=" + cwareId + ",videoId=" + videoId + ",downType=" + downType);
                    return false;
                }
                var fileName = item.LocalFile;
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    SystemInfo.TryDeleteFile(fileName);
                    //SystemInfo.TryDeleteFile(fileName + Util.DownloadConfigExtension);
                    //SystemInfo.TryDeleteFile(fileName + Util.DownloadFileExtension);
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
            }
            return ExecuteNonQuery("Delete from StudentCwareDownload where downId=$downId", new SQLiteParameter("$downId", item.DownId)) >= 0;
        }
        /// <summary>
        /// 批量删除已下载的视频 dgh 2016.06.03
        /// </summary>
        /// <param name="cwareId">章节编号</param>
        /// <param name="videoId">视频编号</param>
        /// <returns></returns>
        public void DeleteMultDonwload(int cwareId, string videoId)
        {
            IEnumerable<ViewStudentCwareDownLoad> list = GetCwareDownloadItemList(cwareId, videoId);
            if (list == null || list.Count() == 0) return;
            try

            {
                foreach (ViewStudentCwareDownLoad item in list)
                {
                    var fileName = item.LocalFile;
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        SystemInfo.TryDeleteFile(fileName);
                    }
                    ExecuteNonQuery("Delete from StudentCwareDownload where downId=$downId", new SQLiteParameter("$downId", item.DownId));
                }

            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
            }
        }

        #endregion

        #region 课程管理

        private const string GetCourseWareItemSql = "Select CourseId,CwareId,CwId,PathURL,Name,CwareClassName,cYearName,CwareImg,VideoType,IsOpen,CTeacherName from StudentCourseWare Where CwId=$CwId";
        private const string GetCourseWareItemByCwareIdSql = "Select CourseId,CwareId,CwId,PathURL,Name,CwareClassName,cYearName,CwareImg,VideoType,IsOpen,CTeacherName from StudentCourseWare Where CwareId=$CwareId";
        //private const string GetStudentCwareDetailSql = "Select C.ChapterID,C.ChapterName,D.CwareId,D.VideoId,D.Length,D.VideoName,D.Title,D.VideoType,CD.DownId,Case When Not CD.State is null then CD.State When D.VideoZipUrl='' And D.VideoHDZipUrl='' Then -2 else -1 End As VideoState,CD.LocalFile,D.[modTime] from StudentCwareChapter C Inner Join StudentCourseDetail D On C.ChapterId = D.ChapterId And C.CwareId = D.CwareId Left Join StudentCwareDown CD On D.CwareId=CD.CwareId And D.VideoId=CD.VideoId Where D.CwareId=$CwareId And D.VideoType <> 3 Order by C.[Order] Desc, D.OrderBy";
        private const string GetStudentCwareDetailSql = "Select C.ChapterID,C.ChapterName,D.CwareId,D.VideoId,D.Length,D.VideoName,D.Title,D.VideoType,CD.DownId,Case When Not CD.State is null then CD.State When D.VideoZipUrl='' And D.VideoHDZipUrl='' Then -2 else -1 End As VideoState,CD.LocalFile,D.[modTime],CD.modTime CDModTime from StudentCwareChapter C Inner Join StudentCourseDetail D On C.ChapterId = D.ChapterId And C.CwareId = D.CwareId Left Join StudentCwareDown CD On D.CwareId=CD.CwareId And D.VideoId=CD.VideoId Where D.CwareId=$CwareId And D.VideoType <> 3 Order by C.[Order] Desc, D.OrderBy";
        private const string GetStudentCwareDetailItemSql = "Select CwareId,VideoId,ChapterId,DemoType,VideoUrl,AudioUrl,Length,Title,VideoType,VideoName,VideoZipUrl,AudioZipurl,VideoHDZipUrl,VideoHDUrl from StudentCourseDetail Where CwareId=$CwareId And VideoId=$VideoId";
        private const string GetStudentWareKcjySql = "Select CwareId,VideoId,NodeId,TimeStart,VideoTime,NodeText from StudentCwareKcjy Where CwareId=$CwareId And VideoId=$VideoId Order By VideoTime";
        private const string VerifyCourseSql = "select CwId From StudentCourseWare";

        private const string GetMobileDownDetailSql = "Select C.ChapterID,C.ChapterName,D.CwareId,D.VideoId,D.Length,D.VideoName,D.Title,D.VideoType,CD.DownId,Case When Not CD.DownState is null then CD.DownState When D.{0}='' Then -2 else -1 End As VideoState, CD.LocalFile from StudentCwareChapter C Inner Join StudentCourseDetail D On C.ChapterId = D.ChapterId And C.CwareId = D.CwareId Left Join StudentCwareDownload CD On D.CwareId=CD.CwareId And D.VideoId=CD.VideoId And CD.DownType = $DownType Where D.CwareId=$CwareId And D.VideoType <> 3 Order by C.[Order] Desc, D.OrderBy";

        internal StudentCourseWare GetCourseWareItem(string cwId)
        {
            const string sql = "Select CwareId,CwId,CwareUrl,CwareName,CwareClassName,cYearName,CwareImg,VideoType,MobileCourseOpen,TeacherName from StudentCware Where CwId=$CwId";
            DataTable dt = ExecuteTable(sql, new SQLiteParameter("$CwId") { Value = cwId });
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow rw = dt.Rows[0];
                return new StudentCourseWare
                {
                    //CourseId = rw.Field<string>("CourseId"),
                    CWareClassName = rw.Field<string>("CWareClassName"),
                    CWareId = rw.Field<int>("CWareId"),
                    VideoType = rw.Field<int>("VideoType").ToString(CultureInfo.InvariantCulture),
                    PathUrl = rw.Field<string>("CwareUrl"),
                    Name = rw.Field<string>("CwareName"),
                    IsOpen = rw.Field<int>("MobileCourseOpen"),
                    CYearName = rw.Field<string>("cYearName"),
                    CwId = rw.Field<string>("CwId"),
                    CWareImg = rw.Field<string>("CWareImg"),
                    CTeacherName = rw.Field<string>("TeacherName")
                };
            }
            return null;
        }

        internal StudentCourseWare GetCourseWareItem(int cwareId)
        {
            const string sql = "Select CwareId,CwId,CwareUrl,CwareName,CwareClassName,cYearName,CwareImg,VideoType,MobileCourseOpen,TeacherName from StudentCware Where CwareId=$CwareId";
            DataTable dt = ExecuteTable(sql, new SQLiteParameter("$CwareId") { Value = cwareId });
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow rw = dt.Rows[0];
                return new StudentCourseWare
                {
                    //CourseId = rw.Field<string>("CourseId"),
                    CWareClassName = rw.Field<string>("CWareClassName"),
                    CWareId = rw.Field<int>("CWareId"),
                    VideoType = rw.Field<int>("VideoType").ToString(CultureInfo.InvariantCulture),
                    PathUrl = rw.Field<string>("CwareUrl"),
                    Name = rw.Field<string>("CwareName"),
                    IsOpen = rw.Field<int>("MobileCourseOpen"),
                    CYearName = rw.Field<string>("cYearName"),
                    CwId = rw.Field<string>("CwId"),
                    CWareImg = rw.Field<string>("CWareImg"),
                    CTeacherName = rw.Field<string>("TeacherName")
                };
            }
            return null;
        }

        public IEnumerable<int> GetStudentCourseEduList()
        {
            const string sql = "Select Distinct CourseEduId From StudentEduSubject";
            DataTable dt = ExecuteTable(sql);
            if (dt == null || dt.Rows.Count == 0) return new int[0];
            return dt.AsEnumerable().Select(x => x.Field<int>(0));
        }

        public string[] VerifyCourse(IEnumerable<string> course)
        {
            DataTable dt = ExecuteTable("Select CwId From StudentCware");
            return dt.AsEnumerable().Select(x => x.Field<string>("Cwid")).Where(course.Contains).ToArray();
        }

        /// <summary>
        /// 获取答疑板ID dgh 2017.02.15
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="eduSubjectId"></param>
        /// <returns></returns>
        public IEnumerable<int> GetStudentCwareBordId(int cwareId, int eduSubjectId)
        {
            var sql = "Select Distinct BoardId From StudentCware where cwareId=$cwareId and eduSubjectId=$eduSubjectId";
            DataTable dt = ExecuteTable(sql, new SQLiteParameter("$cwareId") { Value = cwareId }, new SQLiteParameter("$eduSubjectId") { Value = eduSubjectId });
            if (dt == null || dt.Rows.Count == 0) return new int[0];
            return dt.AsEnumerable().Select(x => x.Field<int>(0));
        }


        public IEnumerable<int> GetStudentEduSubjectList()
        {
            const string sql = "Select Distinct EduSubjectId From StudentEduSubject";
            DataTable dt = ExecuteTable(sql);
            if (dt == null || dt.Rows.Count == 0) return new int[0];
            return dt.AsEnumerable().Select(x => x.Field<int>(0));
        }



        /// <summary>
        /// 通过科目获取课件列表
        /// </summary>
        public List<ViewStudentCourseWare> GetStudentSubjectCourseWareList()
        {
            DataTable ddt = ExecuteTable("Select CwareId,LocalFile from StudentCwareDown Where state=3");
            var sizeList = ddt.AsEnumerable().Select(x => new
            {
                CwareId = x.Field<int>("CwareId"),
                Size = SystemInfo.GetFileSize(x.Field<string>("LocalFile"))
            });
            var lastOrder = 0;
            //有班次设置的语句

            // var sqlGet = "Select S.CourseEduId,S.OrderNo,S.BoardId,S.EduSubjectId,S.EduSubjectName As CourseName,C.DateEnd As EndDate,C.CwareName as CourseWareName,C.CwareId,C.CwId,C.CwareUrl,C.MobileCourseOpen As IsOpen,C.Download,C.TeacherName As CTeacherName,C.CwareClassName,C.CYearName,C.ClassOrder,C.VideoType,Cs.UserHide,Cs.UserShowOrder From StudentEduSubject S Inner Join StudentCware C On S.EduSubjectId = C.EduSubjectId Left Outer Join StudentCwareSetting Cs On C.EduSubjectId = Cs.EduSubjectId And C.CwareID = Cs.Cwareid And Cs.Uid = $Uid Where C.DateEnd > datetime('now', 'localtime')";
            // 新增了IsFree字段需要处理
            var sqlGet = "Select S.CourseEduId,S.OrderNo,S.BoardId,S.EduSubjectId,S.EduSubjectName As CourseName,C.IsFree,C.DateEnd As EndDate,C.CwareName as CourseWareName,C.CwareId,C.CwId,C.CwareUrl,C.MobileCourseOpen As IsOpen,C.Download,C.TeacherName As CTeacherName,C.CwareClassName,C.CYearName,C.ClassOrder,C.VideoType,Cs.UserHide,Cs.UserShowOrder From StudentEduSubject S Inner Join StudentCware C On S.EduSubjectId = C.EduSubjectId Left Outer Join StudentCwareSetting Cs On C.EduSubjectId = Cs.EduSubjectId And C.CwareID = Cs.Cwareid And Cs.Uid = $Uid Where C.DateEnd > datetime('now', 'localtime')";

#if MED || FOR68
            sqlGet = "Select S.CourseEduId,S.OrderNo,S.BoardId,S.EduSubjectId,S.EduSubjectName As CourseName,C.DateEnd As EndDate,C.CwareName as CourseWareName,C.CwareId,C.CwId,C.CwareUrl,C.MobileCourseOpen As IsOpen,C.Download,(Case (C.CwareTitle Is null Or C.CwareTitle='') When 1 Then C.TeacherName Else C.CwareTitle End) As CTeacherName,C.CwareClassName,C.CYearName,C.ClassOrder,C.VideoType,Cs.UserHide,Cs.UserShowOrder From StudentEduSubject S Inner Join StudentCware C On S.EduSubjectId = C.EduSubjectId Left Outer Join StudentCwareSetting Cs On C.EduSubjectId = Cs.EduSubjectId And C.CwareID = Cs.Cwareid And Cs.Uid = $Uid Where C.DateEnd > datetime('now', 'localtime')";
#endif
            DataTable dt = ExecuteTable(sqlGet, new SQLiteParameter("$Uid") { Value = Util.SsoUid });

            if (dt == null)
            {
                return new List<ViewStudentCourseWare>();
            }

            EnumerableRowCollection<ViewStudentCourseWare> lst = dt.AsEnumerable().Select(x =>
            {
                var item = new ViewStudentCourseWare
                {
                    CourseEduId = x.Field<int>("CourseEduId"),
                    BoardId = x.Field<int>("BoardID"),
                    //CourseId = x.Field<string>("CourseId"),
                    CourseName = x.Field<string>("CourseName"),
                    CwareId = x.Field<int>("CwareId"),
                    CwId = x.Field<string>("CwId"),
                    IsOpen = x.Field<string>("Download") == "Y" && x.Field<int>("IsOpen") == 1,
                    CanDownload = x.Field<string>("Download") == "Y",
                    CTeacherName = x.Field<string>("CTeacherName"),
                    CWareClassName = x.Field<string>("CWareClassName"),
                    CYearName = x.Field<string>("cYearName"),
                    Order = x.Field<int>("ClassOrder") * 10,
                    VideoType = x.Field<int>("VideoType").ToString(CultureInfo.InvariantCulture),
                    CourseWareName = x.Field<string>("CourseWareName"),
                    FileCount = sizeList.Count(y => y.CwareId == x.Field<int>("CwareId") && y.Size > 0),
                    FileSizes = GetFileSizeStrByByte(sizeList.Where(y => y.CwareId == x.Field<int>("CwareId")).Sum(y => y.Size)),
                    BigOrder = x.Field<int>("OrderNo"),
                    EduSubjectId = x.Field<int>("EduSubjectId"),
                    //新添加 dgh 2017.07.05
                    UserHide = x.Field<int?>("UserHide") ?? 0,
                    UserShowOrder = x.Field<int?>("UserShowOrder") ?? 0,
                    //添加该字段 用于同步听课记录 dgh 2017.11.30
                    CwareUrl = x.Field<string>("CwareUrl"),
                    // 新增IsFree字段，用于区分已购课程和赠送课程
                    IsFree = x.Field<int>("IsFree")
                };
                if (lastOrder == item.Order)
                {
                    item.Order = lastOrder + 1;
                }
                else
                {
                    lastOrder = item.Order;
                }
                //var showOrder = x.Field<int?>("UserShowOrder");
                //item.UserShowOrder = showOrder.HasValue ? showOrder.Value : 0;
                //var userHide = x.Field<int?>("UserHide");
                //item.UserHide = userHide.HasValue ? userHide.Value : 0;
                return item;
            });

            return lst.Where(x => x.UserHide == 0)
                .OrderByDescending(x => x.BigOrder)
                .ThenBy(x => x.UserShowOrder)
                .ThenByDescending(x => x.CYearName)
                .ThenBy(x => x.Order)
                .ThenBy(x => x.CwareId)
                .ToList();
        }
        /// <summary>
        /// 获取指定的课程信息  dgh 2017.03.22
        /// </summary>
        /// <param name="eduSubjectId">科目ID</param>
        /// <param name="cwareId">班次ID</param>
        /// <returns></returns>
        public ViewStudentCourseWare GetStudentSubjectCourseWareItem(int eduSubjectId, int cwareId)
        {
            DataTable ddt = ExecuteTable("Select CwareId,LocalFile from StudentCwareDown Where state=3");
            var sizeList = ddt.AsEnumerable().Select(x => new
            {
                CwareId = x.Field<int>("CwareId"),
                Size = SystemInfo.GetFileSize(x.Field<string>("LocalFile"))
            });
            var lastOrder = 0;
            const string sqlGet = "Select S.CourseEduId,S.OrderNo,S.BoardId,S.EduSubjectId,S.EduSubjectName As CourseName,C.DateEnd As EndDate,C.CwareName as CourseWareName,C.CwareId,C.CwId,C.CwareUrl,C.MobileCourseOpen As IsOpen,C.Download,C.TeacherName As CTeacherName,C.CwareClassName,C.CYearName,C.ClassOrder,C.VideoType From StudentEduSubject S Inner Join StudentCware C On S.EduSubjectId = C.EduSubjectId Where C.DateEnd > datetime('now', 'localtime') and C.EduSubjectId=$EduSubjectId and C.CwareId=$CwareId";
            DataTable dt = ExecuteTable(sqlGet,
                new SQLiteParameter("$Uid") { Value = Util.SsoUid },
                new SQLiteParameter("$EduSubjectId") { Value = eduSubjectId },
                new SQLiteParameter("$CwareId") { Value = cwareId }
                );
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            DataRow rw = dt.Rows[0];
            var item = new ViewStudentCourseWare
            {
                CourseEduId = rw.Field<int>("CourseEduId"),
                BoardId = rw.Field<int>("BoardID"),
                CourseName = rw.Field<string>("CourseName"),
                CwareId = rw.Field<int>("CwareId"),
                CwId = rw.Field<string>("CwId"),
                IsOpen = rw.Field<string>("Download") == "Y" && rw.Field<int>("IsOpen") == 1,
                CanDownload = rw.Field<string>("Download") == "Y",
                CTeacherName = rw.Field<string>("CTeacherName"),
                CWareClassName = rw.Field<string>("CWareClassName"),
                CYearName = rw.Field<string>("cYearName"),
                Order = rw.Field<int>("ClassOrder") * 10,
                VideoType = rw.Field<int>("VideoType").ToString(CultureInfo.InvariantCulture),
                CourseWareName = rw.Field<string>("CourseWareName"),
                FileCount = sizeList.Count(y => y.CwareId == rw.Field<int>("CwareId") && y.Size > 0),
                FileSizes = GetFileSizeStrByByte(sizeList.Where(y => y.CwareId == rw.Field<int>("CwareId")).Sum(y => y.Size)),
                BigOrder = rw.Field<int>("OrderNo"),
                EduSubjectId = rw.Field<int>("EduSubjectId"),
                //添加该字段 用于同步听课记录 dgh 2017.11.30
                CwareUrl = rw.Field<string>("CwareUrl")
            };
            if (lastOrder == item.Order)
            {
                item.Order = lastOrder + 1;
            }
            else
            {
                lastOrder = item.Order;
            }
            return item;
        }
        private string GetFileSizeStrByByte(long size)
        {
            if (size > 1024 * 1024 * 1024)
            {
                return ((double)size / (1024 * 1024 * 1024)).ToString("0.0 GB");
            }
            if (size > 1024 * 1024)
            {
                return ((double)size / (1024 * 1024)).ToString("0.0 MB");
            }
            if (size > 1024)
            {
                return ((double)size / 1024).ToString("0.0 KB");
            }
            if (size == 0)
            {
                return "0";
            }
            return size + " Byte";
        }

        public List<ViewStudentWareDetail> GetStudentCWareDetail(int cwareId)
        {
            DataTable dt = ExecuteTable(GetStudentCwareDetailSql,
                new SQLiteParameter("$CwareId") { Value = cwareId });
            if (dt == null) return new List<ViewStudentWareDetail>();
            return dt.AsEnumerable().Select(x =>
            {
                var downId = x.Field<long?>("DownId");
                var item = new ViewStudentWareDetail
                {
                    ChapterId = x.Field<int>("ChapterId"),
                    ChapterName = x.Field<string>("ChapterName"),
                    CwareId = x.Field<int>("CwareId"),
                    VideoId = x.Field<string>("VideoId"),
                    VideoLength = x.Field<string>("Length"),
                    VideoName = x.Field<string>("VideoName"),
                    Title = x.Field<string>("Title"),
                    VideoState = (int)x.Field<long>("VideoState"),
                    VideoPath = x.Field<string>("LocalFile"),
                    DownId = downId.HasValue ? downId.Value : 0,
                    VideoType = x.Field<int>("VideoType"),
                    ModTime = x.Field<string>("modTime"),
                    VideoModTime = x.Field<string>("CDModTime")
                };
                return item;
            }).ToList();
        }

        public StudentCourseDetail GetStudentCWareDetailItem(int cwareId, string videoId)
        {
            DataTable dt = ExecuteTable(GetStudentCwareDetailItemSql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$VideoId") { Value = videoId });
            if (dt == null || dt.Rows.Count <= 0) return null;
            DataRow rw = dt.Rows[0];
            return new StudentCourseDetail
            {
                VideoUrl = rw.Field<string>("VideoUrl"),
                VideoName = rw.Field<string>("VideoName"),
                VideoId = rw.Field<string>("VideoId"),
                Title = rw.Field<string>("Title"),
                Length = rw.Field<string>("Length"),
                DemoType = rw.Field<int>("DemoType"),
                CWareId = rw.Field<int>("CWareId"),
                ChapterId = rw.Field<int>("ChapterId"),
                AudioUrl = rw.Field<string>("AudioUrl"),
                VideoZipUrl = rw.Field<string>("VideoZipUrl"),
                AudioZipUrl = rw.Field<string>("AudioZipUrl"),
                VideoHdZipUrl = rw.Field<string>("VideoHDZipUrl"),
                VideoHdUrl = rw.Field<string>("VideoHDUrl"),
                VideoType = rw.Field<int>("VideoType")
            };
        }
        /// <summary>
        /// 获取下载后的课件信息 dgh 2017.03.23
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public ViewStudentWareDetail GetViewStudentCwareDetailItem(int cwareId, string videoId)
        {
            const string sql = "Select C.ChapterName,D.CwareId,D.VideoId,D.[VideoType],D.Length,D.VideoName,D.Title,CD.DownId,Case When CD.State is null then -1 else CD.State End As VideoState,CD.LocalFile from StudentCwareChapter C Inner Join StudentCourseDetail D On C.ChapterId = D.ChapterId Left Join StudentCwareDown CD On D.CwareId=CD.CwareId And D.VideoId=CD.VideoId Where C.CwareId=$CwareId And D.VideoId = $VideoId";
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$videoId") { Value = videoId });
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            DataRow dr = dt.Rows[0];
            var downId = dr.Field<long?>("DownId");
            return new ViewStudentWareDetail
            {
                ChapterName = dr.Field<string>("ChapterName"),
                CwareId = dr.Field<int>("CwareId"),
                VideoId = dr.Field<string>("VideoId"),
                VideoLength = dr.Field<string>("Length"),
                VideoName = dr.Field<string>("VideoName"),
                Title = dr.Field<string>("Title"),
                VideoState = (int)dr.Field<long>("VideoState"),
                VideoType = dr.Field<int>("VideoType"),
                VideoPath = dr.Field<string>("LocalFile"),
                DownId = downId.HasValue ? downId.Value : 0
            };
        }
        public ViewStudentWareDetail GetStudentCwareDetailPreItem(int cwareId, string videoId)
        {
            const string sql = "Select C.ChapterName,D.CwareId,D.VideoId,D.Length,D.VideoName,D.Title,CD.DownId,Case When CD.State is null then -1 else CD.State End As VideoState,CD.LocalFile from StudentCwareChapter C Inner Join StudentCourseDetail D On C.ChapterId = D.ChapterId Left Join StudentCwareDown CD On D.CwareId=CD.CwareId And D.VideoId=CD.VideoId Where C.CwareId=$CwareId And D.VideoId < $VideoId Order by D.VideoId DESC Limit 0,1";
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$videoId") { Value = videoId });
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            DataRow dr = dt.Rows[0];
            var downId = dr.Field<long?>("DownId");
            return new ViewStudentWareDetail
            {
                ChapterName = dr.Field<string>("ChapterName"),
                CwareId = dr.Field<int>("CwareId"),
                VideoId = dr.Field<string>("VideoId"),
                VideoLength = dr.Field<string>("Length"),
                VideoName = dr.Field<string>("VideoName"),
                Title = dr.Field<string>("Title"),
                VideoState = (int)dr.Field<long>("VideoState"),
                VideoPath = dr.Field<string>("LocalFile"),
                DownId = downId.HasValue ? downId.Value : 0
            };
        }

        public ViewStudentWareDetail GetStudentCwareDetailNextItem(int cwareId, string videoId)
        {
            const string sql = "Select C.ChapterName,D.CwareId,D.VideoId,D.Length,D.VideoName,D.Title,CD.DownId,Case When CD.State is null then -1 else CD.State End As VideoState,CD.LocalFile from StudentCwareChapter C Inner Join StudentCourseDetail D On C.ChapterId = D.ChapterId Left Join StudentCwareDown CD On D.CwareId=CD.CwareId And D.VideoId=CD.VideoId Where C.CwareId=$CwareId And D.VideoId > $VideoId Order by D.VideoId Limit 0,1";
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$videoId") { Value = videoId });
            if (dt == null || dt.Rows.Count == 0)
            {
                return null;
            }
            DataRow dr = dt.Rows[0];
            var downId = dr.Field<long?>("DownId");
            return new ViewStudentWareDetail
            {
                ChapterName = dr.Field<string>("ChapterName"),
                CwareId = dr.Field<int>("CwareId"),
                VideoId = dr.Field<string>("VideoId"),
                VideoLength = dr.Field<string>("Length"),
                VideoName = dr.Field<string>("VideoName"),
                Title = dr.Field<string>("Title"),
                VideoState = (int)dr.Field<long>("VideoState"),
                VideoPath = dr.Field<string>("LocalFile"),
                DownId = downId.HasValue ? downId.Value : 0
            };
        }

        public List<StudentCwareKcjy> GetStudentWareKcjy(int cwareId, string videoId)
        {
            DataTable dt = ExecuteTable(GetStudentWareKcjySql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$VideoId") { Value = videoId });
            if (dt == null) return new List<StudentCwareKcjy>();
            return dt.AsEnumerable().Select(x => new StudentCwareKcjy
            {
                CWareId = x.Field<int>("CWareId"),
                NodeId = x.Field<string>("NodeId"),
                NodeText = Crypt.Rc4DecryptString(x.Field<string>("NodeText")),
                TimeStart = x.Field<string>("TimeStart"),
                VideoId = x.Field<string>("VideoId"),
                VideoTime = x.Field<int>("VideoTime")
            }).ToList();
        }
        /// <summary>
        /// 删除视频文件
        /// </summary>
        /// <param name="downId"></param>
        /// <returns></returns>
        public bool DeleteVideo(long downId)
        {
            DataTable dt = ExecuteTable("Select LocalFile,CwareId,VideoId From StudentCwareDown where downId=$downId",
                new SQLiteParameter("$downId") { Value = downId });
            var cwareId = Convert.ToInt32(dt.Rows[0]["CwareId"]);
            var videoId = dt.Rows[0]["VideoId"] as string;
            Log.RecordData("DeleteVideoDown", cwareId, videoId);
            var fileName = dt.Rows[0]["LocalFile"].ToString();
            SystemInfo.TryDeleteFile(fileName);
            ExecuteNonQuery("Delete From StudentCwareDown where downId=$downId", new SQLiteParameter("$downId", downId));
            //CancelDonwloadForImport(cwareId, videoId);
            return true;
        }

        /// <summary>
        /// 取消正在下载的并且需要导入的任务
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <returns></returns>
        public bool CancelDonwloadForImport(int cwareId, string videoId)
        {
            DataTable dt = ExecuteTable("Select LocalFile,CwareId,VideoId,ForImport From StudentCwareDownload where cwareId = $Cwareid And VideoId = $VideoId And ForImport = 1 And DownState<>3",
                new SQLiteParameter("$cwareId") { Value = cwareId },
                new SQLiteParameter("$videoId") { Value = videoId });
            try
            {
                if (dt != null && dt.Rows.Count > 0)
                {
                    var fileName = dt.Rows[0]["LocalFile"].ToString();
                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        SystemInfo.TryDeleteFile(fileName);
                        SystemInfo.TryDeleteFile(fileName + Util.DownloadConfigExtension);
                        SystemInfo.TryDeleteFile(fileName + Util.DownloadFileExtension);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
            }
            return ExecuteNonQuery("Delete from StudentCwareDownload where cwareId = $Cwareid And VideoId = $VideoId And ForImport = 1",
                new SQLiteParameter("$cwareId") { Value = cwareId },
                new SQLiteParameter("$videoId") { Value = videoId }) >= 0;
        }

        public bool CancelDonwload(long downId)
        {
            DataTable dt = ExecuteTable("Select LocalFile,CwareId,VideoId,ForImport From StudentCwareDownload where downId=$downId",
                new SQLiteParameter("$downId", downId));
            if (dt == null || dt.Rows.Count == 0) return true;
            var cwareId = Convert.ToInt32(dt.Rows[0]["CwareId"]);
            var videoId = dt.Rows[0]["VideoId"] as string;
            var forImport = (int)dt.Rows[0]["ForImport"];
            Log.RecordData("DeleteDownTask", cwareId, videoId);
            try
            {
                if (forImport == 1)
                {
                    ExecuteNonQuery("Delete From StudentCwareDown Where CwareId= $CwareId And VideoId = $VideoId And State <> 3",
                        new SQLiteParameter("$CwareId", cwareId),
                        new SQLiteParameter("$VideoId", videoId));
                }
                var fileName = dt.Rows[0]["LocalFile"].ToString();
                if (!string.IsNullOrWhiteSpace(fileName))
                {
                    SystemInfo.TryDeleteFile(fileName);
                    SystemInfo.TryDeleteFile(fileName + Util.DownloadConfigExtension);
                    SystemInfo.TryDeleteFile(fileName + Util.DownloadFileExtension);
                }
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
            }
            return ExecuteNonQuery("Delete from StudentCwareDownload where downId=$downId", new SQLiteParameter("$downId", downId)) >= 0;
        }

        /// <summary>
        /// 更新上次听课时间
        /// </summary>
        /// <param name="ssoUid"></param>
        /// <param name="cwareId"></param>
        /// <param name="videoId"></param>
        /// <param name="lastPosition"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        public bool UpdateTime(int ssoUid, int cwareId, string videoId, int lastPosition, int duration)
        {
            //var has = (long)ExecuteScalar("Select Count(*) From StudentVideoRecord Where CwareId=$CwareId And VideoId=$VideoId And SSOUId=$SSOUId",
            //    new SQLiteParameter("$CwareId", cwareId),
            //    new SQLiteParameter("$VideoId", videoId),
            //    new SQLiteParameter("$SSOUId", ssoUid)
            //    );
            DataTable has = ExecuteTable("Select * From StudentVideoRecord Where CwareId=$CwareId And VideoId=$VideoId And SSOUId=$SSOUId",
                new SQLiteParameter("$CwareId", cwareId),
                new SQLiteParameter("$VideoId", videoId),
                new SQLiteParameter("$SSOUId", ssoUid)
                );
            var maxLastPostion = 0;
            if (has != null && has.Rows.Count > 0)
            {
                var obj = has.Rows[0]["maxLastPosition"].ToString();
                if (!string.IsNullOrEmpty(obj))
                {
                    maxLastPostion = Convert.ToInt32(obj);
                    if (maxLastPostion < lastPosition)
                    {
                        maxLastPostion = lastPosition;
                    }
                }
                else
                {
                    maxLastPostion = lastPosition;
                }
                return ExecuteNonQuery("Update StudentVideoRecord Set LastPosition=$LastPosition,Duration=Duration + $Duration,LastTime=datetime('now'),MaxLastPosition=$MaxLastPosition Where CwareId=$CwareId And VideoId=$VideoId And SSOUId=$SSOUId",
                       new SQLiteParameter("$CwareId", cwareId),
                       new SQLiteParameter("$VideoId", videoId),
                       new SQLiteParameter("$LastPosition", lastPosition),
                       new SQLiteParameter("$Duration", duration),
                       new SQLiteParameter("$MaxLastPosition", maxLastPostion),
                       new SQLiteParameter("$SSOUId", ssoUid)
                       ) >= 0;
            }
            return ExecuteNonQuery("Insert Into StudentVideoRecord(SSOUId,CwareId,VideoId,LastPosition,Duration,MaxLastPosition,LastTime) Values($SSOUId,$CwareId,$VideoId,$LastPosition,$Duration,$MaxLastPosition,datetime('now'))",
                new SQLiteParameter("$CwareId", cwareId),
                new SQLiteParameter("$VideoId", videoId),
                new SQLiteParameter("$LastPosition", lastPosition),
                new SQLiteParameter("$Duration", duration),
                new SQLiteParameter("$MaxLastPosition", lastPosition),
                new SQLiteParameter("$SSOUId", ssoUid)
                ) >= 0;
        }

        public int GetVideoPosition(int ssoUid, int cwareId, string videoId)
        {
            const string sql = "Select LastPosition from StudentVideoRecord Where ssouid=$ssouid And Cwareid=$CwareId And VideoId=$VideoId";
            var obj = ExecuteScalar(sql, new SQLiteParameter("$CwareId", cwareId),
                new SQLiteParameter("$VideoId", videoId),
                new SQLiteParameter("$SSOUId", ssoUid));
            if (obj == null || obj == DBNull.Value)
            {
                return 0;
            }
            return Convert.ToInt32(obj);
        }

        public void ClearCourse()
        {
            try
            {
                ExecuteNonQuery("Delete From StudentCourse");
                ExecuteNonQuery("Delete From StudentCourseWare");
                //ExecuteNonQuery("Delete From StudentCourseDetail");
                //ExecuteNonQuery("Delete From StudentCwareChapter");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        //
        public IEnumerable<ViewStudentWareKcjyDown> GetCwareKcjyDown(int cwareId)
        {
            DataTable dt = ExecuteTable("Select SmallListId,SmallListName,CwareId,JiangyiFile,SmallOrder from StudentCwareKcjyDown Where CwareID =$CwareID Order By SmallOrder desc", new SQLiteParameter("$CwareID") { Value = cwareId });
            if (dt == null || dt.Rows.Count == 0) return new List<ViewStudentWareKcjyDown>();
            return dt.AsEnumerable().Select(x => new ViewStudentWareKcjyDown
            {
                CwareId = x.Field<int>("CwareId"),
                JiangyiFile = x.Field<string>("JiangyiFile"),
                SmallListId = x.Field<int>("SmallListId"),
                SmallListName = x.Field<string>("SmallListName"),
                SmallOrder = x.Field<int>("SmallOrder")
            });
        }

        internal bool UpdateCwareKey(string cwId, string cwareKey)
        {
            const string sql = "Update StudentCourseWare Set CwareKey = $CwareKey Where CwId = $CwId";
            return ExecuteNonQuery(sql, new SQLiteParameter("$CwareKey") { Value = cwareKey }, new SQLiteParameter("$CwId") { Value = cwId }) >= 0;
        }

        public IEnumerable<ViewStudentWareDetail> GetMobileDownloadDetail(int cwareId, int downloadType)
        {
            var downType = downloadType == 1 ? "VideoHdZipUrl" : downloadType == 2 ? "AudioHdZipUrl" : downloadType == 3 ? "VideoZipUrl" : "AudioZipUrl";
            var sql = string.Format(GetMobileDownDetailSql, downType);
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$DownType") { Value = downloadType });
            if (dt == null) return new List<ViewStudentWareDetail>();
            return dt.AsEnumerable().Select(x =>
            {
                var downId = x.Field<long?>("DownId");
                return new ViewStudentWareDetail
                {
                    ChapterId = x.Field<int>("ChapterId"),
                    ChapterName = x.Field<string>("ChapterName"),
                    CwareId = x.Field<int>("CwareId"),
                    VideoId = x.Field<string>("VideoId"),
                    VideoLength = x.Field<string>("Length"),
                    VideoName = x.Field<string>("VideoName"),
                    Title = x.Field<string>("Title"),
                    VideoState = (int)x.Field<long>("VideoState"),
                    VideoPath = x.Field<string>("LocalFile"),
                    DownId = downId.HasValue ? downId.Value : 0,
                    VideoType = x.Field<int>("VideoType")
                };
            }).ToList();
        }

        public IEnumerable<PointTestStartTimeItem> GetPointTestStartTimeList(int cwareId, string videoId)
        {
            const string sql = "Select CwareId,VideoId,PointName,TestId,PointTestStartTime,PointOpenType,BackTime From PointTestStartTime Where CwareId = $Cwareid And VideoId = $VideoId";
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$CwareId") { Value = cwareId },
                new SQLiteParameter("$VideoId") { Value = videoId });
            return dt.AsEnumerable().Select(x => new PointTestStartTimeItem
            {
                CwareId = x.Field<int>("CwareId"),
                //LastUpdate = x.Field<DateTime>("CwareId"),
                PointName = x.Field<string>("PointName"),
                PointOpenType = x.Field<string>("PointOpenType"),
                PointTestStartTime = x.Field<int>("PointTestStartTime"),
                TestId = x.Field<int>("TestId"),
                VideoId = x.Field<string>("VideoId"),
                BackTime = x.Field<int>("BackTime")
            }).OrderBy(x => new object().GetHashCode());
        }

        public IEnumerable<PointTestQuestionItem> GetPointTestQuestionList(int testId)
        {
            const string sql = "Select QuestionId,ParentId,PointTestId,QuesViewType,Content,Score,RightAnswer,Analysis,SplitScore,PointId,QuesType From PointTestQuestion Where PointTestId = $PointTestId";
            DataTable dt = ExecuteTable(sql, new SQLiteParameter("$PointTestId") { Value = testId });
            return dt.AsEnumerable().Select(x =>
            {
                var item = new PointTestQuestionItem
                {
                    Analysis = x.Field<string>("Analysis"),
                    Content = x.Field<string>("Content"),
                    ParentId = x.Field<int>("ParentId"),
                    PointId = x.Field<int>("PointId"),
                    PointTestId = x.Field<int>("PointTestId"),
                    QuestionId = x.Field<int>("QuestionId"),
                    QuesType = x.Field<int>("QuesType"),
                    QuesViewType = x.Field<int>("QuesViewType"),
                    RightAnswer = x.Field<string>("RightAnswer"),
                    Score = x.Field<double>("Score"),
                    SplitScore = x.Field<double>("SplitScore")
                };
                const string sqlop =
                    "Select QuestionId,QuesValue,QuesOption,Sequence From PointTestQuestionOption Where QuestionId = $QuestionId Order By Sequence";
                DataTable dtt = ExecuteTable(sqlop, new SQLiteParameter("$QuestionId") { Value = item.QuestionId });
                item.QuestionOptionList = dtt.AsEnumerable().Select(y => new PointTestQuestionOptionItem
                {
                    QuesOption = y.Field<string>("QuesOption"),
                    QuestionId = y.Field<int>("QuestionId"),
                    QuesValue = y.Field<string>("QuesValue"),
                    Sequence = y.Field<int>("Sequence")
                });
                return item;
            });
        }

        public IEnumerable<ViewStudentCwareSetting> GetCwareSetting(string subjectName)
        {
            const string sql = "Select C.EduSubjectId,C.CwareName,C.CwareId,Cs.UserHide,Cs.UserShowOrder From StudentEduSubject S Inner Join StudentCware C On S.EduSubjectId = C.EduSubjectId Left outer Join StudentCwareSetting Cs On C.EduSubjectId = Cs.EduSubjectId And C.CwareId = Cs.CwareId And Cs.Uid=$Uid Where C.DateEnd > datetime('now', 'localtime') And S.EduSubjectName = $EduSubjectName Order By Cs.[UserShowOrder],C.CYearName Desc, C.ClassOrder";
            DataTable dt = ExecuteTable(sql,
                new SQLiteParameter("$Uid") { Value = Util.SsoUid },
                new SQLiteParameter("$EduSubjectName") { Value = subjectName }
                );
            if (dt == null || dt.Rows.Count == 0) return new ViewStudentCwareSetting[0];
            return dt.AsEnumerable().Select(x =>
            {
                var item = new ViewStudentCwareSetting
                {
                    EduSubjectId = x.Field<int>("EduSubjectId"),
                    CwareId = x.Field<int>("CwareId"),
                    CwareName = x.Field<string>("CwareName")
                };
                var userHide = x.Field<int?>("UserHide");
                item.UserHide = userHide.HasValue ? userHide.Value : 0;
                var userShowOrder = x.Field<int?>("UserShowOrder");
                item.UserShowOrder = userShowOrder.HasValue ? userShowOrder.Value : 0;
                return item;
            });
        }

        public bool UpdateStudentCwareSetting(IEnumerable<ViewStudentCwareSetting> list)
        {
            const string sql = "Insert or Replace Into StudentCwareSetting(Uid,EduSubjectId,CwareId,UserHide,UserShowOrder) Values($Uid,$EduSubjectId,$CwareId,$UserHide,$UserShowOrder)";
            OpenConn();
            SQLiteTransaction tran = Conn.BeginTransaction();
            try
            {
                foreach (ViewStudentCwareSetting item in list)
                {
                    var cmd = new SQLiteCommand(sql, Conn, tran);
                    cmd.Parameters.Add(new SQLiteParameter("$Uid") { Value = Util.SsoUid });
                    cmd.Parameters.Add(new SQLiteParameter("$EduSubjectId") { Value = item.EduSubjectId });
                    cmd.Parameters.Add(new SQLiteParameter("$CwareId") { Value = item.CwareId });
                    cmd.Parameters.Add(new SQLiteParameter("$UserHide") { Value = item.UserHide });
                    cmd.Parameters.Add(new SQLiteParameter("$UserShowOrder") { Value = item.UserShowOrder });
                    cmd.ExecuteNonQuery();
                }
                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
                tran.Rollback();
                return false;
            }
            finally
            {
                CloseConn();
            }

        }

        #endregion
        /// <summary>
        /// 对下载视频路径需要加参数
        /// </summary>
        /// <param name="downUrl"></param>
        /// <returns></returns>
        public string GetDownUrl(string downUrl)
        {
            if (string.IsNullOrWhiteSpace(downUrl)) return string.Empty;
            var time = Util.GetNowString();
            var key = Crypt.Md5(Util.SessionId, time, "fJ3UjIFyTu");
            var url = downUrl + "?uid=" + Util.SessionId + "&ptime=" + time + "&pkey=" + key;
            return url;
        }
        /// <summary>
        /// 获取听课记录信息  dgh 2017.03.21
        /// </summary>
        /// <returns></returns>
        public List<ViewCourseRecord> GetCourseRecord()
        {
            var sql = "select VR.[CwareID],C.[EduSubjectId],S.[EduSubjectName] as CourseName,C.[CwareName] as CourseWareName,SD.[VideoName],VR.[VideoID],SC.[LocalFile],max(VR.[LastTime]) as LastTime from StudentVideoRecord VR inner join StudentCWareDown SC on VR.[CwareID]=SC.CwareID and VR.VideoID=SC.VideoID inner join StudentCourseDetail SD on VR.[CwareID]=SD.CwareID and VR.VideoID=SD.VideoID inner join StudentCware C on SD.[CwareID]=C.CwareID inner join StudentEduSubject S on C.[EduSubjectId]=S.EduSubjectId where VR.[SSOUID]=$SSOUID group by VR.[CwareID]";
            DataTable dt = ExecuteTable(sql, new SQLiteParameter("$SSOUID") { Value = Util.SsoUid });
            if (dt == null) return new List<ViewCourseRecord>();
            return dt.AsEnumerable().Select(x =>
            {
                var item = new ViewCourseRecord
                {
                    EduSubjectId = x.Field<int>("EduSubjectId"),
                    CourseName = x.Field<string>("CourseName"),
                    CwareId = x.Field<int>("CwareId"),
                    VideoId = x.Field<string>("VideoId"),
                    CourseWareName = x.Field<string>("CourseWareName"),
                    VideoName = x.Field<string>("VideoName"),
                    LocalFile = x.Field<string>("LocalFile")
                };
                return item;
            }).ToList();
        }
        /// <summary>
        /// 获取听课记录的相关信息  主要使用于时长的百分比 dgh 2018.03.07
        /// </summary>
        /// <param name="cwareId"></param>
        /// <returns></returns>
        public IEnumerable<CourseRecordOtherInfo> GetCourseRecordByCwareId(int cwareId)
        {
            var sql = @"select SD.[CwareID],C.[CwareName] as CourseWareName,SD.[VideoName],SD.[Length],SD.[VideoID],VR.[LastPosition],VR.MaxLastPosition,VR.[SSOUID] from StudentCourseDetail SD  inner join StudentCware C on SD.[CwareID]=C.CwareID left join  StudentVideoRecord VR on VR.[CwareID]=SD.CwareID and VR.VideoID=SD.VideoID where C.[MobileCourseOpen]=1 and C.[Download]='Y' and SD.VideoType <> 3 and SD.[CwareID]=$CwareID";
            DataTable dt = ExecuteTable(sql, new SQLiteParameter("$CwareID") { Value = cwareId });
            if (dt == null) return new List<CourseRecordOtherInfo>();
            return dt.AsEnumerable().Select(x =>
            {
                var item = new CourseRecordOtherInfo
                {
                    LastPosition = x.Field<int?>("LastPosition"),
                    MaxLastPosition = x.Field<int?>("MaxLastPosition"),
                    SSOUID = x.Field<int?>("SSOUID"),
                    VideoLength = x.Field<string>("Length")
                };
                return item;
            }).ToList();
        }


        /// <summary>
        /// 删除听课记录信息 dgh 2017.03.23
        /// </summary>
        /// <param name="cwareId">课程ID</param>
        /// <param name="videoId">视频ID</param>
        /// <returns></returns>
        public bool DeleteStudentVideoRecord(int cwareId, string videoId)
        {
            try
            {
                return ExecuteNonQuery("Delete From StudentVideoRecord Where CwareId=$CwareId And VideoId =$VideoId And SSOUID=$SSOUID",
                        new SQLiteParameter("$SSOUID", Util.SsoUid),
                        new SQLiteParameter("$CwareId", cwareId),
                        new SQLiteParameter("$VideoId", videoId)) > 0;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                return false;
            }
        }

        #region 同步学习记录表 dgh 2018.06.01
        /// <summary>
        /// 添加学习记录
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddStudentVideoTimebaseItem(TimebaseStr item)
        {
            OpenConn();

            try
            {
                const string sql = "Insert Into StudentVideoTimebase(SSOUID,CwareID,VideoID,videoStartTime,videoEndTime,speed,studyTimeEnd,studyTimeStart,LastTime) Values($SSOUID,$CwareID,$VideoID,$videoStartTime,$videoEndTime,$speed,$studyTimeEnd,$studyTimeStart,datetime('now', 'localtime'))";
                var cmd = new SQLiteCommand(sql, Conn);
                cmd.Parameters.Add(new SQLiteParameter("$SSOUID") { Value = Util.SsoUid });
                cmd.Parameters.Add(new SQLiteParameter("$CwareID") { Value = item.CwareId });
                cmd.Parameters.Add(new SQLiteParameter("$VideoID") { Value = item.VideoID });
                cmd.Parameters.Add(new SQLiteParameter("$videoStartTime") { Value = item.VideoStartTime });
                cmd.Parameters.Add(new SQLiteParameter("$videoEndTime") { Value = item.VideoEndTime });
                cmd.Parameters.Add(new SQLiteParameter("$speed") { Value = item.Speed });
                cmd.Parameters.Add(new SQLiteParameter("$studyTimeEnd") { Value = item.StudyTimeEnd });
                cmd.Parameters.Add(new SQLiteParameter("$studyTimeStart") { Value = item.StudyTimeStart });
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.Message);
                return false;
            }
            finally
            {
                CloseConn();
            }
        }

        /// <summary>
        /// 获取学习记录信息 dgh 2018.06.01
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VideoStr> StudyVideoStrList()
        {
            try
            {
                const string sql = "Select distinct D.[CwareID],D.[VideoID],D.Length from StudentCourseDetail D Inner Join StudentVideoTimebase A On A.[CwareID]=D.CwareId And A.[VideoID] = D.VideoId Where A.[SSOUID]=$SSOUID order by A.[CwareID],A.[VideoID],A.[studyTimeEnd]";
                DataTable dt = ExecuteTable(sql, new SQLiteParameter("$SSOUID") { Value = Util.SsoUid });
                if (dt == null || dt.Rows.Count == 0) return new List<VideoStr>();
                return dt.AsEnumerable().Select(x =>
                {
                    var rangeEnd = Convert.ToDateTime(x.Field<string>("Length")).TimeOfDay.TotalSeconds;
                    var item = new VideoStr
                    {
                        CwareId = x.Field<int>("CwareID").ToString(CultureInfo.InvariantCulture),
                        DeviceID = Util.DeviceId,
                        RangeEnd = rangeEnd.ToString(CultureInfo.InvariantCulture),
                        RangeStart = "0",
                        VideoID = x.Field<string>("VideoID"),

                    };
                    const string sql2 = "Select A.[SSOUID],A.[CwareID],A.[VideoID],A.[videoStartTime],A.[videoEndTime],A.[speed],A.[studyTimeStart],A.[studyTimeEnd],A.[LastTime] from StudentVideoTimebase A Inner Join StudentCourseDetail D On A.[CwareID]=D.CwareId And A.[VideoID] = D.VideoId Where A.[SSOUID]=$SSOUID and A.[CwareID]=$CwareID and A.VideoID=$VideoID order by A.[CwareID],A.[VideoID],A.[studyTimeEnd] limit 50";

                    DataTable dtt = ExecuteTable(sql2,
                        new SQLiteParameter("$SSOUID") { Value = Util.SsoUid },
                        new SQLiteParameter("$CwareID") { Value = item.CwareId },
                        new SQLiteParameter("$VideoID") { Value = item.VideoID }
                        );
                    item.Timebase = dtt.AsEnumerable().Select(y => new TimebaseStr
                    {
                        VideoStartTime = y.Field<int>("videoStartTime").ToString(CultureInfo.InvariantCulture),
                        VideoEndTime = y.Field<int>("videoEndTime").ToString(CultureInfo.InvariantCulture),
                        Speed = y.Field<string>("speed"),
                        StudyTimeEnd = y.Field<string>("studyTimeEnd"),
                        StudyTimeStart = y.Field<string>("studyTimeStart"),
                        CwareId = x.Field<int>("CwareID"),
                        VideoID = x.Field<string>("VideoID"),
                    }).ToList();

                    return item;
                });
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.Message);
                throw;
            }

        }
        /// <summary>
        /// 删除学习记录 dgh 2018.06.01
        /// </summary>
        /// <param name="cwareId">课件号</param>
        /// <param name="videoId">视频编号</param>
        /// <param name="studyEndTime">最后一次听课时间戳</param>
        /// <returns></returns>
        public bool DeleteTimebase(string cwareId, string videoId, string studyEndTime)
        {
            try
            {
                return ExecuteNonQuery("Delete From StudentVideoTimebase Where CwareId=$CwareId And VideoId =$VideoId And SSOUID=$SSOUID and studyTimeEnd=$studyTimeEnd",
                        new SQLiteParameter("$SSOUID", Util.SsoUid),
                        new SQLiteParameter("$CwareId", cwareId),
                        new SQLiteParameter("$VideoId", videoId),
                        new SQLiteParameter("$studyTimeEnd", studyEndTime)
                        ) > 0;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// 查询听课信息
        /// </summary>
        /// <param name="cwareId"></param>
        /// <param name="videoID"></param>
        /// <param name="pst">开始时间点</param>
        /// <param name="pe">结束时间点</param>
        /// <returns></returns>
        public int StudyVideoStrById(int cwareId, string videoID, int pst, int pe)
        {
            try
            {

                const string sql2 = "select * from StudentVideoTimebase A where A.[CwareID]=$CwareID and A.[VideoID]=$VideoID and A.[videoStartTime]=$videoStartTime and A.[videoEndTime]=$videoEndTime";

                DataTable dtt = ExecuteTable(sql2,
                    new SQLiteParameter("$SSOUID") { Value = Util.SsoUid },
                    new SQLiteParameter("$CwareID") { Value = cwareId },
                    new SQLiteParameter("$VideoID") { Value = videoID },
                    new SQLiteParameter("$videoStartTime") { Value = pst },
                    new SQLiteParameter("$videoEndTime") { Value = pe }
                    );
                if (dtt != null && dtt.Rows.Count > 0) return 1;
                return 0;
            }
            catch (Exception ex)
            {
                Log.RecordLog(ex.Message);
                throw;
            }

        }

        #endregion

    }
}
