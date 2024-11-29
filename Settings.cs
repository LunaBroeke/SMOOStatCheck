using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMOOStatCheck.Settings
{
	public class Root
	{
		public int IntervalSeconds { get; set; } = 300;
		public List<Server> Servers { get; set; } = new List<Server>();
	}
	public class Server
	{
		public string Name { get; set; } = "server";
		public string Adress { get; set; } = "127.0.0.1";
		public int Port { get; set; } = 1027;
		public string Token { get; set; } = string.Empty;

		public override string ToString()
		{
			return $"{Name} - {Adress}:{Port} (Token: {Token})";
		}
	}
	public class Settings
	{
		private const string path = "settings.json";
		public Root root = new Root();

		public static Settings LoadSettings()
		{
			if (File.Exists(path))
			{
				Root _root = JsonConvert.DeserializeObject<Root>(File.ReadAllText(path));
				Program.Log($"Loaded {path}");
				File.WriteAllText(path, JsonConvert.SerializeObject(_root,Formatting.Indented));
				return new Settings { root = _root };
			}
			else
			{
				Root _root = new Root();
				_root.Servers.Add(new Server());
				string json = JsonConvert.SerializeObject(_root, Formatting.Indented);
				File.WriteAllText(path, json);
				Program.Log($"Created {path} at {Path.GetFullPath(path)} \n Please remember to edit this file with your servers and reopen the program.");
				return new Settings { root = _root };
			}
		}
	}
}
