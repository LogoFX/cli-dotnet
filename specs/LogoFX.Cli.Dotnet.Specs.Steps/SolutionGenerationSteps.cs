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
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Data.Fake.Containers.Contracts", r=> r.WithFile("IDataContainer.cs", $@"namespace {folderName}.Data.Fake.Containers.Contracts
{{
    public interface IDataContainer
    {{
    }}
}}"))
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Data.Fake.ProviderBuilders", r => r.WithFile("SampleProviderBuilder.cs", $@"using System;
using System.Collections.Generic;
using System.Linq;
using Attest.Fake.Builders;
using Attest.Fake.Core;
using Attest.Fake.Setup.Contracts;
using {folderName}.Data.Contracts.Dto;
using {folderName}.Data.Contracts.Providers;

namespace {folderName}.Data.Fake.ProviderBuilders
{{
    public sealed class SampleProviderBuilder : FakeBuilderBase<ISampleProvider>.WithInitialSetup
    {{
        private readonly List<SampleItemDto> _itemsStorage = new List<SampleItemDto>();

        private SampleProviderBuilder()
        {{

        }}

        public static SampleProviderBuilder CreateBuilder() => new SampleProviderBuilder();

        public void WithItems(IEnumerable<SampleItemDto> items)
        {{
            _itemsStorage.Clear();
            _itemsStorage.AddRange(items);
        }}

        protected override IServiceCall<ISampleProvider> CreateServiceCall(
            IHaveNoMethods<ISampleProvider> serviceCallTemplate) => serviceCallTemplate
            .AddMethodCallWithResult(t => t.GetItems(),
                r => r.Complete(GetItems))
            .AddMethodCallWithResult<Guid, bool>(t => t.DeleteItem(It.IsAny<Guid>()),
                (r, id) => r.Complete(DeleteItem(id)))
            .AddMethodCallWithResult<SampleItemDto, bool>(t => t.UpdateItem(It.IsAny<SampleItemDto>()),
                (r, dto) => r.Complete(k =>
                {{
                    SaveItem(k);
                    return true;
                }}))
            .AddMethodCall<SampleItemDto>(t => t.CreateItem(It.IsAny<SampleItemDto>()),
                (r, dto) => r.Complete(SaveItem));

        private IEnumerable<SampleItemDto> GetItems() => _itemsStorage;

        private bool DeleteItem(Guid id)
        {{
            var dto = _itemsStorage.SingleOrDefault(x => x.Id == id);
            return dto != null && _itemsStorage.Remove(dto);
        }}

        private void SaveItem(SampleItemDto dto)
        {{
            var oldDto = _itemsStorage.SingleOrDefault(x => x.Id == dto.Id);
            if (oldDto == null)
            {{
                _itemsStorage.Add(dto);
                return;
            }}

            oldDto.DisplayName = dto.DisplayName;
            oldDto.Value = dto.Value;
        }}
    }}
}}"))
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Data.Fake.Providers", r => r.WithFile("FakeSampleProvider.cs", $@"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attest.Fake.Builders;
using JetBrains.Annotations;
using {folderName}.Data.Contracts.Dto;
using {folderName}.Data.Contracts.Providers;
using {folderName}.Data.Fake.Containers;
using {folderName}.Data.Fake.ProviderBuilders;

namespace {folderName}.Data.Fake.Providers
{{
    [UsedImplicitly]
    internal sealed class FakeSampleProvider : FakeProviderBase<SampleProviderBuilder, ISampleProvider>, ISampleProvider
    {{
        private readonly Random _random = new Random();

        public FakeSampleProvider(
            SampleProviderBuilder sampleProviderBuilder,
            ISampleContainer sampleContainer)
            : base(sampleProviderBuilder)
        {{
            sampleProviderBuilder.WithItems(sampleContainer.Items);
        }}

        IEnumerable<SampleItemDto> ISampleProvider.GetItems() => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).GetItems();

        bool ISampleProvider.DeleteItem(Guid id) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).DeleteItem(id);

        bool ISampleProvider.UpdateItem(SampleItemDto dto) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).UpdateItem(dto);

