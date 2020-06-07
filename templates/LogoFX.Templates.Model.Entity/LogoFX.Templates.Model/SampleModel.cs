using LogoFX.Templates.Model.Contracts;

namespace LogoFX.Templates.Model
{
    public class SampleModel : AppModel, ISampleModel
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