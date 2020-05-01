using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UninstallTemplate
{
    class Program
    {
        private enum TemplateNameKind
        {
            None,
            Name,
            ShortName,
            Directory
        }

        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowUsage();
                return;
            }

            var kind = GetTemplateNameKind(args[0]);
            if (kind == TemplateNameKind.None)
            {
                ShowUsage();
                return;
            }

            var name = string.Join(' ', args.Skip(1));
            var lines = LaunchApp("dotnet", "new", "-u");

            var uninstallString = FindUninstallString(lines, name, kind);

            if (string.IsNullOrEmpty(uninstallString))
            {
                return;
            }

            var strings = uninstallString.Split(' ');
            lines = LaunchApp(strings[0], strings.Skip(1).ToArray());
            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }
        }

        private static string FindUninstallString(string[] lines, string name, TemplateNameKind kind)
        {
            return @"dotnet new -u c:\Projects\LogoFX\cli-dotnet\templates\LogoFX.Templates.WPF";
        }

        private static string[] LaunchApp(string appName, params string[] args)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = appName,
                    Arguments = string.Join(' ', args),
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                }
            };

            var lines = new List<string>();

            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                lines.Add(line);
            }

            return lines.ToArray();
        }

        private static TemplateNameKind GetTemplateNameKind(string str)
        {
            if (str.Length != 2)
            {
                return TemplateNameKind.None;
            }

            if (str[0] != '-')
            {
                return TemplateNameKind.None;
            }

            switch (str[1])
            {
                case 'n':
                case 'N':
                    return TemplateNameKind.Name;
                case 's':
                case 'S':
                    return TemplateNameKind.ShortName;
                case 'd':
                case 'D':
                    return TemplateNameKind.Directory;
                default:
                    return TemplateNameKind.None;
            }
        }

        private static void ShowUsage()
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
