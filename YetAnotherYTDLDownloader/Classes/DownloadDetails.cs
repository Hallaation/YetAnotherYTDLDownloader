using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherYTDLDownloader.Classes
{
	public class DownloadDetails 
	{
		public DownloadDetails(string url) : this(url, "", "") { }
		public DownloadDetails(string url, string title, string description) 
		{
			URL = url;
			Title = title;
			Description = description;
		}

		public string URL { get; set; } = "";
		public string Title { get; set; } = "";
		public string Description { get; set; } = "";
		public string ThumbnailURL { get; set; } = "";
		public bool IsRestricted { get; set; } = false;

		public List<String> SupportedVideoFromats { get; set; } = new List<String>();
		public List<String> SupportedAudioFromats { get; set; } = new List<String>();

		public int SelectedVideoFormat { get; set; } = 0;
		public int SelectedAudioFormat { get; set; } = 0;
		//is there anything else?

	}
}
