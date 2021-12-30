
using System;
using System.Runtime.Serialization;
namespace Framework.Model
{
    #region
    [DataContract]
    public class StudentCourseDetail
    {
        public int CWareId { get; set; }
        /// <summary>
        /// 视频ID
        /// </summary>
        [DataMember(Name = "NodeID")]
        public string VideoId { get; set; }
        /// <summary>
        /// 章节ID
        /// </summary>
        [DataMember(Name = "chapterid")]
        public int ChapterId { get; set; }
        /// <summary>
        /// 试听
        /// </summary>
        [DataMember(Name = "demotype")]
        public int DemoType { get; set; }
        /// <summary>
        /// 视频类型（1zip,2媒体文件，3仅讲义）
        /// </summary>
        [DataMember(Name = "videotype")]
        public int VideoType { get; set; }
        /// <summary>
        /// 视频地址
        /// </summary>
        [DataMember(Name = "videourl")]
        public string VideoUrl { get; set; }
        /// <summary>
        /// 音频地址
        /// </summary>
        [DataMember(Name = "audiourl")]
        public string AudioUrl { get; set; }
        /// <summary>
        /// 视频长度
        /// </summary>
        [DataMember(Name = "length")]
        public string Length { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }
        /// <summary>
        /// 视频名称
        /// </summary>
        [DataMember(Name = "videoname")]
        public string VideoName { get; set; }
        /// <summary>
        /// 视频ZIP地址(标清)
        /// </summary>
        [DataMember(Name = "videozipurl")]
        public string VideoZipUrl { get; set; }
        /// <summary>
        /// 音频zip地址
        /// </summary>
        [DataMember(Name = "audiozipurl")]
        public string AudioZipUrl { get; set; }
        /// <summary>
        /// 视频hd的地址(高清)
        /// </summary>
        [DataMember(Name = "videoHDzipurl")]
        public string VideoHdZipUrl { get; set; }
        /*新接口中弃用字段*/
        public string VideoHdUrl { get; set; }
        /// <summary>
        /// 知识点ID
        /// </summary>
        public int PointId { get; set; }
      
        /// <summary>
        /// 知识点ID 加该字段的目的在于如果是""或者null来判断PointId的默认值  防止报错
        /// </summary>
        [DataMember(Name = "pointid")]
        public string StrPointId { get; set; }
        /// <summary>
        /// 知识点名称
        /// </summary>
        [DataMember(Name = "pointname")]
        public string PointName { get; set; }
        /// <summary>
        /// 视频排序
        /// </summary>
        [DataMember(Name = "videoOrder")]
        public int OrderBy { get; set; }

        public string AudioHdZipUrl { get; set; }
        /// <summary>
        /// 视频更新时间
        /// </summary>
        [DataMember(Name = "modTime")]
        public string ModTime { get; set; }
    }

    [DataContract]
    public class StudentCourseDetailReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }
        [DataMember(Name = "retry")]
        public bool Retry { get; set; }
        [DataMember(Name = "result")]
        public StudentCourseDetailResult Result { get; set; }
    }

    [DataContract]
    public class StudentCourseDetailResult
    {
        [DataMember(Name = "ret")]
        public string Ret { get; set; }
        [DataMember(Name = "code")]
        public string code { get; set; }
    }
    #endregion
}
