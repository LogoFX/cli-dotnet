using System;
using System.Collections.Generic;
using System.IO;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    class GeneratedFolder
    {
        public string RootPath { get; }
        public string Name { get; }
        public string FullName => Path.Combine(RootPath, Name);

        public GeneratedFolder(string rootPath, string name)
        {
            RootPath = rootPath;
            Name = name;
        }

        public List<GeneratedFolder> Folders { get; set; } = new List<GeneratedFolder>();
        public List<GeneratedFile> Files { get; set; } = new List<GeneratedFile>();

        public GeneratedFolder WithFolder(string name, Func<GeneratedFolder, GeneratedFolder> updateFunc = null)
        {
            var folder = new GeneratedFolder(FullName,name);
            if (updateFunc != null)
            {
                folder = updateFunc(folder);
            }
            Folders.Add(folder);
            return this;
        }

        public GeneratedFolder WithFile(string name, string contents)
        {
            var file = new GeneratedFile(FullName, name, contents);
            Files.Add(file);
            return this;
        }
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
