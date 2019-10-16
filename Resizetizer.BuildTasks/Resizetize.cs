using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Framework;

namespace Resizetizer.BuildTasks
{
	public class Resizetize : AsyncTask
	{
		[Required]
		public ITaskItem[] ConfigFiles { get; set; }

		[Required]
		public string IntermediateOutputPath { get; set; }

		public override bool Execute()
		{
			Log.LogMessage("Starting Resizetizing...");

			if (ConfigFiles == null || !ConfigFiles.Any())
			{
				Log.LogMessage("No ResizetizerConfig Build Action items specified, skipping task.");
				return true;
			}

			Task.Run(() =>
			{
				try
				{
					var resizer = new Resizetizer.Engine();
					foreach (var cf in ConfigFiles)
					{
						var file = CleanPath(cf.ItemSpec);
						var configFileInfo = new FileInfo(file);

						if (!configFileInfo.Exists)
						{
							LogMessage("File does not exist, skipping: {0}", file);
							continue;
						}


						LogMessage("Resizetizing: {0}", file);

						var configBasePath = resizer.GetConfigBasePath(file);
						var configs = resizer.Load(file);

						var intermediatePath = IntermediateOutputPath ?? configBasePath;

						var stampsFile = Path.Combine(intermediatePath, "resizetizer.stamps");

						var stamps = LoadStamps(stampsFile);

						foreach (var config in configs)
						{
							var absoluteOutputBasePath = config.GetAbsoluteOutputBasePath(configBasePath);

							foreach (var asset in config.Assets)
							{
								var assetFileInfo = new FileInfo(asset.GetAbsoluteFile(absoluteOutputBasePath));

								foreach (var output in config.Outputs.ToArray())
								{
									var outputFileInfo = new FileInfo(output.GetAbsoluteFilename(absoluteOutputBasePath, asset.File));

									if (stamps.ContainsKey(outputFileInfo.FullName)
										&& assetFileInfo.LastWriteTimeUtc <= stamps[outputFileInfo.FullName]
										&& configFileInfo.LastWriteTimeUtc <= stamps[outputFileInfo.FullName])
									{
										LogMessage("Source unchanged, skipped resizing: {0}", outputFileInfo.FullName);
										config.Outputs.Remove(output);
									}
									else
									{
										LogMessage("Resizing: {0}", outputFileInfo.FullName);
									}
								}
							}

							// Run the resizing operation on any remaining assets
							resizer.Run(configBasePath, config);

							// For any asset that is left and was processed
							foreach (var asset in config.Assets)
							{
								var assetFileInfo = new FileInfo(asset.GetAbsoluteFile(absoluteOutputBasePath));

								var newestModifyTime = assetFileInfo.LastWriteTimeUtc;
								if (configFileInfo.LastWriteTimeUtc > assetFileInfo.LastWriteTimeUtc)
									newestModifyTime = configFileInfo.LastWriteTimeUtc;
								
								foreach (var output in config.Outputs.ToArray())
								{
									var outputFileInfo = new FileInfo(output.GetAbsoluteFilename(absoluteOutputBasePath, asset.File));

									if (stamps.ContainsKey(outputFileInfo.FullName))
										stamps[outputFileInfo.FullName] = newestModifyTime;
									else
										stamps.Add(outputFileInfo.FullName, newestModifyTime);
								}
							}
						}

						SaveStamps(stampsFile, stamps);
					}
				}
				catch (Exception ex)
				{
					LogErrorFromException(ex);
				}
				finally
				{
					Complete();
				}
			});

			var result = base.Execute();

			Log.LogMessage("Finished Resizetizing.");

			return result && !Log.HasLoggedErrors;
		}


		Dictionary<string, DateTime> LoadStamps(string stampsFile)
		{
			if (!File.Exists(stampsFile))
				return new Dictionary<string, DateTime>();
			try
			{
				return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(stampsFile));
			}
			catch
			{
				return new Dictionary<string, DateTime>();
			}
		}

		void SaveStamps(string stampsFile, Dictionary<string, DateTime> stamps)
		{
			try
			{
				File.WriteAllText (stampsFile, Newtonsoft.Json.JsonConvert.SerializeObject (stamps));
			}
			catch
			{
			}
		}

		static string CleanPath(params string[] paths)
		{
			var combined = Path.Combine(paths);
			return combined.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
		}
	}
}
