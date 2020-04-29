using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Mvvm.Commanding;

namespace Samples.Specifications.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        public App()
        {
            Solid.Practices.Composition.AssemblyLoader.LoadAssembliesFromPaths = AssemblyLoader.Get;

            var bootstrapper =
                new AppBootstrapper()
                    .UseResolver()
                    .UseCommanding()
                    .UseShared();
            bootstrapper.Initialize();
        }
    }
}
