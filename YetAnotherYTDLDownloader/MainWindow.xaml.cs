using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YetAnotherYTDLDownloader.Classes;
using YetAnotherYTDLDownloader.YTDLP;


//global namespace

namespace YetAnotherYTDLDownloader
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		public MainWindow()
		{
			InitializeComponent();
		}

	}

	public class MainWindowViewModel : PropertyNotifiable
	{
		Regex downloadRgx = new Regex("[0-9.]{0,5}%");
		public string InputURL { get; set; } = "";
		public ObservableCollection<VideoDetails> VideoDetailList { get; set; } = new ObservableCollection<VideoDetails>();
		public YTDLPArgs DownloadArgs { get; set; } = new YTDLPArgs();

		public VideoDetails? SelectedVideo { get; private set; } = null;

		public float DownloadProgress { get; set; } = 0.0f;
		public int SelectedVideoFormatIdx { get; set; } = -1;
		public int SelectedAudioFormatIdx { get; set; } = -1;

		public SimpleCommand AnalyzeAndAdd => new SimpleCommand(ex => { this.AnalyzeVideo(InputURL); });
		public SimpleCommand StartDownload => new SimpleCommand(ex => { this.DownloadVideo(InputURL); });
		public SimpleCommand ClearLog => new SimpleCommand(ex => { OutputLog = string.Empty; Notify(nameof(OutputLog)); });
		public string OutputLog { get; set; } = "";

		void OnExit(string error) 
		{
			OutputLog += "Process exited\n";
			Notify(nameof(OutputLog));
		}

		void OnError(string error) 
		{
			OutputLog += $"{error}\n";
			Notify(nameof(OutputLog));
		}

		private void AnalyzeVideo(string url)
		{
			//nothing, just leave
			if (string.IsNullOrEmpty(url)) return;

			OutputLog += "Starting analysis\n";
			//Build my arguments
			YTDLPHandler handler = new YTDLPHandler();

			DownloadArgs.URL = url;
			DownloadArgs.AnalyzeMode = true;

			handler.Args = DownloadArgs.BuildArgs();
			VideoDetails? outVideoDets = null;
			Trace.WriteLine($"Starting from thread{Thread.CurrentThread.ManagedThreadId}");

			Action<string> onstdout = (string stdout) => 
			{
				try
				{
					outVideoDets = JsonSerializer.Deserialize<VideoDetails>(stdout);
					if (outVideoDets != null)
					{
						Application.Current.Dispatcher.BeginInvoke(() =>
						{
							Trace.WriteLine($"OnTestDLPComplete Thread {Thread.CurrentThread.ManagedThreadId}");
							SelectedVideo = outVideoDets;
							Trace.Assert(!string.IsNullOrEmpty(outVideoDets.ID));
							Notify(nameof(SelectedVideo));
						});
					}

				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			};

			handler.Exec(null, onstdout, OnError, OnExit);
		}

		private void DownloadVideo(string url)
		{
			//nothing, just leave
			if (string.IsNullOrEmpty(url)) return;

			YTDLPHandler handler = new YTDLPHandler();

			DownloadArgs.URL = url;
			DownloadArgs.AnalyzeMode = false;

			//get the video format
			if (SelectedVideoFormatIdx != -1)
			{
				if (SelectedVideo != null && SelectedVideo?.VideoFormats?.Count > 0)
				{
					DownloadArgs.SelectedAudioFormatID = SelectedVideo.VideoFormats[SelectedVideoFormatIdx].FormatID;
					//DownloadArgs.SelectedFormatID = SelectedVideo.Formats[SelectedFormatIdx].FormatID;
				}
			}
			//get the audio format
			if (SelectedAudioFormatIdx != -1)
			{
				if (SelectedVideo != null && SelectedVideo?.AudioFormats?.Count > 0)
				{
					DownloadArgs.SelectedAudioFormatID = SelectedVideo.AudioFormats[SelectedAudioFormatIdx].FormatID;
					//DownloadArgs.SelectedFormatID = SelectedVideo.Formats[SelectedFormatIdx].FormatID;
				}
			}
			if (SelectedVideo?.AudioFormats?.Count > 0 && SelectedAudioFormatIdx < 0)
			{
				//Audio not selected but audio formats exist, do we want to do anything?
			} 

			handler.Args = DownloadArgs.BuildArgs();

			Action<string> downloadOutput = (string stdout) =>
			{
				Match? progressMatch = downloadRgx.Match(stdout);
				if (stdout != null && progressMatch.Captures.Count > 0)
				{
					string progress = progressMatch.Captures[0].Value;
					int percentIdx = progress.IndexOf('%');
					string edited = progress.Remove(percentIdx);

					if (!string.IsNullOrEmpty(edited))
					{
						this.DownloadProgress = float.Parse(edited);
						Notify(nameof(DownloadProgress));
					}
				}
			};

			handler.Exec(null, downloadOutput, OnError, OnExit);
		}
	}

}