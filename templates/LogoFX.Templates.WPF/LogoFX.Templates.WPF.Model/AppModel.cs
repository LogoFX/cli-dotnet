using System;
using LogoFX.Client.Mvvm.Model;
using LogoFX.Templates.WPF.Model.Contracts;

namespace LogoFX.Templates.WPF.Model
{
    internal abstract class AppModel : EditableModel<Guid>, IAppModel
    {
        public bool IsNew { get; set; }
    }
}