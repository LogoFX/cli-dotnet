using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ModifyTool
{
    internal sealed partial class FakeProviderEngine
    {
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void RegisterProvider(string entityName)
        {
            var moduleFilePath = Path.Combine(GetProjectFolder(), Consts.ModuleFileName);
            FileHelper.CreateFile("Provider", moduleFilePath, ReplaceSolutionName);

            var moduleText = File.ReadAllText(moduleFilePath);
            var tree = CSharpSyntaxTree.ParseText(moduleText);
            var node = tree.GetRoot();

            var rewriter = new FakeProviderSyntaxRewriter(entityName);
            node = rewriter.Visit(node).NormalizeWhitespace();
            rewriter.NormalizeWhitespaceOnly = true;
            node = rewriter.Visit(node);

            File.WriteAllText(moduleFilePath, node.ToFullString());
        }
    }
}