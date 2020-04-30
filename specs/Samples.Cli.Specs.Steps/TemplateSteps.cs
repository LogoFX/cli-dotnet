using TechTalk.SpecFlow;

namespace Samples.Cli.Specs.Steps
{
    [Binding]
    internal sealed class TemplateSteps
    {
        private readonly ScenarioContext _scenarioContext;

        public TemplateSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [When(@"I install the basics template via dotnet Cli")]
        public void WhenIInstallTheBasicsTemplateViaDotnetCli()
        {
            _scenarioContext.Pending();
        }

        [Then(@"The template for '(.*)' is installed with the following parameters")]
        public void ThenTheTemplateForIsInstalledWithTheFollowingParameters(string name, Table table)
        {
            _scenarioContext.Pending();
        }
    }
}
