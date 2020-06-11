using Common.Infra;
using Solid.Patterns.ChainOfResponsibility;

namespace UninstallTemplate.Handlers
{
    internal class ArgumentsLengthHandler : ChainElementBase<string[], int>
    {
        protected override bool IsMine(string[] data)
        {
            return data.Length < 2;
        }

        protected override int HandleData(string[] data)
        {
            UsageHelper.ShowUsage();
            return ReturnCode.IncorrectFunction;
        }
    }
}
