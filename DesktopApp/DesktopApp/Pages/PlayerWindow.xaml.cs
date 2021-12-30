using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

using DesktopApp.ViewModel;

using DownloadClass.Toolkit.Controls;

using Framework.Model;
using Framework.NewModel;
using Framework.Utility;

namespace DesktopApp.Pages
{
    /// <summary>
    /// PlayerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerWindow : Window
    {

        private readonly PlayerWindowViewModel _viewModel;
        private bool _isPointTestShowing;
        private bool _isPlayingBackup;

        private static readonly FieldInfo s_menuDropAlignmentField = typeof(SystemParameters).GetField("_menuDropAlignment", BindingFlags.NonPublic | BindingFlags.Static);

        static PlayerWindow()
        {
            EnsureStandardPopupAlignment();
            SystemParameters.StaticPropertyChanged += SystemParameters_StaticPropertyChanged;
        }

        private static void SystemParameters_StaticPropertyChanged(object sender, PropertyChangedEventArgs e) => EnsureStandardPopupAlignment();

        private static void EnsureStandardPopupAlignment()
        {
            if (SystemParameters.MenuDropAlignment && s_menuDropAlignmentField != null)
            {
                s_menuDropAlignmentField.SetValue(null, false);
            }
        }

        public PlayerWindow(PlayerWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
            SetBinding(LecturesProperty, nameof(PlayerWindowViewModel.Lectures));
            SetBinding(CurrentNodeProperty, new Binding(nameof(PlayerWindowViewModel.CurrentNode))
            {
                Mode = BindingMode.OneWayToSource
            });
            SetBinding(PointTestsProperty, nameof(PlayerWindowViewModel.PointTests));
        }

        public IEnumerable<StudentCwareKcjy> Lectures
        {
            get => (IEnumerable<StudentCwareKcjy>)GetValue(LecturesProperty);
            set => SetValue(LecturesProperty, value);
        }

        public static readonly DependencyProperty LecturesProperty =
            DependencyProperty.Register(nameof(Lectures), typeof(IEnumerable<StudentCwareKcjy>), typeof(PlayerWindow), new PropertyMetadata(default, OnLecturesChanged));

        private static void OnLecturesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as PlayerWindow;
            if (e.NewValue is IEnumerable<StudentCwareKcjy> lectures)
            {
                control.Insert(lectures);
            }
        }

        public IEnumerable<PointTestStartTimeItem> PointTests
        {
            get => (IEnumerable<PointTestStartTimeItem>)GetValue(PointTestsProperty);
            set => SetValue(PointTestsProperty, value);
        }

        public static readonly DependencyProperty PointTestsProperty =
            DependencyProperty.Register(nameof(PointTests), typeof(IEnumerable<PointTestStartTimeItem>), typeof(PlayerWindow), new PropertyMetadata(default));

        public string CurrentNode
        {
            get => (string)GetValue(CurrentNodeProperty);
            set => SetValue(CurrentNodeProperty, value);
        }

        public static readonly DependencyProperty CurrentNodeProperty =
            DependencyProperty.Register(nameof(CurrentNode), typeof(string), typeof(PlayerWindow), new PropertyMetadata(default));

        private void Insert(IEnumerable<StudentCwareKcjy> lectures)
        {
            MSHTML.IHTMLElement body = (WebBrowser.Document as MSHTML.HTMLDocument)?.body;
            if (body == default)
                return;

            var stringBuilder = new StringBuilder();
            foreach (StudentCwareKcjy item in lectures)
            {
                stringBuilder.AppendLine($@"<a class=""ant"" name=""A{item.NodeId}""></a>");
                stringBuilder.AppendLine($@"<div id=""{item.NodeId}"" class=""textNode"" ondblclick=""setPosition('{item.NodeId}')"">");
                stringBuilder.AppendLine(item.NodeText);
                stringBuilder.AppendLine("</div>");
            }
            body.innerHTML = stringBuilder.ToString();
        }

