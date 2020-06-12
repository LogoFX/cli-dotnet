using System;

namespace UninstallTemplate
{
    internal static class UsageHelper
    {
        internal static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    UninstallTemplate -n <template name>");
            Console.WriteLine("    or");
            Console.WriteLine("    UninstallTemplate -s <template short name>");
            Console.WriteLine("    or");
            Console.WriteLine("    UninstallTemplate -d <template source directory>");
        }
    }
}
