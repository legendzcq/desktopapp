using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Model
{
   public class StudentFaqQues
    {
        /// <summary>
        /// 答疑ID
        /// </summary>
        public int FaqId { get; set; }
        /// <summary>
        /// 主题ID
        /// </summary>
        public int TopicId { get; set; }
        /// <summary>
        /// 提问方式ID
        /// </summary>
        public string CategoryId { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        public int BoardId { get; set; }
        /// <summary>
        /// 答疑标题 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 问题正文
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 辅导ID
        /// </summary>
        public int MajorId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreatePtime { get; set; }
    }
}
