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
using Microsoft.WindowsAPICodePack.Dialogs;

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

			cb_versions.SelectionChanged += VersionChanged;
			rtb_changes.IsReadOnly = true;
			rtb_log.IsReadOnly = true;
			bt_install.IsEnabled = false;
			bt_install.Click += InstallClicked;

			Settings.Load();
			if(Settings.State == Settings.SettingsState.Loaded)
			{
				Title = Settings.file.project + " - Powered by GitInstaller " + Assembly.GetExecutingAssembly().GetName().Version;
				installer = new Installer(Settings.ApiUrl, this);
			}
			else
			{
				MessageBox.Show("The Installer can't progress because the settings file couldn't loaded!");
				Environment.Exit(1);
			}
		}

		private void InstallClicked(object sender, RoutedEventArgs e)
		{
			int idx = cb_versions.SelectedIndex;
			using (var dialog = new CommonOpenFileDialog())
			{
				dialog.Title = "Choose an installation directory:";
				dialog.IsFolderPicker = true;
				CommonFileDialogResult result = dialog.ShowDialog();
				if (result == CommonFileDialogResult.Ok)
				{
					installer.StartInstallation(idx, dialog.FileName);
				}
			}	
		}

		private void VersionChanged(object sender, SelectionChangedEventArgs e)
		{
			int idx = cb_versions.SelectedIndex;
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
			string content = release.Body;
			Paragraph para = new Paragraph();
			para.Inlines.Add(content);
			rtb_changes.Document.Blocks.Clear();
			rtb_changes.Document.Blocks.Add(para);
		}
	}
}
