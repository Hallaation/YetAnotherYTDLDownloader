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
		public YTDLPArgs(string downloadURL)
		{
			mURL = downloadURL;
		}
		public YTDLPArgs(string videoFormatID, string audioFormatID)
		{
			
		}

		public string OutputDir { get; set; } = Directory.GetCurrentDirectory();

		public bool AnalyzeMode { get; set; } = false;

		public bool VerboseOutput { get; set; } = true;
		public bool WaitForVideo { get; set; } = false;
		public int WaitForVideoTimeInSeconds { get; set; } = 30; //30 by default

		public bool LiveFromStart { get; set; } = false;

		public bool EmbedMetadata { get; set; } = true;
		public bool EmbedThumbnail { get; set; } = true;

		public bool SplitTracks { get; set; } = false;

		private string mURL = "";

		public String Args 
		{ 
			get 
			{
				Trace.Assert(mURL != null || !string.IsNullOrEmpty(mURL));

				List<string> args = new List<string>(); 
				
				if (AnalyzeMode)
				{
					args.Add("-j");
					string ret = args.Aggregate((curr, next) => $"{curr} {next} ");
					ret += " " + mURL;
					return ret;

				}
				#region downloadArgs	
				///-------------------------------------------------------- Download Args -------------------------------------------------------------------
				if (VerboseOutput)
				{
					args.Add("-v");
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

				if (!SplitTracks)
				{
					args.Add("--no-split-chapters");
				}

				//add any arguments we want by default
				//avoids corruption, don't think we need an option for this
				args.Add("--hls-use-mpegts");
				args.Add("--progress");

				args.Add($"-o {OutputDir}/%(uploader)s/%(uploader%s-%(title)s.%(ext)s");

				string temp = "";
				temp = args.Aggregate( (curr, next) => $"{curr} {next}" );
				temp += " " + mURL;
				return temp;
				#endregion
			}
		}

	}
}
