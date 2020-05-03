using System;
using System.Collections.Generic;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Data.Contracts.Providers;

namespace LogoFX.Templates.WPF.Data.Real.Providers
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