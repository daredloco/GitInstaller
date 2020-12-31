using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
	/// Interaktionslogik für LicenseWindow.xaml
	/// </summary>
	public partial class LicenseWindow : Window
	{
		public LicenseWindow()
		{
			InitializeComponent();
			LoadLicense();
			bt_accept.Click += AcceptClick;
			bt_decline.Click += DeclineClick;

			bt_accept.IsEnabled = false;
		}

		private void DeclineClick(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void AcceptClick(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		async void LoadLicense()
		{
			using (var client = new WebClient())
			{
				client.Headers.Add("user-agent", "GitInstaller");
				string licenseJson = await client.DownloadStringTaskAsync(Settings.LicenseUrl.AbsoluteUri);
				JObject job = JsonConvert.DeserializeObject<JObject>(licenseJson);
				//License
				JToken license = job.Value<JToken>("license");
				client.Headers["user-agent"] = "gitinstaller";
				client.Headers.Add("accept", "application/vnd.github.v3+json");
				string licJson = await client.DownloadStringTaskAsync(new Uri(license.Value<string>("url")).AbsoluteUri);
				JObject licobj = JsonConvert.DeserializeObject<JObject>(licJson);
				string licenseBody = licobj.Value<string>("body");
				rtb_license.Document.Blocks.Clear();
				Paragraph para = new Paragraph();
				para.Inlines.Add(licenseBody);
				rtb_license.Document.Blocks.Add(para);
			}

			bt_accept.IsEnabled = true;
		}
	}
}
