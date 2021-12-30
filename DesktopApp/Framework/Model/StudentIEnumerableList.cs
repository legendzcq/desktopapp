using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.Model
{
    [DataContract]
    public class StudentIEnumerableList
    {
        /// <summary>
        /// 课件视频集合
        /// </summary>
        [DataMember(Name = "courseware")]
        public IEnumerable<StudentCourseDetail> CourseWareList { get; set; }
        /// <summary>
        /// 章节集合
        /// </summary>
        [DataMember(Name = "chapterlist")]
        public IEnumerable<StudentCwareChapter> ChapterList { get; set; }
    }
}
