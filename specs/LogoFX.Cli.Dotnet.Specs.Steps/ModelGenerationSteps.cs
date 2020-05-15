using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ModelGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated model entity objects for solution name '(.*)'")]
        public void ThenTheFolderContainsGeneratedModelEntityObjectsForSolutionName(string folderName,
            string solutionName)
        {
            var tempPath = Path.GetTempPath();
            var path = Path.Combine(tempPath, folderName);

            //TODO: Improve building process to avoid duplication
            var structure = new GeneratedFolder(tempPath, folderName)
                .WithFolder($"{solutionName}.Data.Contracts.Dto",
                    r =>
                        r.WithFile("SampleDto.cs", @"namespace Test.Data.Contracts.Dto
{
    public class SampleDto
    {

    }
}"))
                .WithFolder($"{solutionName}.Model")
                .WithFolder($"{solutionName}.Model.Contracts");
            structure.AssertGeneratedCode();
        }
    }
}
