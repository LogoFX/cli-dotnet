using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LogoFX.Client.Core;
using LogoFX.Core;
using LogoFX.Templates.WPF.Data.Contracts.Providers;
using LogoFX.Templates.WPF.Model.Contracts;
using LogoFX.Templates.WPF.Model.Mappers;

namespace LogoFX.Templates.WPF.Model
{
    [UsedImplicitly]
    internal sealed class DataService : NotifyPropertyChangedBase<DataService>, IDataService
    {
        private readonly ISampleProvider _sampleProvider;
        private readonly SampleMapper _sampleMapper;

        private readonly RangeObservableCollection<ISampleItem> _items =
            new RangeObservableCollection<ISampleItem>();

        public DataService(ISampleProvider sampleProvider, SampleMapper sampleMapper)
        {
            _sampleProvider = sampleProvider;
            _sampleMapper = sampleMapper;
        }

        IEnumerable<ISampleItem> IDataService.Items => _items;

        Task IDataService.GetItems() => MethodRunner.RunAsync(Method);

        private void Method()
        {
            var items = _sampleProvider.GetItems().Select(_sampleMapper.MapToSampleItem);
            _items.Clear();
            _items.AddRange(items);
        }

        Task<ISampleItem> IDataService.NewItem() => MethodRunner.RunWithResultAsync<ISampleItem>(() =>
            new SampleItem("New Item", 1)
            {
                IsNew = true
            });

        public Task SaveItem(ISampleItem item) => MethodRunner.RunAsync(() =>
        {
            var dto = _sampleMapper.MapToSampleItemDto(item);

            if (item.IsNew)
            {
                _sampleProvider.CreateItem(dto);
            }
            else
            {
                _sampleProvider.UpdateItem(dto);
            }
        });

        Task IDataService.DeleteItem(ISampleItem item) => MethodRunner.RunAsync(() =>
        {
            _sampleProvider.DeleteItem(item.Id);
            _items.Remove(item);
        });
    }
}