namespace LogoFX.Templates.WPF.Model.Contracts
{
    public interface ISampleItem : IAppModel
    {
        string DisplayName { get; set; }   
        int Value { get; set; }
    }
}