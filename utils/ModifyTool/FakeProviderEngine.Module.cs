using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal sealed partial class FakeProviderEngine
    {
        private const string ModuleFileName = "Module.cs";
        private const string RegisterModuleMethodName = "RegisterModule";

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

        private ClassDeclarationSyntax ModifyRegistrationMethod(ClassDeclarationSyntax moduleClass, string entityName)
        {
            var method = moduleClass.Members
                .OfType<MethodDeclarationSyntax>()
                .First(x => x.Identifier.Text == RegisterModuleMethodName);
            var dependencyRegistratorParam = method.ParameterList.ChildNodes().OfType<ParameterSyntax>().First();
            var st = method.Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Reverse()
                .FirstOrDefault(x => CheckExpression(x, dependencyRegistratorParam.Identifier));

            return moduleClass;
        }

        private void CreateModule(string moduleFilePath)
        {
            if (true || !File.Exists(moduleFilePath))
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

        private static bool CheckExpression(CSharpSyntaxNode expression, SyntaxToken name)
        {
            if (expression is IdentifierNameSyntax ins)
            {
                return SyntaxFactory.AreEquivalent(ins.Identifier, name);
            }

            if (expression is ExpressionStatementSyntax ess)
            {
                return CheckExpression(ess.Expression, name);
            }

            if (expression is InvocationExpressionSyntax ies)
            {
                return CheckExpression(ies.Expression, name);
            }

            if (expression is MemberAccessExpressionSyntax maes)
            {
                return CheckExpression(maes.Expression, name);
            }

            return false;
        }
    }
}