
using System.Windows;
using System.Windows.Forms;

namespace DesktopApp.Controls
{
    /// <summary>
    /// AutoMessageBox.xaml 的交互逻辑
    /// </summary>
    public partial class AutoMessageBox : Window
    {
        Timer timer = new Timer();
        public AutoMessageBox()
        {
            InitializeComponent();
            timer.Interval = 3000;    //10秒启动
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg">要在消息框中显示的文本</param>
        /// <param name="title">要在消息框的标题栏中显示的文本</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        ///  <param name="interval">间隔时间</param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public void Show(string msg, string title = "提示", int interval = 3000, int width = 400, int height = 150, Window owner = null)
        {
            var msgBox = new AutoMessageBox
            {
                TxtTitle = { Text = title },
                TxtContent = { Text = msg },
                Width = width,
                Height = height,
            };
            if (owner != null)
            {
                msgBox.Owner = owner;
            }
            msgBox.ImgClose.MouseLeftButtonDown += (s, e) =>
            {
                msgBox.Close();
            };
            msgBox.timer.Tick += (s, e) =>
            {
                msgBox.timer.Enabled = false;
                msgBox.Close();
            };
            msgBox.timer.Enabled = true;
            msgBox.ShowDialog();
            this.Close();
        }
    }
}
