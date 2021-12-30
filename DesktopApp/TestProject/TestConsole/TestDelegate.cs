//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestConsole
//{
//	public class TestDelegate
//	{
//		private static void Main(string[] args)
//		{
//			var my = new MyClass();
//			my.ShowMessage += ( msg) =>
//			{
//				{
//					msg = DateTime.Now + ":" + msg;
//					Console.WriteLine(msg);
//				}
//			};
//			my.ShowMessage += (msg) =>
//			{
//				{
//					msg = DateTime.Now.Ticks + ":" + msg;
//					Console.WriteLine(msg);
//				}
//			};
//			my.GetMyNum = GetMyNum;
//			my.Execute();
//			Console.ReadLine();
//		}

//		public static int GetMyNum(string msg)
//		{
//			return msg.Length;
//		}
//	}

//	public delegate void ShowMessageDelagate(string message);

//	public delegate int GetNum(string message);

//	public class MyClass
//	{
//		public event ShowMessageDelagate ShowMessage;

//		public GetNum GetMyNum;

//		public void Execute()
//		{
//			int value = 0;
//			if (GetMyNum != null)
//			{
//				GetMyNum("asdfa");
//			}
//			if (ShowMessage != null)
//			{
//				ShowMessage("abc" + value);
//			}
//		}
//	}
//}
