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

<img src=https://ci.appveyor.com/api/projects/status/github/logofx/cli-dotnet>
