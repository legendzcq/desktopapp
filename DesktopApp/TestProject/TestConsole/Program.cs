//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Diagnostics;

//namespace TestConsole
//{
//	public class Program
//	{
//		static void Main(string[] args)
//		{
//			//var rnd = new Random();

//			//for (int i = 0; i < 10; i++)
//			//{
//			//	var idx = i;
//			//	Task.Factory.StartNew(() => Console.WriteLine(idx + ":" + rnd.Next().ToString(CultureInfo.InvariantCulture)));
//			//}
//			//Console.ReadLine();
//			long tick1 = 0, tick2 = 0, tick3 = 0, tick4 = 0;
//			for (int j = 0; j < 10; j++)
//			{
//				var sw = new Stopwatch();
//				sw.Start();
//				for (int i = 0; i < 100000; i++)
//				{
//					GetColor();
//				}
//				sw.Stop();
//				tick1 += sw.ElapsedTicks;
//				sw.Reset();
//				sw.Start();
//				for (int i = 0; i < 100000; i++)
//				{
//					GetColorNew();
//				}
//				sw.Stop();
//				tick2 += sw.ElapsedTicks;
//				sw.Reset();
//				sw.Start();
//				for (int i = 0; i < 100000; i++)
//				{
//					GetColorNew3();
//				}
//				sw.Stop();
//				tick3 += sw.ElapsedTicks;
//				sw.Reset();
//				sw.Start();
//				for (int i = 0; i < 100000; i++)
//				{
//					GetColorNew4();
//				}
//				sw.Stop();
//				tick4 += sw.ElapsedTicks;
//			}
//			Console.ForegroundColor = ConsoleColor.Yellow;
//			Console.WriteLine("{0}:{1}:{2}:{3}", tick1, tick2, tick3, tick4);
//			Console.ForegroundColor = ConsoleColor.White;
//			for (int i = 0; i < 10; i++)
//			{
//				var th = new Thread(() => Console.WriteLine(GetColorNew4()));
//				th.Start();
//			}
//			Console.ReadLine();
//		}


//		/// <summary>
//		/// 获取随机颜色值
//		/// </summary>
//		/// <returns></returns>
//		public static string GetColor()
//		{
//			var color = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f" };
//			var r = new Random(Guid.NewGuid().GetHashCode());
//			var sb = new StringBuilder();
//			sb.Append("#");
//			for (int i = 0; i < 6; i++)
//			{
//				sb.Append(color[r.Next(0, 15)]);
//			}
//			return sb.ToString();
//		}

//		public static string GetColor2()
//		{
//			var rnd = new Random(new object().GetHashCode());
//			var bys = new byte[3];
//			rnd.NextBytes(bys);
//			return "#" + string.Join(string.Empty, bys.Select(x => x.ToString("X2")));
//		}

//		private static readonly Random Rand = new Random();
//		public static string GetColorNew()
//		{
//			var bys = new byte[3];
//			Rand.NextBytes(bys);
//			return "#" + string.Join(string.Empty, bys.Select(x => x.ToString("X2")));
//		}

//		public static string GetColorNew3()
//		{
//			var cl = System.Drawing.Color.FromArgb(Rand.Next(int.MinValue, int.MaxValue));
//			return string.Format("#{0}{1}{2}", cl.R.ToString("X2"), cl.G.ToString("X2"), cl.B.ToString("X2"));
//		}

//		public static string GetColorNew4()
//		{
//			var bys = new byte[3];
//			Rand.NextBytes(bys);
//			return string.Format("#{0}{1}{2}", bys[0].ToString("X2"), bys[1].ToString("X2"), bys[2].ToString("X2"));
//		}

//		public static string GetColorNew2()
//		{
//			using(var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
//			{ 
//				var bys = new byte[3];
//				rng.GetBytes(bys);
//				return "#" + string.Join(string.Empty, bys.Select(x => x.ToString("X2")));
//			}
//		}
//	}
//}
