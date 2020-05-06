using System;
using JetBrains.Annotations;
using LogoFX.Templates.WPF.Model.Contracts;

namespace LogoFX.Templates.WPF.Model
{    
    [UsedImplicitly]
    internal sealed class SampleItem : AppModel, ISampleItem
    {
        public SampleItem(string displayName, int value)
        {
            Id = Guid.NewGuid();
            _displayName = displayName;
            _value = value;
        }

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
