using System;
using System.IO;

namespace ModifyTool
{
    internal abstract class EngineBase
    {
        private readonly string _solutionFolder;

        protected EngineBase(string solutionFolder)
        {
            _solutionFolder = Path.GetFullPath(solutionFolder);
        }

        protected abstract string GetProjectSuffix();

        protected string GetProjectFolder()
        {
            return $"{_solutionFolder}.{GetProjectSuffix()}";
        }

        protected void ReplaceSolutionName(string filePath)
        {
            const string namespaceKey = "namespace";

            var solutionName = Path.GetFileName(_solutionFolder);

            var lines = File.ReadAllText(filePath);
            var index1 = lines.IndexOf(namespaceKey, StringComparison.Ordinal) + namespaceKey.Length;
            var index2 = lines.IndexOf($".{GetProjectSuffix()}", index1, StringComparison.Ordinal);
            var oldSolutionName = lines.Substring(index1, index2 - index1).Trim();
            lines = lines.Replace(oldSolutionName, solutionName);
            File.WriteAllText(filePath, lines);
        }
    }
}