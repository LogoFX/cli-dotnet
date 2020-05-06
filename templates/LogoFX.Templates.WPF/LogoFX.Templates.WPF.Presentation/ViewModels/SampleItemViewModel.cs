using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel;
using LogoFX.Templates.WPF.Model.Contracts;
using LogoFX.Templates.WPF.Presentation.Contracts;

namespace LogoFX.Templates.WPF.Presentation.Shell.ViewModels
{
    [UsedImplicitly]
    public sealed class SampleItemViewModel : ObjectViewModel<ISampleItem>, ISampleItemViewModel
    {
        public SampleItemViewModel(
            ISampleItem model) : base(model)
        {
        }
    }
}