﻿namespace LogoFX.Cli.Dotnet.Specs.Tests.Contracts
{
    public interface IProcessManagementService
    {
        //TODO: get from config.json. Use same source across all usages
        ExecutionInfo Start(string tool, string args, int? pause = 2000);
        void Stop(int processId);
    }

    public sealed class ExecutionInfo
    {
        public int ProcessId { get; set; }

        public string[] OutputStrings { get; set; }

        public string[] ErrorStrings { get; set; }

        public int ExitCode { get; set; }
    }
}
