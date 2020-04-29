using System;
using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Bootstrapping.Adapters.SimpleContainer;
using Samples.Specifications.Presentation.Contracts.Shell;
using Solid.Practices.Composition;

namespace Samples.Specifications.Launcher
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
                "Samples.Specifications.Infra",
                "Samples.Specifications.Data",
                "Samples.Specifications.Model",
                "Samples.Specifications.Presentation",
                "Samples.Specifications.Modules",
                "Samples.Specifications.Shell",
                "Samples.Specifications.Licensing"
            }
        };

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            _container?.Dispose();
        }
    }
}