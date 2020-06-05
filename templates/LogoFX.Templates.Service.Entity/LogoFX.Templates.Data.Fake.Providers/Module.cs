using System;
using JetBrains.Annotations;
using LogoFX.Templates.Data.Contracts.Dto;
using LogoFX.Templates.Data.Contracts.Providers;
using LogoFX.Templates.Data.Fake.Containers;
using LogoFX.Templates.Data.Fake.ProviderBuilders;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace LogoFX.Templates.Data.Fake.Providers
{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {
            dependencyRegistrator
                .AddInstance(InitializeSampleContainer())
                .AddSingleton<ISampleModelDataProvider, FakeSampleModelDataProvider>();

            dependencyRegistrator.RegisterInstance(SampleModelProviderBuilder.CreateBuilder());
        }

        private static ISampleModelDataContainer InitializeSampleContainer()
        {
            var sampleContainer = new SampleModelDataContainer();
            sampleContainer.UpdateItems(new[]
            {
                new SampleModelDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "PC",
                    Value = 8
                },

                new SampleModelDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Acme",
                    Value = 10
                },

                new SampleModelDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Bacme",
                    Value = 3
                },

                new SampleModelDto
                {
                    Id = Guid.NewGuid(),
                    DisplayName = "Exceed",
                    Value = 100
                },

                new SampleModelDto
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