using System;
using System.Collections.Generic;
using Samples.Basics.Data.Contracts.Dto;
using Samples.Basics.Data.Contracts.Providers;

namespace Samples.Basics.Data.Real.Providers
{
    internal sealed class WarehouseProvider : IWarehouseProvider
    {
        public IEnumerable<WarehouseItemDto> GetWarehouseItems()
        {
            throw new NotImplementedException();
        }

        public bool DeleteWarehouseItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateWarehouseItem(WarehouseItemDto dto)
        {
            throw new NotImplementedException();
        }

        public void CreateWarehouseItem(WarehouseItemDto dto)
        {
            throw new NotImplementedException();
        }
    }
}