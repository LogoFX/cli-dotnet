using TechTalk.SpecFlow;

namespace Samples.Cli.Specs.Steps
{
    [Binding]
    internal sealed class SolutionGenerationSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public SolutionGenerationSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"I install the '(.*)' template via dotnet Cli")]
        public void WhenIInstallTheTemplateViaDotnetCli(string name)
        {
            //_scenarioContext.Pending();
        }

        [Then(@"The template for '(.*)' is installed with the following parameters")]
        public void ThenTheTemplateForIsInstalledWithTheFollowingParameters(string name, Table table)
        {
            //_scenarioContext.Pending();
        }
    }
}
