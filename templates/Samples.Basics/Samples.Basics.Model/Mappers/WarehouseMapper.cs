using AutoMapper;
using JetBrains.Annotations;
using Samples.Basics.Data.Contracts.Dto;
using Samples.Basics.Model.Contracts;

namespace Samples.Basics.Model.Mappers
{
    [UsedImplicitly]
    internal sealed class WarehouseItemMapper
    {
        private readonly IMapper _mapper;

        public WarehouseItemMapper(IMapper mapper) => _mapper = mapper;

        public IWarehouseItem MapToWarehouseItem(WarehouseItemDto warehouseItemDto) => 
            _mapper.Map<IWarehouseItem>(warehouseItemDto);

        public WarehouseItemDto MapToWarehouseDto(IWarehouseItem warehouseItem) =>
            _mapper.Map<WarehouseItemDto>(warehouseItem);

    }
}