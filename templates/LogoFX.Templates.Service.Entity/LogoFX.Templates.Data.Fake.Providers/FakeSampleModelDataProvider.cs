using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Attest.Fake.Builders;
using JetBrains.Annotations;
using LogoFX.Templates.Data.Contracts.Dto;
using LogoFX.Templates.Data.Contracts.Providers;
using LogoFX.Templates.Data.Fake.Containers;
using LogoFX.Templates.Data.Fake.ProviderBuilders;

namespace LogoFX.Templates.Data.Fake.Providers
{
    [UsedImplicitly]
    internal sealed class FakeSampleModelDataProvider : FakeProviderBase<SampleModelProviderBuilder, ISampleModelDataProvider>, ISampleModelDataProvider
    {
        private readonly Random _random = new Random();

        public FakeSampleModelDataProvider(
            SampleModelProviderBuilder sampleProviderBuilder,
            ISampleModelDataContainer sampleContainer)
            : base(sampleProviderBuilder)
        {
            sampleProviderBuilder.WithItems(sampleContainer.Items);
        }

        IEnumerable<SampleModelDto> ISampleModelDataProvider.GetItems() => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).GetItems();

        bool ISampleModelDataProvider.DeleteItem(Guid id) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).DeleteItem(id);

        bool ISampleModelDataProvider.UpdateItem(SampleModelDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).UpdateItem(dto);

        void ISampleModelDataProvider.CreateItem(SampleModelDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).CreateItem(dto);
    }
}