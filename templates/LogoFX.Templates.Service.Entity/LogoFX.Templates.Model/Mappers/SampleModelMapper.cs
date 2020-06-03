using LogoFX.Templates.Data.Contracts.Dto;
using LogoFX.Templates.Model.Contracts;

namespace LogoFX.Templates.Model.Mappers
{
    public abstract class SampleModelMapper
    {
        public abstract ISampleModel MapToSampleModel(SampleModelDto dto);

        public abstract SampleModelDto MapToSampleModelDto(ISampleModel model);
    }
}