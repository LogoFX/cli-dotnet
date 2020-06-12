using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common.Infra;
using Solid.Patterns.ChainOfResponsibility;

namespace UninstallTemplate.Handlers
{
    internal class DefaultHandler : ChainElementBase<string[], int>
    {
        protected override bool IsMine(string[] data)
        {
            return true;
        }

        protected override int HandleData(string[] data)
        {
            var kind = TemplateNameParser.GetTemplateNameKind(data[0]);
            var name = String.Join(' ', data.Skip(1));
            var exitInfo = LaunchApp("dotnet new -u");

            if (exitInfo.IsError)
            {
                return ReturnCode.Error;
            }

            var uninstallString = FindUninstallString(exitInfo.Output, name, kind);

            if (String.IsNullOrEmpty(uninstallString))
            {
                return ReturnCode.Successful;
            }

            exitInfo = LaunchApp(uninstallString);

            if (exitInfo.IsError)
            {
                return ReturnCode.Error;
            }

            foreach (var line in exitInfo.Output)
            {
                Console.WriteLine(line);
            }

            return ReturnCode.Successful;
        }

        private static IProcessExitInfo LaunchApp(string launchString)
        {
            //TODO:Make this code clearer
            var strings = launchString.Split(' ');
            return LaunchApp(strings[0], strings.Skip(1).ToArray());
        }

        private static IProcessExitInfo LaunchApp(string appName, string[] args)
        {
            return ProcessExtensions.LaunchApp(appName, args);
        }

        private static string FindUninstallString(string[] lines, string name, TemplateNameParser.TemplateNameKind kind)
        {
            var index = 0;
            while (true)
            {
                var sourceName = GetNextSource(lines, ref index);
                if (String.IsNullOrEmpty(sourceName))
                    break;

                if (kind == TemplateNameParser.TemplateNameKind.Directory)
                {
                    var dirName = Path.GetFileName(sourceName);
                    if (String.Compare(dirName, name, StringComparison.OrdinalIgnoreCase) != 0)
                    {
                        continue;
                    }
                }
                else
                {
                    var templates = GetTemplates(lines, ref index);
                    var found = FindMatchingTemplate(name, kind, templates);
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

        private static string GetNextSource(string[] lines, ref int index)
        {
            while (index < lines.Length && GetIndents(lines, index) != 2)
            {
                index += 1;
            }

            return index >= lines.Length ? null : lines[index++].Trim();
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

        private static void ExtractName(string line, out string name, out string shortName, out string lang)
        {
            var brIndex1 = line.IndexOf('(');
            var brIndex2 = line.IndexOf(')');

            name = line.Substring(0, brIndex1).Trim();
            shortName = line.Substring(brIndex1 + 1, brIndex2 - brIndex1 - 1).Trim();
            lang = line.Substring(brIndex2 + 1).Trim();
        }

        private static bool FindMatchingTemplate(string name, TemplateNameParser.TemplateNameKind kind, string[] templates)
        {
            var found = false;
            foreach (var template in templates)
            {
                ExtractName(template, out var name1, out var shortName, out _);
                var nameToCompare = GetNameToCompare(kind, name1, shortName);
                if (string.Compare(nameToCompare, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    found = true;
                    break;
                }
            }
            return found;
        }

        private static string GetNameToCompare(TemplateNameParser.TemplateNameKind kind, string name1, string shortName)
        {
            string nameToCompare;
            switch (kind)
            {
                case TemplateNameParser.TemplateNameKind.Name:
                    nameToCompare = name1;
                    break;
                case TemplateNameParser.TemplateNameKind.ShortName:
                    nameToCompare = shortName;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }

            return nameToCompare;
        }
    }
}