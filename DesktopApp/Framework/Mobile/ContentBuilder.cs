using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Framework.Mobile
{
    public class ContentBuilder
    {
        static readonly Regex ReImg = new Regex(@"<img(.*?)src=""[^""]*\\(.*?)""(.*?)>", RegexOptions.IgnoreCase | RegexOptions.Singleline);

        public string GetKcjyContent(int cwareId, string videoId)
        {
            var kcjylist = new Local.StudentWareData().GetStudentWareKcjy(cwareId, videoId);
            var content =  string.Join("<br />", kcjylist.Select(x => string.Format(
                @"<div onMouseOver=""""><a name=""TimeNode"" id=""{0}"" title=""双击跳到本段播放({1})"" style=""tskclass;cursor:pointer;border:0px solid gray;"" onClick=""OpenStatusDiv(this);"">{2}</a></div>",
                x.NodeId, x.TimeStart, x.NodeText)));
            return ReImg.Replace(content,@"<img$1src=""img/$2""$3>");
        }

        public string GetTimeListString(int cwareId, string videoId)
        {
            var kcjylist = new Local.StudentWareData().GetStudentWareKcjy(cwareId, videoId);
            var sb = new StringBuilder();
            sb.AppendLine(@"<?xml version=""1.0"" encoding=""utf-8"" ?>");
            sb.AppendLine(@"<ret>");
            for (int i = 0; i < kcjylist.Count; i++)
            {
                sb.AppendLine("<timeNode>");
                sb.AppendLine("<timestart>" + kcjylist[i].TimeStart + "</timestart>");
                if (i < kcjylist.Count - 1)
                {
                    sb.AppendLine("<timeEnd>" + kcjylist[i + 1].TimeStart + "</timeEnd>");
                }
                else
                {
                    sb.AppendLine("<timeEnd>00:00:00</timeEnd>");
                }
                sb.AppendLine("<id>" + kcjylist[i].NodeId + "</id>");
                sb.AppendLine("<FlashUrl></FlashUrl>");
                sb.AppendLine("</timeNode>");
            }
            sb.AppendLine(@"</ret>");
            return sb.ToString();
        }
    }
}
