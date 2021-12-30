//using System;
//using System.Collections.Specialized;
//using System.Xml;

//namespace Framework.Utility
//{
//    /// <summary>
//    /// 接口配置文件读取
//    /// </summary>
//    public class InterfaceConfig : NameObjectCollectionBase
//    {
//        private static InterfaceConfig _instance;
//        public static InterfaceConfig Instance
//        {
//            get { return _instance ?? (_instance = new InterfaceConfig()); }
//        }

//        private InterfaceConfig()
//        {
//            var appPath = AppDomain.CurrentDomain.BaseDirectory;
//            var configFile = appPath + "\\interface.config";
//            var configXml = new XmlDocument();
//            configXml.Load(configFile);
//            //todo 先做不加密的
//            var xmllist = configXml.SelectNodes("config/item");
//            if (xmllist == null) return;
//            foreach (XmlNode node in xmllist)
//            {
//                try
//                {
//                    if (node.Attributes != null)
//                    {
//                        var key = node.Attributes["key"].Value;
//                        var value = node.Attributes["value"].Value;
//                        BaseSet(key, value);
//                    }
//                }
//                catch (Exception ex)
//                {
//                    Log.RecordLog(ex.ToString());
//                }
//            }
//        }

//        public string this[string key]
//        {
//            get
//            {
//                return BaseGet(key).ToString();
//            }
//        }
//    }
//}
