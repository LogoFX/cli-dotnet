using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ModelGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated model entity objects")]
        public void ThenTheFolderContainsGeneratedModelEntityObjects(string folderName)
        {
            var tempPath = Path.GetTempPath();
            var path = Path.Combine(tempPath, folderName);


        }
    }
}
