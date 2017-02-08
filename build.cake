#tool nuget:?package=NUnit.ConsoleRunner&version=3.6.0

var sln = "./Resizetizer.sln";
var nuspec = "./Resizetizer.nuspec";

var target = Argument ("target", "all");
var configuration = Argument ("configuration", "Release");

var NUGET_VERSION = EnvironmentVariable("APPVEYOR_BUILD_VERSION") ?? Argument("nugetversion", "0.9999");

Task ("libs").Does (() => 
{
	NuGetRestore (sln);

	DotNetBuild (sln, c => c.Configuration = configuration);
});

Task ("nuget").IsDependentOn ("libs").Does (() => 
{
	DeleteFiles ("./**/.DS_Store");
	
	NuGetPack (nuspec, new NuGetPackSettings { 
		Verbosity = NuGetVerbosity.Detailed,
		OutputDirectory = "./",
		Version = NUGET_VERSION,
		// NuGet messes up path on mac, so let's add ./ in front again
		BasePath = "././",
	});	
});

Task("tests").IsDependentOn("libs").Does(() =>
{
	NUnit3("./**/bin/" + configuration + "/*.Tests.dll");
});

Task ("clean").Does (() => 
{
	CleanDirectories ("./**/bin");
	CleanDirectories ("./**/obj");

	CleanDirectories ("./**/Components");
	CleanDirectories ("./**/tools");

	CleanDirectories ("./android-sdk");
	
	DeleteFiles ("./**/*.apk");
});

Task ("all").IsDependentOn("nuget").IsDependentOn ("tests");

RunTarget (target);