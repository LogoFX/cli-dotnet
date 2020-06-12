using System;

namespace ModifyTool
{
    internal interface IErrorMessageRenderer
    {
        void RenderError(string message, bool showUsage = true);
    }

    internal class ErrorMessageConsoleRenderer : IErrorMessageRenderer
    {
        public void RenderError(string message, bool showUsage = true)
        {
            Console.WriteLine(message);
            if (showUsage)
            {
                ShowUsage();
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    ModifyTool <solution folder> --entity <entity name>");
            Console.WriteLine("or");
            Console.WriteLine("    ModifyTool <solution folder> --service <entity name>");
        }
    }
}