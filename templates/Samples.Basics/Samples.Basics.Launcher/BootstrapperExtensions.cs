using LogoFX.Client.Mvvm.ViewModel.Services;
using LogoFX.Client.Mvvm.ViewModelFactory.SimpleContainer;
using Solid.Bootstrapping;
using Solid.Core;
using Solid.Extensibility;
using Solid.Practices.Composition.Contracts;

namespace Samples.Basics.Launcher
{
    public static class BootstrapperExtensions
    {
        public static IInitializable UseShared<TBootstrapper>(
            this TBootstrapper bootstrapper)
            where TBootstrapper : class, IExtensible<TBootstrapper>, IHaveRegistrator, ICompositionModulesProvider, IInitializable =>
            bootstrapper
                .UseViewModelCreatorService()
                .UseViewModelFactory();
    }
}