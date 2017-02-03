using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace Resizetizer
{
	public class Engine
	{
		public List<Config> Load(string configFilename)
		{
			var fileInfo = new FileInfo(configFilename);
			var basePath = fileInfo.Directory.FullName;

			var configs = DeserializeConfigFile(fileInfo.FullName);

			return configs;
		}

		public string GetConfigBasePath(string configFilename)
		{
			var fileInfo = new FileInfo(configFilename);
			return fileInfo.Directory.FullName;
		}

		public void Run(string configFilename)
		{
			var fileInfo = new FileInfo(configFilename);
			var basePath = fileInfo.Directory.FullName;

			var configs = DeserializeConfigFile(fileInfo.FullName);

			foreach (var c in configs)
				Run(basePath, c);
		}

		public void Run(string basePath, Config config)
		{
			if (config.AutoAddPlatformSizes)
					config.AddPlatformSizes();

			foreach (var s in config.Outputs)
			{
				var outputPath = config.GetAbsoluteOutputBasePath (basePath);

				foreach (var inputAsset in config.Assets)
				{
					var inputFileInfo = new FileInfo(inputAsset.GetAbsoluteFile (basePath));

					var outputFile = Path.Combine(outputPath, string.IsNullOrEmpty(s.Path) ? "./" : s.Path, s.GetFilename (inputFileInfo.Name));
					var outputFileInfo = new FileInfo(outputFile);

					outputFileInfo.Directory.Create();

					var resizer = ImageResizerFactory.Create(inputFileInfo.Extension);

					resizer.Resize(inputFileInfo.FullName, outputFile, inputAsset.Width, inputAsset.Height, s.Ratio);
				}
			}
		}

		public List<Config> DeserializeConfigFile(string configFile)
		{
			var fileInfo = new FileInfo(configFile);
			var isYaml = fileInfo.Extension.EndsWith("yaml", StringComparison.OrdinalIgnoreCase) || fileInfo.Extension.EndsWith("yml", StringComparison.OrdinalIgnoreCase);

			var configs = new List<Config>();

			var text = File.ReadAllText(fileInfo.FullName);

			if (isYaml)
			{
				var s = new YamlDotNet.Serialization.DeserializerBuilder ()
				                      .WithNamingConvention (new CamelCaseNamingConvention())
				                      .Build ();
				configs = s.Deserialize<List<Config>>(text);
			}
			else
			{
				configs = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Config>>(text);
			}

			return configs;
		}
	}
}
