using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using McMaster.NETCore.Plugins;

namespace LogoFX.Templates.WPF.Launcher
{
    public static class AssemblyLoader
    {
        public static IEnumerable<Assembly> Get(IEnumerable<string> paths)
        {
            return paths.Select(path =>
                PluginLoader.CreateFromAssemblyFile(assemblyFile:
                    Path.Combine(Directory.GetCurrentDirectory(), path),
                    t => t.PreferSharedTypes = true
                ).LoadDefaultAssembly());
        }
    }
}