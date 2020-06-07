using System;
using System.IO;
using Common.Infra;

namespace GetProjValue
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: GetPackageVersion <path to templatepack.csproj> <key>");
                return ReturnCode.IncorrectFunction;
            }

            var fullName = args[0];
            fullName = Path.GetFullPath(fullName);

            if (!File.Exists(fullName))
            {
                Console.WriteLine($"File '{fullName}' not found.");
                return ReturnCode.Error;
            }

            var key = args[1];
            var openKey = $"<{key}>";
            var closeKey = $"</{key}>";

            var lines = File.ReadAllLines(fullName);

            foreach (var line in lines)
            {
                var index1 = line.IndexOf(openKey, StringComparison.OrdinalIgnoreCase);
                if (index1 < 0)
                {
                    continue;
                }

                var index2 = line.IndexOf(closeKey, StringComparison.OrdinalIgnoreCase);
                if (index2 < 0)
                {
                    Console.WriteLine($"File '{fullName}' corrupted.");
                    return ReturnCode.Error;
                }

                var packageVersion = line.Substring(index1 + openKey.Length, index2 - index1 - openKey.Length);
                Console.WriteLine(packageVersion);
                return ReturnCode.Successful;
            }

            Console.WriteLine($"In file '{fullName}' package version not found.");
            return ReturnCode.Error;
        }
    }
}
