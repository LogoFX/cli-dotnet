using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Threading;

namespace Common.Infra
{
    public interface IProcessExitInfo
    {
        int ProcessId { get; }
        string[] Output { get; }
        string Errors { get; }
        int ExitCode { get; }
        bool IsError { get; }
    }

    public static class ProcessExtensions
    {
        private sealed class ProcessExitInfo : IProcessExitInfo
        {
            public int ProcessId { get; set; }
            public string[] Output { get; set; }
            public string Errors { get; set; }
            public int ExitCode { get; set; }
            public bool IsError { get; set; }
        }

        public static void KillProcessAndChildren(this Process process)
        {
            KillProcessAndChildrenImpl(process?.Id ?? 0);
        }

        public static void KillProcessAndChildren(this int pid)
        {
            KillProcessAndChildrenImpl(pid);
        }

        private static void KillProcessAndChildrenImpl(int pid)
        {
            // Cannot close 'system idle process'.
            if (pid == 0)
            {
                return;
            }

            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                ("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (var mo in moc)
            {
                Convert.ToInt32(mo["ProcessID"]).KillProcessAndChildren();
            }

            try
            {
                Process proc = Process.GetProcessById(pid);
                proc.Kill();
            }
            catch (ArgumentException)
            {
                // Process already exited.
            }
            catch (Win32Exception)
            {
                // TODO: Handle Access is denied case
            }
        }

        public static IProcessExitInfo LaunchApp(string appName, params string[] args)
        {
            var result = new ProcessExitInfo();

            var outputFileName = Path.GetTempFileName();

            var arguments = $"/c {appName} {string.Join(" ", args)} > {outputFileName}";
            var process = new Process
            {
                StartInfo =
                {
                    FileName = "cmd",
                    Arguments = arguments,
                    CreateNoWindow = false,
                    UseShellExecute = false,
                    RedirectStandardOutput = false,
                    RedirectStandardError = true
                }
            };

            process.Start();

            result.ProcessId = process.Id;

            var exited = process.WaitForExit(Consts.ProcessExecutionTimeout);

            if (exited)
            {
                result.ExitCode = process.ExitCode;
                result.IsError = result.ExitCode != ReturnCode.Successful;
            }
            else
            {
                result.IsError = true;
                process.KillProcessAndChildren();
            }

            result.Errors = process.StandardError.ReadToEnd();

            process.Close();

            if (File.Exists(outputFileName))
            {
                var count = 5;
                while (count > 0)
                {
                    try
                    {
                        result.Output = File.ReadAllLines(outputFileName);
                        break;
                    }
                    catch (IOException err)
                    {
                        Debug.WriteLine(err.Message);
                        count -= 1;
                        Thread.Sleep(500);
                    }
                }
            }

            File.Delete(outputFileName);
            return result;
        }
    }
}