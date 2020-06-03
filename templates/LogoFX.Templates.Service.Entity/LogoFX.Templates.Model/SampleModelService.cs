using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LogoFX.Client.Core;
using LogoFX.Core;
using LogoFX.Templates.Data.Contracts.Providers;
using LogoFX.Templates.Model.Contracts;
using LogoFX.Templates.Model.Mappers;
#if IN_PROJECT
using LogoFX.Templates.WPF.Model;
#endif

namespace LogoFX.Templates.Model
{
    [UsedImplicitly]
    internal sealed class SampleModelService : NotifyPropertyChangedBase<SampleModelService>, ISampleModelService
    {
        private readonly ISampleModelDataProvider _provider;
        private readonly SampleModelMapper _mapper;

        private readonly RangeObservableCollection<ISampleModel> _items =
            new RangeObservableCollection<ISampleModel>();

        public SampleModelService(ISampleModelDataProvider provider, SampleModelMapper mapper)
        {
            _provider = provider;
            _mapper = mapper;
        }

        private void GetItems()
        {
            var items = _provider.GetItems().Select(_mapper.MapToSampleModel);
            _items.Clear();
            _items.AddRange(items);
        }

        IEnumerable<ISampleModel> ISampleModelService.Items => _items;

        Task ISampleModelService.GetItems() => MethodRunner.RunAsync(GetItems);

        Task<ISampleModel> ISampleModelService.NewItem() => MethodRunner.RunWithResultAsync<ISampleModel>(() => new SampleModel());

        Task ISampleModelService.SaveItem(ISampleModel item) => MethodRunner.RunAsync(() =>
        {
            var dto = _mapper.MapToSampleModelDto(item);

            if (item.IsNew)
            {
                _provider.CreateItem(dto);
            }
            else
            {
                _provider.UpdateItem(dto);
            }
        });

        Task ISampleModelService.DeleteItem(ISampleModel item) => MethodRunner.RunAsync(() =>
        {
            _provider.DeleteItem(item.Id);
            _items.Remove(item);
        });
    }
}