using System;

namespace Framework.Model
{
    public class StudentSiteCourse
    {
        public int SiteCourseId { get; set; }
        public int CourseId { get; set; }
        public string SiteCourseName { get; set; }
        public string CourseChapter { get; set; }
        public int Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string BoardId { get; set; }
    }
}
