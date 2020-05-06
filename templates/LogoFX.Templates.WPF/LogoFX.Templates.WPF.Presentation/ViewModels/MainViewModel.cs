using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel;
using LogoFX.Client.Mvvm.ViewModel.Extensions;
using LogoFX.Client.Mvvm.ViewModel.Services;
using LogoFX.Templates.WPF.Model.Contracts;
using LogoFX.Templates.WPF.Presentation.Contracts;

namespace LogoFX.Templates.WPF.Presentation.Shell.ViewModels
{
    [UsedImplicitly]
    public sealed class MainViewModel : BusyScreen, IMainViewModel
    {
        private readonly IViewModelCreatorService _viewModelCreatorService;
        private readonly IDataService _dataService;

        public MainViewModel(
            IViewModelCreatorService viewModelCreatorService,
            IDataService dataService)
        {
            _viewModelCreatorService = viewModelCreatorService;
            _dataService = dataService;
        }

        private WrappingCollection.WithSelection _items;
        public WrappingCollection.WithSelection Items => _items ??= CreateItems();

        private WrappingCollection.WithSelection CreateItems()
        {
            var wc = new WrappingCollection.WithSelection
            {
                FactoryMethod = o =>
                    _viewModelCreatorService.CreateViewModel<ISampleItem, SampleItemViewModel>((ISampleItem) o)
            }.WithSource(_dataService.Items);

            return wc;
        }

        protected override async void OnInitialize()
        {
            base.OnInitialize();
            await _dataService.GetItems();
        }

    }
}