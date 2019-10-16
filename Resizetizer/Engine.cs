using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			foreach (var outputConfig in config.Outputs)
			{
				var outputPath = config.GetAbsoluteOutputBasePath (basePath);

				foreach (var inputAsset in config.Assets)
				{
					var inputFileInfo = new FileInfo(inputAsset.GetAbsoluteFile (basePath));

					var outputFile = Path.Combine(outputPath, string.IsNullOrEmpty(outputConfig.Path) ? "./" : outputConfig.Path, outputConfig.GetFilename (inputFileInfo.Name));
					var outputFileInfo = new FileInfo(outputFile);

					outputFileInfo.Directory.Create();

					var resizer = ImageResizerFactory.Create(inputFileInfo.Extension);

					resizer.Resize(inputFileInfo.FullName, outputFile, inputAsset, outputConfig);

					// Run any external commands that were specified
					foreach (var extCmd in config.ExternalCommands)
						RunExternalCommand(config, inputAsset, outputConfig, outputFileInfo, extCmd);
					foreach (var extCmd in outputConfig.ExternalCommands)
						RunExternalCommand(config, inputAsset, outputConfig, outputFileInfo, extCmd);

					// Should we optimize?
					var doOptimize = config.Optimize;
					if (outputConfig.Optimize.HasValue)
						doOptimize = outputConfig.Optimize.Value;

					var optimizerEngine = config.Optimizer;
					if (outputConfig.Optimizer.HasValue)
						optimizerEngine = outputConfig.Optimizer.Value;

					if (doOptimize)
					{
						var optimizer = PngOptimizerFactory.Create(optimizerEngine);
						optimizer.Optimize(outputFileInfo.FullName);
					}
				}
			}
		}

		List<string> RunExternalCommand(Config config, ImageAsset asset, OutputConfig outputConfig, FileInfo outputFileInfo, string command)
		{
			var output = new List<string>();

			int width = (int)((double)asset.Width * outputConfig.Ratio);
			int height = (int)((double)asset.Width * outputConfig.Ratio);

			command = command.Replace("{outputFile}", outputFileInfo.FullName);
			command = command.Replace("{width}", width.ToString ());
			command = command.Replace("{height}", height.ToString ());

			var args = ArgumentsParser.GetArguments(command);

			if (!args?.Any() ?? true)
				return output;

			var file = args[0];
			args.RemoveAt(0);

			var argStr = string.Join(" ", args);

			var process = new System.Diagnostics.Process();

			process.StartInfo = new System.Diagnostics.ProcessStartInfo(file, argStr);
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.OutputDataReceived += (sender, e) =>
			{
				output.Add(e.Data);
			};
			process.ErrorDataReceived += (sender, e) =>
			{
				output.Add(e.Data);
			};
			process.Start();
			process.WaitForExit();

			return output;
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
