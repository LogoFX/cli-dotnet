using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;
using LogoFX.Cli.Dotnet.Specs.Tests.Infra;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class SolutionGenerationSteps
    {
        private readonly IProcessManagementService _processManagementService;

        public SolutionGenerationSteps(
            IProcessManagementService processManagementService)
        {
            _processManagementService = processManagementService;
        }

        [When(@"I install the '(.*)' template via dotnet Cli")]
        public void WhenIInstallTheTemplateViaDotnetCli(string name)
        {
            var processId = _processManagementService.Start("dotnet", "new " + "-i" + " " + ".." + "\\" + ".." + "\\" + "templates" + "\\" + name);
            Task.Delay(3000).Wait();
            processId.KillProcessAndChildren();
        }

        [Then(@"The template for '(.*)' is installed with the following parameters")]
        public void ThenTheTemplateForIsInstalledWithTheFollowingParameters(string shortName, Table table)
        {
            var expectedResult = table.CreateSet<TemplateAssertionData>().Single();
            var tempFileName = "output.txt";
            var processId = _processManagementService.Start("dotnet", $"new {shortName} -l > {tempFileName}");
            Task.Delay(3000).Wait();
            processId.KillProcessAndChildren();
            var lines = File.ReadAllLines(tempFileName);
            var dashLine = lines[1];
            var infoLine = lines[2];
            const int InitStart = -1;
            const int InitLength = 0;
            int start = InitStart;
            int length = InitLength;
            var words = new List<string>();
            for (int i = 0; i < dashLine.Length; i++)
            {
                if (dashLine[i] == ' ')
                {
                    if (length == InitLength)
                    {
                        continue;
                    }
                }
                if (dashLine[i] == '-')
                {
                    if (start == InitStart)
                    {
                        start = i;
                    }
                    length++;
                    if (i != dashLine.Length - 1)
                        continue;
                }
                words.Add(infoLine[new Range(new Index(start),new Index(start + length))].Trim());
                start = InitStart;
                length = InitLength;
            }

            var actualDescription = words[0];
            var actualShortName = words[1];
            var actualLanguages = words[2];
            var actualTags = words[3];
            actualDescription.Should().Be(expectedResult.Description);
            actualShortName.Should().Be(expectedResult.ShortName);
            actualLanguages.Should().Be(expectedResult.Languages);
            actualTags.Should().Be(expectedResult.Tags);
        }
    }

    class TemplateAssertionData
    {
        public string Description { get; set; }
        public string ShortName { get; set; }
        public string Languages { get; set; }
        public string Tags { get; set; }
    }
}
