# cli-dotnet
Command-line interface for generating LogoFX-based desktop .NET apps via `dotnet` tool

## Create template

```
dotnet new -i <path-to-solution-folder>
```

This command will create new template with name `logofx-wpf` (in `template.json`) from the project where `<path-to-solution-folder>` is full or relative path to project folder ```LogoFX.Templates.WPF```.
  
## Create project from this template

```
dotnet new logofx-wpf
```

The solution will be created in current folder with name of this folder. For set custom solution name use `-n` switch.

```
dotnet new logofx-wpf -n <new-solution-name>
```
