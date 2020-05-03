using System;
using System.Collections.Generic;
using LogoFX.Templates.WPF.Data.Contracts.Dto;

namespace LogoFX.Templates.WPF.Data.Contracts.Providers
{
    public interface IWarehouseProvider
    {
        IEnumerable<WarehouseItemDto> GetWarehouseItems();
        bool DeleteWarehouseItem(Guid id);
        bool UpdateWarehouseItem(WarehouseItemDto dto);
        void CreateWarehouseItem(WarehouseItemDto dto);
    }
}
