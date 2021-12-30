#if MOBILE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Framework.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DesktopApp.ViewModel
{
    public class CwareDownItemViewModel : ViewModelBase
    {
        private double _rate;
        private TransState _state;
        private string _rateStr;
        public int CwareId { get; set; }
        public string VideoId { get; set; }
        public string VideoName { get; set; }
        public ViewStudentCwareDown ThisItem { get; private set; }

        public double Rate
        {
            get { return _rate; }
            set
            {
                if (value > 100 || value < 0) return;
                _rate = value;
                RaisePropertyChanged(() => Rate);
            }
        }

        public string RateStr
        {
            get { return _rateStr; }
            set
            {
                _rateStr = value;
                RaisePropertyChanged(() => RateStr);
            }
        }

        public TransState State
        {
            get { return _state; }
            set
            {
                if (_state != value)
                {
                    _state = value;
                    RaisePropertyChanged(() => State);
                }
            }
        }

        public ICommand StartCommand { get; private set; }

        public ICommand ContinueCommand { get; private set; }

        public CwareDownItemViewModel(ViewStudentCwareDown item)
        {
            ThisItem = item;
            CwareId = item.CwareId;
            VideoId = item.VideoId;
            VideoName = item.VideoName;
            State = TransState.Normal;

            StartCommand = new RelayCommand(() =>
            {
                State = TransState.NeedTrasfer;
            });

            ContinueCommand = new RelayCommand(() =>
            {
                State = TransState.NeedTrasfer;
            });
        }

        private bool HasSetEvent;

        public void StartTransfer()
        {
            if (!HasSetEvent)
            {
                HasSetEvent = true;
                App.MobileConnector.OnTransStart += SetStart;
                App.MobileConnector.OnProgress += SetProgress;
                App.MobileConnector.OnTrasFinished += SetFinished;
                App.MobileConnector.OnTrasError += SetTrasError;
                App.MobileConnector.OnNoSpace += SetFinished;
                App.MobileConnector.OnVideoExists += SetFinished;
            }
            App.MobileConnector.SendCware(CwareId, VideoId);
        }

        private void SetStart()
        {
            State = TransState.OnTrasfer;
        }

        private void SetProgress(string str)
        {
            double rate = double.TryParse(str, out rate) ? rate : 0;
            Rate = rate;
            RateStr = str + "%";
        }

        private void SetTrasError()
        {
            State = TransState.TrasError;
            if (HasSetEvent)
            {
                App.MobileConnector.OnTransStart -= SetStart;
                App.MobileConnector.OnProgress -= SetProgress;
                App.MobileConnector.OnTrasFinished -= SetFinished;
                App.MobileConnector.OnTrasError -= SetTrasError;
                App.MobileConnector.OnNoSpace -= SetFinished;
                App.MobileConnector.OnVideoExists -= SetFinished;
            }
        }

        private void SetFinished()
        {
            State = TransState.TrasFinished;
            if (HasSetEvent)
            {
                App.MobileConnector.OnTransStart -= SetStart;
                App.MobileConnector.OnProgress -= SetProgress;
                App.MobileConnector.OnTrasFinished -= SetFinished;
                App.MobileConnector.OnTrasError -= SetTrasError;
                App.MobileConnector.OnNoSpace -= SetFinished;
                App.MobileConnector.OnVideoExists -= SetFinished;
            }
        }
    }

    public enum TransState
    {
        Normal,
        NeedTrasfer,
        OnTrasfer,
        TrasError,
        TrasFinished
    }
}

#endif