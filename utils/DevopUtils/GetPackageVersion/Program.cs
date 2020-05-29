using System;
using System.IO;
using Common.Infra;

namespace GetPackageVersion
{
    internal static class Program
    {
        private const string OpenKey = "<PackageVersion>";
        private const string CloseKey = "</PackageVersion>";

        private static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Usage: GetPackageVersion <path to templatepack.csproj>");
                return ReturnCode.IncorrectFunction;
            }

            var fullName = args[0];
            fullName = Path.GetFullPath(fullName);

            if (!File.Exists(fullName))
            {
                Console.WriteLine($"File '{fullName}' not found.");
                return ReturnCode.Error;
            }

            var lines = File.ReadAllLines(fullName);

            foreach (var line in lines)
            {
                var index1 = line.IndexOf(OpenKey, StringComparison.OrdinalIgnoreCase);
                if (index1 < 0)
                {
                    continue;
                }

                var index2 = line.IndexOf(CloseKey, StringComparison.OrdinalIgnoreCase);
                if (index2 < 0)
                {
                    Console.WriteLine($"File '{fullName}' corrupted.");
                    return ReturnCode.Error;
                }

                var packageVersion = line.Substring(index1 + OpenKey.Length, index2 - index1 - OpenKey.Length);
                Console.WriteLine(packageVersion);
                return ReturnCode.Successful;
            }

            Console.WriteLine($"In file '{fullName}' package version not found.");
            return ReturnCode.Error;
        }
    }
}
