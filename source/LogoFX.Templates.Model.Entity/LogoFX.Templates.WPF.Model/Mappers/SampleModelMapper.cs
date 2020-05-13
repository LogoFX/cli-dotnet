using AutoMapper;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Model.Contracts;

namespace LogoFX.Templates.WPF.Model.Mappers
{
    internal sealed class SampleModelMapper
    {
        private readonly IMapper _mapper;

        public SampleModelMapper(IMapper mapper) => _mapper = mapper;

        public ISampleModel MapToCameraValue(SampleModelDto sampleModelDto) =>
            _mapper.Map<ISampleModel>(sampleModelDto);
    }
}