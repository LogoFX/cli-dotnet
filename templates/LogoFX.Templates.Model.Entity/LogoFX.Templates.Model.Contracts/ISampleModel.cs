#if IN_PROJECT
using LogoFX.Templates.WPF.Model.Contracts;
#endif
namespace LogoFX.Templates.Model.Contracts
{
    public interface ISampleModel : IAppModel
    {
        string DisplayName { get; }

        int Value { get; set; }
    }
}