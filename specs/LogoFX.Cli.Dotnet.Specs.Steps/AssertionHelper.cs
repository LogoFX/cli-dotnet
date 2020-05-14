using System.IO;
using FluentAssertions;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    internal static class AssertionHelper
    {
        internal static void AssertSubFolders(this string rootPath, string[] subFolders)
        {
            foreach (var projectFolder in subFolders)
            {
                Directory.Exists(Path.Combine(rootPath, projectFolder)).Should().BeTrue();
            }
        }
    }
}
