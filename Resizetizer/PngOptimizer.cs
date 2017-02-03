using System;
namespace Resizetizer
{
	public interface IPngOptimizer
	{
		void Optimize(string pngFilename);
	}

	public enum PngOptimizerEngine
	{
		ImageSharp
	}

	public static class PngOptimizerFactory
	{
		static ImageSharpPngOptimizer imageSharp;

		public static PngOptimizer Create(PngOptimizerEngine engine)
		{
			if (engine == PngOptimizerEngine.ImageSharp)
			{
				if (imageSharp == null)
					imageSharp = new ImageSharpPngOptimizer();
				return imageSharp;
			}

			return null;
		}
	}

	public abstract class PngOptimizer : IPngOptimizer
	{
		public abstract void Optimize(string pngFilename);
	}

	public class ImageSharpPngOptimizer : PngOptimizer
	{
		public override void Optimize(string pngFilename)
		{
			// TODO: Actually optimize ;)
		}
	}
}
