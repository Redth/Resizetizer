using System;
using System.IO;
using Newtonsoft.Json;

namespace Resizetizer
{
	public class ImageAsset
	{
		public ImageAsset()
		{
		}

		public string File { get; set; }
		public string Size { get; set; }

		public string FillColor { get; set; }

		[JsonIgnore]
		public int Width
		{
			get
			{
				var s = ParseSize(Size);
				return s.Item1;
			}
		}

		[JsonIgnore]
		public int Height
		{
			get
			{
				var s = ParseSize(Size);
				return s.Item2;
			}
		}

		public string GetAbsoluteFile(string basePath)
		{
			if (Path.IsPathRooted(File))
				return Path.GetFullPath (File);

			return Path.GetFullPath (Path.Combine(basePath, File));
		}

		static Tuple<int, int> ParseSize(string sizeStr)
		{
			var width = -1;
			var height = -1;

			var parts = sizeStr.Split(new char[] { 'x', '*', ';', ',', '|' }, 2);

			if (parts?.Length == 2)
			{
				var w = parts[0].Replace("w", "").Replace("dp", "").Replace("x", "");
				int.TryParse(w, out width);

				var h = parts[0].Replace("h", "").Replace("dp", "").Replace("y", "");
				int.TryParse(h, out height);
			}

			return new Tuple<int, int>(width, height);
		}
	}
}
