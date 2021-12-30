using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;

using DesktopApp.Controls;
using DesktopApp.Infrastructure;
using DesktopApp.Logic;

using Framework.Model;
using Framework.NewModel;
using Framework.Utility;

using GalaSoft.MvvmLight.Command;

namespace DesktopApp.ViewModel
{
    public class KcjyDetailViewModel : NavigationViewModelBase
    {
        private ViewStudentCourseWare _course;
        private string _pageTitle;
        private ObservableCollection<ViewStudentWareKcjyDown> _kcjyList;

        public KcjyDetailViewModel()
        {
            ShowKcjyCommand = new RelayCommand<ViewStudentWareKcjyDown>(item =>
            {
                var jiangyiFile = item.JiangyiFile;
                if (string.IsNullOrEmpty(jiangyiFile))
                {
                    CustomMessageBox.Show("本章讲义尚未提供");
                    return;
                }
                var courseId = _course.CwareId;
                var path = Util.JiangyiSavePath + "\\" + courseId + "\\" + Path.GetFileName(jiangyiFile);
                var wordFile = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".doc";
                if (File.Exists(wordFile))
                {
                    OpenWordFile(wordFile);
                    return;
                }
                if (File.Exists(path))
                {
                    //解决zip文件解压
                    if (Path.GetExtension(path).ToLower() == ".zip")
                    {
                        //解压zip文件
                        var fileName = string.Empty;//解压后的文件
                        new Framework.Import.ImportZip().DeCompressionZipForJY(path, Path.GetDirectoryName(path), ref fileName);
                        if (File.Exists(fileName))
                        {
                            if (Path.GetExtension(fileName).ToLower() == ".exe")
                            {
                                ExtractRarFile(fileName);//解压exe文件
                                OpenWordFile(wordFile);
                            }
                            else if (Path.GetExtension(fileName).ToLower() == ".doc")
                            {
                                OpenWordFile(fileName);
                            }

                        }
                    }
                    else
                    {
                        ExtractRarFile(path);
                        OpenWordFile(wordFile);
                    }

                    return;
                }
                if (!Util.IsOnline)
                {
                    CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                    return;
                }
                var url = "http://down.chnedu.com" + jiangyiFile;
                //加入下载任务;
                var sd = new Framework.Download.Downloader(url, path, 0);
                sd.DownloadComplate += (s, e) =>
                {
                    //MessageBox.Show("讲义下载完毕");
                    sd.Stop();
                    var temp = Path.GetDirectoryName(path) + "\\" + Path.GetFileName(path) + Util.DownloadFileExtension;
                    try
                    {
                        File.Move(temp, path);
                    }
                    catch
                    {
                        ;
                    }
                    if (Path.GetExtension(path).ToLower() == ".zip")
                    {
                        //解压zip文件
                        var fileName = string.Empty;//解压后的文件
                        new Framework.Import.ImportZip().DeCompressionZipForJY(path, Path.GetDirectoryName(path), ref fileName);
                        if (File.Exists(fileName))
                        {
                            if (Path.GetExtension(fileName).ToLower() == ".exe")
                            {
                                ExtractRarFile(fileName);//解压exe文件
                                OpenWordFile(wordFile);
                            }
                            else if (Path.GetExtension(fileName).ToLower() == ".doc")
                            {
                                OpenWordFile(fileName);
                            }

                        }
                    }
                    else
                    {
                        ExtractRarFile(path);
                        OpenWordFile(wordFile);
                    }

                };
                sd.DownloadError += (s, e) => Dispatcher.Invoke(new Action(() => CustomMessageBox.Show("讲义下载失败，请重试")));
                //sd.DownloadStarted += (s, e) => { };
                //sd.DownloadProcess += (s, e) => Trace.WriteLine(string.Format("{0},{1},{2}", id, all, down));
                sd.Start();
            });

            DownloadKcjyCommand = new RelayCommand<ViewStudentWareKcjyDown>(item =>
            {
                var jiangyiFile = item.JiangyiFile;
                if (string.IsNullOrEmpty(jiangyiFile))
                {
                    CustomMessageBox.Show("本章讲义尚未提供");
                    return;
                }
                var courseId = _course.CwareId;
                var path = Util.JiangyiSavePath + "\\" + courseId + "\\" + Path.GetFileName(jiangyiFile);
                var wordFile = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".doc";
                if (!Util.IsOnline)
                {
                    CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                    return;
                }
                var url = "http://down.chnedu.com" + jiangyiFile;
                //加入下载任务;
                var sd = new Framework.Download.Downloader(url, path, 0);
                sd.DownloadComplate += (s, e) =>
                {
                    //MessageBox.Show("讲义下载完毕");
                    sd.Stop();
                    var temp = Path.GetDirectoryName(path) + "\\" + Path.GetFileName(path) + Util.DownloadFileExtension;
                    try
                    {
                        File.Move(temp, path);
                    }
                    catch
                    {
                        ;
                    }
                    if (Path.GetExtension(path).ToLower() == ".zip")
                    {
                        //解压zip文件
                        var fileName = string.Empty;//解压后的文件
                        new Framework.Import.ImportZip().DeCompressionZipForJY(path, Path.GetDirectoryName(path), ref fileName);
                        if (File.Exists(fileName))
                        {
                            if (Path.GetExtension(fileName).ToLower() == ".exe")
                            {
                                ExtractRarFile(fileName);//解压exe文件
                                // 不需要打开
                                // OpenWordFile(wordFile);
                            }
                            // 不需要打开
                            //else if (Path.GetExtension(fileName).ToLower() == ".doc")
                            //{
                            //    OpenWordFile(fileName);
                            //}

                        }
                    }
                    else
                    {
                        ExtractRarFile(path);
                        // 不需要打开
                        // OpenWordFile(wordFile);
                    }

                    // 下载完成后，更新按钮状态
                    item.ExistState = ViewStudentWareKcjyDown.KcjyState.DownLoaded;
                };
                sd.DownloadError += (s, e) => Dispatcher.Invoke(new Action(() => CustomMessageBox.Show("讲义下载失败，请重试")));
                //sd.DownloadStarted += (s, e) => { };
                //sd.DownloadProcess += (s, e) => Trace.WriteLine(string.Format("{0},{1},{2}", id, all, down));
                sd.Start();
            });

            OpenKcjyCommand = new RelayCommand<ViewStudentWareKcjyDown>(item =>
            {
                var jiangyiFile = item.JiangyiFile;
                if (string.IsNullOrEmpty(jiangyiFile))
                {
                    CustomMessageBox.Show("本章讲义尚未提供");
                    return;
                }
                var courseId = _course.CwareId;
                var path = Util.JiangyiSavePath + "\\" + courseId + "\\" + Path.GetFileName(jiangyiFile);
                var wordFile = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".doc";
                if (File.Exists(wordFile))
                {
                    OpenWordFile(wordFile);
                    return;
                }
                else
                {
                    // 文件不存在
                    CustomMessageBox.Show("讲义文件已被移动或者删除，请重新下载。");
                    BindLocalData();
                }
            });

            RefreshCommand = new RelayCommand(() =>
            {
                if (!Util.IsOnline)
                {
                    CustomMessageBox.Show("系统检测到您没有连接网络，请先连接网络");
                    return;
                }
                SystemInfo.StartBackGroundThread("更新讲义列表", () =>
                {
                    new Framework.Remote.StudentWareRemote().GetSmallListKcjy(_course.CwareId, _course.CwId);
                    Dispatcher.Invoke(new Action(BindLocalData));
                });
            });
        }

