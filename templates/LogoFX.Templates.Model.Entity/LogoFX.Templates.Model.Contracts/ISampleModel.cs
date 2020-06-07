namespace LogoFX.Templates.Model.Contracts
{
    public interface ISampleModel : IAppModel
    {
        string DisplayName { get; }

        int Value { get; set; }
    }
}