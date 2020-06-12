using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;
using LogoFX.Cli.Dotnet.Specs.Tests.Infra;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace LogoFX.Cli.Dotnet.Specs.Steps
{
    [Binding]
    internal sealed class TemplateSteps
    {
        private readonly IProcessManagementService _processManagementService;

        public TemplateSteps(IProcessManagementService processManagementService)
        {
            _processManagementService = processManagementService;
        }

        [When(@"I install the template pack '(.*)' from local package")]
        public void WhenIInstallTheTemplatePackFromLocalPackage(string templatePackName)
        {
            //TODO: Use template pack name
            var execInfo = _processManagementService.Start("../../devops/install-template-pack", string.Empty);
            execInfo.ShouldBeSuccessful();
        }

        [Then(@"The template for '(.*)' is installed with the following parameters")]
        public void ThenTheTemplateForIsInstalledWithTheFollowingParameters(string shortName, Table table)
        {
            var expectedResult = table.CreateSet<TemplateAssertionData>().Single();
            var execInfo = _processManagementService.Start("dotnet", $"new {shortName} -l");
            execInfo.ShouldBeSuccessful();
            
            var words = GetWords(execInfo);
            var actualDescription = words[0];
            var actualShortName = words[1];
            var actualLanguages = words[2];
            var actualTags = words[3];
            actualDescription.Should().Be(expectedResult.Description);
            actualShortName.Should().Be(expectedResult.ShortName);
            actualLanguages.Should().Be(expectedResult.Languages);
            actualTags.Should().Be(expectedResult.Tags);
        }

        private static List<string> GetWords(ExecutionInfo execInfo)
        {
            var lines = execInfo.OutputStrings;
            var dashLine = lines[1];
            var infoLine = lines[2];
            var words = new List<string>();
            var currentIndex = 0;
            while (currentIndex < dashLine.Length)
            {
                var wordInfo = GetWordInfo(currentIndex, dashLine);
                if (wordInfo.Length > 0)
                {
                    words.Add(infoLine[new Range(new Index(wordInfo.Start), 
                        new Index(wordInfo.Start + wordInfo.Length))].Trim());
                }

                currentIndex = wordInfo.Index + 1;
            }

            return words;
        }

        private static WordInfo GetWordInfo(in int currentIndex, string dashLine)
        {
            var (start, length) = InitHelper.InitRange();

            for (var i = currentIndex; i < dashLine.Length; i++)
            {
                if (dashLine[i] == ' ' && !InitHelper.HasStartedBuilding(length))
                    continue;

                if (dashLine[i] == '-')
                {
                    if (InitHelper.ShouldInitStart(start))
                        start = i;

                    length++;
                    if (i != dashLine.Length - 1)
                        continue;
                }

                return new WordInfo
                {
                    Start = start,
                    Length = length,
                    Index = i
                };
            }
            return new WordInfo();
        }

        private static class InitHelper
        {
            private const int InitStart = -1;
            private const int InitLength = 0;

            internal static bool ShouldInitStart(int start)
            {
                return start == InitStart;
            }

            internal static bool HasStartedBuilding(int length)
            {
                return length > InitLength;
            }

            internal static Tuple<int, int> InitRange()
            {
                return new Tuple<int, int>(InitStart, InitLength);
            }
        }

        private class WordInfo
        {
            public int Start { get; set; }
            public int Length { get; set; }
            public int Index { get; set; }
        }
    }
}
