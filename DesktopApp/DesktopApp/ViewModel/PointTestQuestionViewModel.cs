using System;
using System.Text;
using Framework.NewModel;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
	public class PointTestQuestionViewModel : ViewModelBase
	{
		#region HTML����

		/// <summary>
		/// ����������ҳ�ṹ
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
		/// δѡ��ͼƬ·��
		/// </summary>
		private static readonly string ImgUncheckedPath = "file://" + AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + @"images/uncheck.png" + @"";

		/// <summary>
		/// ��ѡ��ͼƬ·��
		/// </summary>
		private static readonly string ImgCheckedPath = "file://" + AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/") + @"images/check.png" + @"";

		/// <summary>
		/// ��ѡ����ʽ
		/// </summary>
		private static readonly string SingleImgStyle = string.Format(@"
            .inputradio {{ display:none; }}
            .inputRspan {{ display:block; float: left; width: 40px; height: 40px; vertical-align:middle; background:url('{0}') no-repeat; cursor:pointer;}}
            .checked {{background:url('{1}') no-repeat;}}", ImgUncheckedPath, ImgCheckedPath);

		/// <summary>
		/// ���õ�ѡ��ѡ��
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
            setTimeout('window.external.ClickEvent()',300);// ��ʱ�Զ�������һ��
            var curAnswer = getAnswer();
            window.external.SelectStateChang(curAnswer);// ѡ��ı��л�ͼƬ״̬
        }
";

		/// <summary>
		/// ��ѡ�����û���Js
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
		/// ��ѡ��ȡ�û���Js
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
		/// ��ѡ��ʾ��ȷ��
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
		/// ��ѡ����ʽ
		/// </summary>
		private static readonly string MultyCheckStyle = string.Format(@"
            .inputchk {{ display:none; }}
            .inputchkspan {{display:block;float: left; width: 40px; height: 40px; vertical-align:middle; background:url('{0}') no-repeat; cursor:pointer; }}
            .checked {{ background:url('{1}') no-repeat; }}", ImgUncheckedPath, ImgCheckedPath);

		/// <summary>
		/// ��ѡ�����ø�ѡ��
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
            window.external.SelectStateChang(curAnswer);// ѡ��ı��л�ͼƬ״̬
        }";

		/// <summary>
		/// ��ѡ���ȡ�û���
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
		/// ��ѡ�������û���
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
		/// ��ѡ����ʾ�û���
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
		/// �������ȡ�û���
		/// </summary>
		private const string TextGetAnswerJs = "return document.getElementById('txt_area').value;";

		/// <summary>
		/// �����������û���
		/// </summary>
		private const string TextSetAnswerJs = "document.getElementById('txt_area').value = s;";

		/// <summary>
		/// ��������ʾ��
		/// </summary>
		private const string TextShowAnswerJs = @"
            /*document.getElementById('RightAnswer').innerHTML = answer;*/
            document.getElementById('Analysis').innerHTML = analysis;
            if(isDisabled=='true'){ 
                document.getElementById('txt_area').disabled='true'; 
            }";

		/// <summary>
		/// ���е���ʽ
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
                div.style.display = 'none'; btn.innerText='��ʾ���';
            }} 
            else {{
                div.style.display = ''; btn.innerText='�������';
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
		/// �������ͣ�������ţ����� һ����ѡ�⣩
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
		/// ���
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
		/// ����״̬ͼƬ
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
		/// HTML����
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
		/// �û���
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
		/// �û��ش��Ƿ���ȷ(0:δ����1:��ȷ��2:����)
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
		/// ����ת��ΪHTML
		/// </summary>
		private void ConvertToHtml()
		{
			var builder = new StringBuilder();

			var imgStyle = string.Empty;

			builder.Append("���ͣ�");
			builder.Append(QuestionItem.QuesTypeName);// ����
			//builder.Append("[" + question.QuestionId + "]");
			//if (question.Parent != null)
			//{
			//	var hiddenStyle = question.SubId == 1 ? "" : "display:none;";
			//	var tipTxt = question.SubId == 1 ? "�������" : "��ʾ���";

			//	builder.AppendLine(string.Format(@"<div id=""parentContentDiv"" style=""{0}"">{1}. {2}</div>", hiddenStyle, question.MainId, question.Parent.Content));// ���
			//	builder.AppendLine(string.Format(@"<div><a id=""btnTip"" href=""javascript:toggleShowQuestion();"" class=""btn"">{0}</a></div>", tipTxt));
			//	builder.AppendLine(string.Format(@"<div> &lt;{0}&gt;. {1}</div>", question.SubId, question.Content));// ����Ŀ���
			//}
			//else
			//{
			builder.AppendLine(string.Format("<div>{0}. {1}</div>", QuestionItem.QuestionId, QuestionItem.Content));// ����Ŀ���
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
					// JSѭ����ȡ����ĸ
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
			builder.AppendLine(@"<div id=""IsRight"" style=""display:none;color:#008000;""><p>�ش���ȷ</p></div>");
			builder.AppendLine(@"<div id=""IsError"" style=""display:none;color:#ff0087;""><p>�ش����</p></div>");
			builder.AppendLine(string.Format(@"<div id=""Answer"" style=""display:none""><div><span style=""color:#2D64B3"">��ȷ�𰸣�</span><span id=""RightAnswer"" style=""color:red"">{0}</span></div><div><span style=""color:green"">������</span><span id=""Analysis""></span></div></div><br /><br />", ((QuestionItem.QuesType == 4 || QuestionItem.QuesType == 7) ? "<br />" + QuestionItem.RightAnswer : string.Empty)));

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
		/// ����û��ش��Ƿ���ȷ(0:δ����1:��ȷ��2:����)
		/// </summary>
		private void CheckUserAnswer()
		{
			if (QuestionItem.QuesType >= 4)
			{
				IsZhuguanHasAnswer = !string.IsNullOrEmpty(UserAnswer);
			}
			if (string.IsNullOrEmpty(UserAnswer) || QuestionItem.QuesType == 4)// δ������Ϊ�Ķ�������
			{
				IsRight = 0;
				return;
			}
			IsRight = UserAnswer == QuestionItem.RightAnswer ? 1 : 2;
		}
	}
}