using System.IO;
using System.Reflection;

namespace ModifyTool
{
    internal sealed class ResourceHelper
    {
        private const string ExtraFolderName = "Extra";

        private readonly string _resourceName;

        public ResourceHelper(string resourceName)
        {
            _resourceName = resourceName;
        }

        public Stream GetResourceStream()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name;
            var resource = assembly.GetManifestResourceStream($"{name}.{ExtraFolderName}.{_resourceName}");
            return resource;
        }
    }
}