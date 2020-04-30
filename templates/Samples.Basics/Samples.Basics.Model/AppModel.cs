using System;
using LogoFX.Client.Mvvm.Model;
using Samples.Basics.Model.Contracts;


namespace Samples.Basics.Model
{    
    internal abstract class AppModel : EditableModel<Guid>, IAppModel
    {        
        public bool IsNew { get; set; }
    }
}
