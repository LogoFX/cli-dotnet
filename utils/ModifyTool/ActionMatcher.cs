using System;
using System.Collections.Generic;

namespace ModifyTool
{
    internal class ActionMatcher
    {
        private const string EntityKey = "entity";
        private const string ServiceKey = "service";

        private static readonly Dictionary<string, Action<string, string>> _engineActions =
            new Dictionary<string, Action<string, string>>();

        public ActionMatcher()
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

        internal Action<string, string> Match(string key)
        {
            foreach (var (objectType, action) in _engineActions)
            {
                if (string.Compare(key, $"{Consts.KeyPrefix}{objectType}", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return action;
                }
            }
            return null;
        }
    }
}
