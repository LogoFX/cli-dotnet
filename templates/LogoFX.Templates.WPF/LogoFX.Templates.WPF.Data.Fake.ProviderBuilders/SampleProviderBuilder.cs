using System;
using System.Collections.Generic;
using System.Linq;
using Attest.Fake.Builders;
using Attest.Fake.Core;
using Attest.Fake.Setup.Contracts;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Data.Contracts.Providers;

namespace LogoFX.Templates.WPF.Data.Fake.ProviderBuilders
{
    public sealed class SampleProviderBuilder : FakeBuilderBase<ISampleDataProvider>.WithInitialSetup
    {
        private readonly List<SampleItemDto> _itemsStorage = new List<SampleItemDto>();

        private SampleProviderBuilder()
        {

        }

        public static SampleProviderBuilder CreateBuilder() => new SampleProviderBuilder();

        public void WithItems(IEnumerable<SampleItemDto> items)
        {
            _itemsStorage.Clear();
            _itemsStorage.AddRange(items);
        }

        protected override IServiceCall<ISampleDataProvider> CreateServiceCall(
            IHaveNoMethods<ISampleDataProvider> serviceCallTemplate) => serviceCallTemplate
            .AddMethodCallWithResult(t => t.GetItems(),
                r => r.Complete(GetItems))
            .AddMethodCallWithResult<Guid, bool>(t => t.DeleteItem(It.IsAny<Guid>()),
                (r, id) => r.Complete(DeleteItem(id)))
            .AddMethodCallWithResult<SampleItemDto, bool>(t => t.UpdateItem(It.IsAny<SampleItemDto>()),
                (r, dto) => r.Complete(k =>
                {
                    SaveItem(k);
                    return true;
                }))
            .AddMethodCall<SampleItemDto>(t => t.CreateItem(It.IsAny<SampleItemDto>()),
                (r, dto) => r.Complete(SaveItem));

        private IEnumerable<SampleItemDto> GetItems() => _itemsStorage;

        private bool DeleteItem(Guid id)
        {
            var dto = _itemsStorage.SingleOrDefault(x => x.Id == id);
            return dto != null && _itemsStorage.Remove(dto);
        }

        private void SaveItem(SampleItemDto dto)
        {
            var oldDto = _itemsStorage.SingleOrDefault(x => x.Id == dto.Id);
            if (oldDto == null)
            {
                _itemsStorage.Add(dto);
                return;
            }

            oldDto.DisplayName = dto.DisplayName;
            oldDto.Value = dto.Value;
        }
    }
}