# Samples.Cli
Command-line interface for generating and managing desktop .NET apps

## Create template

```
dotnet new -i <path-to-solution-folder>
```

This command will create new template with name `logofx-basics` (in `template.json`) from the project where `<path-to-solution-folder>` is full or relative path to project folder ```Samples.Basics```.
  
## Create project from this template

```
dotnet new logofx-basics
```

The solution will be created in current folder with name of this folder. For set custom solution name use `-n` switch.

```
dotnet new logofx-basics -n <new-solution-name>
```
