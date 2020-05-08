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
    internal sealed class FakeSampleProvider : FakeProviderBase<SampleProviderBuilder, ISampleProvider>, ISampleProvider
    {
        private readonly Random _random = new Random();

        public FakeSampleProvider(
            SampleProviderBuilder sampleProviderBuilder,
            ISampleContainer sampleContainer)
            : base(sampleProviderBuilder)
        {
            sampleProviderBuilder.WithItems(sampleContainer.Items);
        }

        IEnumerable<SampleItemDto> ISampleProvider.GetItems() => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).GetItems();

        bool ISampleProvider.DeleteItem(Guid id) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).DeleteItem(id);

        bool ISampleProvider.UpdateItem(SampleItemDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).UpdateItem(dto);

        void ISampleProvider.CreateItem(SampleItemDto dto) => GetService(r =>
        {
            Task.Delay(_random.Next(2000)).Wait();
            return r;
        }).CreateItem(dto);
    }
}