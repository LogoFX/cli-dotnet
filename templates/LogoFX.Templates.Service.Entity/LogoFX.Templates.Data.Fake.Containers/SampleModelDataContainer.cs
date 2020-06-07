using System.Collections.Generic;
using LogoFX.Templates.Data.Contracts.Dto;
#if IN_PROJECT
using LogoFX.Templates.WPF.Data.Fake.Containers.Contracts;
#else
using LogoFX.Templates.Data.Fake.Containers.Contracts;
#endif

namespace LogoFX.Templates.Data.Fake.Containers
{
    public interface ISampleModelDataContainer : IDataContainer
    {
        IEnumerable<SampleModelDto> Items { get; }
    }

    public sealed class SampleModelDataContainer : ISampleModelDataContainer
    {
        private readonly List<SampleModelDto> _items = new List<SampleModelDto>();
        public IEnumerable<SampleModelDto> Items => _items;

        public void UpdateItems(IEnumerable<SampleModelDto> items)
        {
            _items.Clear();
            _items.AddRange(items);
        }
    }
}