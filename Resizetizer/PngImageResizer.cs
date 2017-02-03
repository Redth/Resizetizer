using System;
using System.IO;
using SkiaSharp;

namespace Resizetizer
{
	public class PngImageResizer : ImageResizer
	{
		public PngImageResizer() : base()
		{
		}

		public override void Resize(string sourceFile, string destinationFile, ImageAsset asset, OutputConfig outputConfig)
		{
			int sourceNominalWidth = asset.Width;
			int sourceNominalHeight = asset.Height;
			double resizeRatio = outputConfig.Ratio;

			var bmp = SKBitmap.Decode(sourceFile);

			var sourceActualWidth = bmp.Width;
			var sourceActualHeight = bmp.Height;

			var nominalRatio = Math.Max	((double)sourceNominalWidth / (double)sourceActualWidth, (double)sourceNominalHeight / (double)sourceActualHeight);

			var adjustRatio = nominalRatio * resizeRatio;


			var newBmp = bmp.Resize(new SKImageInfo((int)(bmp.Width * adjustRatio), (int)(bmp.Height * adjustRatio)), SKBitmapResizeMethod.Box);

			var img = SKImage.FromBitmap(newBmp);
			var data = img.Encode(SKImageEncodeFormat.Png, 100);

			using (var fs = File.Open(destinationFile, FileMode.Create))
				data.SaveTo(fs);
		}
	}
}
