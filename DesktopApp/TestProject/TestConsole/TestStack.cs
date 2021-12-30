//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TestConsole
//{
//	public class TestStack
//	{
//		private static void Main(string[] args)
//		{
//			int num = 0;
//			double sum = 0.0;
//			CallStackWithThree(string.Empty, ref num, ref sum);
//			Console.ReadLine();
//		}

//		private static int cnt = 0;

//		private static void CallStack()
//		{
//			cnt++;
//			Console.WriteLine(cnt);
//			CallStack();
//		}

//		private static void CallStackWithNum(ref int cnt)
//		{
//			cnt++;
//			Console.WriteLine(cnt);
//			CallStackWithNum(ref cnt);
//		}

//		private static void CallStackWithTwo(string value, ref int cnt)
//		{
//			cnt++;
//			Console.WriteLine(cnt);
//			CallStackWithTwo(value, ref cnt);
//		}

//		private static void CallStackWithThree(string value, ref int cnt, ref double sum)
//		{
//			cnt++;
//			sum += cnt;
//			Console.WriteLine("{0},{1},{2}", value, cnt, sum);
//			CallStackWithThree(value, ref cnt, ref sum);
//		}
//	}
//}
