using System;
using AutoMapper;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Model.Contracts;

namespace LogoFX.Templates.WPF.Model.Mappers
{
    internal sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateCameraMaps();
        }

        private void CreateCameraMaps()
        {
            CreateDomainObjectMap<WarehouseItemDto, IWarehouseItem, WarehouseItem>();
        }

        private void CreateDomainObjectMap<TDto, TContract, TModel>()
            where TModel : TContract
            where TContract : class => CreateDomainObjectMap(typeof(TDto), typeof(TContract), typeof(TModel));

        private void CreateDomainObjectMap(Type dtoType, Type contractType, Type modelType)
        {
            CreateMap(dtoType, contractType).As(modelType);
            CreateMap(dtoType, modelType);
            CreateMap(contractType, dtoType);
            CreateMap(modelType, dtoType);
        }
    }
}