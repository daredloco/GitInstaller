using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitInstaller
{
	public class ZipSettings
	{
		public List<SubFolder> Subfolders = new List<SubFolder>();

		public ZipSettings() { }

		public string GetSubfolder(string fname, string sub = "")
		{
			SubFolder sf = Subfolders.Find(x => x.Files.Find(y => y == fname) != null);
			if (sf == null)
				return "";

			return sf.Name;
		}

		public static ZipSettings FromFile(string fname)
		{
			if (File.Exists(fname))
			{
				using (FileStream fs = new FileStream(fname, FileMode.Open))
					return FromStream(fs);
			}
			return null;
		}

		public static ZipSettings FromStream(Stream stream)
		{
			using (StreamReader reader = new StreamReader(stream))
			{
				try
				{
					string jsonstr = reader.ReadToEnd();
					ZipSettings zs = new ZipSettings();
					JObject obj = JsonConvert.DeserializeObject<JObject>(jsonstr);
					JArray foldersarray = obj.Value<JArray>("Subfolders");
					foreach (JObject subdir in foldersarray.Children<JObject>())
					{
						SubFolder sub = new SubFolder();
						foreach (var dict in subdir)
						{
							sub.Name = dict.Key;
							foreach (JToken token in dict.Value.Children<JToken>().ToList())
							{
								sub.Files.Add(token.Value<string>());
							}
						}
						zs.Subfolders.Add(sub);
					}
					return zs;
				}
				catch (Exception ex)
				{
					MainWindow.Instance.WriteLog("There was an error while reading the ZipSettings! => " + ex.Message);
					return null;
				}
			}
		}

		public class SubFolder
		{
			public string Name;
			public List<string> Files = new List<string>();
		}
	}
}
