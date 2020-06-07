using LogoFX.Templates.Model.Contracts;
#if IN_PROJECT
using LogoFX.Templates.WPF.Model;
#endif

namespace LogoFX.Templates.Model
{
    internal class SampleModel : AppModel, ISampleModel
    {
        private string _displayName;

        public string DisplayName
        {
            get => _displayName;
            set => SetProperty(ref _displayName, value);
        }

        private int _value;

        public int Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }
    }
}