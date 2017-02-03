using NUnit.Framework;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Resizetizer.Tests
{
	[TestFixture]
	public class Test
	{
		string GetBasePath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "testdata");
		}

		[OneTimeSetUp]
		public void Setup()
		{
			var outputPath = Path.Combine(GetBasePath(), "output");

			if (Directory.Exists(outputPath))
				Directory.Delete(outputPath, true);

			Directory.CreateDirectory(outputPath);
		}

		[Test]
		public void Test_Simple_PNG_Resize()
		{
			var basePath = GetBasePath();

			var r = new Resizetizer.Engine();

			r.Run(basePath, new Config {
				Assets = new List<ImageAsset> {
					new ImageAsset {
						File = "./input/happy.png",
						Size = "100x100"
					},
				},
				OutputBasePath = "./output/",
				AutoAddPlatformSizes = false,
				Outputs = new List<OutputConfig> {
					new OutputConfig {
						Ratio = 1.0
					},
					new OutputConfig {
						Ratio = 2.0,
						FileSuffix = "x2"
					},
					new OutputConfig {
						Ratio = 3.0,
						FileSuffix = "x3"
					},
				}
			});

			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happy.png")));
			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happyx2.png")));
			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happyx3.png")));
		}

		[Test()]
		public void Test_Simple_SVG_Resize()
		{
			var basePath = GetBasePath();

			var r = new Engine();

			r.Run(basePath, new Config {
				Assets = new List<ImageAsset> {
					new ImageAsset {
						File = "./input/happy.svg",
						Size = "100x100"
					},
				},
				OutputBasePath = "./output/",
				AutoAddPlatformSizes = false,
				Outputs = new List<OutputConfig> {
					new OutputConfig {
						Ratio = 1.0
					},
					new OutputConfig {
						Ratio = 2.0,
						FileSuffix = "x2"
					},
					new OutputConfig {
						Ratio = 3.0,
						FileSuffix = "x3"
					},
				}
			});

			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happy.png")));
			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happyx2.png")));
			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happyx3.png")));
		}

		[Test]
		public void Test_YAML_Config()
		{
			var file = Path.Combine(GetBasePath(), "./output/", "config.yaml");

			File.WriteAllText(file,
@"- platform: Android
  outputBasePath: ""./MyProj.Droid/Resources/""
  assets:
   - file: ""./assets/cog.svg""
     size: 48x48
  outputs:
   - path: ""./drawable-mdpi""
     ratio: 1.0
   - path: ""./drawable-hdpi""
     ratio: 1.5
   - path: ""./drawable-xhdpi""
     ratio: 2.0
   - path: ""./drawable-xxhdpi""
     ratio: 3.0
   - path: ""./drawable-xxxhdpi""
     ratio: 4.0
- platform: iOS
  outputBasePath: ""./MyProj.iOS/Images/""
  assets:
   - file: ""./assets/cog.svg""
     size: 36x36
  outputs:
   - ratio: 1.0
   - fileSuffix: ""@2x""
     ratio: 2.0
   - fileSuffix: ""@3x""
     ratio: 3.0");

			var r = new Engine();
			var configs = r.DeserializeConfigFile(file);

			Assert.IsNotNull(configs);
			Assert.IsTrue(configs.Any(
				c => c.Assets.Any(a => a.File == "./assets/cog.svg") 
				&& c.Outputs.Any(o => o.Path == "./drawable-mdpi")));
		}

		[Test]
		public void Test_JSON_Config()
		{
			var file = Path.Combine(GetBasePath(), "./output/", "config.json");

			File.WriteAllText(file,
@"[{
	""platform"" : ""android"",
	""outputBasePath"" : ""./MyProj.Droid/Resources/"",

	""assets"" : [
		{
			""file"" : ""./assets/cog.svg"",
			""nominalSize"" : ""48x48""
		}
	],

	""outputs"" : [
		{ ""path"" : ""./drawable-mdpi"", ""ratio"" : 1.0, },
		{ ""path"" : ""./drawable-hdpi"", ""ratio"" : 1.5, },
		{ ""path"" : ""./drawable-xhdpi"", ""ratio"" : 2.0, },
		{ ""path"" : ""./drawable-xxhdpi"", ""ratio"" : 3.0, },
		{ ""path"" : ""./drawable-xxxhdpi"", ""ratio"" : 4.0, },
	]
},
{
	""platform"" : ""ios"",
	""outputBasePath"" : ""./MyProj.iOS/Images/"",

	""assets"" : [
		{
			""file"" : ""./assets/cog.svg"",
			""nominalSize"" : ""36x36""
		}
	],

	""outputs"" : [
		{ ""ratio"" : 1.0, },
		{ ""fileSuffix"" : ""@2x"", ""ratio"" : 2.0, },
		{ ""fileSuffix"" : ""@3x"", ""ratio"" : 3.0, },
	]
}]");
			var r = new Engine();
			var configs = r.DeserializeConfigFile(file);

			Assert.IsNotNull(configs);
			Assert.IsTrue(configs.Any(
				c => c.Assets.Any(a => a.File == "./assets/cog.svg")
				&& c.Outputs.Any(o => o.Path == "./drawable-mdpi")));
		}

		[Test]
		public void Test_SVG_FillColor ()
		{
			var basePath = GetBasePath();

			var r = new Resizetizer.Engine();

			r.Run(basePath, new Config
			{
				Assets = new List<ImageAsset> {
					new ImageAsset {
						File = "./input/happy.svg",
						Size = "100x100"
					},
				},
				OutputBasePath = "./output/",
				AutoAddPlatformSizes = false,
				Outputs = new List<OutputConfig> {
					new OutputConfig {
						Ratio = 1.0,
						FillColor = "#000000",
					},
					new OutputConfig {
						Ratio = 2.0,
						FileSuffix = "x2",
						FillColor = "#000000",
					},
					new OutputConfig {
						Ratio = 3.0,
						FileSuffix = "x3",
						FillColor = "#000000",
					},
				}
			});

			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happy.png")));
			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happyx2.png")));
			Assert.IsTrue(File.Exists(Path.Combine(basePath, "output", "happyx3.png")));
		}


		[Test]
		public void Test_ExternalCommand ()
		{
			var basePath = GetBasePath();

			var r = new Resizetizer.Engine();

			var config = new Config
			{
				Assets = new List<ImageAsset> {
					new ImageAsset {
						File = "./input/happy.svg",
						Size = "100x100"
					},
				},
				OutputBasePath = "./output/",
				AutoAddPlatformSizes = false,
				Outputs = new List<OutputConfig> {
					new OutputConfig {
						Ratio = 1.0,
					},
					new OutputConfig {
						Ratio = 2.0,
						FileSuffix = "x2",
					},
					new OutputConfig {
						Ratio = 3.0,
						FileSuffix = "x3",
					},
				}
			};

			r.Run(basePath, config);

			var happyFile = new FileInfo(Path.Combine(basePath, "output", "happy.png"));
			var happyx2File = new FileInfo(Path.Combine(basePath, "output", "happyx2.png"));
			var happyx3File = new FileInfo(Path.Combine(basePath, "output", "happyx3.png"));

			Assert.IsTrue(happyFile.Exists);
			Assert.IsTrue(happyx2File.Exists);
			Assert.IsTrue(happyx3File.Exists);

			var happySize = happyFile.Length;
			var happyx2Size = happyx2File.Length;
			var happyx3Size = happyx3File.Length;

			config.ExternalCommands.Add("/usr/local/bin/optipng -o7 \"{outputFile}\"");
			r.Run(basePath, config);

			Assert.IsTrue(happyFile.Exists);
			Assert.IsTrue(happyx2File.Exists);
			Assert.IsTrue(happyx3File.Exists);

			var happySize2 = happyFile.Length;
			var happyx2Size2 = happyx2File.Length;
			var happyx3Size2 = happyx3File.Length;

			Assert.LessOrEqual(happySize2, happySize);
			Assert.LessOrEqual(happyx2Size2, happyx2Size);
			Assert.LessOrEqual(happyx3Size2, happyx3Size);
		}
	}
}
