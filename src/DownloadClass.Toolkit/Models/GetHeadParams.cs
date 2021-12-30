using System;

using Refit;

namespace DownloadClass.Toolkit.Models
{
    public class GetHeadParams
    {
        public GetHeadParams(Courseware courseware, string username, byte[] primaryKey, string hash)
        {
            if (courseware is null)
                throw new ArgumentNullException(nameof(courseware));
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException($"'{nameof(username)}' cannot be null or whitespace.", nameof(username));
            if (primaryKey is null)
                throw new ArgumentNullException(nameof(primaryKey));
            if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException($"'{nameof(hash)}' cannot be null or whitespace.", nameof(hash));

            Username = username;
            CoursewareId = courseware.CoursewareId;
            VideoId = courseware.VideoId;
            PackagedType = courseware.PackagedType;
            Timestamp = courseware.Timestamp.ToUnixTimeSeconds();
            HeadHash = courseware.HeadHash;
            PrimaryKey = Convert.ToBase64String(primaryKey);
            Hash = hash;
        }

        /// <summary>
        /// 用户名
        /// </summary>
        [AliasAs("userName")]
        public string Username { get; }

        /// <summary>
        /// 课件ID
        /// </summary>
        [AliasAs("cwareid")]
        public long CoursewareId { get; }

        /// <summary>
        /// 视频编号
        /// </summary>
        [AliasAs("videoid")]
        public string VideoId { get; }

        /// <summary>
        /// 打包类型
        /// </summary>
        [AliasAs("VideoType")]
        public int PackagedType { get; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [AliasAs("time")]
        public long Timestamp { get; }

        /// <summary>
        /// 文件头哈希值
        /// </summary>
        [AliasAs("videoheadhash")]
        public string HeadHash { get; }

        /// <summary>
        /// 由Hash加密而来
        /// </summary>
        [AliasAs("prikey")]
        public string PrimaryKey { get; }

        /// <summary>
        /// 从GenKey接口获取的值
        /// </summary>
        [AliasAs("hash")]
        public string Hash { get; }
    }
}
