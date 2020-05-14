﻿using System.Diagnostics.CodeAnalysis;
using System.IO;
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
                    _processManagementService.Start(Path.Combine(path, "dotnet"), $"build -c {config}", 30000);
                execInfo.ShouldBeSuccessful();
                execInfo.OutputStrings.Should().ContainMatch("Build succeeded.");
            }

            var projectFolders = new[]
            {
                "Common.Bootstrapping",
                "Common.Data.Fake.Setup",
                $"{folderName}.Data.Contracts.Dto",
                $"{folderName}.Data.Contracts.Providers",
                $"{folderName}.Data.Fake.Containers",
                $"{folderName}.Data.Fake.Containers.Contracts",
                $"{folderName}.Data.Fake.ProviderBuilders",
                $"{folderName}.Data.Fake.Providers",
                $"{folderName}.Data.Real.Providers",
                $"{folderName}.Launcher",
                $"{folderName}.Model",
                $"{folderName}.Model.Contracts",
                $"{folderName}.Presentation",
                $"{folderName}.Presentation.Contracts"
            };
            path.AssertSubFolders(projectFolders);
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
