using System;
using System.Collections.Generic;
using System.IO;

namespace Resizetizer
{
	public class Config
	{
		public Config()
		{
		}

		public Platform Platform { get; set; } = Platform.None;

		public string OutputBasePath { get; set; }

		public string GetAbsoluteOutputBasePath(string basePath)
		{
			if (Path.IsPathRooted(OutputBasePath))
				return Path.GetFullPath (OutputBasePath);

			return Path.GetFullPath (Path.Combine(basePath, OutputBasePath));
		}

		public bool AutoAddPlatformSizes { get; set; } = true;

		public List<OutputConfig> Outputs { get; set; } = new List<OutputConfig>();

		public List<ImageAsset> Assets { get; set; } = new List<ImageAsset>();

		public List<string> ExternalCommands { get; set; } = new List<string>();

		public bool Optimize { get; set; } = true;
		public PngOptimizerEngine Optimizer { get; set; } = PngOptimizerEngine.ImageSharp;

		internal void AddPlatformSizes()
		{
			var toAdd = new List<OutputConfig>();

			if (Platform == Platform.Android)
			{
				toAdd.Add(new OutputConfig { Ratio = 0.75, Path = "drawable-ldpi" });
				toAdd.Add(new OutputConfig { Ratio = 1.0, Path = "drawable-mdpi" });
				toAdd.Add(new OutputConfig { Ratio = 1.5, Path = "drawable-hdpi" });
				toAdd.Add(new OutputConfig { Ratio = 2.0, Path = "drawable-xhdpi" });
				toAdd.Add(new OutputConfig { Ratio = 3.0, Path = "drawable-xxhdpi" });
				toAdd.Add(new OutputConfig { Ratio = 4.0, Path = "drawable-xxxhdpi" });
			}
			else if (Platform == Platform.iOS)
			{
				toAdd.Add(new OutputConfig { Ratio = 1.0 });
				toAdd.Add(new OutputConfig { Ratio = 2.0, FileSuffix = "@2x" });
				toAdd.Add(new OutputConfig { Ratio = 3.0, FileSuffix = "@3x" });
			}

			foreach (var i in toAdd)
				if (!Outputs.Contains(i))
					Outputs.Add(i);
		}
	}

	public enum Platform
	{
		None,
		Android,
		iOS,
		UWP
	}
}