        /// <summary>
        /// 调用Rar解压文件
        /// </summary>
        /// <param name="path"></param>
        private void ExtractRarFile(string path)
        {
            var ext = Path.GetExtension(path);
            if (ext != null)
            {
                ext = ext.ToLower();
                if (ext != ".exe" && ext != ".rar") return;
            }
            try
            {
                var process = new Process
                {
                    StartInfo =
                    {
                        FileName = AppDomain.CurrentDomain.BaseDirectory + "rar.exe",
                        Arguments = "e -y \"" + path + "\" \"" + Path.GetDirectoryName(path) + "\"",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    }
                };
                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        /// <summary>
        /// 打开word文件
        /// </summary>
        /// <param name="wordFile"></param>
        private void OpenWordFile(string wordFile)
        {
            //try
            //{
            //	var process = new Process
            //	{
            //		StartInfo =
            //		{
            //			FileName = wordFile
            //		}
            //	};
            //	Trace.WriteLine(string.Format("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments));
            //	process.Start();
            //}
            //catch (Exception)
            //{
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "Explorer.exe",
                    Arguments = "/select,\"" + wordFile + "\""
                }
            };
            Trace.WriteLine(string.Format("{0} {1}", process.StartInfo.FileName, process.StartInfo.Arguments));
            process.Start();
            //}
        }
        public string PageTitle
        {
            get => _pageTitle;
            set
            {
                _pageTitle = value;
                RaisePropertyChanged(() => PageTitle);
            }
        }

