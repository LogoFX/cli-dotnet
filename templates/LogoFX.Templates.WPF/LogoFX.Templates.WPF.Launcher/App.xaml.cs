using Common.Bootstrapping;
using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Mvvm.Commanding;
using Solid.Core;

namespace LogoFX.Templates.WPF.Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        public App()
        {
            var bootstrapper = new AppBootstrapper();
            bootstrapper.UseDynamicLoad();
            bootstrapper
                .UseResolver()
                .UseCommanding()
                .UseShared();
            
            ((IInitializable)bootstrapper).Initialize();
        }
    }
}
