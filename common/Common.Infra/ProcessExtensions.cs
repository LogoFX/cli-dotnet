using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace Common.Infra
{
    public static class ProcessExtensions
    {
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

        public static string[] LaunchApp(string appName, string[] args)
        {
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
                    RedirectStandardError = false
                }
            };

            process.Start();

            var exited = process.WaitForExit(Consts.ProcessExecutionTimeout);

            bool isError;

            if (exited)
            {
                isError = process.ExitCode != ReturnCode.Successful;
            }
            else
            {
                isError = true;
                process.KillProcessAndChildren();
            }

            process.Close();

            if (isError)
            {
                return null;
            }

            var lines = File.ReadAllLines(outputFileName);
            File.Delete(outputFileName);
            return lines;
        }
    }
}