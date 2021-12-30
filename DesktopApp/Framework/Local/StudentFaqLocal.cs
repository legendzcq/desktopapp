using Framework.Model;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Framework.Local
{
	public class StudentFaqLocal : DataAccessBase
	{
        private const string AddFaq = @"INSERT or REPLACE INTO StudentFaq(faqID,topicID,categoryID,boardID,title,content,majorID,createptime) values($faqID,$topicID,$categoryID,$boardID,$title,$content,$majorID,$createptime)";
        /// <summary>
        /// 添加课堂提问信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddFaqInfo(StudentFaqQues item)
        {
            var pars = new SQLiteParameter[] { 
            new SQLiteParameter("$faqID",item.FaqId),
            new SQLiteParameter("$topicID",item.TopicId),
            new SQLiteParameter("$categoryID",item.CategoryId),
            new SQLiteParameter("$boardID",item.BoardId),
            new SQLiteParameter("$title",item.Title),
            new SQLiteParameter("$content",item.Content),
            new SQLiteParameter("$majorID",item.MajorId),
            new SQLiteParameter("$createptime",item.CreatePtime)
            };
            return ExecuteNonQuery(AddFaq, pars) > 0;

        }
	}
}
