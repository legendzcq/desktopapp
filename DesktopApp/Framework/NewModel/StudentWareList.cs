using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.NewModel
{
    [DataContract]
    public class StudentWareList
    {
        [DataMember(Name = "uid")]
        public string Uid { get; set; }

        [DataMember(Name = "courseID")]
        public string CourseId { get; set; }

        [DataMember(Name = "cwList")]
        public IEnumerable<StudentWareItem> CwList { get; set; }

        [DataContract]
        public class StudentWareItem
        {
            [DataMember(Name = "cwareUrl")]
            public string CwareUrl { get; set; }

            [DataMember(Name = "boardID")]
            public string BoardId { get; set; }

            [DataMember(Name = "cwareImg")]
            public string CwareImg { get; set; }

            [DataMember(Name = "updateTime")]
            public string UpdateTime { get; set; }

            [DataMember(Name = "cwareName")]
            public string CwareName { get; set; }

            [DataMember(Name = "classOrder")]
            public string ClassOrder { get; set; }

            [DataMember(Name = "cwareClassName")]
            public string CwareClassName { get; set; }

            [DataMember(Name = "teacherName")]
            public string TeacherName { get; set; }

            [DataMember(Name = "mobileCourseOpen")]
            public string MobileCourseOpen { get; set; }

            [DataMember(Name = "videoType")]
            public string VideoType { get; set; }

            [DataMember(Name = "cYearName")]
            public string CYearName { get; set; }

            [DataMember(Name = "cwareID")]
            public string CwareId { get; set; }

            [DataMember(Name = "cwID")]
            public string CwId { get; set; }

            [DataMember(Name = "cwareTitle")]
            public string CwareTitle { get; set; }

            [DataMember(Name = "cwareClassID")]
            public string CwareClassId { get; set; }

            [DataMember(Name = "rowNum")]
            public string RowNum { get; set; }

            [DataMember(Name = "useFul")]
            public string UseFul { get; set; }

        }
    }
}
