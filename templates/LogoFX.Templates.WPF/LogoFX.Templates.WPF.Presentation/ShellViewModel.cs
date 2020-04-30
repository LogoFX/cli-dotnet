using System.ComponentModel;
using System.Windows.Input;
using Caliburn.Micro;
using JetBrains.Annotations;
using LogoFX.Client.Mvvm.Commanding;
using LogoFX.Client.Mvvm.ViewModel.Services;
using LogoFX.Templates.WPF.Presentation.Contracts;

namespace LogoFX.Templates.WPF.Presentation.Shell
{
    [UsedImplicitly]
    public class ShellViewModel : Conductor<INotifyPropertyChanged>.Collection.OneActive, IShellViewModel
    {
        private readonly IViewModelCreatorService _viewModelCreatorService;

        public ShellViewModel(IViewModelCreatorService viewModelCreatorService)
        {
            _viewModelCreatorService = viewModelCreatorService;
        }
        
        private ICommand _closeCommand;

        public ICommand CloseCommand => _closeCommand ??= ActionCommand
            .When(() => true)
            .Do(() =>
            {
                TryClose();
            });


        protected override void OnActivate()
        {
            base.OnActivate();

            ActivateItem(_viewModelCreatorService.CreateViewModel<MainViewModel>());
        }
    }
}