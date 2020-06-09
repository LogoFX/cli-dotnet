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
    internal sealed class FakeSampleDataProvider : FakeProviderBase<SampleProviderBuilder, ISampleDataProvider>, ISampleDataProvider
    {
        private readonly Random _random = new Random();

        public FakeSampleDataProvider(
            SampleProviderBuilder providerBuilder,
            ISampleDataContainer container)
            : base(providerBuilder)
        {
            providerBuilder.WithItems(container.Items);
        }

        IEnumerable<SampleItemDto> ISampleDataProvider.GetItems() => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).GetItems();

        bool ISampleDataProvider.DeleteItem(Guid id) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).DeleteItem(id);

        bool ISampleDataProvider.UpdateItem(SampleItemDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).UpdateItem(dto);

        void ISampleDataProvider.CreateItem(SampleItemDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).CreateItem(dto);
    }
}