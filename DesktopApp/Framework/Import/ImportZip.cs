using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

using Framework.Local;
using Framework.Model;
using Framework.Remote;
using Framework.Utility;

namespace Framework.Import
{
    /// <summary>
    /// 导入Zip包
    /// </summary>
    public class ImportZip
    {
        private static readonly object LockObj = new object();

        public Func<string, bool> ConfirmFun { private get; set; }

        public Action<string, string> StatusAction { private get; set; }

        public Action<string, string> MessageAction { private get; set; }

        public Action<string> CompleteAction { private get; set; }

        //public Action<string> FileError { private get; set; }

        public void ImportFileAsync(string uniqueId, string zipFile, string videoSavePath) => SystemInfo.StartBackGroundThread("异步导入主线程", () => ImportFromFile(uniqueId, zipFile, videoSavePath));

        /// <summary>
        ///
        /// </summary>
        ///<remarks>
        /// 导入步骤如下：
        /// 1.解压文件
        /// 2.判断学员是否有权限（判断课程码）
        /// 3.处理讲义和时间点
        /// 4.请求文件头
        /// 4.处理视频文件
        /// 5.删除临时文件
        ///</remarks>
        public bool ImportFromFile(string uniqueId, string zipFile, string videoSavePath)
        {
            Trace.WriteLine("导入文件:" + zipFile);
            //string zipPath = Path.GetDirectoryName(zipFile) + "\\" + Path.GetFileNameWithoutExtension(zipFile) + "_zip";
            var zipPath = Path.GetTempPath() + "\\CDELTemp\\" + Path.GetFileNameWithoutExtension(zipFile) + "_zip";
            try
            {
                #region todo在导入时检查临时文件空间是否足够盘空间 dgh 2016.06.21
                if (StatusAction != null) StatusAction(uniqueId, "检查磁盘空间");
                var tempSize = SystemInfo.GetFolderFreeSpaceInMb(Path.GetTempPath());
                var videoPathSize = SystemInfo.GetFolderFreeSpaceInMb(videoSavePath);
                if (tempSize < 512)
                {
                    var driveName = Path.GetTempPath().Substring(0, 1);
                    Trace.WriteLine(driveName + "盘空间不足");
                    if (MessageAction != null) MessageAction(uniqueId, driveName + "盘空间不足,最小需512M");
                    if (StatusAction != null) StatusAction(uniqueId, "导入失败");
                    if (CompleteAction != null) CompleteAction(uniqueId);
                    return false;
                }
                if (videoPathSize < 512)
                {
                    var driveName = videoSavePath.Substring(0, 1);
                    Trace.WriteLine(driveName + "盘空间不足");
                    if (MessageAction != null) MessageAction(uniqueId, driveName + "盘空间不足,最小需512M");
                    if (StatusAction != null) StatusAction(uniqueId, "导入失败");
                    if (CompleteAction != null) CompleteAction(uniqueId);
                    return false;
                }
                #endregion
                if (StatusAction != null) StatusAction(uniqueId, "校验文件");
                if (!SystemInfo.CheckZipFile(zipFile))
                {
                    Trace.WriteLine("文件校验失败");
                    if (MessageAction != null) MessageAction(uniqueId, "校验失败");
                    if (StatusAction != null) StatusAction(uniqueId, "失败");
                    if (CompleteAction != null) CompleteAction(uniqueId);
                    //if (FileError != null) FileError(uniqueId);
                    return false;
                }
                if (StatusAction != null) StatusAction(uniqueId, "解压中");
                if (!DeCompressionZip(zipFile, zipPath))
                {
                    Trace.WriteLine("文件解压失败");
                    if (MessageAction != null) MessageAction(uniqueId, "解压失败");
                    if (StatusAction != null) StatusAction(uniqueId, "失败");
                    if (CompleteAction != null) CompleteAction(uniqueId);
                    //if (FileError != null) FileError(uniqueId);
                    return false;
                }
                var cwdirs = Directory.GetDirectories(zipPath);
                var local = new StudentWareData();
                var dirs = cwdirs.Select(x => x.Substring(x.LastIndexOf("\\", StringComparison.Ordinal) + 1)).ToArray();
                dirs = local.VerifyCourse(dirs);
                if (dirs.Length == 0)
                {
                    Trace.WriteLine("无权限");
                    if (MessageAction != null) MessageAction(uniqueId, "不是有效课件或您没有看课权限");
                    if (StatusAction != null) StatusAction(uniqueId, "失败");
                    if (CompleteAction != null) CompleteAction(uniqueId);
                    //if (FileError != null) FileError(uniqueId);
                    return false;
                }
                var hasSuccess = false;
                foreach (var name in dirs)
                {
                    if (StatusAction != null) StatusAction(uniqueId, "正在导入");
                    var configFileContent = File.ReadAllText(zipPath + "\\" + name + "\\config.xml");
                    configFileContent = Crypt.TransformOtherFile(configFileContent);
                    var kcjyFileContent = File.ReadAllText(zipPath + "\\" + name + "\\paper.xml");
                    kcjyFileContent = Crypt.TransformOtherFile(kcjyFileContent);
                    var timeNodeFileContent = File.ReadAllText(zipPath + "\\" + name + "\\timepoint.xml");
                    timeNodeFileContent = Crypt.TransformOtherFile(timeNodeFileContent);
                    var imgPath = zipPath + "\\" + name + "\\";
                    ZipConfig config = Helper.GetConfigFromXml(configFileContent);
                    if (config.PlayType == 1)
                    {
                        if (MessageAction != null) MessageAction(uniqueId, "不能导入音频课件");
                        if (StatusAction != null) StatusAction(uniqueId, "失败");
                        if (CompleteAction != null) CompleteAction(uniqueId);
                        //if (FileError != null) FileError(uniqueId);
                        return false;
                    }
                    StudentCourseWare vitem = new StudentWareData().GetCourseWareItem(config.CwId);
                    var wareName = vitem.Name + " " + config.VideoName;
                    if (MessageAction != null) MessageAction(uniqueId, wareName);
                    if (local.TestVideoExists(config.CwId, config.VideoId))
                    {
                        if (StatusAction != null) StatusAction(uniqueId, "重复导入");
                        continue;
                    }
                    Helper.GetKcjyAndTimeNodeFromXml(config, timeNodeFileContent, kcjyFileContent, imgPath);
                    var web = new StudentWareRemote();
                    //先用新方法导入
                    var headby = web.GetVideoHead(config.CwId, config.VideoId, config.VideoType, config.Ts, Util.UserName, config.HMd5);
                    if (headby.Length == 0) headby = web.GetVideoHeader(config.CwId, config.VideoId, config.VideoType, config.Ts, Util.UserName, config.HMd5);
                    if (headby.Length == 0)
                    {
                        if (MessageAction != null) MessageAction(uniqueId, "视频授权失败，重新尝试授权");
                        //if (StatusAction != null) StatusAction(uniqueId, "导入失败");
                        //if (CompleteAction != null) CompleteAction(uniqueId);
                    }
                    else if (headby.Length != 4096)
                    {
                        if (MessageAction != null) MessageAction(uniqueId, "视频授权失败，尝试其他授权方式");
                        //if (StatusAction != null) StatusAction(uniqueId, "导入失败");
                        //if (CompleteAction != null) CompleteAction(uniqueId);
                        //if (FileError != null) FileError(uniqueId);
                    }
                    else
                    {
                        var filePath = videoSavePath + "\\" + config.CwId + "\\" + config.VideoId + Util.FormatExtension;
                        //为该方法加try....catch 解决文件被占用 dgh 2016.07.01
                        try
                        {
                            Crypt.TransformVideo(headby, zipPath + "\\videofile.mp4", filePath);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine("Crypt.TransformVideo方法" + ex.Message);
                        }
                        StudentCourseDetail courseDetaiItem = local.GetCourseDetailItem(config.CwId, config.VideoId);
                        var downitem = new StudentCWareDown
                        {
                            CwareId = config.CwId,
                            State = 3,
                            Rate = 0,
                            Url = "",
                            VideoId = config.VideoId,
                            LocalFile = filePath,
                            ModTime = courseDetaiItem == null ? string.Empty : courseDetaiItem.ModTime
                        };
                        local.AddCwareDown(downitem);
                        if (StatusAction != null) StatusAction(uniqueId, "导入成功");
                        hasSuccess = true;
                    }
                    if (!hasSuccess)
                    {
                        //兼容旧版本
                        var key = web.GetCwareKeyNew(config.CwareId);
                        var filePath = videoSavePath + "\\" + config.CwId + "\\" + config.VideoId + Util.FormatExtension;
                        if (key != null && key.Length == 8)
                        {
                            var keys = System.Text.Encoding.UTF8.GetBytes(key);
                            //为该方法加try....catch 解决文件被占用 dgh 2016.07.01
                            try
                            {
                                Crypt.TransformVideo(zipPath + "\\videofile.key", zipPath + "\\videofile.mp4", filePath, keys);
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine("Crypt.TransformVideo方法" + ex.Message);
                            }
                            StudentCourseDetail courseDetaiItem = local.GetCourseDetailItem(config.CwId, config.VideoId);
                            var downitem = new StudentCWareDown
                            {
                                CwareId = config.CwId,
                                State = 3,
                                Rate = 0,
                                Url = "",
                                VideoId = config.VideoId,
                                LocalFile = filePath,
                                ModTime = courseDetaiItem == null ? string.Empty : courseDetaiItem.ModTime
                            };
                            local.AddCwareDown(downitem);
                            if (StatusAction != null) StatusAction(uniqueId, "导入成功");
                        }
                        else
                        {
                            if (MessageAction != null) MessageAction(uniqueId, "视频授权失败，重新尝试授权");
                            if (StatusAction != null) StatusAction(uniqueId, "导入失败");
                            //if (FileError != null) FileError(uniqueId);
                        }
                    }
                }
                if (CompleteAction != null) CompleteAction(uniqueId);
                Trace.WriteLine("导入文件完成");
                return hasSuccess;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("导入文件失败");
                Log.RecordLog(ex.ToString());
                if (StatusAction != null) StatusAction(uniqueId, "导入失败");
                if (CompleteAction != null) CompleteAction(uniqueId);
                return false;
            }
            finally
            {
                SystemInfo.TryDeleteDirectory(zipPath);
            }
        }

