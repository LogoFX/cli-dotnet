﻿using System;
using LogoFX.Client.Bootstrapping;
using LogoFX.Client.Bootstrapping.Adapters.SimpleContainer;
using Samples.Basics.Presentation.Contracts;
using Solid.Practices.Composition;

namespace Samples.Basics.Launcher
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
                "Samples.Basics.Infra",
                "Samples.Basics.Data",
                "Samples.Basics.Model",
                "Samples.Basics.Presentation",
                "Samples.Basics.Modules",
                "Samples.Basics.Shell",
                "Samples.Basics.Licensing"
            }
        };

        protected override void OnExit(object sender, EventArgs e)
        {
            base.OnExit(sender, e);
            _container?.Dispose();
        }
    }
}