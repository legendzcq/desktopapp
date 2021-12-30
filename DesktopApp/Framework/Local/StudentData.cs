using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Text;

using Framework.Model;
using Framework.Push;
using Framework.Remote;
using Framework.Utility;

namespace Framework.Local
{
    public class StudentData : DataAccessBase
    {
        private const string GetStudentSql = "Select UserName,Password,SSOUID,LastLogin from Studentinfo";
        private const string UpdateLoginInfo = "INSERT or REPLACE INTO Studentinfo(UserName,Password,SSOUID,LastLogin) Values($UserName,$Password,$SSOUID,$LastLogin)";

        /// <summary>
        /// 获取当前已登录的用户
        /// </summary>
        /// <returns></returns>
        public StudentInfo GetLoginedStudent()
        {
            DataTable dt = ExecuteTable(GetStudentSql);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow rw = dt.Rows[0];
                var item = new StudentInfo
                {
                    UserName = rw.Field<string>("UserName"),
                    Password = Crypt.Rc4DecryptString(rw.Field<string>("Password")).Trim(),
                    Ssouid = rw.Field<int>("SSOUID"),
                    LastLogin = rw.Field<DateTime>("LastLogin")
                };
                return item;
            }
            return null;
        }

        /// <summary>
        /// 添加本地登录信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddLoginInfo(StudentInfo item)
        {
            item.Password = new string(' ', 5) + item.Password + new string(' ', 5);
            return ExecuteNonQuery(UpdateLoginInfo,
                new SQLiteParameter("$UserName", DbType.String) { Value = item.UserName },
                new SQLiteParameter("$Password", DbType.String) { Value = Crypt.Rc4EncryptString(item.Password) },
                new SQLiteParameter("$SSOUID", DbType.Int32) { Value = item.Ssouid },
                new SQLiteParameter("$LastLogin", DbType.DateTime) { Value = item.LastLogin }) > 0;
        }

        /// <summary>
        /// 注销
        /// </summary>
        public void Logout()
        {
            ExecuteNonQuery("Delete From StudentCWareDown Where State<>3");
            ExecuteNonQuery("Delete From StudentCwareDownload Where DownState<>3");
            ExecuteNonQuery("Delete From StudentSiteCourse");
            ExecuteNonQuery("Delete From StudentCenter");
            ExecuteNonQuery("Delete From StudentCourse");
            ExecuteNonQuery("Delete From StudentCourseWare");
            ExecuteNonQuery("Delete From StudentCourseDetail");
            ExecuteNonQuery("Delete From StudentCwareChapter");
            ExecuteNonQuery("Delete From Studentinfo");
            ExecuteNonQuery("Delete From StudentCware");
            ExecuteNonQuery("Delete From StudentEduSubject");
            ExecuteNonQuery("Delete From StudentSubjectCourseRelation");
            ExecuteNonQuery("Delete From PushMessage");
            ExecuteNonQuery("Delete From StudentCenterSubject");
            //删除题库数据 dgh 2017.04.24
            ExecuteNonQuery("Delete From StudentPaper");
            ExecuteNonQuery("Delete From StudentPaperPart");
            ExecuteNonQuery("Delete From StudentPaperView");
            ExecuteNonQuery("Delete From StudentQuestion");
            ExecuteNonQuery("Delete From StudentQuestionOption");
            //清除知识点的数据 dgh 2017.05.26
            ExecuteNonQuery("Delete From PointTestStartTime");
            ExecuteNonQuery("Delete From PointTestQuestion");
            ExecuteNonQuery("Delete From PointTestQuestionOption");
            SystemInfo.SaveSetting("LastMessage", 0);
        }

        public bool AddMessage(PushMessage message)
        {
            const string sql = "Select Count(*) From PushMessage Where Id = $Id";
            var obj = ExecuteScalar(sql, new SQLiteParameter("$Id") { Value = message.MessageId });
            if (obj != null && obj != DBNull.Value)
            {
                var cnt = Convert.ToInt32(obj);
                if (cnt > 0) return false;
            }
            const string sqlinsert = "Insert into PushMessage(Id,Type,Content,PushTime) Values($Id,$Type,$Content,datetime('now', 'localtime'))";
            return ExecuteNonQuery(sqlinsert,
                new SQLiteParameter("$Id") { Value = message.MessageId },
                new SQLiteParameter("$Type") { Value = message.MessageType },
                new SQLiteParameter("$Content") { Value = message.MessageBody }) > 0;
        }

        public bool RemoveMessage(int messageId)
        {
            const string sql = "Delete From PushMessage Where Id = $Id";
            return ExecuteNonQuery(sql, new SQLiteParameter("$id") { Value = messageId }) >= 0;
        }

        public IEnumerable<PushMessage> GetMessageList()
        {
            const string sql = "Select Id,Type,Content,PushTime From PushMessage Order By Id Desc";
            DataTable dt = ExecuteTable(sql);
            return dt.AsEnumerable().Select(x =>
            {
                var type = x.Field<int>("Type");
                var content = x.Field<string>("Content");
                DateTime time = x.Field<DateTime>("PushTime");
                if (type == 1)
                {
                    PushMessage item = WebProxyClient.JsonDeserialize<PushMessage>(content, Encoding.UTF8);
                    item.MessageBody = content;
                    item.MessageTime = time;
                    item.MessageType = type;
                    return item;
                }
                if (type == 2)
                {
                    PushLinkMessage item = WebProxyClient.JsonDeserialize<PushLinkMessage>(content, Encoding.UTF8);
                    item.MessageBody = content;
                    item.MessageTime = time;
                    item.MessageType = type;
                    return item;
                }
                return null;
            }).Where(x => x != null);
        }
    }
}
