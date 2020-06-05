namespace ModifyTool
{
    internal sealed partial class FakeProviderEngine : EngineBase
    {
        public FakeProviderEngine(string solutionFolder) 
            : base(solutionFolder)
        {
        }

        protected override string GetProjectSuffix() => "Data.Fake.Providers";
    }
}