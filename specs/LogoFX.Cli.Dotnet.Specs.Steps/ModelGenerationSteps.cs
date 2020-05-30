using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ModelGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated model entity objects for solution name '(.*)'")]
        public void ThenTheFolderContainsGeneratedModelEntityObjectsForSolutionName(string folderName,
            string solutionName)
        {
            var tempPath = Path.GetTempPath();

            var structure = new GeneratedFolder(tempPath, folderName)
                .WithFolder($"{solutionName}.Data.Contracts.Dto",
                    r =>
                        r.WithFile("SampleDto.cs", @"namespace Test.Data.Contracts.Dto
{
    public class SampleDto
    {

    }
}")).WithFolder($"{solutionName}.Model.Contracts", r => r.WithFile("ISample.cs", @"namespace Test.Model.Contracts
{
    public interface ISample : IAppModel
    {
        
    }
}"))
                .WithFolder($"{solutionName}.Model", r =>
                    r.WithFile("Sample.cs", @"using Test.Model.Contracts;

namespace Test.Model
{
    public class Sample : AppModel, ISample
    {
        
    }
}").WithFile("Module.cs", @"using System.Reflection;
using AutoMapper;
using JetBrains.Annotations;
using Test.Model.Contracts;
using Test.Model.Mappers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace Test.Model
{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {
            dependencyRegistrator
                .RegisterAutomagically(
                    Assembly.LoadFrom(AssemblyInfo.AssemblyName),
                    Assembly.GetExecutingAssembly());

            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MappingProfile());
            });
            var mapper = config.CreateMapper();
            dependencyRegistrator
                .AddInstance(mapper)
                .AddSingleton<SampleMapper>();
        }
    }
}
").WithFolder("Mappers", s => 
                        s.WithFile("MappingProfile.cs", @"using System;
using AutoMapper;
using Test.Data.Contracts.Dto;
using Test.Model.Contracts;

namespace Test.Model.Mappers
{
    internal sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateSampleItemMaps();
            CreateSampleMaps();
        }

        private void CreateSampleMaps()
        {
            CreateDomainObjectMap<SampleDto, ISample, Sample>();
        }

        private void CreateSampleItemMaps()
        {
            CreateDomainObjectMap<SampleItemDto, ISampleItem, SampleItem>();
        }

        private void CreateDomainObjectMap<TDto, TContract, TModel>()
            where TModel : TContract where TContract : class => CreateDomainObjectMap(typeof(TDto), typeof(TContract), typeof(TModel));
        private void CreateDomainObjectMap(Type dtoType, Type contractType, Type modelType)
        {
            CreateMap(dtoType, contractType).As(modelType);
            CreateMap(dtoType, modelType);
            CreateMap(contractType, dtoType);
            CreateMap(modelType, dtoType);
        }
    }
}").WithFile("SampleMapper.cs", @"using AutoMapper;
using Test.Data.Contracts.Dto;
using Test.Model.Contracts;

namespace Test.Model.Mappers
{
    internal sealed class SampleMapper
    {
        private readonly IMapper _mapper;

        public SampleMapper(IMapper mapper) => _mapper = mapper;

        public ISample MapToSampleValue(SampleDto sampleModelDto) =>
            _mapper.Map<ISample>(sampleModelDto);
    }
}")));
            structure.AssertGeneratedCode();
        }
    }
}
