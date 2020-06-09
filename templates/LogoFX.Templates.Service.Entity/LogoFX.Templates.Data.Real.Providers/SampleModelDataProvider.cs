using System;
using System.Collections.Generic;
using LogoFX.Templates.Data.Contracts.Dto;
using LogoFX.Templates.Data.Contracts.Providers;

namespace LogoFX.Templates.Data.Real.Providers
{
    //TODO: Use explicit implementation
    internal sealed class SampleModelDataProvider : ISampleModelDataProvider
    {
        public IEnumerable<SampleModelDto> GetItems()
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateItem(SampleModelDto dto)
        {
            throw new NotImplementedException();
        }

        public void CreateItem(SampleModelDto dto)
        {
            throw new NotImplementedException();
        }
    }
}