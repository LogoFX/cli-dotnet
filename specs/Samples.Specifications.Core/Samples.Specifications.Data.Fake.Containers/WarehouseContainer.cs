using System.Collections.Generic;
using Samples.Specifications.Data.Contracts.Dto;
using Samples.Specifications.Data.Fake.Containers.Contracts;

namespace Samples.Specifications.Data.Fake.Containers
{
    public interface IWarehouseContainer : IDataContainer
    {
        IEnumerable<WarehouseItemDto> WarehouseItems { get; }
    }

    public sealed class WarehouseContainer : IWarehouseContainer
    {
        private readonly List<WarehouseItemDto> _warehouseItems = new List<WarehouseItemDto>();
        public IEnumerable<WarehouseItemDto> WarehouseItems => _warehouseItems;

        public void UpdateWarehouseItems(IEnumerable<WarehouseItemDto> warehouseItems)
        {
            _warehouseItems.Clear();
            _warehouseItems.AddRange(warehouseItems);
        }
    }   
}
