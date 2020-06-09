using System;
using System.Collections.Generic;
using LogoFX.Templates.WPF.Data.Contracts.Dto;

namespace LogoFX.Templates.WPF.Data.Contracts.Providers
{
    public interface ISampleDataProvider
    {
        IEnumerable<SampleItemDto> GetItems();
        bool DeleteItem(Guid id);
        bool UpdateItem(SampleItemDto dto);
        void CreateItem(SampleItemDto dto);
    }
}
