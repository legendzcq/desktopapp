using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Framework.NewModel
{
    [DataContract]
    public class PointTestQuestionReturn
    {
        [DataMember(Name = "success")]
        public bool Success { get; set; }

        [DataMember(Name = "retry")]
        public bool Retry { get; set; }

        [DataMember(Name = "result")]
        public PointTestQuestionResult Result { get; set; }
    }

    [DataContract]
	public class PointTestQuestionResult
	{
		[DataMember(Name="code")]
		public int Code { get; set; }

		[DataMember(Name = "questionsList")]
		public IEnumerable<PointTestQuestionItem> QuestionList { get; set; }
	}

	[DataContract]
	public class PointTestQuestionItem
	{
		[DataMember(Name = "content")]
		public string Content { get; set; }

		[DataMember(Name = "parentID")]
		public int ParentId { get; set; }

		[DataMember(Name = "pointTestID")]
		public int PointTestId { get; set; }

		[DataMember(Name = "quesViewType")]
		public int QuesViewType { get; set; }

		[DataMember(Name = "questionID")]
		public int QuestionId { get; set; }

		[DataMember(Name = "score")]
		public double Score { get; set; }

		[DataMember(Name = "rightAnswer")]
		public string RightAnswer { get; set; }

		[DataMember(Name = "analysis")]
		public string Analysis { get; set; }

		[DataMember(Name = "splitScore")]
		public double SplitScore { get; set; }

		[DataMember(Name = "pointID")]
		public int PointId { get; set; }

		[DataMember(Name = "quesType")]
		public int QuesType { get; set; }

		public string QuesTypeName {
			get
			{
				switch (QuesType)
				{
					case 1:
						return "单项选择题";
					case 2:
						return "多项选择题";
					case 3:
						return "判断题";
					default:
						return "其他题型";
				}
			}
		}

		[DataMember(Name = "optionList")]
		public IEnumerable<PointTestQuestionOptionItem> QuestionOptionList { get; set; }
	}

	[DataContract]
	public class PointTestQuestionOptionItem
	{
		[DataMember(Name = "questionID")]
		public int QuestionId { get; set; }

		[DataMember(Name = "quesValue")]
		public string QuesValue { get; set; }

		[DataMember(Name = "quesOption")]
		public string QuesOption { get; set; }

		[DataMember(Name = "sequence")]
		public int Sequence { get; set; }
	}
}
