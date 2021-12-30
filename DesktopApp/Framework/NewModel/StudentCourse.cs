using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.NewModel
{
    [DataContract]
    public class StudentCourse
    {
        [DataMember(Name = "uid")]
        public string Uid { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "myCourseInfo")]
        public IEnumerable<CourseItem> MyCourseInfo { get; set; }

        [DataContract]
        public class CourseItem
        {
            [DataMember(Name = "boardID")]
            public string BoardId { get; set; }

            [DataMember(Name = "uid")]
            public string Uid { get; set; }

            [DataMember(Name = "title")]
            public string Title { get; set; }

            [DataMember(Name = "eduSubjectID")]
            public int EduSubjectId { get; set; }

            [DataMember(Name = "courseID")]
            public string CourseId { get; set; }

            [DataMember(Name = "disporder")]
            public int Disporder { get; set; }

            [DataMember(Name = "mobileTitle")]
            public string MobileTitle { get; set; }

            [DataMember(Name = "dateEnd")]
            public string DateEnd { get; set; }

            [DataMember(Name = "downLoad")]
            public string DownLoad { get; set; }

            [DataMember(Name = "courseEduID")]
            public int CourseEduId { get; set; }
        }
    }
    [DataContract]
    public class StudentCourseReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentCourseLIst Result { get; set; }
    }
        [DataContract]
    public class StudentCourseLIst
    {
        [DataMember(Name = "uid")]
        public string Uid { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "myCourseInfo")]
        public IEnumerable<StudentEduSubjectItem> MyCourseInfo { get; set; }

        [DataMember(Name = "subectCourseRelation")]
        public IEnumerable<StudentSubjectCourseRelation> CourseRelation { get; set; }

        [DataContract]
        public class StudentEduSubjectItem
        {
            [DataMember(Name = "boardID")]
            public string BoardId { get; set; }

            [DataMember(Name = "eduSubjectName")]
            public string EduSubjectName { get; set; }

            [DataMember(Name = "orderNo")]
            public int OrderNo { get; set; }

            [DataMember(Name = "eduSubjectID")]
            public int EduSubjectId { get; set; }

            [DataMember(Name = "courseEduID")]
            public int CourseEduId { get; set; }

            //[DataMember(Name = "disporder")]
            [DataMember(Name = "dispOrder")]
            public int Disporder { get; set; }

            [DataMember(Name = "courseID")]
            public string CourseId { get; set; }

        }

        [DataContract]
        public class StudentSubjectCourseRelation
        {
            [DataMember(Name = "eduSubjectID")]
            public int EduSubjectId { get; set; }

            [DataMember(Name = "courseID")]
            public string CourseId { get; set; }
        }
    }

    [DataContract]
    public class StudentCwareReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentCwareList Result { get; set; }
    }

        [DataContract]
    public class StudentCwareList
    {
        [DataMember(Name = "uid")]
        public int Uid { get; set; }

        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "edusubjectID")]
        public string EduSubjectId { get; set; }

        [DataMember(Name = "cwList")]
        public IEnumerable<StudentCwareItem> CwareList { get; set; }

        [DataMember(Name = "videoList")]
        public IEnumerable<CwareProgress> CwareProgressList { get; set; }

        [DataContract]
        public class StudentCwareItem
        {
            /// <summary>
            /// 课件地址
            /// </summary>
            [DataMember(Name = "cwareUrl")]
            public string CwareUrl { get; set; }
            /// <summary>
            /// 答疑版ID
            /// </summary>
            [DataMember(Name = "boardID")]
            public int BoardId { get; set; }
            /// <summary>
            /// 课件图片
            /// </summary>
            [DataMember(Name = "cwareImg")]
            public string CwareImg { get; set; }

            [DataMember(Name = "updateTime")]
            public string UpdateTime { get; set; }
            /// <summary>
            /// 课件名称
            /// </summary>
            [DataMember(Name = "cwareName")]
            public string CwareName { get; set; }
            /// <summary>
            /// 关课时间
            /// </summary>
            [DataMember(Name = "dateEnd")]
            public string DateEnd { get; set; }
            /// <summary>
            /// 班次排序
            /// </summary>
            [DataMember(Name = "classOrder")]
            public int ClassOrder { get; set; }
            /// <summary>
            /// 课件班次名称
            /// </summary>
            [DataMember(Name = "cwareClassName")]
            public string CwareClassName { get; set; }
            /// <summary>
            /// 老师名称
            /// </summary>
            [DataMember(Name = "teacherName")]
            public string TeacherName { get; set; }

            [DataMember(Name = "mobileCourseOpen")]
            public int MobileCourseOpen { get; set; }
            /// <summary>
            /// 课件类型
            /// </summary
            [DataMember(Name = "videoType")]
            public int VideoType { get; set; }
            /// <summary>
            /// 课程年份
            /// </summary>
            [DataMember(Name = "cYearName")]
            public string CYearName { get; set; }

            [DataMember(Name = "rownum")]
            public int Rownum { get; set; }

            [DataMember(Name = "cwareID")]
            public int CwareId { get; set; }
            /// <summary>
            /// 课件ID
            /// </summary>
            [DataMember(Name = "cwID")]
            public string CwId { get; set; }
            /// <summary>
            /// 课件标题
            /// </summary>
            [DataMember(Name = "cwareTitle")]
            public string CwareTitle { get; set; }
            /// <summary>
            /// 课件班次ID
            /// </summary>
            [DataMember(Name = "cwareClassID")]
            public int CwareClassId { get; set; }
            /// <summary>
            /// 是否可下载
            /// </summary>
            [DataMember(Name = "downLoad")]
            public string Download { get; set; }

            [DataMember(Name = "useFul")]
            public int UseFul { get; set; }

            [DataMember(Name = "specialFlag")]
            public int SpecialFlag { get; set; }
            #region 新添加的字段 dgh 2018.01.02
            /// <summary>
            /// 课程开通简述
            /// </summary>
            [DataMember(Name = "courseOpenExplain")]
            public string CourseOpenExplain { get; set; }
            [DataMember(Name = "eduOrder")]
            public int EduOrder { get; set; }
            /// <summary>
            /// 是否显示年份：1、显示，0是不显示
            /// </summary>
            [DataMember(Name = " homeShowYear")]
            public int HomeShowYear { get; set; }
            [DataMember(Name = "isFree")]
            public int IsFree { get; set; }
            [DataMember(Name = " isMobileClass")]
            public int IsMobileClass { get; set; }
           
            /// <summary>
            /// 移动端显示
            /// </summary>
            [DataMember(Name = "mobileTitle")]
            public string MobileTitle { get; set; }
            /// <summary>
            /// 开通状态
            /// </summary>
            [DataMember(Name = "openTime")]
            public string OpenTime { get; set; }
            /// <summary>
            /// 课件提示
            /// </summary>
            [DataMember(Name = "prompt")]
            public string Prompt { get; set; }
            
            /// <summary>
            /// 课件排序
            /// </summary>
            [DataMember(Name = "wareOrder")]
            public string WareOrder { get; set; }
            /// <summary>
            /// 年份排序
            /// </summary>
            [DataMember(Name = "yearOrder")]
            public int YearOrder { get; set; }

            #endregion
        }

        [DataContract]
        public class CwareProgress
        {
            [DataMember(Name = "cwid")]
            public string CwId { get; set; }

            [DataMember(Name = "thisProgress")]
            public double ThisProgress { get; set; }
        }
    }

    /**
     * 新接口返回的数据反序列化对应的类
     * @author ChW
     * @date 2021-06-07
     */
    [DataContract]
    public class CwareKeyReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
        [DataMember(Name = "retry")]
        public bool Retry { get; set; }
        [DataMember(Name = "result")]
        public CwareKeyItem Result { get; set; }
    }

    [DataContract]
    public class CwareKeyItem
    {
        [DataMember(Name = "code")]
        public int Code { get; set; }
        [DataMember(Name = "msg")]
        public string Msg { get; set; }
        [DataMember(Name = "cwareKey")]
        public string CwareKey { get; set; }
    }
}
