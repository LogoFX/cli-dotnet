using System;
using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Bootstrapping.Adapters.SimpleContainer;
using LogoFX.Templates.WPF.Presentation.Contracts;
using Solid.Practices.Composition;

namespace LogoFX.Templates.WPF.Launcher
{
    public sealed class AppBootstrapper : BootstrapperContainerBase<ExtendedSimpleContainerAdapter>
        .WithRootObjectAsContract<IShellViewModel>
    {
        private static readonly ExtendedSimpleContainerAdapter _container = new ExtendedSimpleContainerAdapter();

        public AppBootstrapper()
            : base(_container)
        {
        }

        public override CompositionOptions CompositionOptions => new CompositionOptions
        {
            Prefixes = new[] {
                "Common.Data",
                "LogoFX.Templates.WPF.Data",
                "LogoFX.Templates.WPF.Model",
                "LogoFX.Templates.WPF.Presentation",
            }
        };

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            _container?.Dispose();
        }
    }
}