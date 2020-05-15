using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ModelGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated model entity objects for solution name '(.*)'")]
        public void ThenTheFolderContainsGeneratedModelEntityObjectsForSolutionName(string folderName, string solutionName)
        {
            var tempPath = Path.GetTempPath();
            var path = Path.Combine(tempPath, folderName);

            //TODO: Improve building process to avoid duplication
            var structure = new GeneratedFolder(tempPath, folderName)
                {
                    Folders = new []
                    {
                        new GeneratedFolder(path, $"{solutionName}.Data.Contracts.Dto")
                            {
                                Files = new []{new GeneratedFile(Path.Combine(path, $"{solutionName}.Data.Contracts.Dto"), "SampleDto.cs", @"namespace Test.Data.Contracts.Dto
{
    public class SampleDto
    {

    }
}") }
                        }
                    }
            };

            structure.AssertGeneratedCode();
            //var subFolders = new[]
            //{
            //    $"{solutionName}.Data.Contracts.Dto",
            //    $"{solutionName}.Model",
            //    $"{solutionName}.Model.Contracts"
            //};
            //path.AssertSubFolders(subFolders);
        }
    }
}
