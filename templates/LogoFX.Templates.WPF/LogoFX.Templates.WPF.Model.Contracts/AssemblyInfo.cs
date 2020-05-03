using System.Reflection;

namespace LogoFX.Templates.WPF.Model.Contracts
{
    public static class AssemblyInfo
    {
        public static string AssemblyName { get; } = $"{Assembly.GetExecutingAssembly().GetName().Name}.dll";
    }
}