        public ObservableCollection<ViewStudentWareKcjyDown> KcjyList
        {
            get => _kcjyList;
            set
            {
                _kcjyList = value;
                RaisePropertyChanged(() => KcjyList);
            }
        }

        public ICommand ShowKcjyCommand { get; private set; }

        /**
         * 下载讲义的命令
         * @author ChW
         * @date 2021-05-10
         */
        public ICommand DownloadKcjyCommand { get; private set; }

        /**
         * 打开讲义的命令
         * @author ChW
         * @date 2021-05-10
         */
        public ICommand OpenKcjyCommand { get; private set; }

        public ICommand RefreshCommand { get; private set; }

        private void BindData()
        {
            BindLocalData();
            if (!Util.IsOnline)
            {
                return;
            }
            SystemInfo.StartBackGroundThread("更新讲义列表", () =>
            {
                new Framework.Remote.StudentWareRemote().GetSmallListKcjy(_course.CwareId, _course.CwId);
                Dispatcher.Invoke(new Action(BindLocalData));
            });
        }

        // 对外公开接口，用于刷新按钮状态
        public void BindLocalData()
        {
            if (_course == null)
            {
                return;
            }

            System.Collections.Generic.IEnumerable<ViewStudentWareKcjyDown> lst = StudentWareLogic.GetStudentCwareKcjyDown(_course.CwareId);

            /**
             * 讲义的本地状态获取
             * @author ChW
             * @date 20210511
             */
            lst = lst.Select(x =>
            {
                var path = Util.JiangyiSavePath + "\\" + _course.CwareId + "\\" + Path.GetFileName(x.JiangyiFile);
                var wordFile = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".doc";
                if (File.Exists(wordFile))
                {
                    x.ExistState = ViewStudentWareKcjyDown.KcjyState.DownLoaded;
                }
                else
                {
                    x.ExistState = ViewStudentWareKcjyDown.KcjyState.UnDownload;
                }

                return x;
            });

            KcjyList = new ObservableCollection<ViewStudentWareKcjyDown>(lst);
        }

        public override void OnNavigateTo(System.Windows.Navigation.NavigationEventArgs e, System.Windows.Navigation.NavigationMode mode)
        {
            if (mode == System.Windows.Navigation.NavigationMode.Forward) return;
            if (mode == System.Windows.Navigation.NavigationMode.New)
            {
                var course = e.ExtraData as ViewStudentCourseWare;
                if (course == null) return;
                _course = course;
            }
            if (string.IsNullOrEmpty(_course.CourseWareName))
            {
                PageTitle = string.Format("{0} {1}({2})", _course.CourseName, _course.CWareClassName, _course.CTeacherName); // Title显示班次名称
            }
            else
            {
                PageTitle = _course.CourseWareName;
            }
            BindData();
        }
    }
}
