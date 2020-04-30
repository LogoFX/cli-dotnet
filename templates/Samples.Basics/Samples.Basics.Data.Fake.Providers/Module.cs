using System;
using JetBrains.Annotations;
using Samples.Basics.Data.Contracts.Dto;
using Samples.Basics.Data.Contracts.Providers;
using Samples.Basics.Data.Fake.Containers;
using Samples.Basics.Data.Fake.ProviderBuilders;
using Solid.Practices.IoC;
using Solid.Practices.Modularity;

namespace Samples.Basics.Data.Fake.Providers
{
    [UsedImplicitly]
    internal sealed class Module : ICompositionModule<IDependencyRegistrator>
    {
        public void RegisterModule(IDependencyRegistrator dependencyRegistrator)
        {
            dependencyRegistrator
                .AddInstance(InitializeWarehouseContainer())
                .AddSingleton<IWarehouseProvider, FakeWarehouseProvider>();

            dependencyRegistrator.RegisterInstance(WarehouseProviderBuilder.CreateBuilder());
        }

        private static IWarehouseContainer InitializeWarehouseContainer()
        {
            var warehouseContainer = new WarehouseContainer();
            warehouseContainer.UpdateWarehouseItems(new[]
            {
                new WarehouseItemDto
                {
                    Id = Guid.NewGuid(),
                    Kind = "PC",
                    Price = 25.43,
                    Quantity = 8
                },

                new WarehouseItemDto
                {
                    Id = Guid.NewGuid(),
                    Kind = "Acme",
                    Price = 10,
                    Quantity = 10
                },

                new WarehouseItemDto
                {
                    Id = Guid.NewGuid(),
                    Kind = "Bacme",
                    Price = 20,
                    Quantity = 3
                },

                new WarehouseItemDto
                {
                    Id = Guid.NewGuid(),
                    Kind = "Exceed",
                    Price = 0.4,
                    Quantity = 100
                },

                new WarehouseItemDto
                {
                    Id = Guid.NewGuid(),
                    Kind = "Acme2",
                    Price = 1,
                    Quantity = 10
                }
            });
            return warehouseContainer;
        }
    }
}