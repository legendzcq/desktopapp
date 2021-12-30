
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using Prism.Mvvm;

namespace Framework.NewModel
{
    [DataContract]
    public class StudentWareKcjyDownReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public StudentWareKcjyDownItem Result { get; set; }
    }

    [DataContract]
    public class StudentWareKcjyDownItem
    {
        [DataMember(Name = "code")]
        public string Code { get; set; }

        [DataMember(Name = "msg")]
        public string Message { get; set; }

        [DataMember(Name = "cwareClassList")]
        public IEnumerable<StudentWareKcjyDown> KcjyList { get; set; }
    }

    [DataContract]
    public class StudentWareKcjyDown
    {
        [DataMember(Name = "smallListName")]
        public string SmallListName { get; set; }

        [DataMember(Name = "cwID")]
        public string CwId { get; set; }

        [DataMember(Name = "jiangIiFile")]
        public string JiangyiFile { get; set; }

        [DataMember(Name = "smallOrder")]
        public int SmallOrder { get; set; }

        [DataMember(Name = "smallListID")]
        public int SmallListId { get; set; }
    }

    public class ViewStudentWareKcjyDown : INotifyPropertyChanged //BindableBase
    {
        public int CwareId { get; set; }
        public string SmallListName { get; set; }
        public string JiangyiFile { get; set; }
        public int SmallOrder { get; set; }
        public int SmallListId { get; set; }

        private KcjyState _existState;
        public KcjyState ExistState
        {
            get => _existState;
            set
            {
                _existState = value;
                //SetProperty(ref _existState, value, nameof(ExistState));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ExistState))); //对ExistState进行监听
            }
        }

        public event PropertyChangedEventHandler PropertyChanged; // 声明属性改变的事件处理器

        /**
         * 讲义的状态
         * @author ChW
         * @date 2021-05-11
         */
        public enum KcjyState
        {
            // 未下载
            UnDownload = 0,
            // 已下载
            DownLoaded = 1
        }
    }
}
