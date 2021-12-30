using DesktopApp.Logic;
using Framework.Model;
using Framework.Utility;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DesktopApp.ViewModel
{
    public class PaperSocreQuesViewModel : ViewModelBase
    {
        #region HTML定义
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
        /// 复选框样式
        /// </summary>
        private static readonly string MultyCheckStyle = string.Format(@"
            .inputchk {{ display:none; }}
            .inputchkspan {{display:block;float: left; width: 40px; height: 40px; vertical-align:middle; background:url('{0}') no-repeat; cursor:pointer; }}
            .checked {{ background:url('{1}') no-repeat; }}", ImgUncheckedPath, ImgCheckedPath);

        private const string FlashShowAnswerJs = @"document.getElementById('Analysis').innerHTML = ""<input type=\""button\"" value=\""在线查看解析\"" onclick = \""window.open('"" + analysis + ""')\"" />"";";

        private const string DefaultShowAnswerJs = @"
            document.getElementById('RightAnswer').innerHTML = answer;
            document.getElementById('Analysis').innerHTML = analysis;";

        /// <summary>
        /// 所有的样式
        /// </summary>
        private const string AllStyle = @"
        <style>                            
            textarea {{ -ms-text-size-adjust:auto; width:100%; border:1px solid #272822; height:150px; }}
            body {{ -ms-text-size-adjust:auto; margin:10px; padding:0px; background-color:{0}; }} 
            div#IsRight, div#IsError, div#Answer,div#UserAnwser{{
                border-radius: 10px;
                background-color: #EEE;
                border: 1px solid #DDD;
                margin-top: 10px;
            }}
            div {{ display:block; float:left ; width:100% }}
            div#IsRight p,div#IsError p,div#UserAnwser p,div#Answer p{{
                margin: 10px;
            }}
            {1}
            .btn{{ font-size:14px; text-align:center; line-height:25px; text-decoration:none; display:block;color:#fff;width:75px;height:25px; background-image:url('file://{2}images/btn.png');}}
            .btn:hover{{ background-image:url('file://{2}images/btnhover.png');}}
        </style>";

        private const string AllScript = @"
        <script type=""text/javascript"" charset=""utf-8"">
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
    <a class=""btn"">增加</a> 
    <a class=""btn"">修改</a> 
    <a class=""btn"">删除</a>
</div>
";

        #endregion
        private string _htmlContent;

        private string _statusImage = "/Images/Paper/exam_undo.png";
        private string _number;
        private string _typeTitle;

        public PaperSocreQuesViewModel(int paperId, ViewStudentQuestion question)
        {
            Init(question);
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
        #endregion

        private void Init(ViewStudentQuestion question)
        {
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

        public string SetClassifyResult(string userAnswer)
        {
            var list = new Framework.Remote.StudentQuestionRemote().GetClassifyAnswer(userAnswer);
            var sb = new StringBuilder();
            sb.AppendLine("<table class='classifytable'>");
            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td class='type'>{0}:{1}{2}</td>", item.Type == "借" ? "借" : "贷", item.ValText, item.Money));
                sb.AppendLine("</tr>");

            }
            sb.AppendLine("</table>");
            return sb.ToString();

        }
#endif
        private void ConvertToHtml(ViewStudentQuestion question)
        {
            var builder = new StringBuilder();

            var imgStyle = string.Empty;


            builder.Append("题型：");
            builder.Append(question.PartName);// 题型
            if (question.Parent != null)
            {
                var hiddenStyle = question.SubId == 1 ? "" : "display:none;";
                var tipTxt = question.SubId == 1 ? "隐藏题干" : "显示题干";

                builder.AppendLine(string.Format(@"<div id=""parentContentDiv"" style=""{0}"">{1}. {2}</div>", hiddenStyle, question.MainId, question.Parent.Content));// 题干
                builder.AppendLine(string.Format(@"<div><a id=""btnTip"" href=""javascript:toggleShowQuestion();"" class=""btn"">{0}</a></div>", tipTxt));
                builder.AppendLine(string.Format(@"<div> &lt;{0}&gt;. {1}({2}分)</div>", question.SubId, question.Content, question.Score));// 子题目题干
            }
            else
            {
                builder.AppendLine(string.Format("<div>{0}. {1}({2}分)</div>", question.MainId, question.Content, question.Score));// 子题目题干
            }

           // var additionalJs = string.Empty;
            //var getAnswerJs = string.Empty;
            //var setAnswerJs = string.Empty;
            var showAnswerJs = string.Empty;
            switch (question.QuesTypeId)
            {
                case 1:
                case 3:
                    imgStyle = SingleImgStyle;
                    foreach (var option in question.OptionList)
                    {
                        var spanId = "span" + option.QuesValue;
                        builder.AppendLine("<div style='clear: both; padding-top: 4px; margin-top:10px;'>");
                        //builder.AppendLine(string.Format(@"<input type=""radio"" name=""radio"" id=""{0}"" value=""{0}"" class=""inputradio"" disabled=""disabled"" /><label for=""{0}""><span id='{1}' class=""inputRspan""></span>{0}. {2}</label><br />", option.QuesValue, spanId, option.QuesOption));
                        builder.AppendLine(string.Format(@"{0}. {1}<br />", option.QuesValue, option.QuesOption));
                        builder.AppendLine("</div>");
                    }
                    break;
                case 2:
                    imgStyle = MultyCheckStyle;
                    foreach (var option in question.OptionList)
                    {
                        var spanId = "span" + option.QuesValue;
                        builder.AppendLine("<div style='clear: both; padding-top: 4px; margin-top:10px;'>");
                        //builder.AppendLine(string.Format(@"<input type=""checkbox"" name=""chk"" id=""{0}"" value=""{0}"" class=""inputchk"" disabled=""disabled"" /><label for=""{0}"" ><span id='{1}' class=""inputchkspan""></span>{0}. {2}</label>" + Environment.NewLine + "<br />", option.QuesValue, spanId, option.QuesOption));
                        builder.AppendLine(string.Format(@"{0}. {1}" + Environment.NewLine + "<br />", option.QuesValue,option.QuesOption));
                        builder.AppendLine("</div>");
                    }
                    break;
                case 4:
                    builder.AppendLine(@"<textarea id=""txt_area"" disabled=""disabled""></textarea>");
                    break;
#if CHINAACC
                case 7:
                    imgStyle = ClassifyStyle;
                    builder.AppendLine(string.Format(ClassifyHtml, GetSubjectClassify()));
                    break;
#endif
                case 9:
                    imgStyle = "#txtBlank { border-width: 0 0 1px 0; border-color: #000000; } \r\n #divBlank { margin: 5px 0}";
                    builder.AppendLine(@"<div id='divBlank'>请输入答案: <input type='text' id='txtBlank' disabled=""disabled"" /></div>");
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
            if (string.IsNullOrWhiteSpace(question.UserAnswer))
            {
                builder.AppendLine(@"<div id=""UserAnwser"" style=""color:#008000;"">您的答案：您未答题</div>");
            }
#if CHINAACC
            else if (question.QuesTypeId == 7)
            {
                builder.AppendLine(string.Format(@"<div id=""UserAnwser"" style=""color:#008000;"">您的答案：<br/>{0}</div>", SetClassifyResult(question.UserAnswer)));
            }
#endif
            else
            {
                builder.AppendLine(string.Format(@"<div id=""UserAnwser"" style=""color:#008000;"">您的答案：{0}</div>", question.UserAnswer));
            }

            string analysis = question.Analysis.Replace("</p>", "<br />");
            builder.AppendLine(string.Format(@"<div id=""Answer"">
<div>
<span style=""color:#2D64B3"">正确答案：</span>
<span id=""RightAnswer"" style=""color:red"">{0}</span>
</div>
<div>
<span style=""color:green"">解析：</span>
<span id=""Analysis"">{1}
</span>
</div>
</div>
<br />
<br />", ((question.QuesTypeId == 4 || question.QuesTypeId == 7) ? "<br />" + Question.Answer : Question.Answer), analysis));
            //var js = string.Format(AllScript, additionalJs, getAnswerJs, setAnswerJs, showAnswerJs);
            var js = string.Format(AllScript,showAnswerJs);
            var style = string.Format(AllStyle, Util.QuestionBackColor, imgStyle, AppDomain.CurrentDomain.BaseDirectory.Replace("\\", "/"));
            var html = string.Format(
                QuestionBodyHtml
                , style
                , js
                , builder, Util.QuestionBackColor, Util.QuestionFontSize);
            HtmlContent = html;
        }
    }

}