        /// <summary>
        /// 解压zip文件到一个文件夹
        /// </summary>
        /// <param name="depositPath"></param>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        private bool DeCompressionZip(string depositPath, string folderPath)
        {
            Trace.WriteLine("解压文件:" + depositPath);
            lock (LockObj)
            {
                var result = true;
                try
                {
                    ZipFile.ExtractToDirectory(depositPath, folderPath);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    //todo 加上尝试使用rar解压的代码
                    result = false;
                }
                return result;
            }
        }


        /// <summary>
        /// 用于讲义下载：解压zip文件到一个文件夹(只针对zip中有一个文件)
        /// </summary>
        /// <param name="depositPath">原文件</param>
        /// <param name="folderPath">目标文件</param>
        ///<param name="fileName">解压后的文件路径</param>
        /// <returns></returns>
        public void DeCompressionZipForJY(string depositPath, string folderPath, ref string fileName)
        {
            Trace.WriteLine("解压文件:" + depositPath);
            try
            {
                using (ZipArchive zip = ZipFile.OpenRead(depositPath))
                {
                    ZipArchiveEntry entry = zip.Entries.Single();
                    zip.ExtractToDirectory(folderPath);
                    fileName = Path.Combine(folderPath, entry.FullName);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
            }
        }
    }
}
