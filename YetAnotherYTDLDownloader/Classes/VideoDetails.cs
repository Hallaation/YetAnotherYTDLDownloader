using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace YetAnotherYTDLDownloader.Classes
{
	public class VideoDetails
	{
		[JsonPropertyName("id")]
		public string ID { get; set; } = "";
		[JsonPropertyName("title")]
		public string Title { get; set; } = "";
		[JsonPropertyName("thumbnail")]
		public string Thumbnail { get; set; } = "";
		[JsonPropertyName("description")]
		public string Description { get; set; } = "";
		[JsonPropertyName("channel_id")]
		public string ChannelID { get; set; } = "";
		[JsonPropertyName("channel_url")]
		public string ChannelURL { get; set; } = "";
		//Channel name, just shows as channel in response from ytdlp
		[JsonPropertyName("channel")]
		public string Channel { get; set; } = "";
		[JsonPropertyName("live_status")]
		public string LiveStatus { get; set; } = "";
		[JsonPropertyName("formats")]
		public List<VideoFormatDetails>? Formats { get; set; } = null;


	}

	public class VideoFormatDetails
	{
		[JsonPropertyName("format_id")]
		public string FormatID { get; set; } = "";
		[JsonPropertyName("fps")]
		public double FPS { get; set; }
		[JsonPropertyName("width")]
		public int Width { get; set; }
		[JsonPropertyName("height")]
		public int Height { get; set; }
		[JsonPropertyName("resolution")]
		public string Resolution { get; set; } = "";
		[JsonPropertyName("video_ext")]
		public string VideoExt { get; set; } = "";
		[JsonPropertyName("audio_ext")]
		public string AudioExt { get; set; } = "";
	}

}
