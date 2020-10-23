using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GitInstaller
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		Installer installer;

		public MainWindow()
		{
			InitializeComponent();

			cb_versions.SelectedValuePath = "Key";
			cb_versions.DisplayMemberPath = "Value";
			cb_versions.SelectionChanged += VersionChanged;
			cb_preview.Checked += PreviewChecked;
			cb_preview.Unchecked += PreviewUnchecked;
			rtb_changes.IsReadOnly = true;
			rtb_log.IsReadOnly = true;
			bt_install.IsEnabled = false;
			bt_install.Click += InstallClicked;

			Settings.Load();
			if(Settings.State == Settings.SettingsState.Loaded)
			{
				Title = Settings.file.project + " - Powered by GitInstaller " + Assembly.GetExecutingAssembly().GetName().Version;
				cb_preview.IsChecked = Settings.file.preview;
				installer = new Installer(Settings.ApiUrl, this);
			}
			else
			{
				MessageBox.Show("The Installer can't progress because the settings file couldn't loaded!");
				Environment.Exit(1);
			}
		}

		private void PreviewUnchecked(object sender, RoutedEventArgs e)
		{
			UpdateVersions(false);
		}

		private void PreviewChecked(object sender, RoutedEventArgs e)
		{
			UpdateVersions(true);
		}

		private void InstallClicked(object sender, RoutedEventArgs e)
		{
			int idx = cb_versions.SelectedIndex;
			using (System.Windows.Forms.FolderBrowserDialog fbd = new System.Windows.Forms.FolderBrowserDialog())
			{
				fbd.Description = "Choose an installation directory:";
				var result = fbd.ShowDialog();
				if(result == System.Windows.Forms.DialogResult.OK)
				{
					installer.StartInstallation(idx, fbd.SelectedPath);
				}
			}	
		}

		private void VersionChanged(object sender, SelectionChangedEventArgs e)
		{
			if(cb_versions.SelectedIndex == -1)
			{
				return;
			}
			KeyValuePair<int, string> kvp = (KeyValuePair<int, string>)cb_versions.Items[cb_versions.SelectedIndex];
			int idx = kvp.Key;
			if (idx == -1)
			{
				rtb_changes.Document.Blocks.Clear();
				return;
			}

			try
			{
				UpdateChanges(installer.Releases[idx]);
			}
			catch(Exception ex)
			{
				if (ex.HResult != -2146233086)
				{
					MessageBox.Show("There was an unexpected error while changing the version!");
					throw ex;
				}
			}
		}

		public void WriteLog(string msg)
		{
			msg = DateTime.Now + ": " + msg;
			Paragraph para = new Paragraph();
			para.Inlines.Add(msg);
			rtb_log.Document.Blocks.Add(para);
			rtb_log.ScrollToEnd();
		}

		internal void UpdateChanges(Installer.GitRelease release)
		{
			string[] lines = release.Body.Split('\n');

			rtb_changes.Document.Blocks.Clear();
			foreach (string line in lines)
			{
				string newline = line.Replace("\n", "").Replace("\r","");
				Paragraph para = new Paragraph();
				para.Margin = new Thickness(0);
				if(line.StartsWith("####"))
				{
					newline = line.TrimStart('#');
					para.FontWeight = FontWeights.ExtraBold;
				}
				else if(line.StartsWith("###"))
				{
					newline = line.TrimStart('#');
					para.FontWeight = FontWeights.Bold;
				}
				else if(line.StartsWith("##"))
				{
					newline = line.TrimStart('#');
					para.FontWeight = FontWeights.SemiBold;
				}	
				else if(line.StartsWith("#"))
				{
					newline = line.TrimStart('#');
					para.FontWeight = FontWeights.DemiBold;
				}

				para.Inlines.Add(newline);
				rtb_changes.Document.Blocks.Add(para);
			}

			//Old
			//string content = release.Body;
			//Paragraph para = new Paragraph();
			//para.Inlines.Add(content);
			//rtb_changes.Document.Blocks.Clear();
			//rtb_changes.Document.Blocks.Add(para);
		}

		internal void UpdateVersions(bool withpreviews)
		{
			cb_versions.Items.Clear();
			if (installer == null)
				return;
			foreach (Installer.GitRelease release in installer.Releases)
			{
				if (withpreviews)
					cb_versions.Items.Add(new KeyValuePair<int, string>(release.Id, release.Tag));
				else if (!withpreviews && !release.IsPrerelease)
					cb_versions.Items.Add(new KeyValuePair<int, string>(release.Id, release.Tag));
			}
			if(cb_versions.Items.Count < 1)
			{
				cb_versions.Items.Add(new KeyValuePair<int, string>(-1, "None"));
				bt_install.IsEnabled = false;
			}
			else
			{
				bt_install.IsEnabled = true;
			}
			cb_versions.SelectedIndex = 0;
		}
	}
}
