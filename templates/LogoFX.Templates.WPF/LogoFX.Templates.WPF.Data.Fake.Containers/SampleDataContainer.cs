using System.Collections.Generic;
using LogoFX.Templates.WPF.Data.Contracts.Dto;
using LogoFX.Templates.WPF.Data.Fake.Containers.Contracts;

namespace LogoFX.Templates.WPF.Data.Fake.Containers
{
    public interface ISampleDataContainer : IDataContainer
    {
        IEnumerable<SampleItemDto> Items { get; }
    }

    public sealed class SampleDataContainer : ISampleDataContainer
    {
        private readonly List<SampleItemDto> _items = new List<SampleItemDto>();
        public IEnumerable<SampleItemDto> Items => _items;

        public void UpdateItems(IEnumerable<SampleItemDto> items)
        {
            _items.Clear();
            _items.AddRange(items);
        }
    }   
}
