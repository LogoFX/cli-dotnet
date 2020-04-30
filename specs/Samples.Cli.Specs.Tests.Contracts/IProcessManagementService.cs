namespace Samples.Cli.Specs.Tests.Contracts
{
    public interface IProcessManagementService
    {
        int Start(string tool, string args);
        void Stop(int processId);
    }
}
