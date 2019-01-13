using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SkiaSharp;

namespace Resizetizer
{
    public class SvgImageResizer : ImageResizer
    {
        public SvgImageResizer() : base()
        {
        }

        static readonly string[] rxFillPatterns = new[] {
            @"fill\s?=\s?""(?<fill>.*?)""",
            @"style\s?=\s?""fill:(?<fill>.*?)""",
        };

        public override void Resize(string sourceFile, string destinationFile, ImageAsset asset, OutputConfig outputConfig)
        {
            int sourceNominalWidth = asset.Width;
            int sourceNominalHeight = asset.Height;
            double resizeRatio = outputConfig.Ratio;

            var fillColor = asset.FillColor;
            if (string.IsNullOrEmpty(fillColor))
                fillColor = outputConfig.FillColor;

            var paintColor = asset.PaintColor;
            if (string.IsNullOrEmpty(paintColor))
                paintColor = outputConfig.PaintColor;

            // For SVG's we can optionally change the fill color on all paths
            if (!string.IsNullOrEmpty(fillColor))
            {
                var svgText = File.ReadAllText(sourceFile);

                foreach (var rxPattern in rxFillPatterns)
                {
                    var matches = Regex.Matches(svgText, rxPattern);

                    foreach (Match match in matches)
                    {
                        var fillGroup = match.Groups?["fill"];

                        if (fillGroup != null)
                        {
                            // Replace the matched rx group with our override fill color
                            var a = svgText.Substring(0, fillGroup.Index);
                            var b = svgText.Substring(fillGroup.Index + fillGroup.Length);
                            svgText = a + outputConfig.FillColor.TrimEnd(';') + ";" + b;
                        }
                    }
                }

                // Write our changes out to a temp file so we don't alter the original
                var tempFile = Path.GetTempFileName();
                File.WriteAllText(tempFile, svgText);
                sourceFile = tempFile;
            }

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

            // if paint color is set change all pixels which are not transperent to the set color
            if (!string.IsNullOrEmpty(paintColor))
            {
                SKColor newColor = SKColor.Parse(paintColor);
                for (int x = 0; x < bmp.Width; x++)
                {
                    for (int y = 0; y < bmp.Height; y++)
                    {
                        var p = bmp.GetPixel(x, y);
                        if (p.Alpha != 0)
                        {
                            bmp.SetPixel(x, y, newColor);
                        }
                    }
                }
            }
            // Export the canvas
            var img = SKImage.FromBitmap(bmp);
            var data = img.Encode(SKImageEncodeFormat.Png, 100);
            using (var fs = File.Open(destinationFile, FileMode.Create))
            {
                data.SaveTo(fs);
            }
        }
    }
}
