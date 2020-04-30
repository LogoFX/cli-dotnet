using JetBrains.Annotations;
using Samples.Cli.Specs.Tests.Contracts;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace Samples.Cli.Specs.Tests.Infra
{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {
            dependencyRegistrator
                .AddSingleton<IProcessManagementService, WindowsProcessManagementService>();
        }
    }
}
