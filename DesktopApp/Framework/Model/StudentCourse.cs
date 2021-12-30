using System;
using System.Runtime.Serialization;

namespace Framework.Model
{
    public class StudentCourse
    {
        public string CourseId { get; set; }
        public string Name { get; set; }
        public DateTime EndDate { get; set; }
        public int BoardId { get; set; }
		public int EduSubjectId { get; set; }
		public int CourseEduId { get; set; }
		public int Disporder { get; set; }
    }
}
