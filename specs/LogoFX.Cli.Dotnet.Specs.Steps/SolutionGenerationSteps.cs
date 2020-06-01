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
                .WithBootstrapping()
                .WithDataFakeSetup()
                .WithDataContractsDto()
                .WithDataContractsProviders()
                .WithDataFakeContainers()
                .WithDataFakeContainersContracts()
                .WithDataFakeProviderBuilders()
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
                           .WithFolder($"{folderName}.Model", r => r
                    .WithFolder("Mappers", s =>
                        s.WithFile("MappingProfile.cs", $@"using System;
using AutoMapper;
using {folderName}.Data.Contracts.Dto;
using {folderName}.Model.Contracts;

namespace {folderName}.Model.Mappers
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
using {folderName}.Data.Contracts.Dto;
using {folderName}.Model.Contracts;

namespace {folderName}.Model.Mappers
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

namespace {folderName}.Model.Validation
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
using {folderName}.Model.Contracts;


namespace {folderName}.Model
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
using {folderName}.Data.Contracts.Providers;
using {folderName}.Model.Contracts;
using {folderName}.Model.Mappers;

namespace {folderName}.Model
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

namespace {folderName}.Model
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
using {folderName}.Model.Contracts;
using {folderName}.Model.Mappers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {folderName}.Model
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
using {folderName}.Model.Contracts;
using {folderName}.Model.Validation;

namespace {folderName}.Model
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
"))
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
