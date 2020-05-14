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

            var subFolders = new[]
            {
                $"{solutionName}.Data.Contracts.Dto",
                $"{solutionName}.Model",
                $"{solutionName}.Model.Contracts"
            };
            path.AssertSubFolders(subFolders);
        }
    }
}
