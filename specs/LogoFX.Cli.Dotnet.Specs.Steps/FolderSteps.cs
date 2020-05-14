using System.IO;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class FolderSteps
    {
        [When(@"I create a folder named '(.*)'")]
        public void WhenICreateAFolderNamed(string folderName)
        {
            var tempPath = Path.GetTempPath();
            var path = Path.Combine(tempPath, folderName);

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
            Directory.Exists(path).Should().BeTrue();
        }
    }
}
