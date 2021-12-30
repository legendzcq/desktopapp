using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.Model
{
    public class ViewCourseRecord
    {
        /// <summary>
        /// 课程ID
        /// </summary>
        public int CwareId { get; set; }
        /// <summary>
        /// 科目ID
        /// </summary>
        public int EduSubjectId { get; set; }
        /// <summary>
        /// 科目名称
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 课程名称
        /// </summary>
        public string CourseWareName { get; set; }
        /// <summary>
        /// 视频名称
        /// </summary>
        public string VideoName { get; set; }
        /// <summary>
        /// 视频ID
        /// </summary>
        public string VideoId { get; set; }
        /// <summary>
        /// 视频文件路径
        /// </summary>
        public string LocalFile { get; set; }

        /// <summary>
        /// 名称拼接
        /// </summary>
        public string FullName {
            get { return CourseName + "--" + CourseWareName; }
        }
        /// <summary>
        /// 班次下的总时长
        /// </summary>
        public double TotalLength { get; set; }

        /// <summary>
        /// 班次下完成总时长
        /// </summary>
        public double FinishVideoLength { get; set; }

        /// <summary>
        ///完成的百分比 
        /// </summary>
        public string FinishPersent { get; set; }
    }

    public class CourseRecordOtherInfo
    {
        /// <summary>
        /// 最后听课位置
        /// </summary>
        public int? LastPosition { get; set; }

        /// <summary>
        /// 视频时长
        /// </summary>
        public string VideoLength { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? SSOUID { get; set; }

        /// <summary>
        /// 最后听课的最大位置
        /// </summary>
        public int? MaxLastPosition { get; set; }
    }
}
