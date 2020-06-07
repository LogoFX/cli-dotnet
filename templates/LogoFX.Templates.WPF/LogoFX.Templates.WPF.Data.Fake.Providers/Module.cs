using System;
using JetBrains.Annotations;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Data.Contracts.Providers;
using LogoFX.Templates.WPF.Data.Fake.Containers;
using LogoFX.Templates.WPF.Data.Fake.ProviderBuilders;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace LogoFX.Templates.WPF.Data.Fake.Providers
{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {
            dependencyRegistrator
                .AddInstance(InitializeSampleContainer())
                .AddSingleton<ISampleProvider, FakeSampleProvider>()
                .RegisterInstance(SampleProviderBuilder.CreateBuilder());
        }

        private ISampleContainer InitializeSampleContainer()
        {
            var sampleContainer = new SampleContainer();
            sampleContainer.UpdateItems(new[]
            {
                new SampleItemDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "PC",
                    Value = 8
                },

                new SampleItemDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Acme",
                    Value = 10
                },

                new SampleItemDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Bacme",
                    Value = 3
                },

                new SampleItemDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Exceed",
                    Value = 100
                },

                new SampleItemDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Acme2",
                    Value = 10
                }
            });
            return sampleContainer;
        }
    }
}