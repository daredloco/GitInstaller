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

		public static ZipType FileOrDir(string path)
		{
			if (path.EndsWith("/"))
				return ZipType.Directory;
			else
				return ZipType.File;
		}

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
