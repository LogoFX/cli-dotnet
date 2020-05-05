using FluentAssertions;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;

namespace LogoFX.Cli.Dotnet.Specs.Tests.Infra
{
    public static class ExecutionInfoExtensions
    {
        public static void Test(this ExecutionInfo executionInfo)
        {
            executionInfo.Should().NotBeNull();
            executionInfo.ErrorStrings.Should().BeEmpty();
            executionInfo.ExitCode.Should().Be(0);
        }
    }
}