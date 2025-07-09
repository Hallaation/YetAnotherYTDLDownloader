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
		//only used for debug and layout purposese
		public VideoDetails(string id, string title)
		{
			ID = id;
			Title = title;
		}

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

		public List<VideoFormatDetails>? VideoFormats
		{
			get
			{
				List<VideoFormatDetails>? videos = Formats?.Where(f => f.VideoExt != "none").ToList<VideoFormatDetails>();
				return videos;
			}
		}
		public List<VideoFormatDetails>? AudioFormats
		{
			get
			{
				List<VideoFormatDetails>? audio = Formats?.Where(f => f.AudioExt != "none").ToList<VideoFormatDetails>();
				return audio;
			}
		}

	}

	public class VideoFormatDetails
	{
		[JsonPropertyName("format_id")]
		public string FormatID { get; set; } = "";

		[JsonPropertyName("fps"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public double? FPS { get; set; }

		[JsonPropertyName("width"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? Width { get; set; }

		[JsonPropertyName("height"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
		public int? Height { get; set; }

		[JsonPropertyName("resolution")]
		public string Resolution { get; set; } = "";

		[JsonPropertyName("acodec")]
		public string AudioCodec { get; set; } = "";

		[JsonPropertyName("video_ext")]
		public string VideoExt { get; set; } = "";

		[JsonPropertyName("audio_ext")]
		public string AudioExt { get; set; } = "";

		public String FormattedText
		{
			get
			{
				if (VideoExt != "none")
				{
					return $"{FormatID}: {Resolution}.{VideoExt}";
				}
				else
				{
					return $"{FormatID}: codec:{AudioCodec} ext:{AudioExt}";
				}
			}
		}
	}

}
