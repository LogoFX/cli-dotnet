using System.Collections.Generic;
using System.IO;
using System.Linq;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;
using LogoFX.Cli.Dotnet.Specs.Tests.Infra;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class CodeGenerationSteps
    {
        private readonly IProcessManagementService _processManagementService;

        public CodeGenerationSteps(
            IProcessManagementService processManagementService)
        {
            _processManagementService = processManagementService;
        }

        [When(@"I generate the code in folder named '(.*)' using '(.*)' template with the default options")]
        public void WhenIGenerateTheCodeInFolderNamedUsingTemplateWithTheDefaultOptions(string folderName, string shortName)
        {
            ExecuteTemplate(folderName, shortName);
        }

        private void ExecuteTemplate(string folderName, string shortName, IEnumerable<ArrangeCodeGenerationData> options = null)
        {
            var tempPath = Path.GetTempPath();
            var path = Path.Combine(tempPath, folderName);

            var optionsLine = options == null ? null : string.Join(" ", options.Select(k => $"{k.Name}  {k.Value}"));
            var args = $"new {shortName} {optionsLine}";
            var execInfo = _processManagementService.Start(Path.Combine(path, "dotnet"), args);
            execInfo.ShouldBeSuccessful();
        }

        [When(@"I generate the code in folder named '(.*)' using '(.*)' template with the following options")]
        public void WhenIGenerateTheCodeInFolderNamedUsingTemplateWithTheFollowingOptions(string folderName, string shortName, Table table)
        {
            var options = table.CreateSet<ArrangeCodeGenerationData>();
            ExecuteTemplate(folderName, shortName, options);
        }
    }

    class ArrangeCodeGenerationData
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
