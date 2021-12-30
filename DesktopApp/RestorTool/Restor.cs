using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace RestorTool
{
    public static class Restor
    {
        /// <summary>
        /// 获取文件的哈希值
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public static string Sha1(Stream st)
        {
            using (var sha1 = new SHA1CryptoServiceProvider())
            {
                return BitConverter.ToString(sha1.ComputeHash(st)).Replace("-", "");
            }
        }
        /// <summary>
        /// 判断文件哈希值是否相等
        /// </summary>
        /// <param name="localFile">本地文件</param>
        /// <param name="hash">标准文件哈希值</param>
        /// <returns></returns>
        public static bool FileHashEqual(string localFile, string hash = "")
        {
            //计算文件哈希
            if (!string.IsNullOrWhiteSpace(hash))
            {
                using (var ms = new FileStream(localFile, FileMode.Open, FileAccess.Read))
                {
                    var chash = Sha1(ms);
                    if (hash.ToUpper() != chash)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
