﻿namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    //TODO: Change …DataProvider by …Provider, …DataContainer by Container

    static class GenerationFolderExtensions
    {
        internal static GeneratedFolder WithBootstrapping(this GeneratedFolder folder)
        {
            return folder.WithFolder("Common.Bootstrapping", r => r.WithFile("Common.Bootstrapping.csproj",
                @"<Project Sdk=""Microsoft.NET.Sdk"">

  <PropertyGroup>
    <TargetFramework>netcoreapp3.1</TargetFramework>
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
            return folder.WithFolder("Common.Data.Fake.Setup",
                r =>
                    r.WithFile("Common.Data.Fake.Setup.csproj", AssertionHelper.Any)
                        .WithFile("Module.cs", @"using Attest.Fake.Core;
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
            return folder.WithFolder($"{folder.Name}.Data.Contracts.Dto", r =>
                r.WithFile($"{folder.Name}.Data.Contracts.Dto.csproj", AssertionHelper.Any)
                    .WithFile("SampleItemDto.cs",
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
            return folder
                .WithFolder($"{folder.Name}.Data.Contracts.Providers", r =>
                    r.WithFile($"{folder.Name}.Data.Contracts.Providers.csproj", AssertionHelper.Any)
                        .WithFile("AssemblyInfo.cs",
                            $@"using System.Reflection;

namespace {folder.Name}.Data.Contracts.Providers
{{
    public static class AssemblyInfo
    {{
        public static string AssemblyName {{ get; }} = $""{{Assembly.GetExecutingAssembly().GetName().Name}}.dll"";
    }}
}}").WithFile("ISampleDataProvider.cs", $@"using System;
using System.Collections.Generic;
using {folder.Name}.Data.Contracts.Dto;

namespace {folder.Name}.Data.Contracts.Providers
{{
    public interface ISampleDataProvider
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
            return folder
                .WithFolder($"{folder.Name}.Data.Fake.Containers", r =>
                    r.WithFile($"{folder.Name}.Data.Fake.Containers.csproj", AssertionHelper.Any)
                        .WithFile("SampleDataContainer.cs",
                            $@"using System.Collections.Generic;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Fake.Containers.Contracts;

namespace {folder.Name}.Data.Fake.Containers
{{
    public interface ISampleDataContainer : IDataContainer
    {{
        IEnumerable<SampleItemDto> Items {{ get; }}
    }}

    public sealed class SampleDataContainer : ISampleDataContainer
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
            return folder
                .WithFolder($"{folder.Name}.Data.Fake.Containers.Contracts", r =>
                    r.WithFile($"{folder.Name}.Data.Fake.Containers.Contracts.csproj", AssertionHelper.Any)
                        .WithFile("IDataContainer.cs",
                            $@"namespace {folder.Name}.Data.Fake.Containers.Contracts
{{
    public interface IDataContainer
    {{
    }}
}}"));
        }

        internal static GeneratedFolder WithDataFakeProviderBuilders(this GeneratedFolder folder)
        {
            return folder
                .WithFolder($"{folder.Name}.Data.Fake.ProviderBuilders", r =>
                    r.WithFile($"{folder.Name}.Data.Fake.ProviderBuilders.csproj", AssertionHelper.Any)
                        .WithFile("SampleProviderBuilder.cs",
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
    public sealed class SampleProviderBuilder : FakeBuilderBase<ISampleDataProvider>.WithInitialSetup
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

        protected override IServiceCall<ISampleDataProvider> CreateServiceCall(
            IHaveNoMethods<ISampleDataProvider> serviceCallTemplate) => serviceCallTemplate
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
            return folder
                .WithFolder($"{folder.Name}.Data.Fake.Providers", r =>
                    r.WithFile($"{folder.Name}.Data.Fake.Providers.csproj", AssertionHelper.Any)
                        .WithFile("FakeSampleDataProvider.cs",
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
    internal sealed class FakeSampleDataProvider : FakeProviderBase<SampleProviderBuilder, ISampleDataProvider>, ISampleDataProvider
    {{
        private readonly Random _random = new Random();

        public FakeSampleDataProvider(
            SampleProviderBuilder providerBuilder,
            ISampleDataContainer container)
            : base(providerBuilder)
        {{
            providerBuilder.WithItems(container.Items);
        }}

        IEnumerable<SampleItemDto> ISampleDataProvider.GetItems() => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).GetItems();

        bool ISampleDataProvider.DeleteItem(Guid id) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).DeleteItem(id);

        bool ISampleDataProvider.UpdateItem(SampleItemDto dto) => GetService(r =>
        {{
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }}).UpdateItem(dto);

        void ISampleDataProvider.CreateItem(SampleItemDto dto) => GetService(r =>
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
                .AddInstance(InitializeSampleDataContainer())
                .AddSingleton<ISampleDataProvider, FakeSampleDataProvider>();

            dependencyRegistrator.RegisterInstance(SampleProviderBuilder.CreateBuilder());
        }}

        private static ISampleDataContainer InitializeSampleDataContainer()
        {{
            var container = new SampleDataContainer();
            container.UpdateItems(new[]
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
            return container;
        }}
    }}
}}"));
        }

        internal static GeneratedFolder WithDataRealProviders(this GeneratedFolder folder)
        {
            return folder
                .WithFolder($"{folder.Name}.Data.Real.Providers", r =>
                    r.WithFile($"{folder.Name}.Data.Real.Providers.csproj", AssertionHelper.Any)
                        .WithFile("SampleDataProvider.cs", $@"using System;
using System.Collections.Generic;
using {folder.Name}.Data.Contracts.Dto;
using {folder.Name}.Data.Contracts.Providers;

namespace {folder.Name}.Data.Real.Providers
{{
    internal sealed class SampleDataProvider : ISampleDataProvider
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
            return folder
                .WithFolder($"{folder.Name}.Launcher", r =>
                    r.WithFile($"{folder.Name}.Launcher.csproj", AssertionHelper.Any)
                        .WithFile("App.xaml", $@"<Application x:Class=""{folder.Name}.Launcher.App""
             xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
             xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" />
").WithFile("App.xaml.cs", $@"using Common.Bootstrapping;
using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Mvvm.Commanding;
using Solid.Core;

namespace {folder.Name}.Launcher
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
using {folder.Name}.Presentation.Contracts;
using Solid.Practices.Composition;

namespace {folder.Name}.Launcher
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
                ""{folder.Name}.Data"",
                ""{folder.Name}.Model"",
                ""{folder.Name}.Presentation"",
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

namespace {folder.Name}.Launcher
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

namespace {folder.Name}.Launcher
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
}}"));
        }

        internal static GeneratedFolder WithModel(this GeneratedFolder folder)
        {
            return folder.WithFolder($"{folder.Name}.Model", r =>
                r.WithFile($"{folder.Name}.Model.csproj", AssertionHelper.Any)
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
}}").WithFile("DataService.cs", $@"using System.Collections.Generic;
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
        private readonly ISampleDataProvider _sampleProvider;
        private readonly SampleMapper _sampleMapper;

        private readonly RangeObservableCollection<ISampleItem> _items =
            new RangeObservableCollection<ISampleItem>();

        public DataService(ISampleDataProvider sampleProvider, SampleMapper sampleMapper)
        {{
            _sampleProvider = sampleProvider;
            _sampleMapper = sampleMapper;
        }}

        IEnumerable<ISampleItem> IDataService.Items => _items;

        Task IDataService.GetItems() => MethodRunner.RunAsync(GetItems);

        private void GetItems()
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

        Task IDataService.SaveItem(ISampleItem item) => MethodRunner.RunAsync(() =>
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

        internal static GeneratedFolder WithModelContracts(this GeneratedFolder folder)
        {
            return folder.WithFolder($"{folder.Name}.Model.Contracts", r =>
                r.WithFile($"{folder.Name}.Model.Contracts.csproj", AssertionHelper.Any)
                    .WithFile("AssemblyInfo.cs",
                        @"using System.Reflection;

namespace Generation.Model.Contracts
{
    public static class AssemblyInfo
    {
        public static string AssemblyName { get; } = $""{Assembly.GetExecutingAssembly().GetName().Name}.dll"";
    }
}").WithFile("IAppModel.cs", $@"using System;
using LogoFX.Client.Mvvm.Model.Contracts;

namespace {folder.Name}.Model.Contracts
{{    
    public interface IAppModel : IModel<Guid>, IEditableModel
    {{
        /// <summary>
        /// Designates whether model should be discarded when cancelling changes
        /// The recommended usage is:
        /// <code>
        /// var model = _dataService.CreateModelAsync();
        /// 
        /// public async Task &lt;Model&gt; CreateModelAsync()
        /// {{
        ///     //... wrap into async call
        ///     var dto = _provider.Create();
        ///     var model = Mapper.MapToModel(dto);
        ///     model.IsNew = true;
        ///     return model;
        /// }}
        /// 
        /// _dataService.UpdateModelAsync(Model model);
        /// public async Task UpdateModelAsync(Model model)
        /// {{
        ///     //... wrap into async call
        ///     var dto = Mapper.MapToDto(model);
        ///     _provider.Update(dto);       
        ///     model.IsNew = false;
        /// }}        
        /// 
        /// </code>
        /// </summary>
        bool IsNew {{ get; set; }}
    }}
}}
").WithFile("IDataService.cs", $@"using System.Collections.Generic;
using System.Threading.Tasks;

namespace {folder.Name}.Model.Contracts
{{
    public interface IDataService
    {{
        IEnumerable<ISampleItem> Items {{ get; }}

        Task GetItems();

        Task<ISampleItem> NewItem();

        Task SaveItem(ISampleItem item);

        Task DeleteItem(ISampleItem item);
    }}
}}").WithFile("ISampleItem.cs", $@"namespace {folder.Name}.Model.Contracts
{{
    public interface ISampleItem : IAppModel
    {{
        string DisplayName {{ get; set; }}   
        int Value {{ get; set; }}
    }}
}}"));
        }

        internal static GeneratedFolder WithPresentation(this GeneratedFolder folder)
        {
            return folder.WithFolder($"{folder.Name}.Presentation", r =>
                r.WithFile($"{folder.Name}.Presentation.Shell.csproj", AssertionHelper.Any)
                    .WithFile("Module.cs", $@"using JetBrains.Annotations;
using {folder.Name}.Presentation.Contracts;
using {folder.Name}.Presentation.Shell.ViewModels;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folder.Name}.Presentation.Shell
{{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {{
        void ICompositionModule<IDependencyRegistrator>.RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {{
            dependencyRegistrator.AddSingleton<IShellViewModel, ShellViewModel>();
        }}
    }}
}}").WithFolder("ViewModels", s => s.WithFile("MainViewModel.cs", $@"using Caliburn.Micro;
using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel;
using LogoFX.Client.Mvvm.ViewModel.Services;
using {folder.Name}.Model.Contracts;
using {folder.Name}.Presentation.Contracts;

namespace {folder.Name}.Presentation.Shell.ViewModels
{{
    [UsedImplicitly]
    public sealed class MainViewModel : Screen, IMainViewModel
    {{
        private readonly IViewModelCreatorService _viewModelCreatorService;
        private readonly IDataService _dataService;

        public MainViewModel(
            IViewModelCreatorService viewModelCreatorService,
            IDataService dataService)
        {{
            _viewModelCreatorService = viewModelCreatorService;
            _dataService = dataService;
        }}

        private WrappingCollection.WithSelection _items;
        public WrappingCollection.WithSelection Items => _items ??= CreateItems();

        private WrappingCollection.WithSelection CreateItems()
        {{
            WrappingCollection.WithSelection wc = null;

            // ReSharper disable once AccessToModifiedClosure
            wc = new WrappingCollection.WithSelection(o => wc?.SelectedItem == null)
            {{
                FactoryMethod = o =>
                    _viewModelCreatorService.CreateViewModel<ISampleItem, SampleItemViewModel>((ISampleItem) o)
            }}.WithSource(_dataService.Items);

            return wc;
        }}

        protected override async void OnInitialize()
        {{
            base.OnInitialize();
            await _dataService.GetItems();
        }}

    }}
}}").WithFile("SampleItemViewModel.cs", $@"using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel.Extensions;
using {folder.Name}.Model.Contracts;
using {folder.Name}.Presentation.Contracts;

namespace {folder.Name}.Presentation.Shell.ViewModels
{{
    [UsedImplicitly]
    public sealed class SampleItemViewModel : EditableObjectViewModel<ISampleItem>, ISampleItemViewModel
    {{
        public SampleItemViewModel(
            ISampleItem model) : base(model)
        {{
        }}

        protected override async Task<bool> SaveMethod(ISampleItem model)
        {{
            IsBusy = true;

            try
            {{
                //Delay imitation
                await Task.Delay(4000);
                return true;
            }}
            catch (Exception err)
            {{
                Debug.WriteLine(err.Message);
                return false;
            }}
            finally
            {{
                IsBusy = false;
            }}
        }}
    }}
}}").WithFile("ShellViewModel.cs", $@"using System.ComponentModel;
using System.Windows.Input;
using Caliburn.Micro;
using JetBrains.Annotations;
using LogoFX.Client.Mvvm.Commanding;
using LogoFX.Client.Mvvm.ViewModel.Services;
using {folder.Name}.Presentation.Contracts;

namespace {folder.Name}.Presentation.Shell.ViewModels
{{
    [UsedImplicitly]
    public class ShellViewModel : Conductor<INotifyPropertyChanged>.Collection.OneActive, IShellViewModel
    {{
        private readonly IViewModelCreatorService _viewModelCreatorService;

        public ShellViewModel(IViewModelCreatorService viewModelCreatorService)
        {{
            _viewModelCreatorService = viewModelCreatorService;
        }}
        
        private ICommand _closeCommand;

        public ICommand CloseCommand => _closeCommand ??= ActionCommand
            .When(() => true)
            .Do(() =>
            {{
                TryClose();
            }});


        protected override void OnActivate()
        {{
            base.OnActivate();

            ActivateItem(_viewModelCreatorService.CreateViewModel<MainViewModel>());
        }}
    }}
}}")).WithFolder("Views", s =>
                        s.WithFile("MainView.xaml", AssertionHelper.Any)
                            .WithFile("ShellView.xaml", AssertionHelper.Any)
                    .WithFolder("SampleItem",
                        t => t.WithFile("Display.xaml", AssertionHelper.Any)
                            .WithFile("Edit.xaml", AssertionHelper.Any))));
        }

        internal static GeneratedFolder WithPresentationContracts(this GeneratedFolder folder)
        {
            return folder.WithFolder($"{folder.Name}.Presentation.Contracts", r =>
                r.WithFile($"{folder.Name}.Presentation.Contracts.csproj", AssertionHelper.Any)
                    .WithFile("IMainViewModel.cs", $@"namespace {folder.Name}.Presentation.Contracts
{{
    public interface IMainViewModel
    {{
        
    }}
}}").WithFile("ISampleItemViewModel.cs", $@"using {folder.Name}.Model.Contracts;

namespace {folder.Name}.Presentation.Contracts
{{
    public interface ISampleItemViewModel
    {{
        ISampleItem Model {{ get; }}
    }}
}}").WithFile("IShellViewModel.cs", $@"namespace {folder.Name}.Presentation.Contracts
{{
    public interface IShellViewModel
    {{
        
    }}
}}"));
        }
    }
}