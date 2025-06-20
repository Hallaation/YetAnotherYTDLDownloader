using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

		private int mSelectedURLIndex = 0;
		public int SelectedURLIndex
		{
			get { return mSelectedURLIndex; }
			set 
			{
				mSelectedURLIndex = value; 
				Trace.WriteLine($"Selected URL Index changed to {mSelectedURLIndex}"); 
			}
		}
		public ObservableCollection<VideoDetails> VideoDetailList { get; set; } = new ObservableCollection<VideoDetails>();

		public SimpleCommand AnalyzeAndAdd
		{
			get => new SimpleCommand(ex => 
			{
				//stdout
				this.AnalyzeVideo(InputURL);
				//this.internalAddURL();
			});
		}

		public SimpleCommand OutputURLS
		{
			get => new SimpleCommand(ex =>
			{
				foreach (var url in VideoDetailList)
				{
					Trace.WriteLine(url);
				}
			});
		}

		private void AnalyzeVideo(string url)
		{
			//Build my arguments
			YTDLPHandler handler = new YTDLPHandler();
			handler.Args = $"-j {url}";
			VideoDetails? outVideoDets = null;

			handler.Exec(null, stdout =>
			{
				try
				{
					outVideoDets = JsonSerializer.Deserialize<VideoDetails>(stdout);
					if (outVideoDets != null)
					{
						VideoDetailList.Add(outVideoDets);
						Trace.Assert(!string.IsNullOrEmpty(outVideoDets.ID));
						Notify(nameof(VideoDetailList));
						//will it actually reach here?
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