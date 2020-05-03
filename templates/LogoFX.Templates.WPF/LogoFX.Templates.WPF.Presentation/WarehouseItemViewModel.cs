using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel;
using LogoFX.Templates.WPF.Model.Contracts;
using LogoFX.Templates.WPF.Presentation.Contracts;

namespace LogoFX.Templates.WPF.Presentation.Shell
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