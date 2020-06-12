using System;
using System.Collections.Generic;
using Common.Infra;

namespace ModifyTool
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            //TODO: Introduce IoC
            var parseErrorHandler = new ArgumentParseErrorHandler(
                new ArgumentParseErrorMessageFactory(),
                new ErrorMessageConsoleRenderer());
            if (args.Length < 3)
            {
                parseErrorHandler.Handle(ArgumentParseErrorType.InvalidNumberOfArguments, default);
            }

            try
            {
                var actionList = MatchActions(args, parseErrorHandler);
                if (actionList.Count == 0)
                {
                    parseErrorHandler.Handle(ArgumentParseErrorType.NoKeysFound, default);
                    return ReturnCode.IncorrectFunction;
                }

                var result = ApplyActions(actionList);
                return result ? ReturnCode.Successful : ReturnCode.Error;
            }
            catch
            {
                return ReturnCode.IncorrectFunction;
            }
        }

        private static List<Tuple<Action<string, string>, string, string>> MatchActions(
            IReadOnlyList<string> args,
            IArgumentParseErrorHandler parseErrorHandler)
        {
            var actionMatcher = new ActionMatcher();
            var solutionName = args[0];
            var actionList = new List<Tuple<Action<string, string>, string, string>>();
            var index = 1;
            while (index < args.Count)
            {
                var action = FindAction(args, parseErrorHandler, index, actionMatcher);
                var entityName = args[++index];
                actionList.Add(new Tuple<Action<string, string>, string, string>(action, solutionName, entityName));
                ++index;
            }

            return actionList;
        }

        private static Action<string, string> FindAction(IReadOnlyList<string> args, IArgumentParseErrorHandler parseErrorHandler, int index,
            ActionMatcher actionMatcher)
        {
            var key = args[index];
            if (!key.StartsWith(Consts.KeyPrefix))
            {
                parseErrorHandler.Handle(ArgumentParseErrorType.InvalidKey, key);
                throw new Exception();
            }

            var action = actionMatcher.Match(key);
            if (action == null)
            {
                parseErrorHandler.Handle(ArgumentParseErrorType.UnknownKey, key);
                throw new Exception();
            }

            return action;
        }

        //TODO: Impure function
        private static bool ApplyActions(IEnumerable<Tuple<Action<string, string>, string, string>> actions)
        {
            foreach (var action in actions)
            {
                try
                {
                    action.Item1(action.Item2, action.Item3);
                }
                catch (Exception err)
                {
                    //TODO: Replace with renderer
                    Console.WriteLine(err.Message);
                    return false;
                }
            }

            return true;
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
