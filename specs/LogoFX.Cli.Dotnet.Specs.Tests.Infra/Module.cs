using JetBrains.Annotations;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace LogoFX.Cli.Dotnet.Specs.Tests.Infra
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
