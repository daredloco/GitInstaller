using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GitInstallerNET5
{
	public static class Utils
	{
		public enum ZipType
		{
			File,
			Directory,
			None
		}

		/// <summary>
		/// Checks if the ZipType of the path is ZipType.File or ZipType.Directory
		/// </summary>
		/// <param name="path">The path of the file/directory</param>
		/// <returns>A ZipType object (either ZipType.Directory or ZipType.File)</returns>
		public static ZipType FileOrDir(string path)
		{
			if (path.EndsWith("/"))
				return ZipType.Directory;
			else
				return ZipType.File;
		}

		/// <summary>
		/// Checks if value has wildcard in it
		/// </summary>
		/// <param name="value">The value to check if the wildcard is inside</param>
		/// <param name="wildcard">The wildcard search value</param>
		/// <param name="wildcardchar">The character representing the wildcard</param>
		/// <returns>True if wildcard is inside value or false otherwise</returns>
		public static bool HasWildcard(string value, string wildcard, string wildcardchar = "*")
		{
			
			if(wildcard.StartsWith(wildcardchar))
			{
				wildcard = wildcard.Remove(0, 1);
				if (value.EndsWith(wildcard))
					return true;
			}
			else if(wildcard.EndsWith(wildcardchar))
			{
				wildcard = wildcard.Remove(wildcard.Length - 1);
				if (value.StartsWith(wildcard))
					return true;
			}
			else
			{
				if (value == wildcard)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Checks whether the file can be writte to or if its blocked by another process
		/// </summary>
		/// <param name="fname">The filename</param>
		/// <returns>True if the file can be written to and false if it can't</returns>
		public static bool CanBeWrittenTo(string fname)
		{
			if (!File.Exists(fname))
				return true;
			try
			{
				FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Write);
				fs.Close();
				return true;
			}
			catch (IOException)
			{
				return false;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
	}
}
