using System;
using System.IO;
using System.IO.Compression;

namespace GitInstaller
{
	public static class ZipArchiveExtensions
	{
		// From https://stackoverflow.com/questions/14795197/forcefully-replacing-existing-files-during-extracting-file-using-system-io-compr/30425148
		/// <summary>
		/// Extracts a ZipArchive to a destination (with overwrite functionality)
		/// </summary>
		/// <param name="archive">The archive to unpack</param>
		/// <param name="destinationDirectoryName">The destination directory</param>
		/// <param name="overwrite">If true, it will overwrite the content inside the destination directory</param>
		public static bool ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
		{
			Uninstaller uninstaller = new Uninstaller(destinationDirectoryName);
			bool extractionSuccessful = true;
			if (!overwrite)
			{
				try
				{
					archive.ExtractToDirectory(destinationDirectoryName);
					uninstaller.directories.Add(destinationDirectoryName);
					if (Settings.Uninstall)
						uninstaller.GenerateFile();
					return true;
				}
				catch(Exception ex)
				{
					Console.WriteLine("Exception while extracting files to directory! => " + ex.Message, "Exception");
					return false;
				}				
			}

			foreach (ZipArchiveEntry file in archive.Entries)
			{
				string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
				string directory = Path.GetDirectoryName(completeFileName);

				if (!Directory.Exists(directory))
					Directory.CreateDirectory(directory);

				if (!string.IsNullOrEmpty(file.Name) && file.Name != "zipsettings.json")
				{
					while (!Utils.CanBeWrittenTo(completeFileName))
					{
						Console.WriteLine("Please close the file/process " + completeFileName + " and press any key to progress the installation");
						Console.ReadKey();
					}
					if (!extractionSuccessful)
						break;
					file.ExtractToFile(completeFileName, true);
					uninstaller.files.Add(completeFileName);
				}
			}
			if(Settings.Uninstall)
				uninstaller.GenerateFile();

			if (!extractionSuccessful)
				return false;
			return true;
		}

		//Added by daRedLoCo
		/// <summary>
		/// Extracts a ZipArchive to a destination (with overwrite functionality and ZipSettings inside the archive)
		/// </summary>
		/// <param name="archive">The archive to unpack</param>
		/// <param name="destinationDirectoryName">The destination directory</param>
		/// <param name="overwrite">If true, it will overwrite the content inside the destination directory</param>
		public static bool ExtractWithSettings(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
		{
			ZipSettings settings = null;
			bool extractionSuccessful = true;
			if(archive.GetEntry("zipsettings.json") != null)
				settings = ZipSettings.FromStream(archive.GetEntry("zipsettings.json").Open());
			if(settings == null || settings.Subfolders.Count < 1)
			{
				return ExtractToDirectory(archive, destinationDirectoryName, overwrite);
			}
			Uninstaller uninstaller = new Uninstaller(destinationDirectoryName);
			foreach(ZipArchiveEntry file in archive.Entries)
			{
				string completeFileName = Path.Combine(destinationDirectoryName, settings.GetSubfolder(file.FullName), file.FullName);
				string directory = Path.GetDirectoryName(completeFileName);

				if (!Directory.Exists(directory))
					Directory.CreateDirectory(directory);

				if (!string.IsNullOrEmpty(file.Name) && file.Name != "zipsettings.json")
				{
					while(!Utils.CanBeWrittenTo(completeFileName))
					{
						Console.WriteLine("Please close the file/process " + completeFileName + " and press any key to progress the installation");
						Console.ReadKey();
					}
					if (!extractionSuccessful)
						break;
					file.ExtractToFile(completeFileName, overwrite);
					uninstaller.files.Add(completeFileName);
				}
			}
			if(Settings.Uninstall)
				uninstaller.GenerateFile();

			
			if (!extractionSuccessful)
				return false;
			return true;
		}
	}
}
