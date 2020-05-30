using System.IO;
using FluentAssertions;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    internal static class AssertionHelper
    {
        internal static void AssertGeneratedCode(this GeneratedFolder structure)
        {
            Directory.Exists(Path.Combine(structure.RootPath, structure.Name)).Should().BeTrue();
            foreach (var folder in structure.Folders)
            {
                AssertGeneratedCode(folder);
            }

            foreach (var file in structure.Files)
            {
                var filePath = Path.Combine(file.RootPath, file.Name);
                File.Exists(filePath).Should().BeTrue();
                var contents = File.ReadAllText(filePath);
                contents.Should().BeEquivalentTo(file.Contents);
            }
        }
    }
}
