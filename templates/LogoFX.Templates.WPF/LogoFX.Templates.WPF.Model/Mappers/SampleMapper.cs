using AutoMapper;
using JetBrains.Annotations;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Model.Contracts;

namespace LogoFX.Templates.WPF.Model.Mappers
{
    [UsedImplicitly]
    internal sealed class SampleMapper
    {
        private readonly IMapper _mapper;

        public SampleMapper(IMapper mapper) => _mapper = mapper;

        public ISampleItem MapToSampleItem(SampleItemDto sampleItemDto) => 
            _mapper.Map<ISampleItem>(sampleItemDto);

        public SampleItemDto MapToSampleItemDto(ISampleItem sampleItem) =>
            _mapper.Map<SampleItemDto>(sampleItem);

    }
}