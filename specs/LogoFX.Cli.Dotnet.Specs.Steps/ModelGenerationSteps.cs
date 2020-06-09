using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ModelGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated entity service objects for name '(.*)' for solution name '(.*)'")]
        public void ThenTheFolderContainsGeneratedEntityServiceObjectsForNameForSolutionName(string folderName,
            string entityName,
            string solutionName)
        {
            var tempPath = Path.GetTempPath();

            var structure = new GeneratedFolder(tempPath, folderName)
                .WithFolder($"{solutionName}.Data.Contracts.Dto",
                    r =>
                        r.WithFile($"{entityName}Dto.cs", $@"using System;

namespace {solutionName}.Data.Contracts.Dto
{{
    public class {entityName}Dto
    {{
        public Guid Id {{ get; set; }}

        public string DisplayName {{ get; set; }}

        public int Value {{ get; set; }}
    }}
}}")).WithFolder($"{solutionName}.Model.Contracts", r => r.WithFile($"I{entityName}.cs", $@"namespace {solutionName}.Model.Contracts
{{
    public interface I{entityName} : IAppModel
    {{
        string DisplayName {{ get; }}

        int Value {{ get; set; }}
    }}
}}"))
                .WithFolder($"{solutionName}.Model", r =>
                    r.WithFile($"{entityName}.cs", $@"using {solutionName}.Model.Contracts;

namespace {solutionName}.Model
{{
    internal class {entityName} : AppModel, I{entityName}
    {{
        private string _displayName;

        public string DisplayName
        {{
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }}

        private int _value;

        public int Value
        {{
            get => _value;
            set => SetProperty(ref _value, value);
        }}
    }}
}}").WithFile("Module.cs", $@"using System.Reflection;
using AutoMapper;
using JetBrains.Annotations;
using {solutionName}.Model.Contracts;
using {solutionName}.Model.Mappers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace {solutionName}.Model
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
                .AddInstance(mapper){(entityName=="Sample"?string.Empty:@"
                .AddSingleton<SampleMapper>()")}
                .AddSingleton<{entityName}Mapper>();
        }}
    }}
}}
").WithFolder("Mappers", s => 
                        s.WithFile("MappingProfile.cs", $@"using System;
using AutoMapper;
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Model.Contracts;

namespace {solutionName}.Model.Mappers
{{
    internal sealed class MappingProfile : Profile
    {{
        public MappingProfile()
        {{
            CreateSampleItemMaps();
            Create{entityName}Maps();
        }}

        private void Create{entityName}Maps()
        {{
            CreateDomainObjectMap<{entityName}Dto, I{entityName}, {entityName}>();
        }}

        private void CreateSampleItemMaps()
        {{
            CreateDomainObjectMap<SampleItemDto, ISampleItem, SampleItem>();
        }}

        private void CreateDomainObjectMap<TDto, TContract, TModel>()
            where TModel : TContract where TContract : class => CreateDomainObjectMap(typeof(TDto), typeof(TContract), typeof(TModel));
        private void CreateDomainObjectMap(Type dtoType, Type contractType, Type modelType)
        {{
            CreateMap(dtoType, contractType).As(modelType);
            CreateMap(dtoType, modelType);
            CreateMap(contractType, dtoType);
            CreateMap(modelType, dtoType);
        }}
    }}
}}").WithFile($"{entityName}Mapper.cs", $@"using AutoMapper;
using JetBrains.Annotations;
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Model.Contracts;

namespace {solutionName}.Model.Mappers
{{
    [UsedImplicitly]
    internal sealed class {entityName}Mapper
    {{
        private readonly IMapper _mapper;

        public {entityName}Mapper(IMapper mapper) => _mapper = mapper;

        public I{entityName} MapTo{entityName}({entityName}Dto dto) =>
            _mapper.Map<I{entityName}>(dto);

        public {entityName}Dto MapFrom{entityName}(I{entityName} model) =>
            _mapper.Map<{entityName}Dto>(model);
    }}
}}")));
            structure.AssertGeneratedCode();
        }
    }
}
