
namespace Framework.Model
{
    public class ZipConfig
    {
        /// <summary>
        /// 这里的CwareId和Cwid是反的。
        /// </summary>
        public string CwareId { get; set; }

        /// <summary>
        /// 这里的CwareId和Cwid是反的。
        /// </summary>
        public int CwId { get; set; }

        public string CwareName { get; set; }

        public string PathUrl { get; set; }

        public string VideoId { get; set; }

        public string VideoName { get; set; }

        public int PlayType { get; set; }

        public string VideoUrl { get; set; }

        public long Ts { get; set; }

        public int VideoType { get; set; }

        public string HMd5 { get; set; }
    }
}
