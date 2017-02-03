using System;
namespace Resizetizer
{
	public abstract class ImageResizer
	{
		public ImageResizer()
		{
		}

		public abstract void Resize(string sourceFile, string destinationFile, ImageAsset asset, OutputConfig outputConfig);
	}
}
