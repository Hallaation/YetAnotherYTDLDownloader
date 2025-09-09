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
		//Numbers 0-9 and '.' for percentage up to 5 characters (% inclusive) e.g 99.99% 
		Regex downloadRgx = new Regex("[0-9.]{0,5}%");
		public string InputURL { get; set; } = "";
		public ObservableCollection<VideoDetails> VideoDetailList { get; set; } = new ObservableCollection<VideoDetails>();
		public YTDLPArgs DownloadArgs { get; set; } = new YTDLPArgs();

		public VideoDetails? CurrentVideoDetails { get; private set; } = null;

		public float DownloadProgress { get; set; } = 0.0f;
		public int SelectedVideoFormatIdx { get; set; } = -1;
		public int SelectedAudioFormatIdx { get; set; } = -1;

		public SimpleCommand AnalyzeAndAdd => new SimpleCommand(ex => { this.AnalyzeVideo(InputURL); });
		public SimpleCommand StartDownload => new SimpleCommand(ex => { this.DownloadVideo(InputURL); });
		public SimpleCommand ClearLog => new SimpleCommand(ex => { OutputLog = string.Empty; Notify(nameof(OutputLog)); });
		public SimpleCommand KillProcess => new SimpleCommand(ex =>
		{
			if (currentProcess != null)
			{
				currentProcess.Kill();
				//The rest will be dealt with in OnExit
			}
		});

		public bool DownloadAvailable => (currentProcess == null || (currentProcess != null && currentProcess.HasExited)) && CurrentVideoDetails != null;

		public string OutputLog { get; set; } = "";

		private Process? currentProcess = null;

		void OnExit(string outputString)
		{
			OutputLog += string.IsNullOrEmpty(outputString) ? "Process exited\n" : $"Process exited with message: {outputString}";
			Notify(nameof(OutputLog));

			//make sure current process is now null because it doesn't exist anymore
			if (currentProcess != null)
			{
				Trace.Assert(currentProcess.HasExited);
				currentProcess = null;

				Notify(nameof(DownloadAvailable));
			}
		}

		void OnError(string error)
		{
			OutputLog += $"Error ocurred: {error}\n";
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
							CurrentVideoDetails = outVideoDets;

							bool isCurrentlyLive = CurrentVideoDetails.LiveStatus.CompareTo("is_live") == 0;
							DownloadArgs.IsLiveStream = isCurrentlyLive;
							//Its a live stream, we probably want to download from the start and not mid way
							if (isCurrentlyLive)
							{
								DownloadArgs.LiveFromStart = isCurrentlyLive;
								//we've made a change to the settings, make sure its reflected on UI
								Notify(nameof(DownloadArgs));
							}

							Trace.Assert(!string.IsNullOrEmpty(outVideoDets.ID));
							Notify(nameof(CurrentVideoDetails));
							Notify(nameof(DownloadAvailable));
						});
					}

				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			};

			Process? analyzeProcess = handler.Exec(null, onstdout, OnError, OnExit);
			Trace.Assert(analyzeProcess != null);
		}

		private void DownloadVideo(string url)
		{
			//nothing, just leave
			if (string.IsNullOrEmpty(url)) return;

			YTDLPHandler handler = new YTDLPHandler();

			DownloadArgs.URL = url;
			DownloadArgs.AnalyzeMode = false;
			Trace.Assert(CurrentVideoDetails != null, "Current video should not be null");
			DownloadArgs.VideoTitle = CurrentVideoDetails != null ? CurrentVideoDetails.Title : "Null";

			//get the video format
			if (SelectedVideoFormatIdx != -1)
			{
				if (CurrentVideoDetails != null && CurrentVideoDetails?.VideoFormats?.Count > 0)
				{
					DownloadArgs.SelectedAudioFormatID = CurrentVideoDetails.VideoFormats[SelectedVideoFormatIdx].FormatID;
				}
			}
			//get the audio format
			if (SelectedAudioFormatIdx != -1)
			{
				if (CurrentVideoDetails != null && CurrentVideoDetails?.AudioFormats?.Count > 0)
				{
					DownloadArgs.SelectedAudioFormatID = CurrentVideoDetails.AudioFormats[SelectedAudioFormatIdx].FormatID;
				}
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

			Trace.Assert(currentProcess == null);
			currentProcess = handler.Exec(null, downloadOutput, OnError, OnExit);
			Trace.Assert(currentProcess != null);
			Notify(nameof(DownloadAvailable));
		}
	}

}