using System.Collections.Generic;
using System.Threading.Tasks;

namespace LogoFX.Templates.WPF.Model.Contracts
{
    public interface IDataService
    {
        IEnumerable<IWarehouseItem> WarehouseItems { get; }

        Task GetWarehouseItems();

        Task<IWarehouseItem> NewWarehouseItem();

        Task SaveWarehouseItem(IWarehouseItem item);

        Task DeleteWarehouseItem(IWarehouseItem item);
    }
}