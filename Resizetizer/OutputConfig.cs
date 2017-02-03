using System;
using System.Collections.Generic;
using System.IO;

namespace Resizetizer
{

	public class OutputConfig
	{
		public double Ratio { get; set; }
		public string Path { get; set; }
		public string FilePrefix { get; set; }
		public string FileSuffix { get; set; }

		public List<string> ExternalCommands { get; set; } = new List<string>();

		public bool? Optimize { get; set; }
		public PngOptimizerEngine? Optimizer { get; set; }

		public string FillColor { get; set; }

		public string GetFilename(string originalFilename)
		{
			return string.Format("{0}{1}{2}{3}",
									  FilePrefix ?? "",
									  System.IO.Path.GetFileNameWithoutExtension(originalFilename),
									  FileSuffix ?? "",
									  ".png");
		}

		public string GetAbsoluteFilename(string absoluteBasePath, string originalFilename)
		{
			var file = GetFilename(originalFilename);

			if (System.IO.Path.IsPathRooted(file))
				return System.IO.Path.GetFullPath (file);

			return System.IO.Path.GetFullPath (System.IO.Path.Combine(absoluteBasePath, file));
		}

		public override string ToString()
		{
			return string.Format("[SizeTarget: Ratio={0}, Path={1}, FilePrefix={2}, FileSuffix={3}]", Ratio, Path, FilePrefix, FileSuffix);
		}

		public override bool Equals(object obj)
		{
			return this.ToString() == obj.ToString();
		}

		public override int GetHashCode()
		{
			return Ratio.GetHashCode() ^ Path.GetHashCode() ^ FilePrefix.GetHashCode() ^ FileSuffix.GetHashCode();
		}
	}
	
}
