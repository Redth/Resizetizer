using System;
using System.IO;
using SkiaSharp;

namespace Resizetizer
{
	public enum BitmapResizeMethod
	{
		Box,
		Hamming,
		Lanczos3,
		Mitchell,
		Triangle
	}

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

			var resizeMethod = GetSKBitmapResizeMethod(outputConfig.BitmapResizeMethod ?? BitmapResizeMethod.Box);

			var newBmp = bmp.Resize(new SKImageInfo((int)(bmp.Width * adjustRatio), (int)(bmp.Height * adjustRatio)), resizeMethod);

			var img = SKImage.FromBitmap(newBmp);
			var data = img.Encode(SKImageEncodeFormat.Png, 100);

			using (var fs = File.Open(destinationFile, FileMode.Create))
				data.SaveTo(fs);
		}

		private static SKBitmapResizeMethod GetSKBitmapResizeMethod(BitmapResizeMethod method)
		{
			switch (method)
			{
				case BitmapResizeMethod.Hamming:
					return SKBitmapResizeMethod.Hamming;
				case BitmapResizeMethod.Lanczos3:
					return SKBitmapResizeMethod.Lanczos3;
				case BitmapResizeMethod.Mitchell:
					return SKBitmapResizeMethod.Mitchell;
				case BitmapResizeMethod.Triangle:
					return SKBitmapResizeMethod.Triangle;
				default:
					return SKBitmapResizeMethod.Box;
			}
		}
	}
}
