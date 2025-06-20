using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YetAnotherYTDLDownloader.YTDLP
{
	public class YTDLPHandler
	{
		private string DPLGitHubAPI = @"https://api.github.com/repos/yt-dlp/yt-dlp/releases";
		public enum DLPError { Sign, Unsupported }

		const String defaultDownloadDir = "%userprofile%\\Videos";
		public String DownloadLocation { get; set; } = defaultDownloadDir;
		public HashSet<DLPError> StdErr { get; set; } = new();
		public String YTDLP_PATH { get; set; } = "";
		public String Args { get; set; } = "";
		Process process = new();

		public YTDLPHandler() { }


		public bool CheckForDLP()
		{
			//yt-dlp.exe for windows, yt-dlp for other platforms

			bool isWindows = Environment.OSVersion.Platform == PlatformID.Win32NT;
			String fileName = isWindows ? "yt-dlp.exe" : "yt-dlp";
			//Path.PathSeparator
			if (isWindows)
			{
				//Unsure how paths are handled on other platforms, PATH is guranteed to exist on windows
				string[]? paths = Environment.GetEnvironmentVariable("PATH")?.Split(Path.PathSeparator) ?? Array.Empty<string>();

				if (paths.Length > 0)
				{
					//loop through and find our executable
					foreach (string path in paths)
					{
						string fullPath = Path.Combine(path, fileName);
						if (File.Exists(fullPath))
						{
							YTDLP_PATH = fullPath;
							Trace.WriteLine($"DLP found at {fullPath}");
							//use the first instance found
							return true;
						}
					}
					if (String.IsNullOrEmpty(YTDLP_PATH))
					{
						Trace.WriteLine("No yt-dlp found");
						return false;
					}
				}
			}
			return false;
		}
		public async Task DownloadDLP()
		{
			//we have the API URL
			HttpClient client = new HttpClient();

			HttpResponseMessage response = await client.GetAsync(DPLGitHubAPI);
			if (response.StatusCode == HttpStatusCode.OK)
			{
				Console.WriteLine("Success");
			}
			else
			{
				throw new Exception("Failed to get response");
			}
			//Trace.WriteLine(response);
		}

		public void ChangeDownloadDirectory(String newDirectory)
		{
			// Logic to change the download directory for YTDLP
			Console.WriteLine($"Changing download directory to {newDirectory} using YTDLP...");
			String oldDir = DownloadLocation;
			DownloadLocation = newDirectory;

			//Check if directory exists
			if (!System.IO.Directory.Exists(DownloadLocation))
			{
				//prompt if want to create dir
			}
		}

		private static Regex ErrSign = new Regex(@"^(?=.*?ERROR)(?=.*?sign)(?=.*?confirm)", RegexOptions.IgnoreCase);
		private static Regex ErrUnsupported = new Regex(@"^(?=.*?ERROR)(?=.*?Unsupported)", RegexOptions.IgnoreCase);
		public Process Exec(Action<string> stdall = null, Action<string> stdout = null, Action<string> stderr = null)
		{
			var fn = YTDLP_PATH;
			if (!File.Exists(fn))
			{
				return null;
			}
			var info = new ProcessStartInfo()
			{
				FileName = fn,
				Arguments = Args,
				UseShellExecute = false,
				CreateNoWindow = true,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
			};
			//Debug.WriteLine(Args);
			Debug.WriteLine($"{info.FileName} {info.Arguments}");
			process.StartInfo = info;
			process.EnableRaisingEvents = true;
			process.OutputDataReceived += (s, e) =>
			{
				Debug.WriteLine(e.Data, "STD");
				if (!string.IsNullOrWhiteSpace(e.Data))
				{
					stdall?.Invoke(e.Data);
					stdout?.Invoke(e.Data);
				}
			};

			process.ErrorDataReceived += (s, e) =>
			{
				Debug.WriteLine(e.Data, "ERR");
				if (!string.IsNullOrWhiteSpace(e.Data))
				{
					stdall?.Invoke(e.Data);
					stderr?.Invoke(e.Data);
					if (ErrSign.IsMatch(e.Data))
					{
						StdErr.Add(DLPError.Sign);
					}
					if (ErrUnsupported.IsMatch(e.Data)) StdErr.Add(DLPError.Unsupported);
				}
			};

			process.Start();
			process.BeginErrorReadLine();
			process.BeginOutputReadLine();
			process.WaitForExit();
			return process;
		}

	}
}
