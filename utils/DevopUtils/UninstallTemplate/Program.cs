using JetBrains.Annotations;
using Solid.Patterns.ChainOfResponsibility;
using UninstallTemplate.Handlers;

namespace UninstallTemplate
{
    [UsedImplicitly]
    internal sealed class Program
    {
        private static IChainElement<string[], int> _argsCommander;

        private static int Main(string[] args)
        {
            _argsCommander = new ArgumentsLengthHandler();
            _argsCommander
                .SetSuccessor(new NoneTemplateKindHandler())
                .SetSuccessor(new DefaultHandler());
            return _argsCommander.Handle(args);
        }
    }
}
