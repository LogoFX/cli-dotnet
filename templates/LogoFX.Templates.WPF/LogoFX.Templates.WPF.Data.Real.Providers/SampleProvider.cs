using System;
using System.Collections.Generic;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Data.Contracts.Providers;

namespace LogoFX.Templates.WPF.Data.Real.Providers
{
    internal sealed class SampleProvider : ISampleProvider
    {
        public IEnumerable<SampleItemDto> GetItems()
        {
            throw new NotImplementedException();
        }

        public bool DeleteItem(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool UpdateItem(SampleItemDto dto)
        {
            throw new NotImplementedException();
        }

        public void CreateItem(SampleItemDto dto)
        {
            throw new NotImplementedException();
        }
    }
}