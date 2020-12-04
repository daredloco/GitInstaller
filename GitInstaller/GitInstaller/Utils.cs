using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GitInstaller
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
		/// ???
		/// </summary>
		/// <param name="value"></param>
		/// <param name="wildcard"></param>
		/// <param name="wildcardchar"></param>
		/// <returns></returns>
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
	}
}
