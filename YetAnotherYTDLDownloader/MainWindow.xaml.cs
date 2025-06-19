using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


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

	public class MainWindowViewModel : NotifyPropertyChanged
	{
		public string InputURL { get; set; } = "";

		public int SelectedURLIndex
		{
			get { return mSelectedURLIndex; }
			set 
			{
				mSelectedURLIndex = value; 
				Trace.WriteLine($"Selected URL Index changed to {mSelectedURLIndex}"); 
			}
		}
		public ObservableCollection<String> URLList { get; set; } = new ObservableCollection<string>();

		public SimpleCommand AddURL
		{
			get => new SimpleCommand(ex => this.internalAddURL());
		}

		public SimpleCommand OutputURLS
		{
			get => new SimpleCommand(ex =>
			{
				foreach (var url in URLList)
				{
					Trace.WriteLine(url);
				}
			});
		}

		//private/member variables
		private void internalAddURL()
		{
			URLList.Add(InputURL);

			InputURL = "";
			Notify(nameof(InputURL));
			Notify(nameof(URLList));
		}

		private int mSelectedURLIndex = 0;
	}

}