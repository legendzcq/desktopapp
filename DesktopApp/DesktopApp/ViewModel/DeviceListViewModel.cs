using Framework.Model;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using static Framework.Model.RemoteLogin;

namespace DesktopApp.ViewModel
{
    public class DeviceListViewModel : ViewModelBase
    {
        public string MobilePhone { get; set; }

        public string selectedMid { get; set; }

        public string selectedMname { get; set; }

        internal void fillBindedDeviceList(ObservableCollection<LoginedDevice> deviceList)
        {
            BindedDeviceList = deviceList;
        }
        /**
         * 当前账户已绑定的设备列表，Model
         */
        private ObservableCollection<LoginedDevice> bindedDeviceList;
        /**
         * CLR属性，ViewModel
         */
        public ObservableCollection<LoginedDevice> BindedDeviceList
        {
            get
            {
                return bindedDeviceList;
            }
            set
            {
                bindedDeviceList = value;
                RaisePropertyChanged(() => BindedDeviceList);
            }
        }
    }
}
