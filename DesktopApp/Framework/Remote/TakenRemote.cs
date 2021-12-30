using Framework.NewModel;
using Framework.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace Framework.Remote
{
    public class TakenRemote : RemoteBase
    {
        /// <summary>
        /// 获取访问令牌
        /// </summary>
        public static void GetToken()
        {
            /**新接口的替换
             * @author ChW
             * @date 2021-06-15
             */
            var time = StudentRemote.GetCurrentTimeStamp13().ToString(); // Util.GetNowTimeStamp13().ToString();
            var etime = time;
            var key = Crypt.Md5(Interface.PlatformSource, "2.0.0.0", time, "Nyjh5AEeMw", Interface.AppKey);

            Dictionary<string, string> paramData = new Dictionary<string, string>
            {
                {"appkey",Interface.AppKey},
                {"pkey", key.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"etime", etime},
                {"version", "2.0.0.0"}
            };

            byte[] byte_valueData = MixParamData(paramData, Interface.GetToken_2021);

            try
            {
                WebClient wc = new WebClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                byte[] responseData = wc.UploadData(Interface.gateway, "POST", byte_valueData);
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                var obj = WebProxyClient.JsonDeserialize<TokenReturn>(responseData);
                if (obj != null && obj.Result != null && obj.Result.Code == "1")
                {
                    obj.Result.ParamValue = obj.Result.ParamValue.Replace(".", "+").Replace("-", "/").Replace("_", "=");
                    var buffer = Convert.FromBase64String(obj.Result.ParamValue);
                    var strbuf = Encoding.UTF8.GetString(buffer);
                    buffer = Crypt.DesDecrypt(buffer);
                    var strBuff = Encoding.UTF8.GetString(buffer);
                    Trace.WriteLine(strBuff);
                    var token = WebProxyClient.JsonDeserialize<TokenValue>(buffer);
                    Util.TokenLongTime = token.LongTime;
                    Util.TokenString = token.TokenString;
                    Util.Timeout = token.Timeout;
                }
                else
                {
                    Trace.WriteLine("获取口令失败");
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            /*
            var time = Util.GetNowString();
            var key = Crypt.Md5(Interface.PlatformSource, "2.0.0.0", time, "Nyjh5AEeMw", Interface.AppKey);
            var values = new NameValueCollection
            {
                {"appkey",Interface.AppKey},
                {"pkey", key.ToLower()},
                {"platformSource",Interface.PlatformSource},
                {"time", time},
                {"version", "2.0.0.0"}
            };
            var web = new WebProxyClient();
            try
            {
                var data = web.DownloadData(Interface.GetToken, values);
                var str = Encoding.UTF8.GetString(data);
                //Trace.WriteLine(str);
                var obj = WebProxyClient.JsonDeserialize<TokenResult>(data);
                if (obj.Code == "1")
                {
                    obj.ParamValue = obj.ParamValue.Replace(".", "+").Replace("-", "/").Replace("_", "=");
                    var buffer = Convert.FromBase64String(obj.ParamValue);
                    var strbuf = Encoding.UTF8.GetString(buffer);
                    buffer = Crypt.DesDecrypt(buffer);
                    var strBuff = Encoding.UTF8.GetString(buffer);
                    Trace.WriteLine(strBuff);
                    var token = WebProxyClient.JsonDeserialize<TokenValue>(buffer);
                    Util.TokenLongTime = token.LongTime;
                    Util.TokenString = token.TokenString;
                    Util.Timeout = token.Timeout;
                }
                else
                {
                    Trace.WriteLine("获取口令失败");
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            */
        }
    }
}
