using System;
using System.Linq;
using System.Text;
using DesktopApp.Logic;
using Framework.Model;
using Framework.Utility;
using GalaSoft.MvvmLight;

namespace DesktopApp.ViewModel
{
	public class QuestionItemViewModel : ViewModelBase
	{
		#region HTML定义

		/// <summary>
		/// 试题整体网页结构
		/// </summary>
//        private const string QuestionBodyHtml = @"
//<!DOCTYPE html>
//<html>
//    <head>
//    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
//    {0}
//    {1}
//    </head> 
//    <body> 
//    <div id=""pageWrapper"" style=""color:#000; font-size:18px; background-color:#fff"">
//        {2}
//    </div>
//  </body>
//</html>
//";
        private const string QuestionBodyHtml = @"
<!DOCTYPE html>
<html>
    <head>
    <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"" />
    {0}
    {1}
    </head> 
    <body> 
    <div id=""pageWrapper"" style=""color:#000; font-size:{4}px; background-color:{3}"">
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
		private const string SingleShowAnswerJs = @"
            document.getElementById('RightAnswer').innerHTML = answer;
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
		/// 主观题获取用户答案
		/// </summary>
		private const string BlankGetAnswerJs = "return document.getElementById('txtBlank').value;";

		/// <summary>
		/// 主观题设置用户答案
		/// </summary>
		private const string BlankSetAnswerJs = "document.getElementById('txtBlank').value = s;";

		/// <summary>
		/// 主观题显示答案
		/// </summary>
		private const string BlankShowAnswerJs = @"
            document.getElementById('RightAnswer').innerHTML = answer;
            document.getElementById('Analysis').innerHTML = analysis;
            if(isDisabled=='true'){ 
                document.getElementById('txtBlank').disabled='true'; 
            }";

		//private const string FlashShowAnswerJs = @"document.getElementById('Analysis').innerHTML = ""<embed wmode='Opaque' allowfullscreen='true' src='"" + analysis + ""' type='application/x-shockwave-flash' pluginspage='http://www.adobe.com/shockwave/download/download.cgi?P1_Prod_Version=ShockwaveFlash' width='1007' height='540'>"";";
		private const string FlashShowAnswerJs = @"document.getElementById('Analysis').innerHTML = ""<input type=\""button\"" value=\""在线查看解析\"" onclick = \""window.open('"" + analysis + ""')\"" />"";";

		private const string DefaultShowAnswerJs = @"
            document.getElementById('RightAnswer').innerHTML = answer;
            document.getElementById('Analysis').innerHTML = analysis;";

		/// <summary>
		/// 所有的样式
		/// </summary>
//        private const string AllStyle = @"
//        <style>                            
//            textarea {{ -ms-text-size-adjust:auto; width:100%; border:1px solid #272822; height:150px; }}
//            body {{ -ms-text-size-adjust:auto; margin:10px; padding:0px; background-color:#fff; }} 
//            div#IsRight, div#IsError, div#Answer{{
//                border-radius: 10px;
//                background-color: #EEE;
//                border: 1px solid #DDD;
//                margin-top: 10px;
//            }}
//            div {{ display:block; float:left ; width:100% }}
//            div#IsRight p,div#IsError p,div#Answer p{{
//                margin: 10px;
//            }}
//            {0}
//            .btn{{ font-size:14px; text-align:center; line-height:25px; text-decoration:none; display:block;color:#fff;width:75px;height:25px; background-image:url('file://{1}images/btn.png');}}
//            .btn:hover{{ background-image:url('file://{1}images/btnhover.png');}}
//        </style>";



