namespace ModifyTool
{
    internal sealed partial class ModelEngine : EngineBase
    {
        public ModelEngine(string solutionFolder)
            : base(solutionFolder)
        {
        }

        protected override string GetProjectSuffix() => "Model";
    }
}