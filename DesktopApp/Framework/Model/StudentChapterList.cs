using System;

namespace Framework.Model
{
    public class StudentChapterList
    {
        public int ChapterListId { get; set; }

        public string ChapterListName { get; set; }

        public int Sequence { get; set; }

        public int Status { get; set; }

        public string Creator { get; set; }

        public DateTime CreateTime { get; set; }

        public int CourseId { get; set; }

        public int ChapterNum { get; set; }
    }
}
