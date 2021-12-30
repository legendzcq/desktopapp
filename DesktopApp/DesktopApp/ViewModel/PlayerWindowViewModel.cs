using System;
using System.Collections.Generic;
using System.Linq;

using DesktopApp.Logic;

using Framework.Local;
using Framework.Model;
using Framework.NewModel;
using Framework.Utility;

using Prism.Commands;
using Prism.Mvvm;

namespace DesktopApp.ViewModel
{
    public class PlayerWindowViewModel : BindableBase
    {
        public ViewStudentWareDetail VideoItem { get; init; }
        public ViewStudentCourseWare Course { get; init; }

        private IEnumerable<StudentCwareKcjy> _lectures;
        public IEnumerable<StudentCwareKcjy> Lectures
        {
            get => _lectures;
            set => SetProperty(ref _lectures, value);
        }

        private string _currentNode;
        public string CurrentNode
        {
            get => _currentNode;
            set => SetProperty(ref _currentNode, value);
        }

        private IEnumerable<PointTestStartTimeItem> _pointTests;
        public IEnumerable<PointTestStartTimeItem> PointTests
        {
            get => _pointTests;
            set => SetProperty(ref _pointTests, value);
        }

        private DelegateCommand _loadLecturesCommand;
        public DelegateCommand LoadLecturesCommand => _loadLecturesCommand ??= new DelegateCommand(ExecuteLoadLecturesCommand);

        private void ExecuteLoadLecturesCommand() => Lectures = StudentWareLogic.GetStudentWareKcjyList(VideoItem.CwareId, VideoItem.VideoId);

        private DelegateCommand _loadKnowledgePointsCommand;
        public DelegateCommand LoadKnowledgePointsCommand =>
            _loadKnowledgePointsCommand ?? (_loadKnowledgePointsCommand = new DelegateCommand(ExecuteLoadKnowledgePointsCommand));

        private void ExecuteLoadKnowledgePointsCommand() => PointTests = StudentWareLogic.GetPointTestStartTimeList(VideoItem.CwareId, VideoItem.VideoId);

        private DelegateCommand _questionCommand;
        public DelegateCommand QuestionCommand => _questionCommand ??= new DelegateCommand(ExecuteQuestionCommand, CanExecuteQuestionCommand).ObservesProperty(() => CurrentNode);

        private bool CanExecuteQuestionCommand() => !string.IsNullOrWhiteSpace(CurrentNode);

        private void ExecuteQuestionCommand()
        {
            var boardId = new StudentWareData().GetStudentCwareBordId(Course.CwareId, Course.EduSubjectId).FirstOrDefault();
            var url = string.Format("[KCJYGET,{0},{1},{2},ISNEW]", Course.CwareId, int.Parse(VideoItem.VideoId), CurrentNode);
            StudentWareLogic.GotoLoginedWebSite(Course.CwareId, boardId, CurrentNode, url);
        }

        private DelegateCommand<TimeSpan?> _uploadRecordCommand;
        public DelegateCommand<TimeSpan?> UploadRecordCommand => _uploadRecordCommand ??= new DelegateCommand<TimeSpan?>(ExecuteUploadCommand, CanExecuteUploadCommand);

        private void ExecuteUploadCommand(TimeSpan? parameter)
        {
            var datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var svr = new StudentVideoRecord
            {
                CwareUrl = Course.CwareUrl,
                CwareId = Course.CwareId,
                LastPosition = (int)parameter.Value.TotalSeconds,
                LastTime = datetime,
                VideoID = VideoItem.VideoId,
                Uid = Util.SsoUid
            };
            SystemInfo.StartBackGroundThread("上传视频记录", () => StudentWareLogic.SaveNextBeginTime(svr));
        }

        private bool CanExecuteUploadCommand(TimeSpan? parameter) => parameter != default && Util.IsOnline;

        private DelegateCommand<(TimeSpan position, TimeSpan duration)?> _updateTimeCommand;

        public DelegateCommand<(TimeSpan position, TimeSpan duration)?> UpdateTimeCommand => _updateTimeCommand ??= new DelegateCommand<(TimeSpan position, TimeSpan duration)?>(ExecuteUpdateTimeCommand, CanExecuteUpdateTimeCommand);

        private void ExecuteUpdateTimeCommand((TimeSpan position, TimeSpan duration)? parameter) => StudentWareLogic.UpdateTime(VideoItem.CwareId, VideoItem.VideoId, (int)parameter.Value.position.TotalSeconds, (int)parameter.Value.duration.TotalSeconds);

        private bool CanExecuteUpdateTimeCommand((TimeSpan position, TimeSpan duration)? parameter) => parameter != default;

        private DelegateCommand<(TimeSpan length, TimeSpan position, DateTimeOffset beginTime, DateTimeOffset endTime, double speedRatio)?> _recordWatchingDataCommand;

        public DelegateCommand<(TimeSpan length, TimeSpan position, DateTimeOffset beginTime, DateTimeOffset endTime, double speedRatio)?> RecordWatchingDataCommand =>
            _recordWatchingDataCommand ?? (_recordWatchingDataCommand = new DelegateCommand<(TimeSpan length, TimeSpan position, DateTimeOffset beginTime, DateTimeOffset endTime, double speedRatio)?>(ExecuteWatchingDataCommand, CanExecuteRecordWatchingDataCommand));

        private void ExecuteWatchingDataCommand((TimeSpan length, TimeSpan position, DateTimeOffset beginTime, DateTimeOffset endTime, double speedRatio)? parameter)
        {
            var data = new StudentWareData();
            if (data.StudyVideoStrById(Course.CwareId, VideoItem.VideoId, 0, (int)parameter.Value.length.TotalSeconds) > 0)
                return;
            var timeStr = new TimebaseStr
            {
                VideoStartTime = "0",
                VideoEndTime = $"{parameter.Value.position.TotalSeconds}",
                Speed = $"{parameter.Value.speedRatio}",
                StudyTimeStart = $"{parameter.Value.beginTime.ToUnixTimeMilliseconds()}",
                StudyTimeEnd = $"{parameter.Value.endTime.ToUnixTimeMilliseconds()}",
                CwareId = Course.CwareId,
                VideoID = VideoItem.VideoId
            };
            data.AddStudentVideoTimebaseItem(timeStr);
        }

        private bool CanExecuteRecordWatchingDataCommand((TimeSpan length, TimeSpan position, DateTimeOffset beginTime, DateTimeOffset endTime, double speedRatio)? parameter) => parameter != null && parameter.Value.length > default(TimeSpan) && parameter.Value.endTime > default(DateTimeOffset);

        public override string ToString()
        {
            var content = $"{Course.CourseWareName} {VideoItem.ChapterName} {VideoItem.Title}";
            return content;
        }
    }
}