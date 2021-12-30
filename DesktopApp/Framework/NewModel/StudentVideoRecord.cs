using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Framework.NewModel
{
    /// <summary>
    /// 学习视频记录
    /// </summary>
    [DataContract]
    public class StudentVideoRecord
    {
        /// <summary>
        /// 视频地址
        /// </summary>
        [DataMember(Name = "cwareUrl")]
        public string CwareUrl { get; set; }

        /// <summary>
        /// 课件ID
        /// </summary>
        [DataMember(Name = "cwareid")]
        public int CwareId { get; set; }

        /// <summary>
        /// 最后学习时间点
        /// </summary>
        [DataMember(Name = "nextBegineTime")]
        public int LastPosition { get; set; }

        /// <summary>
        /// 视频ID
        /// </summary>
        [DataMember(Name = "videoid")]
        public string VideoID { get; set; }

        /// <summary>
        /// 听课时间
        /// </summary>
        [DataMember(Name = "updateTime")]
        public string LastTime { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [DataMember(Name = "uid")]
        public int Uid { get; set; }
    }

    /// <summary>
    /// 学习记录
    /// </summary>

    [DataContract]
    public class StudyVideoJson
    {
        /// <summary>
        /// 课件学习详情
        /// </summary>
        [DataMember(Name = "videoStr")]
        public List<VideoStr> StudyVideo { get; set; }
    }
    [DataContract]
    public class VideoStr
    {
        /// <summary>
        /// 课件ID
        /// </summary>
        [DataMember(Name = "cwareID")]
        public string CwareId { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        [DataMember(Name = "deviceID")]
        public string DeviceID { get; set; }

        /// <summary>
        /// 视频总长度结束点
        /// </summary>
        [DataMember(Name = "rangeEnd")]
        public string RangeEnd { get; set; }

        /// <summary>
        /// 视频总长度开始点
        /// </summary>
        [DataMember(Name = "rangeStart")]
        public string RangeStart { get; set; }

        /// <summary>
        /// 视频ID
        /// </summary>
        [DataMember(Name = "videoID")]
        public string VideoID { get; set; }
        /// <summary>
        /// 课件学习详情
        /// </summary>
        [DataMember(Name = "timebase")]
        public List<TimebaseStr> Timebase { get; set; }


    }

    /// <summary>
    /// 课件学习详情
    /// </summary>

    [DataContract]
    public class TimebaseStr
    {
        /// <summary>
        /// 课件ID
        /// </summary>
        public int CwareId { get; set; }

        /// <summary>
        /// 视频ID
        /// </summary>
        public string VideoID { get; set; }
        /// <summary>
        ///开始时间点
        /// </summary>
        [DataMember(Name = "p1")]
        public string VideoStartTime { get; set; }

        /// <summary>
        /// 结束时间点
        /// </summary>
        [DataMember(Name = "p2")]
        public string VideoEndTime { get; set; }

        /// <summary>
        /// 倍速
        /// </summary>
        [DataMember(Name = "sp")]
        public string Speed { get; set; }

        /// <summary>
        /// 学习结束时间
        /// </summary>
        [DataMember(Name = "timeEnd")]
        public string StudyTimeEnd { get; set; }

        /// <summary>
        /// 学习开始时间
        /// </summary>
        [DataMember(Name = "timeStart")]
        public string StudyTimeStart { get; set; }
    }

}