        void ISampleProvider.CreateItem(SampleItemDto dto) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).CreateItem(dto);
    }}
}}").WithFile("Module.cs", $@"using System;
using JetBrains.Annotations;
using {folderName}.Data.Contracts.Dto;
using {folderName}.Data.Contracts.Providers;
using {folderName}.Data.Fake.Containers;
using {folderName}.Data.Fake.ProviderBuilders;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folderName}.Data.Fake.Providers
{{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {{
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {{
            dependencyRegistrator
                .AddInstance(InitializeSampleContainer())
                .AddSingleton<ISampleProvider, FakeSampleProvider>();

            dependencyRegistrator.RegisterInstance(SampleProviderBuilder.CreateBuilder());
        }}

        private static ISampleContainer InitializeSampleContainer()
        {{
            var sampleContainer = new SampleContainer();
            sampleContainer.UpdateItems(new[]
            {{
                new SampleItemDto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""PC"",
                    Value = 8
                }},

                new SampleItemDto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Acme"",
                    Value = 10
                }},

                new SampleItemDto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Bacme"",
                    Value = 3
                }},

                new SampleItemDto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Exceed"",
                    Value = 100
                }},

                new SampleItemDto
                {{
                    Id = Guid.NewGuid(),
                    DisplayName = ""Acme2"",
                    Value = 10
                }}
            }});
            return sampleContainer;
        }}
    }}
}}"))
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Data.Real.Providers", r => r.WithFile("SampleProvider.cs", $@"using System;
using System.Collections.Generic;
using {folderName}.Data.Contracts.Dto;
using {folderName}.Data.Contracts.Providers;

namespace {folderName}.Data.Real.Providers
{{
    internal sealed class SampleProvider : ISampleProvider
    {{
        public IEnumerable<SampleItemDto> GetItems()
        {{
            throw new NotImplementedException();
        }}

        public bool DeleteItem(Guid id)
        {{
            throw new NotImplementedException();
        }}

        public bool UpdateItem(SampleItemDto dto)
        {{
            throw new NotImplementedException();
        }}

        public void CreateItem(SampleItemDto dto)
        {{
            throw new NotImplementedException();
        }}
    }}
}}").WithFile("Module.cs", $@"using System.Reflection;
using JetBrains.Annotations;
using {folderName}.Data.Contracts.Providers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folderName}.Data.Real.Providers
{{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {{
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator) => dependencyRegistrator
            .RegisterAutomagically(
                Assembly.LoadFrom(AssemblyInfo.AssemblyName),
                Assembly.GetExecutingAssembly());
    }}
}}"))
                //TODO: Consider adding csproj as well
                .WithFolder($"{folderName}.Launcher", r =>r.WithFile("App.xaml", $@"<Application x:Class=""{folderName}.Launcher.App""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />
").WithFile("App.xaml.cs", $@"using Common.Bootstrapping;
using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Mvvm.Commanding;
using Solid.Core;

namespace {folderName}.Launcher
{{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {{
        public App()
        {{
            var bootstrapper = new AppBootstrapper();
            bootstrapper.UseDynamicLoad();
            bootstrapper
                .UseResolver()
                .UseCommanding()
                .UseShared();
            
            ((IInitializable)bootstrapper).Initialize();
        }}
    }}
}}
").WithFile("AppBootstrapper.cs", $@"using System;
using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Bootstrapping.Adapters.SimpleContainer;
using {folderName}.Presentation.Contracts;
using Solid.Practices.Composition;

namespace {folderName}.Launcher
{{
    public sealed class AppBootstrapper : BootstrapperContainerBase<ExtendedSimpleContainerAdapter>
        .WithRootObjectAsContract<IShellViewModel>
    {{
        private static readonly ExtendedSimpleContainerAdapter _container = new ExtendedSimpleContainerAdapter();

        public AppBootstrapper()
            : base(_container)
        {{
        }}

        public override CompositionOptions CompositionOptions => new CompositionOptions
        {{
            Prefixes = new[] {{
                ""Common.Data"",
                ""{folderName}.Data"",
                ""{folderName}.Model"",
                ""{folderName}.Presentation"",
            }}
        }};

        protected override void OnExit(object sender, EventArgs e)
        {{
            base.OnExit(sender, e);
            _container?.Dispose();
        }}
    }}
}}").WithFile("AssemblyLoader.cs", $@"using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using McMaster.NETCore.Plugins;

namespace {folderName}.Launcher
{{
    public static class AssemblyLoader
    {{
        public static IEnumerable<Assembly> Get(IEnumerable<string> paths)
        {{
            return paths.Select(path =>
                PluginLoader.CreateFromAssemblyFile(assemblyFile:
                    Path.Combine(Directory.GetCurrentDirectory(), path),
                    t => t.PreferSharedTypes = true
                ).LoadDefaultAssembly());
        }}
    }}
}}").WithFile("BootstrapperExtensions.cs", $@"using LogoFX.Client.Mvvm.ViewModel.Services;
using LogoFX.Client.Mvvm.ViewModelFactory.SimpleContainer;
using Solid.Bootstrapping;
using Solid.Core;
using Solid.Extensibility;
using Solid.Practices.Composition.Contracts;

namespace {folderName}.Launcher
{{
    public static class BootstrapperExtensions
    {{
        public static IInitializable UseShared<TBootstrapper>(
            this TBootstrapper bootstrapper)
            where TBootstrapper : class, IExtensible<TBootstrapper>, IHaveRegistrator, ICompositionModulesProvider, IInitializable =>
            bootstrapper
                .UseViewModelCreatorService()
                .UseViewModelFactory();
    }}
}}"))
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
