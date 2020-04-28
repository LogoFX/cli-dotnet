using JetBrains.Annotations;
using Samples.Specifications.Presentation.Contracts.Shell;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace Samples.Specifications.Presentation.Shell
{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {
        void ICompositionModule<IDependencyRegistrator>.RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {
            dependencyRegistrator.AddSingleton<IShellViewModel, ShellViewModel>();
        }
    }
}