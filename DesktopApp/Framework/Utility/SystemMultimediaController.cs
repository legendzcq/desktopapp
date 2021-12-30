using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Framework.Utility
{
    public class SystemMultimediaController
    {
        /*
                 * 弹出系统音量控制器
                 * */
        public static void PopupController()
        {
            ProcessStartInfo Info = new ProcessStartInfo();
            Info.FileName = "Sndvol32";
            Process.Start(Info);
        }

        public static void SetMute(IntPtr handle)
        {
            NativeMethod.SendMessage(handle, NativeMethod.WM_APPCOMMAND, 0x200eb0, NativeMethod.APPCOMMAND_VOLUME_MUTE * 0x10000);
        }

        /*
         * 获得音量范围和获取/设置当前音量
         * */
        public static int MaxValue
        {
            get { return int.Parse(IMaxValue.ToString()); }
        }
        public static int MinValue
        {
            get { return int.Parse(IMinValue.ToString()); }
        }

        public static int CurrentValue
        {
            get
            {
                GetVolume();
                return _iCurrentValue;
            }
            set
            {
                SetValue(MaxValue, MinValue, value);
            }
        }


        #region Private Static Data Members
        private const UInt32 IMaxValue = 0xFFFF;
        private const UInt32 IMinValue = 0x0000;
        private static int _iCurrentValue = 0;
        #endregion
        #region Private Static Method
        /*
         * 得到当前音量
         **/
        private static void GetVolume()
        {
            UInt32 d, v;
            d = 0;
            long i = NativeMethod.waveOutGetVolume(d, out v);
            UInt32 vleft = v & 0xFFFF;
            UInt32 vright = (v & 0xFFFF0000) >> 16;
            UInt32 all = vleft | vright;
            UInt32 value = (all * UInt32.Parse((MaxValue - MinValue).ToString()) / ((UInt32)IMaxValue));
            _iCurrentValue = int.Parse(value.ToString());
        }

        /*
         * 修改音量值
         * */
        private static void SetValue(int aMaxValue, int aMinValue, int aValue)
        {
            //先把trackbar的value值映射到0x0000～0xFFFF范围  
            var value = (UInt32)((double)0xffff * (double)aValue / (double)(aMaxValue - aMinValue));
            //限制value的取值范围  
            if (value < 0) value = 0;
            if (value > 0xffff) value = 0xffff;
            var left = (UInt32)value;//左声道音量  
            var right = (UInt32)value;//右  
            NativeMethod.waveOutSetVolume(0, left << 16 | right); //"<<"左移，“|”逻辑或运算  
        }
        #endregion
    }
}
