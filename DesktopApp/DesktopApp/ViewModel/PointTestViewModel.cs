using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Framework.Local;
using Framework.NewModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace DesktopApp.ViewModel
{
	public class PointTestViewModel : ViewModelBase
	{
		private PointTestQuestionViewModel _currentItem;
		private int _currentIndex;
		public PointTestStartTimeItem PointTestItem { get; set; }

		public ObservableCollection<PointTestQuestionViewModel> QuestionList { get; set; }

		public ICommand MoveNextCommand { get; set; }

		public ICommand MovePresCommand { get; set; }

		public bool HasSubmit { get; set; }

		public int CurrentIndex
		{
			get { return _currentIndex; }
			set
			{
				_currentIndex = value;
				RaisePropertyChanged(() => CurrentIndex);
				RaisePropertyChanged(() => CanMoveNext);
				RaisePropertyChanged(() => CanMovePres);
			}
		}

		public PointTestQuestionViewModel CurrentItem
		{
			get { return _currentItem; }
			set
			{
				_currentItem = value;
				RaisePropertyChanged(() => CurrentItem);
			}
		}

		public bool CanMoveNext
		{
			get { return QuestionList.Count > CurrentIndex + 1; }
		}

		public bool CanMovePres
		{
			get { return 0 < CurrentIndex; }
		}

		public string TestType
		{
			get
			{
				if (PointTestItem.PointOpenType == "z")
				{
					return "知识点测试";
				}
				if (PointTestItem.PointOpenType == "d")
				{
					return "单元测试";
				}
				if (PointTestItem.PointOpenType == "t")
				{
					return "知识点练习";
				}
				return string.Empty;
			}
		}

		public bool HasAnswer()
		{
			return QuestionList.Count(x => !string.IsNullOrEmpty(x.UserAnswer)) > 0;
		}

		public PointTestViewModel(PointTestStartTimeItem item)
		{
			PointTestItem = item;
			var list = new StudentWareData().GetPointTestQuestionList(item.TestId);
			QuestionList = new ObservableCollection<PointTestQuestionViewModel>();
			foreach (var qitem in list)
			{
				QuestionList.Add(new PointTestQuestionViewModel(qitem));
			}
			_currentIndex = 0;
			if (QuestionList.Count > 0) CurrentItem = QuestionList[0];

			MoveNextCommand = new RelayCommand(() =>
			{
				if (CurrentIndex < QuestionList.Count - 1)
				{
					CurrentItem = QuestionList[CurrentIndex + 1];
					CurrentIndex++;
				}
			});

			MovePresCommand = new RelayCommand(() =>
			{
				if (CurrentIndex > 0)
				{
					CurrentItem = QuestionList[CurrentIndex - 1];
					CurrentIndex--;
				}
			});
		}

		public string GetQuestionInfo()
		{
			return "{\"questions\" : [" + string.Join(",",
				QuestionList.Where(x => !string.IsNullOrEmpty(x.UserAnswer)).Select(x => "{\"questionID\":" + x.QuestionItem.QuestionId + ", \"userAnswer\":\"" + x.UserAnswer + "\"}")) + "]}";
		}
	}
}
