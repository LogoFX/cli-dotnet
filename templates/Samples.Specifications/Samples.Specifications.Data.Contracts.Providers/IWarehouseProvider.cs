using System;
using System.Collections.Generic;
using Samples.Specifications.Data.Contracts.Dto;

namespace Samples.Specifications.Data.Contracts.Providers
{
    public interface IWarehouseProvider
    {
        IEnumerable<WarehouseItemDto> GetWarehouseItems();
        bool DeleteWarehouseItem(Guid id);
        bool UpdateWarehouseItem(WarehouseItemDto dto);
        void CreateWarehouseItem(WarehouseItemDto dto);
    }
}
