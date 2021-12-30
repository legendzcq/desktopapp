using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Model
{
   public class StudentFaqQuestion
    {
        /// <summary>
        /// 答疑ID
        /// </summary>
        public int FaqID { get; set; }
        /// <summary>
        /// 主题ID
        /// </summary>
        public int TopicID { get; set; }
        /// <summary>
        /// 提问方式ID
        /// </summary>
        public string CategoryID { get; set; }
        /// <summary>
        /// 课程ID
        /// </summary>
        public int BoardID { get; set; }
        /// <summary>
        /// 答疑标题 
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 问题正文
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 问题ID 
        /// </summary>
        public string QNo { get; set; }
        /// <summary>
        /// 是否免费
        /// </summary>
        public bool? IsFree { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool? Isvalid { get; set; }
        /// <summary>
        /// 章节号
        /// </summary>
        public string ChapterNum { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime createTime { get; set; }
    }
}
