using System;
using System.Collections.Generic;
using LogoFX.Templates.Data.Contracts.Dto;

namespace LogoFX.Templates.Data.Contracts.Providers
{
    public interface ISampleModelDataProvider
    {
        IEnumerable<SampleModelDto> GetItems();

        bool DeleteItem(Guid id);

        bool UpdateItem(SampleModelDto dto);

        void CreateItem(SampleModelDto dto);
    }
}