using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Common.Infra;
using JetBrains.Annotations;

namespace UninstallTemplate
{
    [UsedImplicitly]
    internal sealed class Program
    {
        private enum TemplateNameKind
        {
            None,
            Name,
            ShortName,
            Directory
        }

        private static class ReturnCode
        {
            public const int Successful = 0;
            public const int IncorrectFunction = 1;
            public const int Error = -1;
        }

        private static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                ShowUsage();
                return ReturnCode.IncorrectFunction;
            }

            var kind = GetTemplateNameKind(args[0]);
            if (kind == TemplateNameKind.None)
            {
                ShowUsage();
                return ReturnCode.IncorrectFunction;
            }

            var name = string.Join(' ', args.Skip(1));
            var lines = LaunchApp("dotnet new -u");

            if (lines == null)
            {
                return ReturnCode.Error;
            }

            var uninstallString = FindUninstallString(lines, name, kind);

            if (string.IsNullOrEmpty(uninstallString))
            {
                return ReturnCode.Successful;
            }

            lines = LaunchApp(uninstallString);
            
            if (lines == null)
            {
                return ReturnCode.Error;
            }

            foreach (var line in lines)
            {
                Console.WriteLine(line);
            }

            return ReturnCode.Successful;
        }

        private static int GetIndents(string line)
        {
            var r = 0;
            while (r < line.Length && line[r] == ' ')
            {
                r += 1;
            }

            return r;
        }

        private static int GetIndents(string[] lines, int index)
        {
            var line = lines[index];
            return GetIndents(line);
        }

        private static string[] GetTemplates(string[] lines, ref int index)
        {
            var result = new List<string>();

            while (index < lines.Length)
            {
                var line = lines[index];
                index += 1;

                if (GetIndents(line) == 4 &&
                    string.Compare(line.Trim(), "Templates:", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    while (index < lines.Length && GetIndents(lines, index) == 6)
                    {
                        result.Add(lines[index]);
                        index += 1;
                    }

                    break;
                }
            }

            return result.ToArray();
        }

        private static string GetUninstallCommand(string[] lines, ref int index)
        {
            var result = new List<string>();

            while (index < lines.Length)
            {
                var line = lines[index];
                index += 1;

                if (GetIndents(line) == 4 &&
                    string.Compare(line.Trim(), "Uninstall Command:", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    while (index < lines.Length && GetIndents(lines, index) == 6)
                    {
                        result.Add(lines[index].Trim());
                        index += 1;
                    }

                    break;
                }
            }

            return result.FirstOrDefault();
        }

        private static string GetNextSource(string[] lines, ref int index)
        {
            while (index < lines.Length && GetIndents(lines, index) != 2)
            {
                index += 1;
            }

            return index >= lines.Length ? null : lines[index++].Trim();
        }

        private static void ExtractName(string line, out string name, out string shortName, out string lang)
        {
            var brIndex1 = line.IndexOf('(');
            var brIndex2 = line.IndexOf(')');

            name = line.Substring(0, brIndex1).Trim();
            shortName = line.Substring(brIndex1 + 1, brIndex2 - brIndex1 - 1).Trim();
            lang = line.Substring(brIndex2 + 1).Trim();
        }

        private static string FindUninstallString(string[] lines, string name, TemplateNameKind kind)
        {
            var index = 0;
            while (true)
            {
                var sourceName = GetNextSource(lines, ref index);
                if (string.IsNullOrEmpty(sourceName))
                {
                    break;
                }

                if (kind == TemplateNameKind.Directory)
                {
                    var dirName = Path.GetFileName(sourceName);
                    if (string.Compare(dirName, name, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        continue;
                    }
                }
                else
                {
                    var templates = GetTemplates(lines, ref index);
                    bool found = false;
                    foreach (var template in templates)
                    {
                        ExtractName(template, out var name1, out var shortName, out _);
                        string cmpName;
                        switch (kind)
                        {
                            case TemplateNameKind.Name:
                                cmpName = name1;
                                break;
                            case TemplateNameKind.ShortName:
                                cmpName = shortName;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
                        }

                        if (string.Compare(cmpName, name, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        continue;
                    }
                }

                var uninstallCommand = GetUninstallCommand(lines, ref index);
                return uninstallCommand;
            }

            return null;
        }

        private static string[] LaunchApp(string launchString)
        {
            var strings = launchString.Split(' ');
            return LaunchApp(strings[0], strings.Skip(1).ToArray());
        }

        private static string[] LaunchApp(string appName, string[] args)
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
                    RedirectStandardError = true
                }
            };

            process.Start();

            var lines = new List<string>();
            process.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }
                lines.Add(e.Data);
                Debug.WriteLine("output>>" + e.Data);
                Console.WriteLine(e.Data);
            };
            process.BeginOutputReadLine();

            var isError = false;
            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }
                isError = true;
                Debug.WriteLine("error>>" + e.Data);
                Console.WriteLine(e.Data);
            };

            process.BeginErrorReadLine();

            process.WaitForExit(Consts.ProcessExecutionTimeout);

            if (!process.HasExited)
            {
                isError = true;
                process.KillProcessAndChildren();
            }

            process.Close();

            if (isError)
            {
                return null;
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
