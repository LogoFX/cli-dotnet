using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LogoFX.Client.Core;
using LogoFX.Core;
using Samples.Basics.Data.Contracts.Providers;
using Samples.Basics.Model.Contracts;
using Samples.Basics.Model.Mappers;

namespace Samples.Basics.Model
{
    [UsedImplicitly]
    internal sealed class DataService : NotifyPropertyChangedBase<DataService>, IDataService
    {
        private readonly IWarehouseProvider _warehouseProvider;
        private readonly WarehouseMapper _warehouseMapper;

        private readonly RangeObservableCollection<IWarehouseItem> _warehouseItems =
            new RangeObservableCollection<IWarehouseItem>();

        public DataService(IWarehouseProvider warehouseProvider, WarehouseMapper warehouseMapper)
        {
            _warehouseProvider = warehouseProvider;
            _warehouseMapper = warehouseMapper;
        }

        IEnumerable<IWarehouseItem> IDataService.WarehouseItems => _warehouseItems;

        Task IDataService.GetWarehouseItems() => MethodRunner.RunAsync(Method);

        private void Method()
        {
            var warehouseItems = _warehouseProvider.GetWarehouseItems().Select(_warehouseMapper.MapToWarehouseItem);
            _warehouseItems.Clear();
            _warehouseItems.AddRange(warehouseItems);
        }

        Task<IWarehouseItem> IDataService.NewWarehouseItem() => MethodRunner.RunWithResultAsync<IWarehouseItem>(() =>
            new WarehouseItem("New Kind", 0d, 1)
            {
                IsNew = true
            });

        public Task SaveWarehouseItem(IWarehouseItem item) => MethodRunner.RunAsync(() =>
        {
            var dto = _warehouseMapper.MapToWarehouseDto(item);

            if (item.IsNew)
            {
                _warehouseProvider.CreateWarehouseItem(dto);
            }
            else
            {
                _warehouseProvider.UpdateWarehouseItem(dto);
            }
        });

        Task IDataService.DeleteWarehouseItem(IWarehouseItem item) => MethodRunner.RunAsync(() =>
        {
            _warehouseProvider.DeleteWarehouseItem(item.Id);
            _warehouseItems.Remove(item);
        });
    }
}