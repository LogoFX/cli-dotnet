using System.Reflection;

namespace Samples.Basics.Data.Contracts.Providers
{
    public static class AssemblyInfo
    {
        public static string AssemblyName { get; } = $"{Assembly.GetExecutingAssembly().GetName().Name}.dll";
    }
}