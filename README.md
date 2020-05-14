# cli-dotnet
Command-line interface for generating LogoFX-based desktop .NET apps via `dotnet` tool

## Install templates pack

```
./devops/install-templates-pack.cmd
```

This command will install all `LogoFX` templates
  
## Create solution from the installed template

```
dotnet new logofx-wpf
```

The solution will be created in current folder with name of this folder. Use `-n` option to set custom solution name

```
dotnet new logofx-wpf -n <new-solution-name>
```

## Add model entity to existing project

Inside solution folder run

```
dotnet new logofx-model -sn <solution name> -n <model name>
```

This command adds `<model name>` model to `<solution name>.Model` project; `I<model name>` contract to `<solution name>.Model.Contracts` project; `<model name>Dto` dto to `<solution name>.Data.Contracts.Dto` project.

Then you need to modify `Mappers\MappingProfile` and `Module` (in `… .Model` project) for setup mapping between the DTO and the model. 

In `MappingProfile` add code as in example:

```csharp
CreateDomainObjectMap<…Dto, I…, S…>();
```

Where `…` is entity name specified in the parameter `-n`.

In `Module` add code `.AddSingleton<…Mapper>()` to `dependencyRegistrator`, where `…` is entity name specified in the parameter `-n`.

### Example

We created the solution `MyApp` (by command `dotnet new logofx-wpf -n MyApp`). Then we need to add entity `MyModel` to this solution. In solution folder run command: `dotnet new logofx-model -sn MyApp -n MyModel`. Then open file `Module.cs` in `MyApp.Model` project. Default content of this file is:

```csharp
using System.Reflection;
using AutoMapper;
using JetBrains.Annotations;
using MyApp.Model.Contracts;
using MyApp.Model.Mappers;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace MyApp.Model
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
```

You must add to `dependencyRegistrator` one more `AddSingleton` instruction.

```csharp
…
            dependencyRegistrator
                .AddInstance(mapper)
                .AddSingleton<SampleMapper>()
                .AddSingleton<MyModelMapper>();
…
```

Save `Module.cs` changes and then open `Mappers\MappingProfile.cs` in `MyApp.Model` project. Default content of this file is:

```csharp
using System;
using AutoMapper;
using MyApp.Data.Contracts.Dto;
using MyApp.Model.Contracts;

namespace MyApp.Model.Mappers
{
    internal sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateCameraMaps();
        }

        private void CreateCameraMaps()
        {
            CreateDomainObjectMap<SampleItemDto, ISampleItem, SampleItem>();
        }

        private void CreateDomainObjectMap<TDto, TContract, TModel>()
            where TModel : TContract
            where TContract : class => CreateDomainObjectMap(typeof(TDto), typeof(TContract), typeof(TModel));

        private void CreateDomainObjectMap(Type dtoType, Type contractType, Type modelType)
        {
            CreateMap(dtoType, contractType).As(modelType);
            CreateMap(dtoType, modelType);
            CreateMap(contractType, dtoType);
            CreateMap(modelType, dtoType);
        }
    }
}
```

You must add another `CreateDomainObjectMap` instruction for registration `MyModel` mapping. For example:

```csharp
…
        public MappingProfile()
        {
            CreateCameraMaps();
            CreateMyModelMaps();
        }

        private void CreateCameraMaps()
        {
            CreateDomainObjectMap<SampleItemDto, ISampleItem, SampleItem>();
        }

        private void CreateMyModelMaps()
        {
            CreateDomainObjectMap<MyModelDto, IMyModel, MyModel>();
        }
…
```

## Azure Devops CI Status
[![Build Status](https://dev.azure.com/LogoFX/cli-dotnet/_apis/build/status/LogoFX.cli-dotnet?branchName=master)](https://dev.azure.com/LogoFX/cli-dotnet/_build/latest?definitionId=1&branchName=master)

## Appveyor CI Status
<img src=https://ci.appveyor.com/api/projects/status/github/logofx/cli-dotnet>
