using System;
using LogoFX.Client.Mvvm.Model;
using Samples.Specifications.Model.Contracts;


namespace Samples.Specifications.Model
{    
    internal abstract class AppModel : EditableModel<Guid>, IAppModel
    {        
        public bool IsNew { get; set; }
    }
}
