using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInstaller
{
	// From https://stackoverflow.com/questions/14795197/forcefully-replacing-existing-files-during-extracting-file-using-system-io-compr/30425148	
	public static class ZipArchiveExtensions
	{
		public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
		{
			if (!overwrite)
			{
				archive.ExtractToDirectory(destinationDirectoryName);
				return;
			}
			foreach (ZipArchiveEntry file in archive.Entries)
			{
				string completeFileName = Path.Combine(destinationDirectoryName, file.FullName);
				string directory = Path.GetDirectoryName(completeFileName);

				if (!Directory.Exists(directory))
					Directory.CreateDirectory(directory);

				if (file.Name != "")
					file.ExtractToFile(completeFileName, true);
			}
		}
	}
}
