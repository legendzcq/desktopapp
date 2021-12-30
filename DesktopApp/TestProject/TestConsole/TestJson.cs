using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
	public class TestJson
	{
		private static void Main(string[] args)
		{
			//var rnd = new Random();
			//var obj = new JsonTest
			//{
			//	Id = rnd.Next(),
			//	Name = "我的value" + rnd.Next(),
			//	Value = rnd.NextDouble(),
			//	Time = DateTime.Now
			//};
			//TestRuntimeJson(obj);
			//TestJsonNet(obj);
			//Console.ReadLine();
			//var str = @"";

			//var obj = JsonDeserialize<JsonTest>(str, Encoding.UTF8);
			//Console.WriteLine(obj.Id + ":" + obj.Name);

			//var time = new DateTime(1970, 1, 1, 0, 0, 0);
			//time = time.AddMilliseconds(1234567890);
			//var aes = new System.Security.Cryptography.AesCryptoServiceProvider();

			//var key = aes.Key;
			//var iv = aes.IV;

			//Console.WriteLine(BitConverter.ToString(key).Replace("-", ",0x"));
			//Console.WriteLine();
			//Console.WriteLine(BitConverter.ToString(iv).Replace("-", ",0x"));
			//Console.ReadLine();

			var rnd = new Random();

			var conn = new SqlConnection("server=192.168.190.110; database=chinaacc_JXJYMemo; uid=sa; pwd=cailiqiang;");
			conn.Open();
			for (int i = 0; i < 100; i++)
			{
				var str = new string('1', rnd.Next(1, 25));
				var cmd = new SqlCommand("Select * From jxjy_PayLog Where PayOrderNo = @PayOrderNo") { Connection = conn };
				var param = new SqlParameter("@PayOrderNo", SqlDbType.VarChar) { Value = str };
				cmd.Parameters.Add(param);
				//cmd.Parameters.AddWithValue("@UserID", rnd.NextDouble());
				var r = cmd.ExecuteReader();
				r.Close();
			}
			conn.Close();
		}

		private static void TestRuntimeJson(JsonTest obj)
		{
			var sw = new Stopwatch();
			sw.Start();
			string str = null;
			for (int i = 0; i < 100000; i++)
			{
				str = JsonSerialize(obj, Encoding.UTF8);
			}
			sw.Stop();
			var t1 = sw.ElapsedTicks;
			sw.Reset();
			sw.Start();
			JsonTest obj2 = null;
			for (int i = 0; i < 100000; i++)
			{
				obj2 = JsonDeserialize<JsonTest>(str, Encoding.UTF8);
			}
			Console.WriteLine(str);
			Console.WriteLine("{0}:{1}:{2}:{3}", obj.Id, obj.Name, obj.Value, obj.Time);
			if (obj2 != null) Console.WriteLine("{0}:{1}:{2}:{3}", obj2.Id, obj2.Name, obj2.Value, obj2.Time);
			sw.Stop();
			var t2 = sw.ElapsedTicks;
			Console.WriteLine();
			Console.WriteLine("{0}:{1}", t1, t2);
			Console.WriteLine();
		}

		private static void TestJsonNet(JsonTest obj)
		{
			var sw = new Stopwatch();
			sw.Start();
			string str = null;
			for (int i = 0; i < 100000; i++)
			{
				str = JsonNetSe(obj);
			}
			sw.Stop();
			var t1 = sw.ElapsedTicks;
			sw.Reset();
			sw.Start();
			JsonTest obj2 = null;
			for (int i = 0; i < 100000; i++)
			{
				obj2 = JsonNetDese<JsonTest>(str);
			}
			Console.WriteLine(str);
			Console.WriteLine("{0}:{1}:{2}:{3}", obj.Id, obj.Name, obj.Value, obj.Time);
			if (obj2 != null) Console.WriteLine("{0}:{1}:{2}:{3}", obj2.Id, obj2.Name, obj2.Value, obj2.Time);
			sw.Stop();
			var t2 = sw.ElapsedTicks;
			Console.WriteLine();
			Console.WriteLine("{0}:{1}", t1, t2);
			Console.WriteLine();
		}


		public static string JsonNetSe<T>(T obj)
		{
			return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
		}

		public static T JsonNetDese<T>(string str)
		{
			return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
		}

		#region Runtime
		static readonly DataContractJsonSerializerSettings Setting = new DataContractJsonSerializerSettings { DateTimeFormat = new DateTimeFormat("yyyy-MM-dd HH:mm:ss") };

		private static string JsonSerialize<T>(T obj, Encoding encoding)
		{

			using (var ms = new MemoryStream())
			{
				var se = new DataContractJsonSerializer(typeof(T));
				se.WriteObject(ms, obj);
				return encoding.GetString(ms.ToArray());
			}
		}

		private static T JsonDeserialize<T>(string s, Encoding encoding)
		{
			using (var ms = new MemoryStream(encoding.GetBytes(s)))
			{
				return (T)new DataContractJsonSerializer(typeof(T)).ReadObject(ms);
			}
		}
		#endregion
	}

	[DataContract]
	public class JsonTest
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public double Value { get; set; }
		[DataMember]
		public DateTime Time { get; set; }
	}
}
