using AutoMapper;
using JetBrains.Annotations;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Model.Contracts;

namespace LogoFX.Templates.WPF.Model.Mappers
{
    [UsedImplicitly]
    internal sealed class WarehouseMapper
    {
        private readonly IMapper _mapper;

        public WarehouseMapper(IMapper mapper) => _mapper = mapper;

        public IWarehouseItem MapToWarehouseItem(WarehouseItemDto warehouseItemDto) => 
            _mapper.Map<IWarehouseItem>(warehouseItemDto);

        public WarehouseItemDto MapToWarehouseDto(IWarehouseItem warehouseItem) =>
            _mapper.Map<WarehouseItemDto>(warehouseItem);

    }
}