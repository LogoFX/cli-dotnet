using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ModelGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated model entity objects for name '(.*)' for solution name '(.*)'")]
        public void ThenTheFolderContainsGeneratedModelEntityObjectsForSolutionName(string folderName,
            string entityName,
            string solutionName)
        {
            var tempPath = Path.GetTempPath();

            var structure = new GeneratedFolder(tempPath, folderName)
                .WithFolder($"{solutionName}.Data.Contracts.Dto",
                    r =>
                        r.WithFile($"{entityName}Dto.cs", $@"namespace {solutionName}.Data.Contracts.Dto
{{
    public class {entityName}Dto
    {{

    }}
}}")).WithFolder($"{solutionName}.Model.Contracts", r => r.WithFile($"I{entityName}.cs", $@"namespace {solutionName}.Model.Contracts
{{
    public interface I{entityName} : IAppModel
    {{
        
    }}
}}"))
                .WithFolder($"{solutionName}.Model", r =>
                    r.WithFile($"{entityName}.cs", $@"using {solutionName}.Model.Contracts;

namespace {solutionName}.Model
{{
    public class {entityName} : AppModel, I{entityName}
    {{
        
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
using {solutionName}.Data.Contracts.Dto;
using {solutionName}.Model.Contracts;

namespace {solutionName}.Model.Mappers
{{
    internal sealed class {entityName}Mapper
    {{
        private readonly IMapper _mapper;

        public {entityName}Mapper(IMapper mapper) => _mapper = mapper;

        public I{entityName} MapTo{entityName}Value({entityName}Dto dto) =>
            _mapper.Map<I{entityName}>(dto);
    }}
}}")));
            structure.AssertGeneratedCode();
        }

        private string Decapitalize(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            var firstChar = char.ToLowerInvariant(str[0]);
            return str.Length > 1 ? firstChar + str.Substring(1) : firstChar.ToString();
        }
    }
}
