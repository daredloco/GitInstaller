using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInstaller
{
	public static class Utils
	{
		public static List<string> Wildcard(string[] list, string wildcard, string wildcardchar = "*")
		{
			List<string> lst = new List<string>();
			bool startswith = false;
			if(wildcard.EndsWith(wildcardchar))
			{
				startswith = false;
				wildcard = wildcard.Remove(wildcard.Length - 1);
			}
			else if(wildcard.StartsWith(wildcardchar))
			{
				startswith = true;
				wildcard = wildcard.Remove(0, 1);
			}else
			{
				throw new Exception("Invalid wildcard (Use wildcard* or *wildcard)");
			}

			foreach(string element in list)
			{
				if(startswith)
				{
					if (element.StartsWith(wildcard))
						lst.Add(element);
				}
				else
				{
					if (element.EndsWith(wildcard))
						lst.Add(element);
				}
			}
			return lst;
		}
	}
}
