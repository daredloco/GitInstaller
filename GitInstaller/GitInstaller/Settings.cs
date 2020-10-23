using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace GitInstaller
{
	internal static class Settings
	{
		internal enum SettingsState
		{
			NotLoaded,
			Loaded,
			NotFound,
			Invalid
		}

		internal static SettingsState State { get; private set; } = SettingsState.NotLoaded;
		static readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"config.json");
		internal static SettingsFile file;

		/// <summary>
		/// Returns the Url to the Releases of the GitHub Api or NULL if settings aren't loaded
		/// </summary>
		internal static Uri ApiUrl
		{
			get { if (State == SettingsState.Loaded) { return new Uri($"https://api.github.com/repos/{file.user}/{file.repo}/releases"); } else { return null; } }
		}

		internal static void Load()
		{
			switch(State)
			{
				case SettingsState.Loaded:
					MessageBox.Show("Settings are already loaded!");
					break;
				case SettingsState.NotLoaded:
					if(!File.Exists(_path))
					{
						State = SettingsState.NotFound;
						Load();
						return;
					}
					file = JsonConvert.DeserializeObject<SettingsFile>(File.ReadAllText(_path));
					if(file == null)
					{
						State = SettingsState.Invalid;
						Load();
						return;
					}
					if(file.user == null || file.project == null || file.repo == null || file.ignored_tags == null)
					{
						State = SettingsState.Invalid;
						Load();
						return;
					}
					State = SettingsState.Loaded;
					break;
				case SettingsState.NotFound:
					MessageBox.Show("Couldn't find settings file!");
					break;
				case SettingsState.Invalid:
					MessageBox.Show("The settings file is invalid!");
					break;
			}
		}

		[Serializable]
		public class SettingsFile
		{
			public string project;
			public string user;
			public string repo;
			public bool unzip;
			public bool preview;
			[JsonProperty("ignored-tags")]
			public string[] ignored_tags;

			public SettingsFile() { }
		}
	}
}
