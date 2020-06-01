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

        internal static GeneratedFolder WithDataFakeProviders(this GeneratedFolder folder)
        {
            return folder //TODO: Consider adding csproj as well
                .WithFolder($"{folder.Name}.Data.Fake.Providers", r => r.WithFile("FakeSampleProvider.cs",
                    $@"using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attest.Fake.Builders;
using JetBrains.Annotations;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Contracts.Providers;
using {folder.Name}.Data.Fake.Containers;
using {folder.Name}.Data.Fake.ProviderBuilders;

namespace {folder.Name}.Data.Fake.Providers
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
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Contracts.Providers;
using {folder.Name}.Data.Fake.Containers;
using {folder.Name}.Data.Fake.ProviderBuilders;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folder.Name}.Data.Fake.Providers
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
}}"));
        }

        internal static GeneratedFolder WithDataRealProviders(this GeneratedFolder folder)
        {
            return folder //TODO: Consider adding csproj as well
                .WithFolder($"{folder.Name}.Data.Real.Providers", r => r.WithFile("SampleProvider.cs", $@"using System;
using System.Collections.Generic;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Contracts.Providers;

namespace {folder.Name}.Data.Real.Providers
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
using {folder.Name}.Data.Contracts.Providers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folder.Name}.Data.Real.Providers
{{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {{
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator) => dependencyRegistrator
            .RegisterAutomagically(
                Assembly.LoadFrom(AssemblyInfo.AssemblyName),
                Assembly.GetExecutingAssembly());
    }}
}}"));
        }

        internal static GeneratedFolder WithLauncher(this GeneratedFolder folder)
        {
            return folder //TODO: Consider adding csproj as well
                .WithFolder($"{folder.Name}.Data.Real.Providers", r => r.WithFile("SampleProvider.cs", $@"using System;
using System.Collections.Generic;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Contracts.Providers;

namespace {folder.Name}.Data.Real.Providers
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
using {folder.Name}.Data.Contracts.Providers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folder.Name}.Data.Real.Providers
{{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {{
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator) => dependencyRegistrator
            .RegisterAutomagically(
                Assembly.LoadFrom(AssemblyInfo.AssemblyName),
                Assembly.GetExecutingAssembly());
    }}
}}"));
        }

        internal static GeneratedFolder WithModel(this GeneratedFolder folder)
        {
            return folder.WithFolder($"{folder.Name}.Model", r => r
                .WithFolder("Mappers", s =>
                    s.WithFile("MappingProfile.cs", $@"using System;
using AutoMapper;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Model.Contracts;

namespace {folder.Name}.Model.Mappers
{{
    internal sealed class MappingProfile : Profile
    {{
        public MappingProfile()
        {{
            CreateSampleItemMaps();
        }}

        private void CreateSampleItemMaps()
        {{
            CreateDomainObjectMap<SampleItemDto, ISampleItem, SampleItem>();
        }}

        private void CreateDomainObjectMap<TDto, TContract, TModel>()
            where TModel : TContract
            where TContract : class => CreateDomainObjectMap(typeof(TDto), typeof(TContract), typeof(TModel));

        private void CreateDomainObjectMap(Type dtoType, Type contractType, Type modelType)
        {{
            CreateMap(dtoType, contractType).As(modelType);
            CreateMap(dtoType, modelType);
            CreateMap(contractType, dtoType);
            CreateMap(modelType, dtoType);
        }}
    }}
}}").WithFile("SampleMapper.cs", $@"using AutoMapper;
using JetBrains.Annotations;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Model.Contracts;

namespace {folder.Name}.Model.Mappers
{{
    [UsedImplicitly]
    internal sealed class SampleMapper
    {{
        private readonly IMapper _mapper;

        public SampleMapper(IMapper mapper) => _mapper = mapper;

        public ISampleItem MapToSampleItem(SampleItemDto sampleItemDto) => 
            _mapper.Map<ISampleItem>(sampleItemDto);

        public SampleItemDto MapToSampleItemDto(ISampleItem sampleItem) =>
            _mapper.Map<SampleItemDto>(sampleItem);

    }}
}}")).WithFolder("Validation", s => s.WithFile("NumberValidation.cs", $@"using System.ComponentModel.DataAnnotations;

namespace {folder.Name}.Model.Validation
{{
    public sealed class NumberValidation : ValidationAttribute
    {{
        public NumberValidation()
        {{
            Minimum = int.MinValue;
            Maximum = int.MaxValue;
        }}

        public int Minimum {{ get; set; }}

        public int Maximum {{ get; set; }}

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {{
            var number = (int)value;

            if (number < Minimum || number > Maximum)
            {{
                return new ValidationResult(ErrorMessage);
            }}

            return ValidationResult.Success;
        }}
    }}
}}")).WithFile("AppModel.cs", $@"using System;
using LogoFX.Client.Mvvm.Model;
using {folder.Name}.Model.Contracts;


namespace {folder.Name}.Model
{{    
    internal abstract class AppModel : EditableModel<Guid>, IAppModel
    {{        
        public bool IsNew {{ get; set; }}
    }}
}}
").WithFile("DataService.cs", $@"using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LogoFX.Client.Core;
using LogoFX.Core;
using {folder.Name}.Data.Contracts.Providers;
using {folder.Name}.Model.Contracts;
using {folder.Name}.Model.Mappers;

namespace {folder.Name}.Model
{{
    [UsedImplicitly]
    internal sealed class DataService : NotifyPropertyChangedBase<DataService>, IDataService
    {{
        private readonly ISampleProvider _sampleProvider;
        private readonly SampleMapper _sampleMapper;

        private readonly RangeObservableCollection<ISampleItem> _items =
            new RangeObservableCollection<ISampleItem>();

        public DataService(ISampleProvider sampleProvider, SampleMapper sampleMapper)
        {{
            _sampleProvider = sampleProvider;
            _sampleMapper = sampleMapper;
        }}

        IEnumerable<ISampleItem> IDataService.Items => _items;

        Task IDataService.GetItems() => MethodRunner.RunAsync(Method);

        private void Method()
        {{
            var items = _sampleProvider.GetItems().Select(_sampleMapper.MapToSampleItem);
            _items.Clear();
            _items.AddRange(items);
        }}

        Task<ISampleItem> IDataService.NewItem() => MethodRunner.RunWithResultAsync<ISampleItem>(() =>
            new SampleItem(""New Item"", 1)
            {{
                IsNew = true
            }});

        public Task SaveItem(ISampleItem item) => MethodRunner.RunAsync(() =>
        {{
            var dto = _sampleMapper.MapToSampleItemDto(item);

            if (item.IsNew)
            {{
                _sampleProvider.CreateItem(dto);
            }}
            else
            {{
                _sampleProvider.UpdateItem(dto);
            }}
        }});

        Task IDataService.DeleteItem(ISampleItem item) => MethodRunner.RunAsync(() =>
        {{
            _sampleProvider.DeleteItem(item.Id);
            _items.Remove(item);
        }});
    }}
}}").WithFile("MethodRunner.cs", $@"using System;
using System.Threading.Tasks;
using Solid.Practices.Scheduling;

namespace {folder.Name}.Model
{{
    public static class MethodRunner
    {{
        public static async Task RunAsync(Action method) => await TaskRunner.RunAsync(method);

        public static async Task<TResult> RunWithResultAsync<TResult>(Func<TResult> method) =>
            await TaskRunner.RunAsync(method);

        public static async Task RunAsync(Func<Task> method) =>
            await await TaskRunner.RunAsync(async () => await method());

        public static async Task<TResult> RunWithResultAsync<TResult>(Func<Task<TResult>> method) =>
            await await TaskRunner.RunAsync(async () => await method());
    }}
}}").WithFile("Module.cs", $@"using System.Reflection;
using AutoMapper;
using JetBrains.Annotations;
using {folder.Name}.Model.Contracts;
using {folder.Name}.Model.Mappers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folder.Name}.Model
{{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {{
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {{
            dependencyRegistrator
                .RegisterAutomagically(
                    Assembly.LoadFrom(AssemblyInfo.AssemblyName),
                    Assembly.GetExecutingAssembly());

            var config = new MapperConfiguration(cfg =>
            {{
                cfg.AddProfile(new MappingProfile());
            }});
            var mapper = config.CreateMapper();
            dependencyRegistrator
                .AddInstance(mapper)
                .AddSingleton<SampleMapper>();
        }}
    }}
}}
").WithFile("SampleItem.cs", $@"using System;
using JetBrains.Annotations;
using {folder.Name}.Model.Contracts;
using {folder.Name}.Model.Validation;

namespace {folder.Name}.Model
{{    
    [UsedImplicitly]
    internal sealed class SampleItem : AppModel, ISampleItem
    {{
        public SampleItem(string displayName, int value)
        {{
            Id = Guid.NewGuid();
            _displayName = displayName;
            _value = value;
        }}

        private string _displayName;
        public string DisplayName
        {{
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }}

        private int _value;
        [NumberValidation(Minimum = 1, ErrorMessage = ""Value must be positive."")]
        public int Value
        {{
            get => _value;
            set => SetProperty(ref _value, value);
        }}        
    }}
}}
"));
        }
    }
}