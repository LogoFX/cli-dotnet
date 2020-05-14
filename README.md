# cli-dotnet
Command-line interface for generating LogoFX-based desktop .NET apps via `dotnet` tool

## Install templates pack

```
call ./devops/install-templates-pack.cmd
```

This command will install all `LogoFX` templates
  
## Create project from the installed template

```
dotnet new logofx-wpf
```

The solution will be created in current folder with name of this folder. For set custom solution name use `-n` switch.

```
dotnet new logofx-wpf -n <new-solution-name>
```

## Add model entity to existing project

Inside solution folder run

```
dotnet new logofx-model -sn <solution name> -n <model name>
```

This command adds `<model name>` model to `<solution name>.Model` project; `I<model name>` contract to `<solution name>.Model.Contracts` project; `<model name>Dto` dto to `<solution name>.Data.Contracts.Dto` project.

Then you need to modify `MappingProfile` (in `….Model` project) for create mapping between the DTO and the model. As in example:

```
CreateDomainObjectMap<SampleItemDto, ISampleItem, SampleItem>();
```

## Azure Devops CI Status
[![Build Status](https://dev.azure.com/LogoFX/cli-dotnet/_apis/build/status/LogoFX.cli-dotnet?branchName=master)](https://dev.azure.com/LogoFX/cli-dotnet/_build/latest?definitionId=1&branchName=master)

## Appveyor CI Status
<img src=https://ci.appveyor.com/api/projects/status/github/logofx/cli-dotnet>
