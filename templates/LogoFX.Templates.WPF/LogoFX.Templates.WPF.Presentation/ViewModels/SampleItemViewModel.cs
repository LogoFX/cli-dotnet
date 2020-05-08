using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LogoFX.Client.Mvvm.ViewModel.Extensions;
using LogoFX.Templates.WPF.Model.Contracts;
using LogoFX.Templates.WPF.Presentation.Contracts;

namespace LogoFX.Templates.WPF.Presentation.Shell.ViewModels
{
    [UsedImplicitly]
    public sealed class SampleItemViewModel : EditableObjectViewModel<ISampleItem>, ISampleItemViewModel
    {
        public SampleItemViewModel(
            ISampleItem model) : base(model)
        {
        }

        protected override async Task<bool> SaveMethod(ISampleItem model)
        {
            IsBusy = true;

            try
            {
                //Delay imitation
                await Task.Delay(4000);
                return true;
            }
            catch (Exception err)
            {
                Debug.WriteLine(err.Message);
                return false;
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}