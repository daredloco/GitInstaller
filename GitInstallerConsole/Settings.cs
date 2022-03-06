using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace GitInstaller
{
	internal static class Settings
	{
		internal static string Project { get; set; }
		internal static string User { get; set; }
		internal static string Repo { get; set; }
		internal static bool Unzip { get; set; }
		internal static bool Preview { get; set; }
		internal static bool Uninstall { get; set; }
		
		/// <summary>
		/// Returns the Url to the Releases of the GitHub Api or NULL if settings aren't loaded
		/// </summary>
		internal static Uri ApiUrl
		{
			get { return new Uri($"https://api.github.com/repos/{User}/{Repo}/releases"); }
		}

		internal static Uri LicenseUrl
		{
			get { return new Uri($"https://api.github.com/repos/{User}/{Repo}/license"); }
		}
	}
}
