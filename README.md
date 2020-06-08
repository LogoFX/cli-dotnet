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
dotnet new logofx-wpf -n <solution-name>
```

## Add model entity to existing project

Inside solution folder run

```
dotnet new logofx-entity -sn <solution-name> -n <entity-name> [--allow-scripts yes]
```

This command adds `<entity-name>` entity to `<solution-name>.Model` project; `I<entity-name>` contract to `<solution-name>.Model.Contracts` project; `<entity-name>Dto` dto to `<solution-name>.Data.Contracts.Dto` project.

If you didn't specify `--allow-scripts yes` parameter the template engine ask you to allow template script launch. This script will change `Mappers\MappingProfile` and `Module` (in `… .Model` project) for setup mapping between the DTO and the model.

## Azure Devops CI Status
[![Build Status](https://dev.azure.com/LogoFX/cli-dotnet/_apis/build/status/LogoFX.cli-dotnet?branchName=master)](https://dev.azure.com/LogoFX/cli-dotnet/_build/latest?definitionId=1&branchName=master)

## Appveyor CI Status
<img src=https://ci.appveyor.com/api/projects/status/github/logofx/cli-dotnet>
