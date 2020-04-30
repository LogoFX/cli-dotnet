using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel;
using Samples.Basics.Model.Contracts;
using Samples.Basics.Presentation.Contracts;

namespace Samples.Basics.Presentation.Shell
{
    [UsedImplicitly]
    public sealed class WarehouseItemViewModel : ObjectViewModel<IWarehouseItem>, IWarehouseItemViewModel
    {
        public WarehouseItemViewModel(
            IWarehouseItem model) : base(model)
        {
        }
    }
}