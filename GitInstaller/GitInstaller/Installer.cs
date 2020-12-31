using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO.Compression;
using System.ComponentModel;

namespace GitInstaller
{
	internal class Installer
	{
		readonly MainWindow _window;
		readonly Uri _url;
		string _releasesjson;
		string _installdir;
		GitRelease _installrelease;
		/// <summary>
		/// List of GitRelease objects fetched from Github
		/// </summary>
		internal List<GitRelease> Releases = new List<GitRelease>();
		
		/// <summary>
		/// Installer backend.
		/// Enter the URL and the backend will start fetching the versions
		/// </summary>
		/// <param name="url">The url to the Github repo, will be created from the config.json content</param>
		internal Installer(Uri url)
		{
			_url = url;
			_window = MainWindow.Instance;
			GetVersions();
		}

		/// <summary>
		/// Fetches the different versions/releases from the Githup repo
		/// </summary>
		async void GetVersions()
		{
			_window.WriteLog("Trying to fetch releases from GitHub...");
			_window.prog_loading.IsIndeterminate = true;

			using (var client = new WebClient())
			{
				client.Headers.Add("user-agent", "GitInstaller");

				//Save as _releasesjson so you don't have to fetch it again
				_releasesjson = await client.DownloadStringTaskAsync(_url.AbsoluteUri);
				_window.WriteLog("Fetched all releases from GitHub...");

				//Create GitRelease objects from the _releasesjson string
				_window.WriteLog("Creating Release objects...");
				JObject[] jobjs = JsonConvert.DeserializeObject<JObject[]>(_releasesjson);
				int idcount = 0;
				Releases.Clear();
				_window.cb_versions.Items.Clear();
				
				foreach (JObject job in jobjs)
				{
					GitRelease robj = new GitRelease();
					robj.Id = idcount;
					robj.Name = job.Value<string>("name");
					robj.Tag = job.Value<string>("tag_name");
					robj.Body = job.Value<string>("body");
					robj.GitUrl = job.Value<string>("html_url");
					robj.IsPrerelease = job.Value<bool>("prerelease");
					robj.CreationDate = job.Value<string>("created_at");

					//Author
					JObject authorobj = job.Value<JObject>("author");
					robj.AuthorName = authorobj.Value<string>("login");
					robj.AuthorUrl = authorobj.Value<string>("html_url");

					//Assets
					JToken assets = job.Value<JToken>("assets");
					
					foreach (JToken asset in assets.Children())
					{
						GitAsset newasset = new GitAsset();
						newasset.Filename = asset.Value<string>("name");
						newasset.DownloadUrl = asset.Value<string>("browser_download_url");
						newasset.Size = asset.Value<float>("size");
						robj.Assets.Add(newasset);
					}


					foreach(string ignoredtags in Settings.Ignored_Tags)
					{
						if(!Utils.HasWildcard(robj.Tag,ignoredtags))
						{
							idcount++;
							Releases.Add(robj);
						}
					}
				}

				_window.cb_versions.SelectedIndex = 0;
			}
			_window.prog_loading.IsIndeterminate = false;
			if(Releases.Count < 1)
			{
				MessageBox.Show("No releases found for this repository! Can't progress with the installation...", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
				Environment.Exit(2);
				return;
			}
			_window.bt_install.IsEnabled = true;
			_window.WriteLog("Done creating release objects, " + Releases.Count + " releases added...");

			_window.UpdateVersions(Settings.Preview);
			_window.UpdateChanges(Releases[0]);
		}

		/// <summary>
		/// Starts the installation of the release
		/// </summary>
		/// <param name="releaseindex">The index of the release as in the Releases List</param>
		/// <param name="installdir">The installation directory</param>
		internal void StartInstallation(int releaseindex, string installdir)
		{
			if(Settings.ShowLicense)
			{
				LicenseWindow licenseWindow = new LicenseWindow();
				if (licenseWindow.ShowDialog() == true)
				{
					_window.bt_install.IsEnabled = false;
					_installrelease = Releases[releaseindex];
					_window.WriteLog("Starting installation of release \"" + _installrelease.Tag + "\" to \"" + installdir + "\"...");
					_installdir = installdir;
					DownloadAssets();
				}
				else
				{
					_window.WriteLog("You need to accept the license to proceed with the installation.");
				}
			}
			else
			{
				_window.bt_install.IsEnabled = false;
				_installrelease = Releases[releaseindex];
				_window.WriteLog("Starting installation of release \"" + _installrelease.Tag + "\" to \"" + installdir + "\"...");
				_installdir = installdir;
				DownloadAssets();
			}
		}

		/// <summary>
		/// Downloads the Assets from the actual Release
		/// </summary>
		async void DownloadAssets()
		{
			
			if(Settings.State != Settings.SettingsState.Loaded)
			{
				throw new Exception("Settings aren't loaded, can't progress!");
			}
			_window.prog_loading.IsIndeterminate = true;
			int assetcount = 0;
			int maxcount = _installrelease.Assets.Count;
			bool haderror = false;
			List<string> downloadedfiles = new List<string>();

			foreach(GitAsset asset in _installrelease.Assets)
			{
				_window.WriteLog("Downloading asset \"" + asset.Filename + " (" + asset.Size + " bytes)\"");
				using (var client = new WebClient())
				{
					client.Headers.Add("user-agent", "GitInstaller");
					client.DownloadProgressChanged += FileDownloadChanged;
					client.DownloadFileCompleted += FileDownloadCompleted;

					try
					{

						foreach(string ignoredfile in Settings.Ignored_Files)
						{
							if(!Utils.HasWildcard(asset.Filename, ignoredfile))
							{
								string installfname = Path.Combine(_installdir, asset.Filename);
								await client.DownloadFileTaskAsync(new Uri(asset.DownloadUrl), installfname);
								downloadedfiles.Add(installfname);
								assetcount++;
								_window.WriteLog($"Asset downloaded... ({assetcount}/{maxcount})");
							}
							else
							{
								_window.WriteLog($"Asset \"{asset.Filename}\" will be ignored...");
								assetcount++;
							}
						}
					}
					catch (Exception ex)
					{
						_window.WriteLog("Error while downloading asset \"" + asset.Filename + "\": " + ex.Message);
						haderror = true;
					}
				}
			}

			_window.prog_loading.IsIndeterminate = false;
			if (haderror)
			{
				//TODO: Do something if the asset download wasn't successful
				return;
			}

			if(Settings.Unzip)
				UnzipInstalledData(downloadedfiles.ToArray());
		}

		private void FileDownloadCompleted(object sender, AsyncCompletedEventArgs e)
		{
			_window.prog_loading.IsIndeterminate = true;			
		}

		private void FileDownloadChanged(object sender, DownloadProgressChangedEventArgs e)
		{
			_window.prog_loading.IsIndeterminate = false;
			_window.prog_loading.Maximum = 100;
			_window.prog_loading.Value = e.ProgressPercentage;
		}

		/// <summary>
		/// Unpacks the files from downloadedfiles
		/// </summary>
		/// <param name="downloadedfiles">The locations of the downloaded files</param>
		async void UnzipInstalledData(string[] downloadedfiles)
		{
			bool wasSuccess = true;
			if(IsUpdate(new FileInfo(downloadedfiles[0]).Directory.FullName))
			{
				_window.WriteLog("Deleting old files from project");
				Uninstaller.DoUninstall(new FileInfo(downloadedfiles[0]).Directory.FullName);
			}

			_window.WriteLog("Unpack zip archives...");
			_window.prog_loading.IsIndeterminate = true;
			await Task.Run(() => {
				foreach (string fname in downloadedfiles)
				{
					FileInfo fi = new FileInfo(fname);
					if (fi.Extension == ".zip")
					{
						using(FileStream fs = new FileStream(fname, FileMode.Open))
						{
							using(ZipArchive archive = new ZipArchive(fs))
							{
								wasSuccess = ZipArchiveExtensions.ExtractWithSettings(archive, fi.DirectoryName, true);
							}
						}
						File.Delete(fname);
					}
				}
			});
			if(!wasSuccess)
			{
				Uninstaller.DoUninstall(new FileInfo(downloadedfiles[0]).DirectoryName);
				_window.WriteLog("There was an error while extracting the archive!");
			}
			else
			{
				_window.WriteLog("Installation complete!");
			}
			_window.prog_loading.IsIndeterminate = false;
			_window.bt_install.IsEnabled = true;
		}

		/// <summary>
		/// Checks if there already exists an installation inside the directory or if its a fresh installation
		/// </summary>
		/// <param name="directory">The installation directory</param>
		/// <returns>True if a gituninstaller.cfg was found, else returns false</returns>
		internal bool IsUpdate(string directory)
		{
			string fpath = Path.Combine(directory, "gituninstaller.cfg");
			if (!File.Exists(fpath))
				return false;

			string[] flines = File.ReadAllLines(fpath);
			string firstline = flines[0];
			if (firstline.EndsWith("###") && firstline.StartsWith("###"))
			{
				firstline = firstline.TrimStart('#').TrimEnd('#').Replace(" Uninstaller", "");
				if (Settings.Project != firstline)
				{
					return false;
				}
			}
			return true;
		}

		internal class GitRelease
		{
			public int Id; //is the number the release starts 0,1,2,3,4 etc
			public string Name; //name:
			public string Tag; //tag_name:
			public string GitUrl; //html_url:
			public string CreationDate; //created_at:
			public bool IsPrerelease; //prerelease:
			public string AuthorName; //author.login:
			public string AuthorUrl; //author.html_url:
			public string Body; //body:
			public List<GitAsset> Assets = new List<GitAsset>();
		}

		internal class GitAsset
		{
			public string Filename; ////{assets.asset.}name:
			public float Size; //{assets.asset.}size:
			public string DownloadUrl; //{assets.asset.}browser_download_url:
		}		
	}
}
