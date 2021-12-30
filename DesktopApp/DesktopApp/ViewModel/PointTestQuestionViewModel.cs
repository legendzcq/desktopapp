using System;
using System.Text;
using Framework.NewModel;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
	public class PointTestQuestionViewModel : ViewModelBase
	{
		#region HTML定义

		/// <summary>
		/// 试题整体网页结构
		/// </summary>
		private const string QuestionBodyHtml = @"
<!DOCTYPE html>
<html>
    <head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    {0}
    {1}
    </head> 
    <body> 
    <div id=""pageWrapper"" style=""color:#000; font-size:18px; background-color:#fff"">
        {2}
    </div>
  </body>
</html>
";
		/// <summary>
		/// 未选中图片路径
		/// </summary>
		private static readonly string ImgUncheckedPath = "file://" + AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + @"images/uncheck.png" + @"";

		/// <summary>
		/// 已选中图片路径
		/// </summary>
		private static readonly string ImgCheckedPath = "file://" + AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + @"images/check.png" + @"";

		/// <summary>
		/// 单选框样式
		/// </summary>
		private static readonly string SingleImgStyle = string.Format(@"
            .inputradio {{ display:none; }}
            .inputRspan {{ display:block; float: left; width: 40px; height: 40px; vertical-align:middle; background:url('{0}') no-repeat; cursor:pointer;}}
            .checked {{background:url('{1}') no-repeat;}}", ImgUncheckedPath, ImgCheckedPath);

		/// <summary>
		/// 设置单选框选中
		/// </summary>
		private const string SingleSetCheckJs = @"
        function setthischeck(v) {
            if(document.getElementById(v).disabled) return;
            var chk = document.getElementById(v).checked;
            document.getElementById(v).checked = true;
            var radios = document.getElementsByName('radio');
            for (var i = 0; i < radios.length; i++) {
                if (radios[i].value != v) {
                    document.getElementById('span' + radios[i].value).className = 'inputRspan';
                }
                else {
                    document.getElementById('span' + radios[i].value).className = 'inputRspan checked';
                }
            }
            setTimeout('window.external.ClickEvent()',300);// 延时自动进入下一题
            var curAnswer = getAnswer();
            window.external.SelectStateChang(curAnswer);// 选项改变切换图片状态
        }
";

		/// <summary>
		/// 单选设置用户答案Js
		/// </summary>
		private const string SingleSetAnswerJs = @"
            var radios = document.getElementsByName('radio');
            for (var i = 0; i < radios.length; i++) {
                if (radios[i].value == s) {
                    radios[i].checked = true;
                    document.getElementById('span' + radios[i].value).className = 'inputRspan checked';
                }
            }";

		/// <summary>
		/// 单选获取用户答案Js
		/// </summary>
		private const string SingleGetAnswerJs = @"
            var answer='';
            var radios = document.getElementsByName('radio'); 
            for(var i=0;i<radios.length;i++){ 
                if(radios[i].checked){ 
                    answer = radios[i].getAttribute('value'); 
                } 
            }
            return answer; ";

		/// <summary>
		/// 单选显示正确答案
		/// </summary>
		private const string SingleShowAnswerJs = @"document.getElementById('RightAnswer').innerHTML = answer;
            document.getElementById('Analysis').innerHTML = analysis;
            if(isDisabled=='true'){
                var radios = document.getElementsByName('radio');
                for (var i = 0; i < radios.length; i++) {
                    radios[i].disabled='true';
                }
            }";

		/// <summary>
		/// 复选框样式
		/// </summary>
		private static readonly string MultyCheckStyle = string.Format(@"
            .inputchk {{ display:none; }}
            .inputchkspan {{display:block;float: left; width: 40px; height: 40px; vertical-align:middle; background:url('{0}') no-repeat; cursor:pointer; }}
            .checked {{ background:url('{1}') no-repeat; }}", ImgUncheckedPath, ImgCheckedPath);

		/// <summary>
		/// 多选题设置复选框
		/// </summary>
		private const string MultiSetCheckJs = @"
        function setCheck(name) {
            if(document.getElementById(name).disabled) return;
            var v = document.getElementById(name).checked;
            v = !v;
            document.getElementById(name).checked = v;
            if (v) {
                document.getElementById('span' + name).className = 'inputchkspan checked';
            }
            else {
                document.getElementById('span' + name).className = 'inputchkspan';
            }
            var curAnswer = getAnswer();
            window.external.SelectStateChang(curAnswer);// 选项改变切换图片状态
        }";

		/// <summary>
		/// 多选题获取用户答案
		/// </summary>
		private const string MultiGetAnswerJs = @"var answer='';
            var chks = document.getElementsByName('chk'); 
            for(var i=0;i<chks.length;i++){ 
                if(chks[i].checked){ 
                    answer = answer + chks[i].getAttribute('value'); 
                } 
            }
            return answer; ";

		/// <summary>
		/// 多选题设置用户答案
		/// </summary>
		private const string MultiSetAnswerJs = @"
            var chks = document.getElementsByName('chk');
            for (var i = 0; i < chks.length; i++) {
                if (s.indexOf(chks[i].value) == -1) {
                    chks[i].checked = false;
                }
                else{
                    chks[i].checked = true;
                    document.getElementById('span' + chks[i].id).className = 'inputchkspan checked';
                }
            }";

		/// <summary>
		/// 多选题显示用户答案
		/// </summary>
		private const string MultiShowAnswerJs = @"
            document.getElementById('RightAnswer').innerHTML = answer;
            document.getElementById('Analysis').innerHTML = analysis;
            if(isDisabled=='true'){
                var chks = document.getElementsByName('chk');
                for (var i = 0; i < chks.length; i++) {
                    chks[i].disabled='true';
                }
            }";
		/// <summary>
		/// 主观题获取用户答案
		/// </summary>
		private const string TextGetAnswerJs = "return document.getElementById('txt_area').value;";

		/// <summary>
		/// 主观题设置用户答案
		/// </summary>
		private const string TextSetAnswerJs = "document.getElementById('txt_area').value = s;";

		/// <summary>
		/// 主观题显示答案
		/// </summary>
		private const string TextShowAnswerJs = @"
            /*document.getElementById('RightAnswer').innerHTML = answer;*/
            document.getElementById('Analysis').innerHTML = analysis;
            if(isDisabled=='true'){ 
                document.getElementById('txt_area').disabled='true'; 
            }";

		/// <summary>
		/// 所有的样式
		/// </summary>
		private const string AllStyle = @"
        <style>                            
            textarea {{ -ms-text-size-adjust:auto; width:100%; border:1px solid #272822; height:150px; }}
            body {{ -ms-text-size-adjust:auto; margin:10px; padding:0px; background-color:#fff; }} 
            div#IsRight, div#IsError, div#Answer{{
                border-radius: 10px;
                background-color: #EEE;
                border: 1px solid #DDD;
                margin-top: 10px;
            }}
            div {{ display:block; float:left ; width:100% }}
            div#IsRight p,div#IsError p,div#Answer p{{
                margin: 10px;
            }}
            {0}
            .btn{{ font-size:14px; text-align:center; line-height:25px; text-decoration:none; display:block;color:#fff;width:75px;height:25px; background-image:url('file://{1}images/btn.png');}}
            .btn:hover{{ background-image:url('file://{1}images/btnhover.png');}}
        </style>";

		private const string AllScript = @"
        <script type=""text/javascript"" charset=""utf-8"">
        function getAnswer(){{ 
            {1} 
        }}

        function setAnswer(s){{ 
            {2} 
        }}
        
        function showAnswer(answer,analysis,isRight,isDisabled){{
            if(isRight=='true') {{
                document.getElementById('IsRight').style.display = '';
                document.getElementById('IsError').style.display = 'none';
            }}
            else if(isRight=='false') {{
                document.getElementById('IsRight').style.display = 'none';
                document.getElementById('IsError').style.display = '';
            }}
            {3} 
            document.getElementById('Answer').style.display = '';
        }}

        function toggleShowQuestion(){{ 
            var div = document.getElementById('parentContentDiv');
            var btn = document.getElementById('btnTip'); 
            if(div.style.display == '') {{
                div.style.display = 'none'; btn.innerText='显示题干';
            }} 
            else {{
                div.style.display = ''; btn.innerText='隐藏题干';
            }}
        }}
        {0}
        </script>";


		#endregion
		private string _htmlContent;
		private string _userAnswer = string.Empty;

		private int _isRight;
		private string _statusImage = "/Images/Paper/exam_undo.png";
		private string _number;
		private string _typeTitle;
		private bool _isZhuguanHasAnswer;

		public PointTestQuestionItem QuestionItem { get; set; }

		/// <summary>
		/// 试题类型（包含题号，比如 一、单选题）
		/// </summary>
		public string TypeTitle
		{
			private get { return _typeTitle; }
			set
			{
				_typeTitle = value;
				RaisePropertyChanged(() => TypeTitle);
			}
		}

		/// <summary>
		/// 题号
		/// </summary>
		public string Number
		{
			get { return _number; }
			set
			{
				_number = value;
				RaisePropertyChanged(() => Number);
			}
		}

		/// <summary>
		/// 做题状态图片
		/// </summary>
		public string StatusImage
		{
			get { return _statusImage; }
			set
			{
				if (_statusImage == value)
					return;
				_statusImage = value;
				RaisePropertyChanged(() => StatusImage);
			}
		}

		/// <summary>
		/// HTML内容
		/// </summary>
		public string HtmlContent
		{
			get { return _htmlContent; }
			set
			{
				_htmlContent = value;
				RaisePropertyChanged(() => HtmlContent);
			}
		}

		/// <summary>
		/// 用户答案
		/// </summary>
		public string UserAnswer
		{
			get { return _userAnswer; }
			set
			{
				_userAnswer = value;
				CheckUserAnswer();
				RaisePropertyChanged(() => UserAnswer);
			}
		}

		/// <summary>
		/// 用户回答是否正确(0:未做，1:正确，2:错误)
		/// </summary>
		public int IsRight
		{
			get { return _isRight; }
			set
			{
				_isRight = value;
				RaisePropertyChanged(() => IsRight);
			}
		}

		private bool IsZhuguanHasAnswer
		{
			get { return _isZhuguanHasAnswer; }
			set
			{
				if (_isZhuguanHasAnswer != value)
				{
					_isZhuguanHasAnswer = value;
				}
			}
		}

		public PointTestQuestionViewModel(PointTestQuestionItem item)
		{
			QuestionItem = item;
			ConvertToHtml();
		}

		/// <summary>
		/// 试题转换为HTML
		/// </summary>
		private void ConvertToHtml()
		{
			var builder = new StringBuilder();

			var imgStyle = string.Empty;

			builder.Append("题型：");
			builder.Append(QuestionItem.QuesTypeName);// 题型
			//builder.Append("[" + question.QuestionId + "]");
			//if (question.Parent != null)
			//{
			//	var hiddenStyle = question.SubId == 1 ? "" : "display:none;";
			//	var tipTxt = question.SubId == 1 ? "隐藏题干" : "显示题干";

			//	builder.AppendLine(string.Format(@"<div id=""parentContentDiv"" style=""{0}"">{1}. {2}</div>", hiddenStyle, question.MainId, question.Parent.Content));// 题干
			//	builder.AppendLine(string.Format(@"<div><a id=""btnTip"" href=""javascript:toggleShowQuestion();"" class=""btn"">{0}</a></div>", tipTxt));
			//	builder.AppendLine(string.Format(@"<div> &lt;{0}&gt;. {1}</div>", question.SubId, question.Content));// 子题目题干
			//}
			//else
			//{
			builder.AppendLine(string.Format("<div>{0}. {1}</div>", QuestionItem.QuestionId, QuestionItem.Content));// 子题目题干
			//}

			var additionalJs = string.Empty;
			var getAnswerJs = string.Empty;
			var setAnswerJs = string.Empty;
			var showAnswerJs = string.Empty;
			switch (QuestionItem.QuesType)
			{
				case 1:
				case 3:
					imgStyle = SingleImgStyle;
					additionalJs = SingleSetCheckJs;
					getAnswerJs = SingleGetAnswerJs;
					setAnswerJs = SingleSetAnswerJs;
					showAnswerJs = SingleShowAnswerJs;
					foreach (var option in QuestionItem.QuestionOptionList)
					{
						var spanId = "span" + option.QuesValue;
						builder.AppendLine("<div style='clear: both; padding-top: 4px;'>");
						builder.AppendLine(string.Format(@"<input type=""radio"" name=""radio"" id=""{0}"" value=""{0}"" class=""inputradio"" /><label for=""{0}"" onclick=""setthischeck('{0}')""><span id='{1}' class=""inputRspan""></span>{0}. {2}</label><br />", option.QuesValue, spanId, option.QuesOption));
						builder.AppendLine("</div>");
					}
					break;
				case 2:
					imgStyle = MultyCheckStyle;
					additionalJs = MultiSetCheckJs;
					getAnswerJs = MultiGetAnswerJs;
					// JS循环获取答案字母
					setAnswerJs = MultiSetAnswerJs;
					showAnswerJs = MultiShowAnswerJs;
					foreach (var option in QuestionItem.QuestionOptionList)
					{
						var spanId = "span" + option.QuesValue;
						builder.AppendLine("<div style='clear: both; padding-top: 4px;'>");
						builder.AppendLine(string.Format(@"<input type=""checkbox"" name=""chk"" id=""{0}"" value=""{0}"" class=""inputchk"" /><label for=""{0}"" onclick=""setCheck('{0}')""><span id='{1}' class=""inputchkspan""></span>{0}. {2}</label><br />", option.QuesValue, spanId, option.QuesOption));
						builder.AppendLine("</div>");
					}
					break;
				case 4:
					getAnswerJs = TextGetAnswerJs;
					setAnswerJs = TextSetAnswerJs;
					showAnswerJs = TextShowAnswerJs;

					builder.AppendLine(@"<textarea id=""txt_area"" onkeyup=""window.external.SelectStateChang(document.getElementById('txt_area').value)""></textarea>");
					break;
			}
			builder.AppendLine(@"<div id=""IsRight"" style=""display:none;color:#008000;""><p>回答正确</p></div>");
			builder.AppendLine(@"<div id=""IsError"" style=""display:none;color:#ff0087;""><p>回答错误</p></div>");
			builder.AppendLine(string.Format(@"<div id=""Answer"" style=""display:none""><div><span style=""color:#2D64B3"">正确答案：</span><span id=""RightAnswer"" style=""color:red"">{0}</span></div><div><span style=""color:green"">解析：</span><span id=""Analysis""></span></div></div><br /><br />", ((QuestionItem.QuesType == 4 || QuestionItem.QuesType == 7) ? "<br />" + QuestionItem.RightAnswer : string.Empty)));

			var js = string.Format(AllScript, additionalJs, getAnswerJs, setAnswerJs, showAnswerJs);

			var style = string.Format(AllStyle, imgStyle, AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"));
			var html = string.Format(
				QuestionBodyHtml
				, style
				, js
				, builder);

			HtmlContent = html;
		}

		/// <summary>
		/// 检查用户回答是否正确(0:未做，1:正确，2:错误)
		/// </summary>
		private void CheckUserAnswer()
		{
			if (QuestionItem.QuesType >= 4)
			{
				IsZhuguanHasAnswer = !string.IsNullOrEmpty(UserAnswer);
			}
			if (string.IsNullOrEmpty(UserAnswer) || QuestionItem.QuesType == 4)// 未做或者为阅读分析题
			{
				IsRight = 0;
				return;
			}
			IsRight = UserAnswer == QuestionItem.RightAnswer ? 1 : 2;
		}
	}
}