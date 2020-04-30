using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attest.Fake.Builders;
using JetBrains.Annotations;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Data.Contracts.Providers;
using LogoFX.Templates.WPF.Data.Fake.Containers;
using LogoFX.Templates.WPF.Data.Fake.ProviderBuilders;

namespace LogoFX.Templates.WPF.Data.Fake.Providers
{
    [UsedImplicitly]
    internal sealed class FakeWarehouseProvider : FakeProviderBase<WarehouseProviderBuilder, IWarehouseProvider>, IWarehouseProvider
    {
        private readonly Random _random = new Random();

        public FakeWarehouseProvider(
            WarehouseProviderBuilder warehouseProviderBuilder,
            IWarehouseContainer warehouseContainer)
            : base(warehouseProviderBuilder)
        {
            warehouseProviderBuilder.WithWarehouseItems(warehouseContainer.WarehouseItems);
        }

        IEnumerable<WarehouseItemDto> IWarehouseProvider.GetWarehouseItems() => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).GetWarehouseItems();

        bool IWarehouseProvider.DeleteWarehouseItem(Guid id) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).DeleteWarehouseItem(id);

        bool IWarehouseProvider.UpdateWarehouseItem(WarehouseItemDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).UpdateWarehouseItem(dto);

        void IWarehouseProvider.CreateWarehouseItem(WarehouseItemDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).CreateWarehouseItem(dto);
    }
}