namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    static class GenerationFolderExtensions
    {
        internal static GeneratedFolder WithBootstrapping(this GeneratedFolder folder)
        {
            return folder.WithFolder("Common.Bootstrapping", r => r.WithFile("Common.Bootstrapping.csproj",
                @"<Project Sdk=""Microsoft.NET.Sdk"">

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
"));
        }

        internal static GeneratedFolder WithDataFakeSetup(this GeneratedFolder folder)
        {
            //TODO: Consider adding csproj as well
            return folder.WithFolder("Common.Data.Fake.Setup", r => r.WithFile("Module.cs", @"using Attest.Fake.Core;
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
"));
        }

        internal static GeneratedFolder WithDataContractsDto(this GeneratedFolder folder)
        {
            return folder.WithFolder($"{folder.Name}.Data.Contracts.Dto", r => r.WithFile("SampleItemDto.cs",
                $@"using System;

namespace {folder.Name}.Data.Contracts.Dto
{{    
    public sealed class SampleItemDto
    {{
        public Guid Id {{ get; set; }}
        public string DisplayName {{ get; set; }}
        public int Value {{ get; set; }}
    }}
}}"));
        }

        internal static GeneratedFolder WithDataContractsProviders(this GeneratedFolder folder)
        {
            return folder //TODO: Consider adding csproj as well
                .WithFolder($"{folder.Name}.Data.Contracts.Providers", r => r.WithFile("AssemblyInfo.cs",
                    $@"using System.Reflection;

namespace {folder.Name}.Data.Contracts.Providers
{{
    public static class AssemblyInfo
    {{
        public static string AssemblyName {{ get; }} = $""{{Assembly.GetExecutingAssembly().GetName().Name}}.dll"";
    }}
}}").WithFile("ISampleProvider.cs", $@"using System;
using System.Collections.Generic;
using {folder.Name}.Data.Contracts.Dto;

namespace {folder.Name}.Data.Contracts.Providers
{{
    public interface ISampleProvider
    {{
        IEnumerable<SampleItemDto> GetItems();
        bool DeleteItem(Guid id);
        bool UpdateItem(SampleItemDto dto);
        void CreateItem(SampleItemDto dto);
    }}
}}
"));
        }

        internal static GeneratedFolder WithDataFakeContainers(this GeneratedFolder folder)
        {
            return folder //TODO: Consider adding csproj as well
                .WithFolder($"{folder.Name}.Data.Fake.Containers", r => r.WithFile("SampleContainer.cs",
                    $@"using System.Collections.Generic;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Fake.Containers.Contracts;

namespace {folder.Name}.Data.Fake.Containers
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
"));
        }

        internal static GeneratedFolder WithDataFakeContainersContracts(this GeneratedFolder folder)
        {
            return folder //TODO: Consider adding csproj as well
                .WithFolder($"{folder.Name}.Data.Fake.Containers.Contracts", r => r.WithFile("IDataContainer.cs",
                    $@"namespace {folder.Name}.Data.Fake.Containers.Contracts
{{
    public interface IDataContainer
    {{
    }}
}}"));
        }

        internal static GeneratedFolder WithDataFakeProviderBuilders(this GeneratedFolder folder)
        {
            return folder //TODO: Consider adding csproj as well
                .WithFolder($"{folder.Name}.Data.Fake.ProviderBuilders", r => r.WithFile("SampleProviderBuilder.cs",
                    $@"using System;
using System.Collections.Generic;
using System.Linq;
using Attest.Fake.Builders;
using Attest.Fake.Core;
using Attest.Fake.Setup.Contracts;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Contracts.Providers;

namespace {folder.Name}.Data.Fake.ProviderBuilders
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
}}"));
        }
    }
}