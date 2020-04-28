using System.ComponentModel;
using Caliburn.Micro;
using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel.Services;
using Samples.Specifications.Presentation.Contracts.Shell;

namespace Samples.Specifications.Presentation.Shell
{
    [UsedImplicitly]
    public class ShellViewModel : Conductor<INotifyPropertyChanged>.Collection.OneActive, IShellViewModel
    {
        private readonly IViewModelCreatorService _viewModelCreatorService;

        public ShellViewModel(IViewModelCreatorService viewModelCreatorService)
        {
            _viewModelCreatorService = viewModelCreatorService;
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            ActivateItem(_viewModelCreatorService.CreateViewModel<MainViewModel>());
        }
    }
}