        /// <summary>
        /// 所有的样式
        /// </summary>
        private const string AllStyle = @"
        <style>                            
            textarea {{ -ms-text-size-adjust:auto; width:100%; border:1px solid #272822; height:150px; }}
            body {{ -ms-text-size-adjust:auto; margin:10px; padding:0px; background-color:{0}; }} 
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
            {1}
            .btn{{ font-size:14px; text-align:center; line-height:25px; text-decoration:none; display:block;color:#fff;width:75px;height:25px; background-image:url('file://{2}images/btn.png');}}
            .btn:hover{{ background-image:url('file://{2}images/btnhover.png');}}
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
			{3}
			if(document.getElementById('Answer').style.display=='')
			{{
				document.getElementById('Answer').style.display = 'none';
				document.getElementById('IsRight').style.display = 'none';
				document.getElementById('IsError').style.display = 'none';
			}}
			else
			{{
				if(isRight=='true') {{
					document.getElementById('IsRight').style.display = '';
					document.getElementById('IsError').style.display = 'none';
				}}
				else if(isRight=='false') {{
					document.getElementById('IsRight').style.display = 'none';
					document.getElementById('IsError').style.display = '';
				}}
				document.getElementById('Answer').style.display = '';
			}}
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

      function changeBackColor(color) {{
            document.body.style.backgroundColor = color;
            document.getElementById('pageWrapper').style.backgroundColor =color;
        }}
        </script>";

		private const string ClassifyStyle = @"
        optgroup { font-style: normal; font-weight: bold }
        .classify{ font-size:10.5pt; height:30px; line-height:30px; background-color:#f0f0f0; }
        .classifytable { width: 500px; margin: 0 auto; }
        .classifytable .check { width: 30px; }
        #divResult { background-color: #f0f0f0; font-size:10.5pt; }
        .btn { float: left; margin-right: 3px; }
        .toolbar {margin-top:5px; margin-bottom:5px;}
        .resulttitle{clear: both; background-color: #f0f0f0; color:#333333; font-size:10.5pt; height:30px; line-height:30px; }
";
		private const string ClassifyGetAnswerJs = @"
            var str = ""["";
            for (var i = 0; i < list.length; i++) {
                str += ""{ \""Type\"": \"""" + list[i].Type + ""\"", \""Val\"": "" + list[i].Val + "", \""Money\"": "" + list[i].Money + "", \""ValText\"": \"""" + list[i].ValText + ""\"" }"";
                if (i < list.length - 1) {
                    str += "","";
                }
            }
            str += ""]"";
            return str;
";

		private const string ClassifySetAnswerJs = @"
            var arr = eval(s);
            if(arr){
                list = arr;
                showResult();
            }";
		private const string ClassifyAdditionalJs = @"
        function delHtmlTag(str) {
            var str = str.replace(/<\/?[^>]*>/gim, """"); //去掉所有的html标记
            var result = str.replace(/(^\s+)|(\s+$)/g, """"); //去掉前后空格
            return result.replace(/\s/g, """"); //去除文章中间空格
        }

        var list = [];
        function AddToList() {
            var type = window.document.getElementById('SType').options[window.document.getElementById('SType').selectedIndex].value;
            var val = window.document.getElementById('SValue').options[window.document.getElementById('SValue').selectedIndex].value;
            var valText = window.document.getElementById('SValue').options[window.document.getElementById('SValue').selectedIndex].text;
            valText = delHtmlTag(valText);
            var money = window.document.getElementById('SMoney').value;
            try {
                money = parseFloat(money);
            }
            catch (ex) {
                window.document.getElementById('SMoney').focus();
            }
            if (val && money > 0) {
                var isModify = false;
                for (var i = 0; i < list.length; i++) {
                    if (list[i].Type == type && list[i].Val == val) {
                        list[i].Money += money;
                        isModify = true;
                    }
                }
                if (!isModify) {
                    list.push({ Type: type, Val: val, Money: money, ValText: valText });
                }
                showResult();
                window.external.SelectStateChang(getAnswer());
            }
        }

        function showResult() {
            var html = ""<table class='classifytable'>"";
            var lastType = """";
            for (var i = 0; i < list.length; i++) {
                var item = list[i];
                html += ""<tr>"";
                html += ""<td class='check'><input type='checkbox' id='chk"" + i + ""'></td>""
                if (lastType != item.Type) {
                    html += ""<td class='type'>"" + (item.Type == ""借"" ? ""借"" : ""&#12288;&#12288;贷"") + ""："" + item.ValText + ""&#12288;"" + item.Money.toFixed(2) + ""</td>"";
                }
                else {
                    html += ""<td class='type'>"" + (item.Type == ""借"" ? ""&#12288;&#12288;"" : ""&#12288;&#12288;&#12288;&#12288;"") + item.ValText + ""&#12288;"" + item.Money.toFixed(2) + ""</td>"";
                }
                lastType = item.Type;
                html += ""</tr>"";
            }
            html += ""</table>""
            window.document.getElementById(""divResult"").innerHTML = html;
        }

        function ModifyItem() {
            var checknum = 0;
            var checkid = -1;
            for (var i = 0; i < list.length; i++) {
                if (document.getElementById(""chk"" + i).checked) {
                    checknum++;
                    checkid = i;
                }
            }
            if (checknum != 1) {
                alert(""请选中要修改的项"");
                return;
            }
            
            var type = window.document.getElementById('SType').options[window.document.getElementById('SType').selectedIndex].value;
            var val = window.document.getElementById('SValue').options[window.document.getElementById('SValue').selectedIndex].value;
            var valText = window.document.getElementById('SValue').options[window.document.getElementById('SValue').selectedIndex].text;
            valText = delHtmlTag(valText);
            var money = window.document.getElementById('SMoney').value;
            try {
                money = parseFloat(money);
            }
            catch (ex) {
                window.document.getElementById('SMoney').focus();
            }
            if (val && money > 0) {
                //alert(checkid);
                list[checkid] = { Type: type, Val: val, Money: money, ValText: valText };
                showResult();
                window.external.SelectStateChang(getAnswer());
            }
        }

        function deleteItem() {
            var newlist = [];
            for (var i = 0; i < list.length; i++) {
                if (!document.getElementById(""chk"" + i).checked) {
                    newlist.push(list[i]);
                }
            }
            list = newlist;
            showResult();
            window.external.SelectStateChang(getAnswer());
        }

        function checkMoney() {
            var money = window.document.getElementById('SMoney').value;
            try {
                money = parseFloat(money);
            }
            catch (ex) { }
            window.document.getElementById('SMoney').value = money;
        }
";

        private const string ClassifyHtml = @"
<div class=""classify"">借贷选择：
<select id=""SType"">
    <option value=""借"">借</option>
    <option value=""贷"">贷</option>
</select>
</div>
<div class=""classify"">科目选择:
<select id=""SValue"">
<option value="""">请选择...</option>
{0}
</select>
</div>
<div class=""classify"">
金额： 
<input id=""SMoney"" type=""text"" value=""0.00"" style=""width:100px;"">
</div>
<div class=""toolbar"">
    <a href=""javascript:AddToList();"" class=""btn"">增加</a> 
    <a href=""javascript:ModifyItem();"" class=""btn"">修改</a> 
    <a href=""javascript:deleteItem();"" class=""btn"">删除</a>
</div>
<div class=""resulttitle"">&nbsp;&nbsp;&nbsp;&nbsp;结果区</div>
<div id=""divResult""></div>
";

		#endregion
		private string _htmlContent;
		private string _userAnswer = string.Empty;

		private int _isRight;
		private string _statusImage = "/Images/Paper/exam_undo.png";
		private string _number;
		private string _typeTitle;
		private bool _isFav;
		private readonly int _paperId;
		private bool _isZhuguanHasAnswer;

		public QuestionItemViewModel(int paperId, ViewStudentQuestion question)
		{
			Init(question);
			_paperId = paperId;
		}

		#region 属性
		public ViewStudentQuestion Question { get; private set; }

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
				if (Question.QuesTypeId == 1 || Question.QuesTypeId == 2 || Question.QuesTypeId == 3) Log.RecordData("TikuSetAnswer", _paperId, Question.QuestionId, value.Replace("\r\n", "\\r\\n"));
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
				if (Question.QuesTypeId == 1 || Question.QuesTypeId == 2 || Question.QuesTypeId == 3)
				{
					Log.RecordData("TikuIsRight", _paperId, Question.QuestionId, value);
				}
			}
		}

		public bool IsZhuguanHasAnswer
		{
			get { return _isZhuguanHasAnswer; }
			set
			{
				if (_isZhuguanHasAnswer != value)
				{
					_isZhuguanHasAnswer = value;
					Log.RecordData("TikuIsRight", _paperId, Question.QuestionId, "2");
				}
			}
		}

		/// <summary>
		/// 是否收藏
		/// </summary>
		public bool IsFav
		{
			get { return _isFav; }
			set
			{
				if (Question != null && _isFav != value) Log.RecordData("TikuFav", _paperId, Question.QuestionId, value);
				_isFav = value;
				RaisePropertyChanged(() => IsFav);
			}
		}
		#endregion

		private void Init(ViewStudentQuestion question)
		{
			IsFav = question.IsFav;
			Question = question;
			ConvertToHtml(Question);
		}

#if CHINAACC
		public string GetSubjectClassify()
		{
			var clist = StudentQuestionLogic.GetSubjectClassify();
			var sb = new StringBuilder();
			clist.Where(x => x.ParentId == -1).ToList().ForEach(x =>
			{
				sb.AppendLine("<optgroup label=\"" + x.SubjectName + "\"></optgroup>");
				clist.Where(y => y.ParentId == x.SubjectClassifyId).ToList().ForEach(y =>
				{
					if (y.TreeType == 1)
					{
						sb.AppendLine("<optgroup label=\"&nbsp;&nbsp;" + y.SubjectName + "\"></optgroup>");
						clist.Where(z => z.ParentId == y.SubjectClassifyId).ToList().ForEach(z => sb.AppendLine("<option value=\"" + z.SubjectClassifyId + "\">&nbsp;&nbsp;&nbsp;&nbsp;" + z.SubjectName + "</option>"));
					}
					else
					{
						sb.AppendLine("<option value=\"" + y.SubjectClassifyId + "\">&nbsp;&nbsp;" + y.SubjectName + "</option>");
					}
				});
			});
			return sb.ToString();
		}
#endif

		/// <summary>
		/// 试题转换为HTML
		/// </summary>
		/// <param name="question"></param>
		private void ConvertToHtml(ViewStudentQuestion question)
		{
			var builder = new StringBuilder();

			var imgStyle = string.Empty;
            

			builder.Append("题型：");
			builder.Append(question.PartName);// 题型
			//builder.Append("[" + question.QuestionId + "]");
			if (question.Parent != null)
			{
				var hiddenStyle = question.SubId == 1 ? "" : "display:none;";
				var tipTxt = question.SubId == 1 ? "隐藏题干" : "显示题干";

				builder.AppendLine(string.Format(@"<div id=""parentContentDiv"" style=""{0}"">{1}. {2}</div>", hiddenStyle, question.MainId, question.Parent.Content));// 题干
				builder.AppendLine(string.Format(@"<div><a id=""btnTip"" href=""javascript:toggleShowQuestion();"" class=""btn"">{0}</a></div>", tipTxt));
                builder.AppendLine(string.Format(@"<div> &lt;{0}&gt;. {1}({2}分)</div>", question.SubId, question.Content,question.Score));// 子题目题干
			}
			else
			{
				builder.AppendLine(string.Format("<div>{0}. {1}({2}分)</div>", question.MainId, question.Content,question.Score));// 子题目题干
			}

			var additionalJs = string.Empty;
			var getAnswerJs = string.Empty;
			var setAnswerJs = string.Empty;
			var showAnswerJs = string.Empty;
			switch (question.QuesTypeId)
			{
				case 1:
				case 3:
					imgStyle = SingleImgStyle;
					additionalJs = SingleSetCheckJs;
					getAnswerJs = SingleGetAnswerJs;
					setAnswerJs = SingleSetAnswerJs;
					showAnswerJs = SingleShowAnswerJs;
					foreach (var option in question.OptionList)
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
					foreach (var option in question.OptionList)
					{
						var spanId = "span" + option.QuesValue;
						builder.AppendLine("<div style='clear: both; padding-top: 4px;'>");
						builder.AppendLine(string.Format(@"<input type=""checkbox"" name=""chk"" id=""{0}"" value=""{0}"" class=""inputchk"" /><label onclick=""setCheck('{0}')""><span id='{1}' class=""inputchkspan""></span>{0}. {2}</label>" + Environment.NewLine + "<br />", option.QuesValue, spanId, option.QuesOption));
						// 去掉label的for属性，会引起多选功能的失效
						//builder.AppendLine(string.Format(@"<input type=""checkbox"" name=""chk"" id=""{0}"" value=""{0}"" class=""inputchk"" /><label for=""{0}"" onclick=""setCheck('{0}')""><span id='{1}' class=""inputchkspan""></span>{0}. {2}</label>" + Environment.NewLine + "<br />", option.QuesValue, spanId, option.QuesOption));
						builder.AppendLine("</div>");
					}
					break;
				case 4:
					getAnswerJs = TextGetAnswerJs;
					setAnswerJs = TextSetAnswerJs;
					showAnswerJs = TextShowAnswerJs;

					builder.AppendLine(@"<textarea id=""txt_area"" onkeyup=""window.external.SelectStateChang(document.getElementById('txt_area').value)""></textarea>");
					break;
#if CHINAACC
				case 7:
					imgStyle = ClassifyStyle;
					getAnswerJs = ClassifyGetAnswerJs;
					setAnswerJs = ClassifySetAnswerJs;
					additionalJs = ClassifyAdditionalJs;

					builder.AppendLine(string.Format(ClassifyHtml, GetSubjectClassify()));

					break;

#endif
				case 9:
					imgStyle = "#txtBlank { border-width: 0 0 1px 0; border-color: #000000; } \r\n #divBlank { margin: 5px 0}";
					getAnswerJs = BlankGetAnswerJs;
					setAnswerJs = BlankSetAnswerJs;
					showAnswerJs = BlankShowAnswerJs;
					builder.AppendLine(@"<div id='divBlank'>请输入答案: <input type='text' id='txtBlank' onkeyup=""window.external.SelectStateChang(document.getElementById('txtBlank').value)"" /></div>");
					break;
				case 6:
					showAnswerJs = FlashShowAnswerJs;
					builder.AppendLine(@"<div style=""color:red"">系统暂未支持该题型，请到线上题库中作答</div>");
					break;
				default:
					showAnswerJs = DefaultShowAnswerJs;
					builder.AppendLine(@"<div style=""color:red"">系统暂未支持该题型，请到线上题库中作答</div>");
					break;
			}
			builder.AppendLine(@"<div id=""IsRight"" style=""display:none;color:#008000;"">回答正确</div>");
			builder.AppendLine(@"<div id=""IsError"" style=""display:none;color:#ff0087;"">回答错误</div>");
			builder.AppendLine(string.Format(@"<div id=""Answer"" style=""display:none"">
<div>
<span style=""color:#2D64B3"">正确答案：</span>
<span id=""RightAnswer"" style=""color:red"">{0}</span>
</div>
<div>
<span style=""color:green"">解析：</span>
<span id=""Analysis"">
</span>
</div>
</div>
<br />
<br />", ((question.QuesTypeId == 4 || question.QuesTypeId == 7) ? "<br />" + question.Answer : string.Empty)));

			var js = string.Format(AllScript, additionalJs, getAnswerJs, setAnswerJs, showAnswerJs);

            var style = string.Format(AllStyle, Util.QuestionBackColor, imgStyle, AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"));
			var html = string.Format(
				QuestionBodyHtml
				, style
				, js
                , builder, Util.QuestionBackColor,Util.QuestionFontSize);

			HtmlContent = html;
		}

		/// <summary>
		/// 检查用户回答是否正确(0:未做，1:正确，2:错误)
		/// </summary>
		private void CheckUserAnswer()
		{
			if (Question.QuesTypeId >= 4)
			{
				IsZhuguanHasAnswer = !string.IsNullOrEmpty(UserAnswer);
			}
			if (string.IsNullOrEmpty(UserAnswer) || !(Question.QuesTypeId == 1 || Question.QuesTypeId == 2 || Question.QuesTypeId == 3 || Question.QuesTypeId == 9))// 未做或者为阅读分析题
			{
				IsRight = 0;
				return;
			}
			IsRight = UserAnswer == Question.Answer ? 1 : 2;
		}
	}
}
