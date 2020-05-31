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
").WithFile("DynamicAssemblyLoader.cs", @"using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using McMaster.NETCore.Plugins;

namespace Common.Bootstrapping
{
    internal static class DynamicAssemblyLoader
    {
        internal static IEnumerable<Assembly> Get(IEnumerable<string> files)
        {
            return files.Select(r =>
                PluginLoader.CreateFromAssemblyFile(Path.Combine(Directory.GetCurrentDirectory(), r),
                        config => config.PreferSharedTypes = true)
                    .LoadDefaultAssembly()).ToArray();
        }
    }
}").WithFile("Extensions.cs", @"using Microsoft.Extensions.DependencyInjection;
using Solid.Core;
using Solid.Practices.Composition;

namespace Common.Bootstrapping
{
    public static class ServiceCollectionExtensions
    {
        public static void UseDynamicLoad(this IServiceCollection serviceCollection)
        {
            AssemblyLoader.LoadAssembliesFromPaths = DynamicAssemblyLoader.Get;
        }
    }

    public static class BootstrappingExtensions
    {
        public static void UseDynamicLoad(this IInitializable initializable)
        {
            AssemblyLoader.LoadAssembliesFromPaths = DynamicAssemblyLoader.Get;
        }
    }
}
"))
                //TODO: Consider adding csproj as well
                .WithFolder("Common.Data.Fake.Setup", r=> r.WithFile("Module.cs", @"using Attest.Fake.Core;
using Attest.Fake.Moq;
using JetBrains.Annotations;
using Solid.Practices.Modularity;

namespace Common.Data.Fake.Setup
{
    [UsedImplicitly]
    public sealed class Module : IPlainCompositionModule
    {
        public void RegisterModule()
        {
            FakeFactoryContext.Current = new FakeFactory();
            ConstraintFactoryContext.Current = new ConstraintFactory();
        }
    }
}
"))
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Data.Contracts.Dto", r => r.WithFile("SampleItemDto.cs", $@"using System;

namespace {folderName}.Data.Contracts.Dto
{{    
    public sealed class SampleItemDto
    {{
        public Guid Id {{ get; set; }}
        public string DisplayName {{ get; set; }}
        public int Value {{ get; set; }}
    }}
}}"))
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Data.Contracts.Providers", r => r.WithFile("AssemblyInfo.cs", $@"using System.Reflection;

namespace {folderName}.Data.Contracts.Providers
{{
    public static class AssemblyInfo
    {{
        public static string AssemblyName {{ get; }} = $""{{Assembly.GetExecutingAssembly().GetName().Name}}.dll"";
    }}
}}").WithFile("ISampleProvider.cs", $@"using System;
using System.Collections.Generic;
using {folderName}.Data.Contracts.Dto;

namespace {folderName}.Data.Contracts.Providers
{{
    public interface ISampleProvider
    {{
        IEnumerable<SampleItemDto> GetItems();
        bool DeleteItem(Guid id);
        bool UpdateItem(SampleItemDto dto);
        void CreateItem(SampleItemDto dto);
    }}
}}
"))
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Data.Fake.Containers", r=> r.WithFile("SampleContainer.cs", $@"using System.Collections.Generic;
using {folderName}.Data.Contracts.Dto;
using {folderName}.Data.Fake.Containers.Contracts;

namespace {folderName}.Data.Fake.Containers
{{
    public interface ISampleContainer : IDataContainer
    {{
        IEnumerable<SampleItemDto> Items {{ get; }}
    }}

    public sealed class SampleContainer : ISampleContainer
    {{
        private readonly List<SampleItemDto> _items = new List<SampleItemDto>();
        public IEnumerable<SampleItemDto> Items => _items;

        public void UpdateItems(IEnumerable<SampleItemDto> items)
        {{
            _items.Clear();
            _items.AddRange(items);
        }}
    }}   
}}
"))
                .WithFolder($"{folderName}.Data.Fake.Containers.Contracts", r=> r.WithFile("IDataContainer.cs", $@"namespace {folderName}.Data.Fake.Containers.Contracts
{{
    public interface IDataContainer
    {{
    }}
}}"))
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
