using System;
using System.Collections.Generic;
using Common.Infra;

namespace ModifyTool
{
    internal static class Program
    {
        private const string EntityKey = "entity";
        private const string ServiceKey = "service";

        private static readonly Dictionary<string, Action<string, string>> _engineActions =
            new Dictionary<string, Action<string, string>>();

        static Program()
        {
            _engineActions.Add(EntityKey, ModelAction);
            _engineActions.Add(ServiceKey, ServiceAction);
        }

        private static void ModelAction(string solutionFolder, string entityName)
        {
            var engine = new ModelEngine(solutionFolder);
            engine.CreateMapping(entityName);
            engine.RegisterMappers(entityName);
        }

        private static void ServiceAction(string solutionFolder, string entityName)
        {
            var engine = new FakeProviderEngine(solutionFolder);
            engine.RegisterProvider(entityName);
        }

        private static int Main(string[] args)
        {
            if (args.Length < 3)
            {
                ShowUsage();
                return ReturnCode.IncorrectFunction;
            }

            var solutionName = args[0];

            var actionList = new List<Tuple<Action<string, string>, string, string>>();
            var index = 1;
            while (index < args.Length)
            {
                var key = args[index];
                if (!key.StartsWith("--"))
                {
                    Console.WriteLine($"{key} is not a key");
                    ShowUsage();
                    return ReturnCode.IncorrectFunction;
                }

                Action<string, string> action = null;
                foreach (var pair in _engineActions)
                {
                    if (string.Compare(key, $"--{pair.Key}", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        action = pair.Value;
                        break;
                    }
                }

                if (action == null)
                {
                    Console.WriteLine($"Unknown key {key}");
                    ShowUsage();
                    return ReturnCode.IncorrectFunction;
                }

                ++index;
                var entityName = args[index];

                actionList.Add(new Tuple<Action<string, string>, string, string>(action, solutionName, entityName));
                
                ++index;
            }


            if (actionList.Count == 0)
            {
                Console.WriteLine($"No keys found");
                ShowUsage();
                return ReturnCode.IncorrectFunction;
            }

            foreach (var action in actionList)
            {
                try
                {
                    action.Item1(action.Item2, action.Item3);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    return ReturnCode.Error;
                }

            }

            return ReturnCode.Successful;
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
