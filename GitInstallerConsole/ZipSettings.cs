﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GitInstaller
{
	public class ZipSettings
	{
		public List<SubFolder> Subfolders = new List<SubFolder>();

		public ZipSettings() { }

		/// <summary>
		/// Gets the subfolder of a file
		/// </summary>
		/// <param name="fname">The file to fetch the subfolder for</param>
		/// <returns>The subfolder for the file</returns>
		public string GetSubfolder(string fname)
		{
			SubFolder sf = null;
			string ffolder = fname.Replace(fname.Split('/')[fname.Split('/').Length - 1], "").Replace(fname.Split('\\')[fname.Split('\\').Length - 1],"");
			sf = Subfolders.Find(x => x.Directories.Find(y => y == ffolder) != null);
			sf = Subfolders.Find(x => x.Files.Find(y => y == fname) != null);
			if (sf == null)
				return "";

			return sf.Name;
		}

		/// <summary>
		/// Returns the ZipSettings from a File
		/// </summary>
		/// <param name="fname">The file containing the ZipSettings</param>
		/// <returns>A ZipSettings object</returns>
		public static ZipSettings FromFile(string fname)
		{
			if (File.Exists(fname))
			{
				using (FileStream fs = new FileStream(fname, FileMode.Open))
					return FromStream(fs);
			}
			return null;
		}

		/// <summary>
		/// Returns the ZipSettings from a Stream
		/// </summary>
		/// <param name="stream">The stream with the ZipSettings</param>
		/// <returns>A ZipSettings object</returns>
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
								Utils.ZipType ztype = Utils.FileOrDir(token.Value<string>());
								if (ztype == Utils.ZipType.File)
									sub.Files.Add(token.Value<string>());
								else if (ztype == Utils.ZipType.Directory)
									sub.Directories.Add(token.Value<string>());	
							}
						}
						zs.Subfolders.Add(sub);
					}
					return zs;
				}
				catch (Exception ex)
				{
					Console.WriteLine("There was an error while reading the ZipSettings! => " + ex.Message);
					return null;
				}
			}
		}

		public class SubFolder
		{
			public string Name;
			public List<string> Files = new List<string>();
			public List<string> Directories = new List<string>();
		}
	}
}
