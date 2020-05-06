# cli-dotnet
Command-line interface for generating LogoFX-based desktop .NET apps via `dotnet` tool

## Install template

```
call install-template-logofx-wpf.cmd
```

This command will install new template with name `logofx-wpf` (in `template.json`).
  
## Create project from the installed template

```
dotnet new logofx-wpf
```

The solution will be created in current folder with name of this folder. For set custom solution name use `-n` switch.

```
dotnet new logofx-wpf -n <new-solution-name>
```

<img src=https://ci.appveyor.com/api/projects/status/github/logofx/cli-dotnet>
