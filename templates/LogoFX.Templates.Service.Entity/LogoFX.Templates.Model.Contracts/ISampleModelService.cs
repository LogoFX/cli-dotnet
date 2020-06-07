using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogoFX.Templates.Model.Contracts
{
    public interface ISampleModelService
    {
        IEnumerable<ISampleModel> Items { get; }

        Task GetItems();

        Task<ISampleModel> NewItem();

        Task SaveItem(ISampleModel item);

        Task DeleteItem(ISampleModel item);
    }
}