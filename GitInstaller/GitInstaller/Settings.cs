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

		/// <summary>
		/// The actual state of the settings.
		/// </summary>
		internal static SettingsState State { get; private set; } = SettingsState.NotLoaded;
		static readonly string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"config.json");
		/// <summary>
		/// a representation of the settingsfile
		/// </summary>
		internal static SettingsFile file;

		internal static string Project { get { return file.project; } }
		internal static string User { get { return file.user; } }
		internal static string Repo { get { return file.repo; } }
		internal static bool Unzip { get { return file.unzip; } }
		internal static bool Preview { get { return file.preview; } }
		internal static bool Uninstall { get { return file.uninstall; } }
		internal static string[] Ignored_Tags { get { return file.ignored_tags; } }
		internal static string[] Ignored_Files { get { return file.ignored_files; } }
		
		/// <summary>
		/// Returns the Url to the Releases of the GitHub Api or NULL if settings aren't loaded
		/// </summary>
		internal static Uri ApiUrl
		{
			get { if (State == SettingsState.Loaded) { return new Uri($"https://api.github.com/repos/{file.user}/{file.repo}/releases"); } else { return null; } }
		}

		/// <summary>
		/// Loads the settings from the config.json inside the applications directory
		/// </summary>
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
			public bool uninstall;
			[JsonProperty("ignored-tags")]
			public string[] ignored_tags;
			[JsonProperty("ignored-files")]
			public string[] ignored_files;

			public SettingsFile() { }
		}
	}
}
