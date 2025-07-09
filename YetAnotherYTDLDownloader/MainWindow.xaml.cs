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
		Regex downloadRgx = new Regex("[0-9]{0,3}%");
		public string InputURL { get; set; } = "";
		public ObservableCollection<VideoDetails> VideoDetailList { get; set; } = new ObservableCollection<VideoDetails>();
		public YTDLPArgs DownloadArgs { get; set; } = new YTDLPArgs();

		public VideoDetails? SelectedVideo { get; private set; } = null;

		private int mSelectedURLIndex = 0;
		public int SelectedURLIndex
		{
			get { return mSelectedURLIndex; }
			set
			{
				mSelectedURLIndex = -1;
				//mSelectedURLIndex = value;
				//Notify(nameof(SelectedVideo));
				//Trace.WriteLine($"Selected URL Index changed to {mSelectedURLIndex}");
			}
		}
		public float DownloadProgress { get; set; } = 0.0f;
		public int SelectedVideoFormatIdx { get; set; } = -1;
		public int SelectedAudioFormatIdx { get; set; } = -1;

		public SimpleCommand AnalyzeAndAdd
		{
			get => new SimpleCommand(ex =>
			{
				this.AnalyzeVideo(InputURL);
			});
		}


		public SimpleCommand StartDownload
		{
			get => new SimpleCommand(ex =>
			{
				this.DownloadVideo(InputURL);
			});
		}

		private void AnalyzeVideo(string url)
		{
			//Build my arguments
			YTDLPHandler handler = new YTDLPHandler();

			DownloadArgs.URL = url;
			DownloadArgs.AnalyzeMode = true;

			handler.Args = DownloadArgs.BuildArgs();
			VideoDetails? outVideoDets = null;
			Trace.WriteLine($"Starting from thread{Thread.CurrentThread.ManagedThreadId}");

			handler.Exec(null, stdout =>
			{
				try
				{
					outVideoDets = JsonSerializer.Deserialize<VideoDetails>(stdout);
					if (outVideoDets != null)
					{
						Application.Current.Dispatcher.BeginInvoke(() =>
						{
							Trace.WriteLine($"OnTestDLPComplete Thread {Thread.CurrentThread.ManagedThreadId}");
							//VideoDetailList.Add(outVideoDets);
							SelectedVideo = outVideoDets;
							Trace.Assert(!string.IsNullOrEmpty(outVideoDets.ID));
							Notify(nameof(SelectedVideo));
							//Notify(nameof(VideoDetailList));
						});
					}

				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			});
		}

		private void DownloadVideo(string url)
		{
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

			Action<string> stdallProcess = (string stdall) => { };
			handler.Exec(stdallProcess, stdout =>
			{
				try
				{
					Application.Current.Dispatcher.BeginInvoke(() =>
					{
						//Match? progressMatch = downloadRgx.Match(stdout);
						//if (progressMatch != null && progressMatch.Groups.Count > 0)
						//{
							//string progress = progressMatch.Groups[0].Value;
							//progress.Remove('%');
							//if (!string.IsNullOrEmpty(progress))
							//{
							//	this.DownloadProgress = float.Parse(progress);
							//}
						//}
					});
				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			});
		}
	}

}