var target = Argument("target", "Default");
var extensionsVersion = Argument("version", "1.0.0");

var buildConfiguration = "Release";
var projectName = "Serilog.Sinks.Sentry";
var solutionFileName = string.Format("./src/{0}.sln", projectName);
var projectFolder = string.Format("./src/{0}/", projectName);

Task("UpdateBuildVersion")
  .WithCriteria(BuildSystem.AppVeyor.IsRunningOnAppVeyor)
  .Does(() =>
{
    var buildNumber = BuildSystem.AppVeyor.Environment.Build.Number;

    BuildSystem.AppVeyor.UpdateBuildVersion(string.Format("{0}.{1}", extensionsVersion, buildNumber));
});

Task("NugetRestore")
  .Does(() =>
{
    DotNetCoreRestore(solutionFileName);
});

Task("UpdateAssemblyVersion")
  .Does(() =>
{
    var assemblyFile = string.Format("{0}/Properties/AssemblyInfo.cs", projectFolder);

    AssemblyInfoSettings assemblySettings = new AssemblyInfoSettings();
    assemblySettings.Title = projectName;
    assemblySettings.FileVersion = extensionsVersion;
    assemblySettings.Version = extensionsVersion;

    CreateAssemblyInfo(assemblyFile, assemblySettings);
});

Task("Build")
  .IsDependentOn("NugetRestore")
  .IsDependentOn("UpdateAssemblyVersion")
  .Does(() =>
{
    DotNetBuild(solutionFileName, 
    settings => settings
        .SetConfiguration(buildConfiguration)
        .SetVerbosity(Verbosity.Minimal));
});

Task("NugetPack")
  .IsDependentOn("Build")
  .Does(() =>
{
     var settings = new DotNetCorePackSettings
     {
         Configuration = buildConfiguration,
         OutputDirectory = "."
     };

     DotNetCorePack(projectFolder, settings);
});

Task("CreateArtifact")
  .IsDependentOn("NugetPack")
  .WithCriteria(BuildSystem.AppVeyor.IsRunningOnAppVeyor)
  .Does(() =>
{
    BuildSystem.AppVeyor.UploadArtifact(string.Format("{0}.{1}.nupkg", projectName, extensionsVersion));
});

Task("Default")
    .IsDependentOn("NugetPack");

Task("CI")
    .IsDependentOn("UpdateBuildVersion")
    .IsDependentOn("CreateArtifact");

RunTarget(target);
