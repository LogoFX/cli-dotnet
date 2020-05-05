using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using JetBrains.Annotations;
using LogoFX.Cli.Dotnet.Specs.Tests.Contracts;

namespace LogoFX.Cli.Dotnet.Specs.Tests.Infra
{
    [UsedImplicitly]
    internal sealed class WindowsProcessManagementService : IProcessManagementService
    {
        public string SetCurrentDir(string path)
        {
            path = Path.GetFullPath(path);
            Directory.SetCurrentDirectory(path);
            return path;
        }

        public ExecutionInfo Start(string tool, string args, int? waitTime = null)
        {
            var outputStrings = new List<string>();
            var errorStrings = new List<string>();

            var processInfo = new ProcessStartInfo("cmd.exe", $"/c {tool} {args}")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true
            };

            var process = Process.Start(processInfo);

            Debug.Assert(process != null, nameof(process) + " != null");

            process.OutputDataReceived += (sender, e) =>
            {
                outputStrings.Add(e.Data);
                Debug.WriteLine("output>>" + e.Data);
            };
            process.BeginOutputReadLine();

            process.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }
                errorStrings.Add(e.Data);
                Debug.WriteLine("error>>" + e.Data);
            };
            process.BeginErrorReadLine();

            if (waitTime.HasValue)
            {
                process.WaitForExit(waitTime.Value);
            }
            else
            {
                process.WaitForExit();
            }

            var result = new ExecutionInfo
            {
                ProcessId = process.Id,
                OutputStrings = outputStrings.ToArray(),
                ErrorStrings = errorStrings.ToArray(),
                ExitCode = process.ExitCode
            };

            process.Close();

            return result;
        }

        public void Stop(int processId)
        {
            Action killAction = () => processId.KillProcessAndChildren();
            killAction.Execute();
        }
    }
}
