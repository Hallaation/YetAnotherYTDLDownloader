using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
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
		public string InputURL { get; set; } = "";

		public ObservableCollection<VideoDetails> VideoDetailList { get; set; } = new ObservableCollection<VideoDetails>();

		public VideoDetails? SelectedVideo
		{
			get
			{
				if (SelectedURLIndex > -1 && SelectedURLIndex < VideoDetailList.Count)
				{
					return VideoDetailList[SelectedURLIndex];
				}
				return null;
			}
		}

		private int mSelectedURLIndex = 0;
		public int SelectedURLIndex
		{
			get { return mSelectedURLIndex; }
			set
			{
				mSelectedURLIndex = value;
				Notify(nameof(SelectedVideo));
				Trace.WriteLine($"Selected URL Index changed to {mSelectedURLIndex}");
			}
		}

		public SimpleCommand AnalyzeAndAdd
		{
			get => new SimpleCommand(ex =>
			{
				this.AnalyzeVideo(InputURL);
			});
		}

		private void AnalyzeVideo(string url)
		{
			//Build my arguments
			YTDLPHandler handler = new YTDLPHandler();
			handler.Args = $"-j {url}";
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
							VideoDetailList.Add(outVideoDets);
							Trace.Assert(!string.IsNullOrEmpty(outVideoDets.ID));
							InputURL = "";
							Notify(nameof(VideoDetailList));
						});
					}

				}
				catch (Exception ex)
				{
					Trace.WriteLine(ex.ToString());
				}
			});
		}
	}

}