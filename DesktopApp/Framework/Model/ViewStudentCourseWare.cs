
namespace Framework.Model
{
    public class ViewStudentCourseWare
    {
        public int CourseEduId { get; set; }
        public int BoardId { get; set; }
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public string CourseWareName { get; set; }
        public string CWareClassName { get; set; }
        public string CTeacherName { get; set; }
        public string CYearName { get; set; }
        public int CwareId { get; set; }
        public string CwId { get; set; }
        public bool IsOpen { get; set; }
        public bool CanDownload { get; set; }
        public string VideoType { get; set; }
        public int EduSubjectId { get; set; }

        public int FileCount { get; set; }
        public string FileSizes { get; set; }

        public int BigOrder { get; set; }
        public int Order { get; set; }

        public string CwareUrl { get; set; }

        internal int UserShowOrder { get; set; }
        internal int UserHide { get; set; }

        public int IsFree { get; set; }

        public bool CanDelete => FileCount > 0;
    }
}
