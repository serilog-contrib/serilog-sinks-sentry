#tool nuget:?package=MSBuild.SonarQube.Runner.Tool
#addin nuget:?package=Cake.Sonar

var target = Argument("target", "Default");
var projectName = Argument("project", "Serilog.Sinks.Sentry");

var buildConfiguration = "Release";
var solutionName = "Serilog.Sinks.Sentry";
var solutionFileName = string.Format("./src/{0}.sln", solutionName);
var projectFolder = string.Format("./src/{0}/", projectName);
var projectFile = string.Format("{0}{1}.csproj", projectFolder, projectName);

var extensionsVersion = XmlPeek(projectFile, "Project/PropertyGroup[1]/VersionPrefix/text()");

Task("Build")
  .Does(() =>
{
    DotNetCoreBuild(solutionFileName, new DotNetCoreBuildSettings {
		Configuration = buildConfiguration
    });
});

Task("NugetPack")
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

Task("SonarBegin")
  .Does(() => {
     SonarBegin(new SonarBeginSettings {
        Url = "https://sonarcloud.io",
        Login = EnvironmentVariable("sonar:apikey"),
        Key = "serilog-sinks-sentry",
        Name = "Serilog.Sinks.Sentry",
        ArgumentCustomization = args => args
            .Append($"/o:olsh-github"),
        Version = "1.0.0.0"
     });
  });

Task("SonarEnd")
  .Does(() => {
     SonarEnd(new SonarEndSettings {
        Login = EnvironmentVariable("sonar:apikey")
     });
  });

Task("Sonar")
  .IsDependentOn("SonarBegin")
  .IsDependentOn("Build")
  .IsDependentOn("SonarEnd");

Task("Default")
    .IsDependentOn("NugetPack");

RunTarget(target);
