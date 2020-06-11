using Common.Infra;
using Solid.Patterns.ChainOfResponsibility;

namespace UninstallTemplate.Handlers
{
    internal class NoneTemplateKindHandler : ChainElementBase<string[], int>
    {
        protected override bool IsMine(string[] data)
        {
            var kind = TemplateNameParser.GetTemplateNameKind(data[0]);
            return kind == TemplateNameParser.TemplateNameKind.None;
        }

        protected override int HandleData(string[] data)
        {
            UsageHelper.ShowUsage();
            return ReturnCode.IncorrectFunction;
        }
    }
}