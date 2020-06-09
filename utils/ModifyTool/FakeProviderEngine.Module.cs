using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ModifyTool
{
    internal sealed partial class FakeProviderEngine
    {
        private const string ModuleFileName = "Module.cs";

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void RegisterProvider(string entityName)
        {
            var moduleFilePath = Path.Combine(GetProjectFolder(), ModuleFileName);
            CreateModule(moduleFilePath);

            var moduleText = File.ReadAllText(moduleFilePath);
            var tree = CSharpSyntaxTree.ParseText(moduleText);
            var node = tree.GetRoot();

            var rewriter = new FakeProviderSyntaxRewriter(entityName);
            node = rewriter.Visit(node).NormalizeWhitespace();
            rewriter.NormalizeWhitespaceOnly = true;
            node = rewriter.Visit(node);

            File.WriteAllText(moduleFilePath, node.ToFullString());
        }

        private void CreateModule(string moduleFilePath)
        {
            if (!File.Exists(moduleFilePath))
            {
                var helper = new ResourceHelper("Provider", ModuleFileName);

                using var stream = helper.GetResourceStream();
                using (var fileStream = File.Create(moduleFilePath))
                {
                    stream.CopyTo(fileStream);
                }

                ReplaceSolutionName(moduleFilePath);
            }
        }
    }
}