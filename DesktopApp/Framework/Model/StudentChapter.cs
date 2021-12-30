using System;

namespace Framework.Model
{
    public class StudentChapter
    {
        public int ChapterId { get; set; }

        public int ChapterListId { get; set; }

        public string ChapterName { get; set; }

        public int Sequence { get; set; }

        public int ShowStatus { get; set; }

        public int Status { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public int CourseId { get; set; }

        public int ChapterNum { get; set; }
    }
}
