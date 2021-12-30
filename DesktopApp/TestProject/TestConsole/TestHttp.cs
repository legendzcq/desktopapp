//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestConsole
//{
//	public class TestHttp
//	{
//		private static void Main(string[] args)
//		{
//			//var rsa = new RSACryptoServiceProvider(512);
//			//Console.WriteLine(rsa.ToXmlString(true));
//			//Console.WriteLine();
//			//Console.WriteLine();
//			//Console.WriteLine();
//			//Console.WriteLine(rsa.ToXmlString(false));
//			//Console.ReadLine();
//			var list = new List<BBB>();
//			var grouplist =
//				list.Select(x => x.Time)
//				.Distinct()
//				.Select(x => new AAA
//				{
//					Time = x,
//					ItemList = list.Where(y => y.Time == x)
//				});
//			//var list = Enumerable.Range(0, 10).ToArray();
//		}
//	}

//	public class AAA
//	{
//		public string Time { get; set; }

//		public IEnumerable<BBB> ItemList { get; set; }
//	}

//	public class BBB
//	{
//		public string Time { get; set; }
//		public int Id { get; set; }
//		public string Name { get; set; }
//	}
//}
