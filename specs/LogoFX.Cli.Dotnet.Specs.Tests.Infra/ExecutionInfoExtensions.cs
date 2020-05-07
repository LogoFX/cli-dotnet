using System;
using FluentAssertions;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;

namespace LogoFX.Cli.Dotnet.Specs.Tests.Infra
{
    public static class ExecutionInfoExtensions
    {
        public static void ShouldBeSuccessful(this ExecutionInfo executionInfo)
        {
            executionInfo.Should().NotBeNull();
            executionInfo.ExitCode.Should().Be(0, string.Join(Environment.NewLine, executionInfo.ErrorStrings));
            //executionInfo.ErrorStrings.Should().BeEmpty();
        }
    }
}