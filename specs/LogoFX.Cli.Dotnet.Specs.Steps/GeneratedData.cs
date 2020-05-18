namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    class GeneratedFolder
    {
        public string RootPath { get; }
        public string Name { get; }

        public GeneratedFolder(string rootPath, string name)
        {
            RootPath = rootPath;
            Name = name;
        }

        public GeneratedFolder[] Folders { get; set; } = {};
        public GeneratedFile[] Files { get; set; } = {};
    }

    class GeneratedFile
    {
        public string RootPath { get; }
        public string Name { get; }
        public string Contents { get; }

        public GeneratedFile(string rootPath, string name, string contents)
        {
            RootPath = rootPath;
            Name = name;
            Contents = contents;
        }
    }
}
