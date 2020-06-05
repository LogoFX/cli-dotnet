using System.IO;

namespace ModifyTool
{
    internal sealed partial class FakeProviderEngine
    {
        private const string ModuleFileName = "Module.cs";
        private const string ModuleClassName = "Module";

        public void RegisterProvider(string entityName)
        {
            var moduleFilePath = Path.Combine(GetProjectFolder(), ModuleFileName);

            if (!File.Exists(moduleFilePath))
            {
                var helper = new ResourceHelper("Provider", ModuleFileName);

                using var stream = helper.GetResourceStream();
                using (var fileStream = File.Create(moduleFilePath))
                {
                    stream.CopyTo(fileStream);
                }

                ReplaceSolutionName(moduleFilePath);
            }

        }
    }
}