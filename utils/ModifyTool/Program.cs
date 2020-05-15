using System;

namespace ModifyTool
{
    class Program
    {
        private const string EntityKey = "entity";

        static int Main(string[] args)
        {
            if (args.Length < 3 ||
                string.Compare(args[1], $"--{EntityKey}", StringComparison.OrdinalIgnoreCase) != 0)
            {
                ShowUsage();
                return 1;
            }

            var engine = new Engine(args[0]);
            try
            {
                engine.RegisterMappers(args[2]);
            }
            catch (Exception err)
            {
                Console.WriteLine(err.Message);
                return 1;
            }

            return 0;
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("    ModifyTool <project folder> --entity <entity name>");
        }
    }
}
