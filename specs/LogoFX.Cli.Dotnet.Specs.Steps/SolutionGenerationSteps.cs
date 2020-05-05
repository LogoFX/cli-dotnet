using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using JetBrains.Annotations;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;
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

        [When(@"I install the template for location '(.*)' via batch file")]
        public void WhenIInstallTheTemplateForLocationViaBatchFile(string location)
        {
            _processManagementService.SetCurrentDir("../../devops");
            _processManagementService.Start("install-template", location, 30000);
        }

        [Then(@"The template for '(.*)' is installed with the following parameters")]
        public void ThenTheTemplateForIsInstalledWithTheFollowingParameters(string shortName, Table table)
        {
            var expectedResult = table.CreateSet<TemplateAssertionData>().Single();
            var execInfo = _processManagementService.Start("dotnet", $"new {shortName} -l");
            var lines = execInfo.OutputStrings;
            var dashLine = lines[1];
            var infoLine = lines[2];
            const int initStart = -1;
            const int initLength = 0;
            int start = initStart;
            int length = initLength;
            var words = new List<string>();
            for (int i = 0; i < dashLine.Length; i++)
            {
                if (dashLine[i] == ' ')
                {
                    if (length == initLength)
                    {
                        continue;
                    }
                }
                if (dashLine[i] == '-')
                {
                    if (start == initStart)
                    {
                        start = i;
                    }
                    length++;
                    if (i != dashLine.Length - 1)
                        continue;
                }
                words.Add(infoLine[new Range(new Index(start),new Index(start + length))].Trim());
                start = initStart;
                length = initLength;
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

    [UsedImplicitly]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    class TemplateAssertionData
    {
        public string Description { get; set; }
        public string ShortName { get; set; }
        public string Languages { get; set; }
        public string Tags { get; set; }
    }
}
