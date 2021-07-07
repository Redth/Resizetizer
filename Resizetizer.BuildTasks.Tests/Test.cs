// Copyright (c) 2015-2016 Xamarin Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using NUnit.Framework;

namespace Resizetizer.BuildTasks.Tests
{
	[TestFixture]
	class Test : TestsBase
	{
		string GetBasePath()
		{
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "testdata");
		}

		public void AddCoreTargets (ProjectRootElement el)
		{
			var skiaSharpTargets = Path.Combine(
				Path.GetDirectoryName(GetType().Assembly.Location),
				"..", "..", "..", "packages", "SkiaSharp.1.56.1", "build", "net45", "SkiaSharp.targets");
			                           
			//var props = Path.Combine (
			//	Path.GetDirectoryName (GetType ().Assembly.Location),
			//	"..", "..", "..", "Resizetizer.BuildTasks", "bin", "Debug", "Resizetizer.props"
			//);
			//el.AddImport (props);
			var targets = Path.Combine (
				Path.GetDirectoryName (GetType ().Assembly.Location),
				"..", "..", "..", "Resizetizer.BuildTasks", "bin", "Debug", "Resizetizer.targets"
			);

			if (!File.Exists(targets)) {
				targets = Path.Combine(
					Path.GetDirectoryName(GetType().Assembly.Location),
					"..", "..", "..", "Resizetizer.BuildTasks", "bin", "Release", "Resizetizer.targets"
				);
			}

			el.AddProperty("ShouldIncludeNativeSkiaSharp", "True");
			//< ShouldIncludeNativeSkiaSharp > True </ ShouldIncludeNativeSkiaSharp >
			el.AddImport (targets);
			el.AddImport(skiaSharpTargets);

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
		public void Test_Simple_Resize_Task ()
		{
			var configFile = new FileInfo (Path.Combine(GetBasePath(), "output", "config.yaml"));
			configFile.Directory.Create();

			File.WriteAllText(configFile.FullName,
@"- platform: Android
  outputBasePath: ""./MyProj.Droid/Resources/""
  assets:
   - file: ""../input/happy.svg""
     size: 100x100
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
   - file: ""../input/happy.svg""
     size: 36x36
  outputs:
   - ratio: 1.0
   - fileSuffix: ""@2x""
     ratio: 2.0
   - fileSuffix: ""@3x""
     ratio: 3.0");

			var engine = new ProjectCollection ();
			var prel = ProjectRootElement.Create (Path.Combine (TempDir, "project.csproj"), engine);

			prel.AddItem("ResizetizerConfig", configFile.FullName);
			
			AddCoreTargets (prel);

			var project = new ProjectInstance (prel);
			var log = new MSBuildTestLogger ();

			var success = BuildProject (engine, project, "Resizetize", log);

			AssertNoMessagesOrWarnings (log);
			Assert.IsTrue (success);

			Assert.IsTrue(File.Exists(Path.Combine(GetBasePath(), "output", "MyProj.Droid", "Resources", "drawable-xxhdpi", "happy.png")));
			Assert.IsTrue(File.Exists(Path.Combine(GetBasePath(), "output", "MyProj.iOS", "Images", "happy@2x.png")));
		}




		[Test]
		public void Test_Cached_Resize_Task()
		{
			var configFile = new FileInfo(Path.Combine(GetBasePath(), "output", "config.yaml"));
			configFile.Directory.Create();

			File.WriteAllText(configFile.FullName,
@"- platform: Android
  outputBasePath: ""./MyProj.Droid/Resources/""
  assets:
   - file: ""../input/happy.svg""
     size: 100x100
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
   - file: ""../input/happy.svg""
     size: 36x36
  outputs:
   - ratio: 1.0
   - fileSuffix: ""@2x""
     ratio: 2.0
   - fileSuffix: ""@3x""
     ratio: 3.0");

			var engine = new ProjectCollection();
			var prel = ProjectRootElement.Create(Path.Combine(TempDir, "project.csproj"), engine);

			prel.AddItem("ResizetizerConfig", configFile.FullName);

			AddCoreTargets(prel);

			var project = new ProjectInstance(prel);
			var log = new MSBuildTestLogger();

			var success = BuildProject(engine, project, "Resizetize", log);
			AssertNoMessagesOrWarnings(log);
			Assert.IsTrue(success);

			success = BuildProject(engine, project, "Resizetize", log);
			AssertNoMessagesOrWarnings(log);
			Assert.IsTrue(success);

			Assert.IsTrue(log.Events.Any(e => e.Message.Contains("skipped")));
		}

	}
}