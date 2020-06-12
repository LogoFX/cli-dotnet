using AutoMapper;
using JetBrains.Annotations;
using LogoFX.Templates.Data.Contracts.Dto;
using LogoFX.Templates.Model.Contracts;

namespace LogoFX.Templates.Model.Mappers
{
    [UsedImplicitly]
    internal sealed class SampleModelMapper
    {
        private readonly IMapper _mapper;

        public SampleModelMapper(IMapper mapper) => _mapper = mapper;

        public ISampleModel MapToSampleModel(SampleModelDto dto) =>
            _mapper.Map<ISampleModel>(dto);

        public SampleModelDto MapFromSampleModel(ISampleModel model) =>
            _mapper.Map<SampleModelDto>(model);
    }
}