using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogoFX.Templates.WPF.Model.Contracts
{
    public interface IDataService
    {
        IEnumerable<ISampleItem> Items { get; }

        Task GetItems();

        Task<ISampleItem> NewItem();

        Task SaveItem(ISampleItem item);

        Task DeleteItem(ISampleItem item);
    }
}