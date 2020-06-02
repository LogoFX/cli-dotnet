using System.IO;
using TechTalk.SpecFlow;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class ServiceGenerationSteps
    {
        [Then(@"The folder '(.*)' contains generated service entity objects for name '(.*)' for solution name '(.*)'")]
        public void ThenTheFolderContainsGeneratedServiceEntityObjectsForNameForSolutionName(string folderName,
            string entityName,
            string solutionName)
        {
            var tempPath = Path.GetTempPath();

            var structure = new GeneratedFolder(tempPath, folderName)
                .WithFolder($"{solutionName}.Model.Contracts",
                    r => r.WithFile($"I{entityName}Service.cs",
                        $@"using System.Collections.Generic;
using System.Threading.Tasks;

namespace {solutionName}.Model.Contracts
{{
    public interface I{entityName}Service
    {{
        IEnumerable<I{entityName}> Items {{ get; }}

        Task GetItemsAsync();

        Task<I{entityName}> NewItemAsync();

        Task SaveItemAsync(I{entityName} item);

        Task DeleteItemAsync(I{entityName} item);
    }}
}}"));

            structure.AssertGeneratedCode();
        }
    }
}