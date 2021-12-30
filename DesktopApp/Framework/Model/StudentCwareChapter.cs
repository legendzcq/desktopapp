
using System.Runtime.Serialization;
namespace Framework.Model
{
    [DataContract]
    public class StudentCwareChapter
    {
        public int CWareId { get; set; }
        /// <summary>
        /// 章节ID
        /// </summary>
        [DataMember(Name = "chapterid")]
        public int ChapterId { get; set; }
        /// <summary>
        /// 章节名称
        /// </summary>
        [DataMember(Name = "chaptertname")]
        public string ChapterName { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        [DataMember(Name = "order")]
        public int Order { get; set; }
    }
}
