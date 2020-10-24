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
	[Serializable]
	public class ZipSettings
	{
		public string Subfolder;
		public JObject Subfolders;

		public ZipSettings() { }

		public static ZipSettings FromFile(string fname)
		{
			if (File.Exists(fname))
			{
				return JsonConvert.DeserializeObject<ZipSettings>(File.ReadAllText(fname));
			}
			return null;
		}

		public static ZipSettings FromStream(Stream stream)
		{
			using (StreamReader reader = new StreamReader(stream))
			{
				try
				{
					ZipSettings zs = JsonConvert.DeserializeObject<ZipSettings>(reader.ReadToEnd());
					return zs;
				}
				catch (Exception ex)
				{
					System.Windows.Forms.MessageBox.Show("There was an error while reading the ZipSettings! => " + ex.Message);
					return null;
				}
			}
		}
	}
}
