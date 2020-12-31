using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace GitInstallerNET5
{
	/// <summary>
	/// Interaktionslogik für ManualWindow.xaml
	/// </summary>
	public partial class ManualWindow : Window
	{
		public ManualWindow()
		{
			InitializeComponent();
			Title = "Select a Repository:";
			bt_confirm.Click += ClickConfirm;
		}

		private void ClickConfirm(object sender, RoutedEventArgs e)
		{
			try
			{

				Uri url = new Uri(tb_url.Text);
				if (url.Host != "github.com" && url.Host != "www.github.com")
				{
					MessageBox.Show("Invalid URL. Needs to be a github.com url!", "Warning!", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				string ustr = url.AbsoluteUri;
				ustr = ustr.Replace("://", "");

				Settings.SettingsFile sfile = new Settings.SettingsFile()
				{
					ignored_files = new string[] { "" },
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
			catch (Exception ex)
			{
				MessageBox.Show("Error creating configuration file... => " + ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
		}
	}
}
