using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GitInstaller
{
	/// <summary>
	/// Interaktionslogik für ManualWindow.xaml
	/// </summary>
	public partial class ManualWindow : Window
	{
		public ManualWindow()
		{
			InitializeComponent();
			bt_confirm.Click += ClickConfirm;
		}

		private void ClickConfirm(object sender, RoutedEventArgs e)
		{
			try
			{

				Uri url = new Uri(tb_url.Text);
				if (url.Host != "github.com" && url.Host != "www.github.com")
				{
					MessageBox.Show("Invalid URL. Needs to be a github.com url!");
					return;
				}

				string ustr = url.AbsoluteUri;
				ustr = ustr.Replace("://", "");

				Settings.SettingsFile sfile = new Settings.SettingsFile() {
					ignored_files = new string[]{ "" },
					ignored_tags = new string[] { "" },
					project = ustr.Split('/')[2],
					uninstall = false,
					unzip = true,
					preview = true,
					user = ustr.Split('/')[1],
					repo = ustr.Split('/')[2]
				};
				string jsondata = JsonConvert.SerializeObject(sfile);
				File.WriteAllText("config.json", jsondata);
				Process.Start("GitInstaller.exe");
				Environment.Exit(4);
			}
			catch(Exception ex)
			{
				MessageBox.Show("Error creating configuration file... => " + ex.Message);
				return;
			}
		}
	}
}
