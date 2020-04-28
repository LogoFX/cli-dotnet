using System;
using System.Collections.Generic;
using Samples.Specifications.Data.Contracts.Dto;
using Samples.Specifications.Data.Contracts.Providers;

namespace Samples.Specifications.Data.Real.Providers
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