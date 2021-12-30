using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CdelService.Utility
{
   public static class Common
    {
        internal const string PlatformSource = "10";
        /// <summary>
        /// 上传用户使用信息接口
        /// </summary>
       internal const string UploadBigDataLogUrl = "http://data.cdeledu.com/SaveFileData";
      /// <summary>
      /// 相关文件存放路径
      /// </summary>
       public static string _path;
       /// <summary>
       /// 产品名称
       /// </summary>
       public static string ProductName = "";
       /// <summary>
       /// 冻结用户连接
       /// </summary>
       public static string CheckUserFrozenUrl = "";
       /// <summary>
       /// 数据采集站点定义
       /// </summary>
       public static int SiteId = 3;
       public static string MemberSalt = "";
       /// <summary>
       /// 初始化数据
       /// </summary>
       /// <param name="type">网址类别</param>
       /// <param name="path">文件路径</param>
       public static void IniData(YXType type,string path)
       {
           _path = path;
           switch (type)
           {
               case YXType.CHINAACC:
                   ProductName = "ChinaaccDownClass";
                   CheckUserFrozenUrl = "http://member.chinaacc.com/mobile/classroom/member/getUsserWaring.shtm";
                   SiteId = 3;
                   MemberSalt = "fJ3UjIFyTu";
                   break;
               case YXType.CHENGKAO:
                   ProductName = "ChengkaoDownClass";
                   CheckUserFrozenUrl = "http://member.chengkao365.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 10;
                   MemberSalt = "fJ3UjIFyTu";
                   break;
               case YXType.CHINATAT:
                   ProductName = "ChinatatDownClass";
                   CheckUserFrozenUrl = "http://member.chinatat.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 5;
                   MemberSalt = "It1UjIJyYu";
                   break;
               case YXType.FOR68:
                   ProductName = "For68DownClass";
                   CheckUserFrozenUrl = "http://member.for68.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 9;
                   MemberSalt = "LyBsw3Ai1b";
                   break;
               case YXType.G12E:
                   ProductName = "G12eDownClass";
                   CheckUserFrozenUrl = "http://member.g12e.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 6;
                   MemberSalt = "L3iyA1nHui";
                   break;
               case YXType.JIANSHE:
                   ProductName = "Jianshe99DownClass";
                   CheckUserFrozenUrl = "http://member.jianshe99.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 1;
                   MemberSalt = "fJ3UjIFyTu";
                   break;
               case YXType.KAOYAN:
                   ProductName = "KaoyanDownClass";
                   CheckUserFrozenUrl = "http://member.cnedu.cn/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 8;
                   MemberSalt = "hgDfgYghKj";
                   break;
               case YXType.LAW:
                   ProductName = "LawDownClass";
                   CheckUserFrozenUrl = "http://member.chinalawedu.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 4;
                   MemberSalt = "Yu3hUifOvJ";
                   break;
               case YXType.MED:
                   ProductName = "Med66DownClass";
                   CheckUserFrozenUrl = "http://member.med66.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 2;
                   MemberSalt = "tFdfJdfRys";
                   break;
               case YXType.ZIKAO:
                   ProductName = "ZikaoDownClass";
                   CheckUserFrozenUrl = "http://member.zikao365.com/newApi/classroom/member/getUsserWaring.shtm";
                   SiteId = 7;
                   MemberSalt = "wY2Y1FMs9n";
                   break;
           }
       }
    }
}
