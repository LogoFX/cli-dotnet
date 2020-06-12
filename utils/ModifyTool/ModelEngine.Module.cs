using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal sealed partial class ModelEngine
    {
        private const string ModuleClassName = "Module";
        private const string RegisterModuleMethodName = "RegisterModule";

        private const string MapperSuffix = "Mapper";
        private const string RegistrationMethodName = "AddSingleton";

        /// <summary>
        /// Create or update Module.cs in .Module project.
        /// </summary>
        /// <param name="entityName"></param>
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void RegisterMappers(string entityName)
        {
            var filePath = Path.Combine(GetProjectFolder(), Consts.ModuleFileName);

            FileHelper.CreateFile("Model", filePath, Consts.ModuleFileName, ReplaceSolutionName);

            var moduleText = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(moduleText);
            var node = tree.GetRoot();
            var moduleClass = node.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single(x => x.Identifier.Text == ModuleClassName);
            var method = moduleClass.Members
                .OfType<MethodDeclarationSyntax>()
                .First(x => x.Identifier.Text == RegisterModuleMethodName);
            var dependencyRegistratorParam = method.ParameterList.ChildNodes().OfType<ParameterSyntax>().First();
            var st = method.Body.Statements
                .OfType<ExpressionStatementSyntax>()
                .Reverse()
                .First(x => CheckExpression(x, dependencyRegistratorParam.Identifier));

            var newSt = AddMapperRegistration(st, entityName);
            if (newSt == null)
            {
                return;
            }

            var stList = method.Body.Statements.Replace(st, newSt);
            var body = method.Body.WithStatements(stList);
            var newMethod = method.WithBody(body);
            var members = moduleClass.Members.Replace(method, newMethod);
            var newClass = moduleClass.WithMembers(members);
            node = node.ReplaceNode(moduleClass, newClass);
            File.WriteAllText(filePath, node.ToFullString());
        }

        private ExpressionStatementSyntax AddMapperRegistration(ExpressionStatementSyntax statement, string entityName)
        {
            var typeId = SyntaxFactory.ParseTypeName($"{entityName}{MapperSuffix}");
            var method = SyntaxFactory.GenericName(RegistrationMethodName);
            method = method.AddTypeArgumentListArguments(typeId);

            if (CheckExpression(statement, method))
            {
                return null;
            }

            var memberAccessExpression = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                statement.Expression,
                method);
            var operatorToken = memberAccessExpression.OperatorToken.WithLeadingTrivia(EndOfLineTrivia, Whitespace(16));
            memberAccessExpression = memberAccessExpression.WithOperatorToken(operatorToken);
            var invocationExpression = SyntaxFactory.InvocationExpression(memberAccessExpression);
            statement = statement.WithExpression(invocationExpression);
            return statement;
        }

        private static bool CheckExpression(CSharpSyntaxNode expression, SyntaxNode name)
        {
            while (true)
            {
                switch (expression)
                {
                    case NameSyntax ns:
                        return SyntaxFactory.AreEquivalent(ns, name);
                    case ExpressionStatementSyntax ess:
                        expression = ess.Expression;
                        continue;
                    case InvocationExpressionSyntax ies:
                        expression = ies.Expression;
                        continue;
                    case MemberAccessExpressionSyntax maes:
                        return CheckExpression(maes.Name, name) || CheckExpression(maes.Expression, name);
                    default:
                        return false;
                }
            }
        }

        private bool CheckExpression(CSharpSyntaxNode expression, SyntaxToken name)
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