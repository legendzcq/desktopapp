using System;
using System.Xml.Serialization;

namespace DownloadClass.Toolkit.Models
{
    [XmlRoot(elementName: "res")]
    public class Courseware
    {
        /// <summary>
        /// 课件识别码
        /// </summary>
        [XmlElement(elementName: "CwareID")]
        public string CoursewareIdCode { get; set; } = default!;

        /// <summary>
        /// 课件ID
        /// </summary>
        [XmlElement(elementName: "cwid")]
        public long CoursewareId { get; set; } = default!;

        /// <summary>
        /// 课件名称
        /// </summary>
        [XmlElement(elementName: "CwareName")]
        public string CoursewareName { get; set; } = default!;

        [XmlElement(elementName: "pathurl")]
        public string PathUrl { get; set; } = default!;

        /// <summary>
        /// 视频编号
        /// </summary>
        [XmlElement(elementName: "VideoID")]
        public string VideoId { get; set; } = default!;

        /// <summary>
        /// 视频名称
        /// </summary>
        [XmlElement(elementName: "VideoName")]
        public string VideoName { get; set; } = default!;

        /// <summary>
        /// 打包类型
        /// </summary>
        [XmlElement(elementName: "VideoType")]
        public int PackagedType { get; set; } = default!;

        /// <summary>
        /// 时间戳XML表示
        /// </summary>
        [XmlElement(elementName: "ts")]
        public long TimestampForXml { get; set; } = default!;

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTimeOffset Timestamp => DateTimeOffset.FromUnixTimeSeconds(TimestampForXml);

        /// <summary>
        /// 文件头哈希值
        /// </summary>
        [XmlElement(elementName: "hmd5")]
        public string HeadHash { get; set; } = default!;

        /// <summary>
        /// 播放类型
        /// </summary>
        [XmlElement(elementName: "PlayType")]
        public int PlayType { get; set; } = default!;

        /// <summary>
        /// 视频URL
        /// </summary>
        [XmlElement(ElementName = "VideoUrl")]
        public string VideoUrl { get; set; } = default!;
    }
}
