using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using LogoFX.Templates.Model.Contracts;

namespace LogoFX.Templates.Model
{
    public class SampleModelService : ISampleModelService
    {
        private ObservableCollection<SampleModel> _items;

        IEnumerable<ISampleModel> ISampleModelService.Items => _items;

        Task ISampleModelService.GetItemsAsync()
        {
            throw new System.NotImplementedException();
        }

        Task<ISampleModel> ISampleModelService.NewItemAsync()
        {
            throw new System.NotImplementedException();
        }

        Task ISampleModelService.SaveItemAsync(ISampleModel item)
        {
            throw new System.NotImplementedException();
        }

        Task ISampleModelService.DeleteItemAsync(ISampleModel item)
        {
            throw new System.NotImplementedException();
        }
    }
}