using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

using Framework.Utility;

namespace Framework.Remote
{
    public abstract class RemoteBase
    {
        private const string Xmlprefix = @"<?xml version=""1.0"" encoding=""utf-8"" ?>";

        protected string FixXmlHead(string xml) => Xmlprefix + xml;

        /**
         * 通过原始接口参数，组合出新接口的参数，并向新接口发送请求
         * @param oldParam 原始接口的参数
         * @param resourcePath 新接口的资源地址参数
         * @author ChW
         * @date 2021-06-07
         */
        protected static byte[] MixParamData(Dictionary<string, string> oldParam, string resourcePath, Dictionary<string, string> doormanParam = null)
        {
            var json_paramData = JsonSerializer.Serialize(oldParam, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            }); // 原有的参数集合（json）

            var string_aesKey = Crypt.AesKeyCreatePassword();
            var aesKey = Encoding.UTF8.GetBytes(string_aesKey);
            var ivKey = Encoding.UTF8.GetBytes(Interface.ivKey);
            var crypt_paramData = Crypt.AesEncrypt(json_paramData, aesKey, ivKey); // 对原有参数进行AES加密
            var final_paramData = Convert.ToBase64String(crypt_paramData);

            var publicKey = GetPublicKey(); // 获取公钥，十六进制的字符串
            if (string.IsNullOrEmpty(publicKey))
            {
                Trace.WriteLine("公钥获取失败");
                return new byte[0];
            }

            var crypt_aesKey = Crypt.RSAEncrypt(aesKey, publicKey); // 对aesKey参数进行RSA加密
            var final_aesKey = Convert.ToBase64String(crypt_aesKey);

            var valueData = new Dictionary<string, object> // 新接口的参数集合
            {
                {"params", final_paramData}, // aes算法加密后的原有参数集合
                {"domain", Interface.domain},
                {"resourcePath", resourcePath},
                {"platform", ""},
                {"appType", ""},
                {"appKey", ""},
                {"appVersion", ""},
                {"publicKey", publicKey}, // 从服务器获取的公钥
                {"aesKey", final_aesKey}, // RSA算法加密后的原始aesKey
                {"sid", ""}
            };

            // 如果有指定的doorman参数，使用doorman参数
            if (doormanParam != null)
            {
                foreach (KeyValuePair<string, string> item in doormanParam)
                {
                    if (valueData.ContainsKey(item.Key))
                    {
                        valueData[item.Key] = item.Value;
                    }
                    else
                    {
                        valueData.Add(item.Key, item.Value);
                    }
                }
            }

            var json_valueData = JsonSerializer.Serialize(valueData, new JsonSerializerOptions()
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
            var byte_result = Encoding.UTF8.GetBytes(json_valueData);

            return byte_result;
        }

        /**
         * 获取服务器加密公钥
         * @author ChW
         * @date 2021-06-07
         */
        public static string GetPublicKey()
        {
            if (!string.IsNullOrEmpty(PublicKey))
            {
                return PublicKey;
            }

            var postData = new Dictionary<string, object>
            {
                { "params", new Dictionary<string, string>{ {"time", Util.GetNowString() } } },
                { "domain", "cdel" },
                { "resourcePath", "+/key/public" },
                { "platform", "" },
                { "appType", "" },
                { "appVersion", "" }
            };
            var postData_json = JsonSerializer.Serialize(postData, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });

            try
            {
                var wc = new WebProxyClient();
                wc.Headers.Add("Content-Type", "application/json;charset=UTF-8");
                var byteData = System.Text.Encoding.UTF8.GetBytes(postData_json);
                var responseData = wc.UploadData(Interface.gateway, "POST", byteData); // 得到返回字符流
                Debug.WriteLine(Encoding.UTF8.GetString(responseData));
                ReturnObject obj = WebProxyClient.JsonDeserialize<ReturnObject>(responseData);
                if (obj != null && !string.IsNullOrEmpty(obj.Result))
                {
                    PublicKey = obj.Result;
                    return obj.Result;
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                return string.Empty;
            }
        }

        private static string PublicKey { get; set; }
    }
}
