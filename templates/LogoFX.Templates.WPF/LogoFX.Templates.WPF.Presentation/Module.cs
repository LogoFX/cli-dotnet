using JetBrains.Annotations;
using LogoFX.Templates.WPF.Presentation.Contracts;
using LogoFX.Templates.WPF.Presentation.Shell.ViewModels;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace LogoFX.Templates.WPF.Presentation.Shell
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