        private void WebBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if (Lectures != default)
                Insert(Lectures);
            WebBrowser.ObjectForScripting = new FromScripting(this);
        }

        private void WebBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            Stream documentStreawm = Application.GetResourceStream(new Uri("/Assets/lecture.html", UriKind.Relative)).Stream;
            WebBrowser.NavigateToStream(documentStreawm);
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combox = sender as ComboBox;
            WebBrowser?.InvokeScript("setFont", combox.SelectedIndex);
        }

        private void MediaPlayerElement_PositionChanged(object sender, RoutedPropertyChangedEventArgs<TimeSpan> e)
        {
            GotoLectureNode(e.NewValue);
            PopupPointTest(e.NewValue);
        }

        private void PopupPointTest(TimeSpan position)
        {
            if (_isPointTestShowing || !Util.IsAutoShowPoint || PointTests?.Any() != true)
                return;

            PointTestStartTimeItem pointTest = PointTests?.Where(x => x.PointOpenType != "t")
                .FirstOrDefault(x => x.PointTestStartTime < position.TotalSeconds && x.PointTestStartTime + 1 > position.TotalSeconds);
            if (pointTest == default)
                return;

            _isPointTestShowing = true;
            if (Commands.TogglePlayPause.CanExecute(default, this))
                Commands.TogglePlayPause.Execute(default, this);
            var win = new PointTest(pointTest, _viewModel.Course) { Owner = this };
            if (win.ShowDialog() == true && Math.Abs(pointTest.BackTime - position.TotalSeconds) > 1)
                MediaPlayerElement.Position = TimeSpan.FromSeconds(pointTest.BackTime);
            if (Commands.TogglePlayPause.CanExecute(default, this))
                Commands.TogglePlayPause.Execute(default, this);
            _isPointTestShowing = false;
        }

        private void GotoLectureNode(TimeSpan position)
        {
            if (Lectures == default)
                return;

            var nodeId = Lectures.Select(x =>
            {
                if (!TimeSpan.TryParse(x.TimeStart, out TimeSpan timePoint))
                    return default;
                TimeSpan diff = (position - timePoint).Duration();
                return new { Diff = diff, x.NodeId };
            })
                                             .OrderBy(x => x.Diff)
                                             .FirstOrDefault()
                                             ?.NodeId;
            if (!string.IsNullOrWhiteSpace(nodeId) && nodeId != CurrentNode)
            {
                WebBrowser?.InvokeScript("gotoNode", nodeId);
                CurrentNode = nodeId;
            }
        }

        [ComVisible(true)]
        public class FromScripting
        {
            private readonly PlayerWindow _playerWindow;
            public FromScripting(PlayerWindow playerWindow) => _playerWindow = playerWindow;
            public void SetPosition(string nodeId)
            {
                if (_playerWindow.Lectures == default)
                    return;

                var positionString = _playerWindow.Lectures.Where(x => x.NodeId == nodeId).Select(x => x.TimeStart).SingleOrDefault();
                if (TimeSpan.TryParse(positionString, out TimeSpan position))
                {
                    _playerWindow.MediaPlayerElement.Position = TimeSpan.Parse(positionString);
                    _playerWindow.CurrentNode = nodeId;
                }
            }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            Width = ActualWidth + 1;
            BeginTime = DateTimeOffset.Now;
        }

        private void Splitter_DragStarted(object sender, DragStartedEventArgs e)
        {
            _isPlayingBackup = MediaPlayerElement.IsPlaying;
            if (_isPlayingBackup == true)
                MediaPlayerElement.IsPlaying = false;
        }

        private void Splitter_DragCompleted(object sender, DragCompletedEventArgs e) => MediaPlayerElement.IsPlaying = _isPlayingBackup;

        public DateTimeOffset BeginTime
        {
            get => (DateTimeOffset)GetValue(BeginTimeProperty);
            set => SetValue(BeginTimeProperty, value);
        }

        public static readonly DependencyProperty BeginTimeProperty =
            DependencyProperty.Register(nameof(BeginTime), typeof(DateTimeOffset), typeof(PlayerWindow), new PropertyMetadata(default(DateTimeOffset)));

        public DateTimeOffset EndTime
        {
            get => (DateTimeOffset)GetValue(EndTimeProperty);
            set => SetValue(EndTimeProperty, value);
        }

        public static readonly DependencyProperty EndTimeProperty =
            DependencyProperty.Register(nameof(EndTime), typeof(DateTimeOffset), typeof(PlayerWindow), new PropertyMetadata(default(DateTimeOffset)));
    }
}