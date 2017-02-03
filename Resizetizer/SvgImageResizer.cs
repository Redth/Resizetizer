using System;
using System.IO;
using SkiaSharp;

namespace Resizetizer
{
	public class SvgImageResizer : ImageResizer
	{
		public SvgImageResizer() : base ()
		{
		}

		public override void Resize(string sourceFile, string destinationFile, int sourceNominalWidth, int sourceNominalHeight, double resizeRatio)
		{
			var svg = new SKSvg();
			svg.Load(sourceFile);

			// Find the actual size of the SVG 
			var sourceActualWidth = svg.Picture.CullRect.Width;
			var sourceActualHeight = svg.Picture.CullRect.Height;

			// Figure out what the ratio to convert the actual image size to the nominal size is
			var nominalRatio = Math.Max((double)sourceNominalWidth / (double)sourceActualWidth, (double)sourceNominalHeight / (double)sourceActualHeight);

			// Multiply nominal ratio by the resize ratio to get our final ratio we actually adjust by
			var adjustRatio = nominalRatio * resizeRatio;

			// Figure out our scaled width and height to make a new canvas for
			var scaledWidth = sourceActualWidth * adjustRatio;
			var scaledHeight = sourceActualHeight * adjustRatio;

			// Make a canvas of the target size to draw the svg onto
			var bmp = new SKBitmap((int)scaledWidth, (int)scaledHeight);
			var canvas = new SKCanvas(bmp);

			// Make a matrix to scale the SVG
			var matrix = SKMatrix.MakeScale((float)adjustRatio, (float)adjustRatio);

			canvas.Clear(SKColors.Transparent);

			// Draw the svg onto the canvas with our scaled matrix
			canvas.DrawPicture(svg.Picture, ref matrix);

			// Save the op
			canvas.Save();

			// Export the canvas
			var img = SKImage.FromBitmap(bmp);
			var data = img.Encode(SKImageEncodeFormat.Png, 100);
			using (var fs = File.Open(destinationFile, FileMode.Create))
				data.SaveTo(fs);
		}
	}
}
