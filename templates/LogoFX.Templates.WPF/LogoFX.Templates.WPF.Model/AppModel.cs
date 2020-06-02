using LogoFX.Client.Mvvm.Model;
using LogoFX.Templates.WPF.Model.Contracts;

namespace LogoFX.Templates.WPF.Model
{    
    internal abstract class AppModel : EditableModel<string>, IAppModel
    {        
        public bool IsNew { get; set; }
    }
}