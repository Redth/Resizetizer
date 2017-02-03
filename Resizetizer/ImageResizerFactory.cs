using System;
namespace Resizetizer
{
	public static class ImageResizerFactory
	{
		static SvgImageResizer svgResizer = new SvgImageResizer();
		static PngImageResizer pngResizer = new PngImageResizer();

		public static ImageResizer Create(string fileExtension)
		{
			if (fileExtension.StartsWith(".", StringComparison.OrdinalIgnoreCase))
				fileExtension = fileExtension.Substring(1);

			fileExtension = fileExtension.ToLowerInvariant();

			if (fileExtension == "svg")
				return svgResizer;
			if (fileExtension == "png")
				return pngResizer;
			
			return null;
		}
	}
	
}
