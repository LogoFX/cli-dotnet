using System.Diagnostics.CodeAnalysis;
using System.IO;
using Common.Infra;
using FluentAssertions;
using JetBrains.Annotations;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;
using LogoFX.Cli.Dotnet.Specs.Tests.Infra;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class SolutionGenerationSteps
    {
        private readonly IProcessManagementService _processManagementService;

        public SolutionGenerationSteps(
            IProcessManagementService processManagementService)
        {
            _processManagementService = processManagementService;
        }

        [Then(@"The folder '(.*)' contains working LogoFX template-based solution")]
        public void ThenTheFolderContainsWorkingLogoFXTemplate_BasedSolution(string folderName)
        {
            var tempPath = Path.GetTempPath();
            var path = Path.Combine(tempPath, folderName);
            var configs = new[] {"DebugWithFake", "Release"};

            foreach (var config in configs)
            {
                var execInfo =
                    _processManagementService.Start(Path.Combine(path, "dotnet"), $"build -c {config}", Consts.ProcessExecutionTimeout);
                execInfo.ShouldBeSuccessful();
                execInfo.OutputStrings.Should().ContainMatch("Build succeeded.");
            }

            var generatedFolder = new GeneratedFolder(tempPath, folderName)
                .WithFolder("Common.Bootstrapping", r => r.WithFile("Common.Bootstrapping.csproj", @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.0</TargetFramework>
    <AppendTargetFrameworkToOutputPath>false</AppendTargetFrameworkToOutputPath>
    <Configurations>Debug;Release</Configurations>
  </PropertyGroup>
    
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Debug|AnyCPU'"">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>..\bin\Debug</OutputPath>
    <DefineConstants />
    <WarningLevel>4</WarningLevel>
    <IntermediateOutputPath>obj\Debug</IntermediateOutputPath>
    <NoWarn />
    <NoStdLib>false</NoStdLib>
  </PropertyGroup>
  <PropertyGroup Condition=""'$(Configuration)|$(Platform)'=='Release|AnyCPU'"">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>..\bin\Release</OutputPath>
    <DefineConstants>NETCOREAPP;NETCOREAPP2_0</DefineConstants>
    <WarningLevel>4</WarningLevel>
    <NoWarn />
    <NoStdLib>false</NoStdLib>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include=""McMaster.NETCore.Plugins"" Version=""1.2.0"" />
    <PackageReference Include=""Microsoft.Extensions.DependencyInjection.Abstractions"" Version=""3.1.3"" />
    <PackageReference Include=""Solid.Practices.Composition.Core"" Version=""2.2.0"" />
  </ItemGroup>

</Project>
"))
                .WithFolder("Common.Data.Fake.Setup")
                .WithFolder($"{folderName}.Data.Contracts.Dto")
                .WithFolder($"{folderName}.Data.Contracts.Providers")
                .WithFolder($"{folderName}.Data.Fake.Containers")
                .WithFolder($"{folderName}.Data.Fake.ProviderBuilders")
                .WithFolder($"{folderName}.Data.Fake.Providers")
                .WithFolder($"{folderName}.Data.Real.Providers")
                .WithFolder($"{folderName}.Launcher")
                .WithFolder($"{folderName}.Model")
                .WithFolder($"{folderName}.Model.Contracts")
                .WithFolder($"{folderName}.Presentation")
                .WithFolder($"{folderName}.Presentation.Contracts")
                .WithFile($"{folderName}.sln", AssertionHelper.Any);
            generatedFolder.AssertGeneratedCode();
        }
    }

    [UsedImplicitly]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    class TemplateAssertionData
    {
        public string Description { get; set; }
        public string ShortName { get; set; }
        public string Languages { get; set; }
        public string Tags { get; set; }
    }
}
