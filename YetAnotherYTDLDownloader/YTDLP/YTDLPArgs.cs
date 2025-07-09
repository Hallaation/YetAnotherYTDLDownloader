using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherYTDLDownloader.YTDLP
{
	public class YTDLPArgs
	{

		public string OutputDir { get; set; } = Directory.GetCurrentDirectory();

		public bool AnalyzeMode { get; set; } = false;

		public int WaitForVideoTimeInSeconds { get; set; } = 30; //30 by default
		public bool VerboseOutput { get; set; } = true;
		public bool WaitForVideo { get; set; } = false;
		public bool LiveFromStart { get; set; } = false;

		public bool EmbedMetadata { get; set; } = true;
		public bool EmbedThumbnail { get; set; } = true;
		public bool SplitTracks { get; set; } = false;
		public bool IncludeUploader { get; set; } = true;

		public string SelectedVideoFormatID { get; set; } = "";
		public string SelectedAudioFormatID { get; set; } = "";

		public string URL { get; set; } = "";

		public String BuildArgs()
		{
			Trace.Assert(URL != null || !string.IsNullOrEmpty(URL));

			List<string> args = new List<string>();

			if (AnalyzeMode)
			{
				args.Add("-j");
				string ret = args.Aggregate((curr, next) => $"{curr} {next} ");
				ret += " " + URL;
				return ret;

			}
			#region downloadArgs	
			///-------------------------------------------------------- Download Args -------------------------------------------------------------------
			if (VerboseOutput)
			{
				args.Add("-v");
			}

			if (!string.IsNullOrEmpty(SelectedVideoFormatID))
			{
				if (string.IsNullOrEmpty(SelectedAudioFormatID))
				{
					args.Add($"-f {SelectedVideoFormatID}+{SelectedAudioFormatID}");
				}
				//only video selected
				else
				{
					args.Add($"-f {SelectedVideoFormatID}");
				}
			}

			if (WaitForVideo && WaitForVideoTimeInSeconds > 0)
			{
				args.Add($"--wait-for-video {WaitForVideoTimeInSeconds}");
			}

			if (LiveFromStart)
			{
				args.Add("--live-from-start");
			}

			if (EmbedMetadata)
			{
				args.Add("--embed-metadata");
			}

			if (EmbedThumbnail)
			{
				args.Add("--embed-thumbnail");
			}

			if (!SplitTracks)
			{
				args.Add("--no-split-chapters");
			}


			//add any arguments we want by default
			//avoids corruption, don't think we need an option for this
			args.Add("--hls-use-mpegts");
			args.Add("--progress");

			if (IncludeUploader)
			{
				args.Add($"-o {OutputDir}\\%(uploader)s\\%(uploader)s-%(title)s.%(ext)s");
			}
			else
			{
				args.Add($"-o {OutputDir}\\%(title)s.%(ext)s");
			}
			string temp = "";
			temp = args.Aggregate((curr, next) => $"{curr} {next}");
			temp += " " + URL;
			return temp;
			#endregion

		}

	}
}
