using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitInstaller
{
	class Program
	{
		public static List<Installer.GitRelease> Releases = new List<Installer.GitRelease>();

		static void Main(string[] args)
		{
			if(args.Length != 3)
			{
				Console.WriteLine("Not enough arguments for the Installer!");
				Console.ReadKey();
				return;
			}
			foreach(string arg in args)
			{
				Console.WriteLine("Arg: " + arg);
				Console.ReadKey();
			}

		}
	}
}
