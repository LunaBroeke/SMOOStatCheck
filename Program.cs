using Newtonsoft.Json;
using System.Globalization;
using System.Net.Sockets;

namespace SMOOStatCheck
{
	public class Costume
	{
		public string Cap { get; set; }
		public string Body { get; set; }
	}

	public class PersistShines
	{
		public bool Enabled { get; set; }
	}

	public class Player
	{
		public string ID { get; set; }
		public string Name { get; set; }
		public string Kingdom { get; set; }
		public int? GameMode { get; set; }
		public string Stage { get; set; }
		public int? Scenario { get; set; }
		public Position Position { get; set; }
		public Rotation Rotation { get; set; }
		public bool? Tagged { get; set; }
		public Costume Costume { get; set; }
		public string Capture { get; set; }
		public bool? Is2D { get; set; }
		public string IPv4 { get; set; }
	}

	public class Position
	{
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
	}

	public class Root
	{
		public Settings Settings { get; set; }
		public List<Player> Players { get; set; }
	}

	public class Rotation
	{
		public double W { get; set; }
		public double X { get; set; }
		public double Y { get; set; }
		public double Z { get; set; }
	}

	public class Scenario
	{
		public bool MergeEnabled { get; set; }
	}

	public class Server
	{
		public int MaxPlayers { get; set; }
	}

	public class Settings
	{
		public PersistShines PersistShines { get; set; }
		public Scenario Scenario { get; set; }
		public Server Server { get; set; }
		public Shines Shines { get; set; }
	}

	public class Shines
	{
		public bool Enabled { get; set; }
	}
	internal class Program
	{
		static void Main(string[] args)
		{
			Log(RequestRawJson("luwuna.smoo.it", 1027));
			Thread server1 = new Thread(() => { Log(RequestRawJson("luwuna.smoo.it", 1027)); }) { IsBackground = true };
			Thread server2 = new Thread(() => { Log(RequestRawJson("luwuna.smoo.it", 1028)); }) { IsBackground = true };
			Thread server3 = new Thread(() => { Log(RequestRawJson("luwuna.smoo.it", 1029)); }) { IsBackground = true };
			server1.Start();
			server2.Start();
			server3.Start();
			Console.ReadKey();
		}

		public static void Log(string message)
		{
			Console.WriteLine(message);
		}

		public static string RequestRawJson(string address, int port) 
		{
			RequestRoot root = new RequestRoot()
			{
				API_JSON_REQUEST = new APIRequest()
				{
					Token = AuthTokens.SMOOtoken,
				}
			};
			string json = JsonConvert.SerializeObject(root);
			Log(json);
			string response = string.Empty;
			try
			{
				using (var client = new TcpClient())
				{
					client.ReceiveTimeout = 500;
					client.SendTimeout = 500;
					var result = client.BeginConnect(address, port, null, null);
					bool success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
					if (!success)
					{
						throw new SocketException();
					}
					using (var stream = client.GetStream())
					using (var writer = new StreamWriter(stream))
					using (var reader = new StreamReader(stream))
					{
						writer.Write(json);
						writer.Flush();
						response = reader.ReadToEnd();
						return response;
					}
				}
			}
			catch
			{
				Log($"failed calling {address}:{port}");
				return string.Empty;
			}
		}
		private class RequestRoot
		{
			public APIRequest API_JSON_REQUEST = new();
		}
		private class APIRequest
		{
			public string Token = "";
			public string Type = "Status";
			public string Data = "";
		}
	}
}