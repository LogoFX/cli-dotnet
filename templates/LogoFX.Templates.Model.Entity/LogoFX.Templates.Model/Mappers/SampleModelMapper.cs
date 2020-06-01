using AutoMapper;
using LogoFX.Templates.Data.Contracts.Dto;
using LogoFX.Templates.Model.Contracts;

namespace LogoFX.Templates.Model.Mappers
{
    internal sealed class SampleModelMapper
    {
        private readonly IMapper _mapper;

        public SampleModelMapper(IMapper mapper) => _mapper = mapper;

        public ISampleModel MapToSampleModelValue(SampleModelDto dto) =>
            _mapper.Map<ISampleModel>(dto);
    }
}