using System.Diagnostics;
using System.IO;
using System.Threading;
using FluentAssertions;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    internal static class AssertionHelper
    {
        public const string Any = "*";
        //TODO: Get from config.json
        private const int DefaultSleepTime = 2000;

        internal static void AssertGeneratedCode(this GeneratedFolder structure)
        {
            var path = Path.Combine(structure.RootPath, structure.Name);
            //TODO: Get from config.json
            var count = 10;
            while (!Directory.Exists(path) && count > 0)
            {
                Debug.WriteLine($"Checking '{path}, attempts: {count}'");
                count -= 1;
                Thread.Sleep(DefaultSleepTime);
            }

            Directory.Exists(path).Should().BeTrue($"Folder doesn't exist: '{path}'");
            foreach (var folder in structure.Folders)
            {
                AssertGeneratedCode(folder);
            }

            foreach (var file in structure.Files)
            {
                var filePath = Path.Combine(file.RootPath, file.Name);
                File.Exists(filePath).Should().BeTrue($"File '{filePath}' not found.");
                if (file.Contents != Any)
                {
                    var contents = File.ReadAllText(filePath);
                    contents.Should().BeEquivalentTo(file.Contents);
                }
            }
        }
    }
}
