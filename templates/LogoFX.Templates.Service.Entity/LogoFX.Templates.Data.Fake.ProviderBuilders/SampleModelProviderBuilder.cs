using System;
using System.Collections.Generic;
using System.Linq;
using Attest.Fake.Builders;
using Attest.Fake.Core;
using Attest.Fake.Setup.Contracts;
using LogoFX.Templates.Data.Contracts.Dto;
using LogoFX.Templates.Data.Contracts.Providers;

namespace LogoFX.Templates.Data.Fake.ProviderBuilders
{
    public sealed class SampleModelProviderBuilder : FakeBuilderBase<ISampleModelDataProvider>.WithInitialSetup
    {
        private readonly List<SampleModelDto> _itemsStorage = new List<SampleModelDto>();

        private SampleModelProviderBuilder()
        {

        }

        public static SampleModelProviderBuilder CreateBuilder() => new SampleModelProviderBuilder();

        public void WithItems(IEnumerable<SampleModelDto> items)
        {
            _itemsStorage.Clear();
            _itemsStorage.AddRange(items);
        }

        protected override IServiceCall<ISampleModelDataProvider> CreateServiceCall(
            IHaveNoMethods<ISampleModelDataProvider> serviceCallTemplate) => serviceCallTemplate
            .AddMethodCallWithResult(t => t.GetItems(),
                r => r.Complete(GetItems))
            .AddMethodCallWithResult<Guid, bool>(t => t.DeleteItem(It.IsAny<Guid>()),
                (r, id) => r.Complete(DeleteItem(id)))
            .AddMethodCallWithResult<SampleModelDto, bool>(t => t.UpdateItem(It.IsAny<SampleModelDto>()),
                (r, dto) => r.Complete(k =>
                {
                    SaveItem(k);
                    return true;
                }))
            .AddMethodCall<SampleModelDto>(t => t.CreateItem(It.IsAny<SampleModelDto>()),
                (r, dto) => r.Complete(SaveItem));

        private IEnumerable<SampleModelDto> GetItems() => _itemsStorage;

        private bool DeleteItem(Guid id)
        {
            var dto = _itemsStorage.SingleOrDefault(x => x.Id == id);
            return dto != null && _itemsStorage.Remove(dto);
        }

        private void SaveItem(SampleModelDto dto)
        {
            var oldDto = _itemsStorage.SingleOrDefault(x => x.Id == dto.Id);
            if (oldDto == null)
            {
                _itemsStorage.Add(dto);
            }
        }
    }
}