using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogoFX.Templates.Model.Contracts
{
    public interface ISampleModelService
    {
        IEnumerable<ISampleModel> Items { get; }

        Task GetItemsAsync();

        Task<ISampleModel> NewItemAsync();

        Task SaveItemAsync(ISampleModel item);

        Task DeleteItemAsync(ISampleModel item);
    }
}
