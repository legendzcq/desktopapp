using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Linq;
using DesktopApp.Controls;
using DesktopApp.ViewModel;
using Framework.Utility;
using Framework.Model;

namespace DesktopApp.Pages
{
    /// <summary>
    /// Interaction logic for PaperPage.xaml
    /// </summary>
    public partial class PaperPage
    {
        private static bool _isAutoNextChecked;

        private WebBrowserOverlay _webBrowserOverlay;
        System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
        public PaperPage()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                // 解决WPF Window设置AllowsTransparency="True"后，WebBrowser无法显示的问题
                _webBrowserOverlay = new WebBrowserOverlay(BorderOwner);
                // 绑定ViewModel的HtmlContent到WebBrowser控件
                var binding = new Binding
                {
                    Path = new PropertyPath("CurrentItem.HtmlContent"),
                    Source = DataContext
                };
                BindingOperations.SetBinding(_webBrowserOverlay, WebBrowserOverlay.HtmlProperty, binding);

                _webBrowserOverlay.WebBrowser.DocumentCompleted += _webBrowserOverlay_LoadCompleted;
                var helper = new ObjectForScriptingHelper(this);
                _webBrowserOverlay.WebBrowser.ObjectForScripting = helper;

                ChkAutoNext.IsChecked = _isAutoNextChecked;
                SetRightDgvBackColor();
            };

            Unloaded += (s, e) =>
            {
                _webBrowserOverlay.WebBrowser.DocumentCompleted -= _webBrowserOverlay_LoadCompleted;
                _webBrowserOverlay.Close();
            };
        }

        void _webBrowserOverlay_LoadCompleted(object s, EventArgs e)
        {
            var viewModel = DataContext as PaperViewModel;
            if (viewModel == null || viewModel.CurrentItem == null)
                return;
            try
            {
                if (viewModel.PageStatus == PageStatus.Done)
                {
                    BtnAnswer_OnClick(null, null);
                }
                if (_webBrowserOverlay.WebBrowser.Document != null)
                {
                    _webBrowserOverlay.WebBrowser.Document.InvokeScript("setAnswer", new object[] { viewModel.CurrentItem.UserAnswer });
                    _webBrowserOverlay.WebBrowser.Document.InvokeScript("changeBackColor", new object[] { Util.QuestionBackColor });
                }


            }
            catch (Exception ex)
            {
                Debug.WriteLine("setAnswer JS调用错误：" + ex.Message);
            }
        }

        private void BtnAnswer_OnClick(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as PaperViewModel;
            if (viewModel == null)
                return;
            try
            {
                string isRight = string.Empty, isDisabled = string.Empty;

                if (viewModel.PageStatus == PageStatus.Done)
                {
                    isDisabled = "true";
                }
                viewModel.CurrentItem.Question.Analysis = viewModel.CurrentItem.Question.Analysis.Replace("<p>", "<br />");
                viewModel.CurrentItem.Question.Analysis = viewModel.CurrentItem.Question.Analysis.Replace("</p>", "<br />");
                if (viewModel.CurrentItem.IsRight == 1)
                    isRight = "true";
                else if (viewModel.CurrentItem.IsRight == 2)
                    isRight = "false";
                if (_webBrowserOverlay.WebBrowser.Document != null)
                    _webBrowserOverlay.WebBrowser.Document.InvokeScript("showAnswer", new object[]{ viewModel.CurrentItem.Question.Answer
                        , viewModel.CurrentItem.Question.Analysis, isRight, isDisabled});
            }
            catch (Exception ex)
            {
                Debug.WriteLine("showAnswer JS调用错误：" + ex.Message);
            }
        }

        private void ChkAutoNext_Click(object sender, RoutedEventArgs e)
        {

            var chk = ChkAutoNext.IsChecked;
            _isAutoNextChecked = chk.HasValue && chk.Value;
            Log.RecordData("TikuAutoNext", _isAutoNextChecked);
        }
        /// <summary>
        /// 设置做题界面的背景颜色
        /// </summary>
        private void SetRightDgvBackColor()
        {
            System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(Util.QuestionBackColor);
            System.Windows.Media.SolidColorBrush scb = new System.Windows.Media.SolidColorBrush(color);
            rightDgv.Background = scb;
        }

        private void TextBlock_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var viewModel = DataContext as PaperViewModel;
            if (viewModel == null || viewModel.CurrentItem == null)
                return;
            try
            {
                colorDialog.AllowFullOpen = true;
                colorDialog.FullOpen = true;
                colorDialog.Color = System.Drawing.ColorTranslator.FromHtml(Util.QuestionBackColor);
                if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string backColor = System.Drawing.ColorTranslator.ToHtml(colorDialog.Color);
                    Util.QuestionBackColor = backColor;
                    SetRightDgvBackColor();
                    if (_webBrowserOverlay.WebBrowser.Document != null)
                        _webBrowserOverlay.WebBrowser.Document.InvokeScript("changeBackColor", new object[] { backColor });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("changeBackColor JS调用错误：" + ex.Message);
            }
        }
    }


    [ComVisible(true)]//将该类设置为com可访问 
    public class ObjectForScriptingHelper
    {
        private readonly PaperPage _page;
        public ObjectForScriptingHelper(PaperPage page)
        {
            _page = page;
        }

        public void ClickEvent()
        {
            var viewModel = _page.DataContext as PaperViewModel;
            if (viewModel != null)
            {
                if (_page.ChkAutoNext.IsChecked == true)
                {
                    viewModel.AutoNextCommand.Execute(null);
                }
            }
        }
        /// <summary>
        /// 试题选择状态更改事件
        /// </summary>
        /// <param name="userAnswer"></param>
        public void SelectStateChang(string userAnswer)
        {
            var viewModel = _page.DataContext as PaperViewModel;
            if (viewModel != null)
            {
                viewModel.QuestionStateCommand.Execute(userAnswer);
            }
        }
    }
}
