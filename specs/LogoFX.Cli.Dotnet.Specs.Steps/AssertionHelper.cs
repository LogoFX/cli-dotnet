using System.Diagnostics;
using System.IO;
using System.Threading;
using FluentAssertions;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    internal static class AssertionHelper
    {
        public const string Any = "*";

        internal static void AssertGeneratedCode(this GeneratedFolder structure)
        {
            var path = Path.Combine(structure.RootPath, structure.Name);
            var count = 10;
            while (!Directory.Exists(path) && count > 0)
            {
                Debug.WriteLine($"Checking '{path}, attempts: {count}'");
                count -= 1;
                Thread.Sleep(2000);
            }

            Directory.Exists(path).Should().BeTrue();
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
