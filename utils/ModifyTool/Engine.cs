using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal sealed class Engine
    {
        private const string ModuleFileName = "Module.cs";
        private const string ModuleClassName = "Module";
        private const string RegisterModuleMethodName = "RegisterModule";
        private const string MapperSuffix = "Mapper";
        private const string RegistrationMethodName = "AddSingleton";
        
        private const string MappersFolderName = "Mappers";
        private const string MappingProfileFileName = "MappingProfile.cs";
        private const string MappingProfileClassName = "MappingProfile";

        private readonly string _projectFolder;

        public Engine(string projectFolder)
        {
            _projectFolder = Path.GetFullPath(projectFolder);
        }

        public void CreateMapping(string entityName)
        {
            var filePath = Path.Combine(Path.Combine(_projectFolder, MappersFolderName), MappingProfileFileName);
            var text = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(text);
            var node = tree.GetRoot();
            var @class = node.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Single(x => x.Identifier.Text == MappingProfileClassName);

            var methodName = $"Create{entityName}Maps";

            var foundMember = @class.Members
                .OfType<MethodDeclarationSyntax>()
                .Any(x => x.Identifier.Text == methodName);

            if (foundMember)
            {
                return;
            }

            var statementMember = SyntaxFactory.GenericName("CreateDomainObjectMap");
            statementMember = statementMember.AddTypeArgumentListArguments(
                SyntaxFactory.ParseTypeName($"{entityName}Dto"),
                SyntaxFactory.ParseTypeName($"I{entityName}"),
                SyntaxFactory.ParseTypeName($"{entityName}"));
            var statement = SyntaxFactory.InvocationExpression(statementMember);

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), methodName);
            method = method.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            method = method.WithBody(SyntaxFactory.Block(SyntaxFactory.ExpressionStatement(statement)));

            var ctorIndex = @class.Members.IndexOf(x => x is ConstructorDeclarationSyntax);
            var members = @class.Members.Insert(ctorIndex + 1, method);
            var newClass = @class.WithMembers(members);

            node = node.ReplaceNode(@class, newClass);
            File.WriteAllText(filePath, node.NormalizeWhitespace().ToFullString());
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void RegisterMappers(string entityName)
        {
            var moduleFilePath = Path.Combine(_projectFolder, ModuleFileName);
            var moduleText = File.ReadAllText(moduleFilePath);
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
            File.WriteAllText(moduleFilePath, node.ToFullString());
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
            var invocationExpression = SyntaxFactory.InvocationExpression(memberAccessExpression);
            statement = statement.WithExpression(invocationExpression);
            return statement;
        }

        private bool CheckExpression(CSharpSyntaxNode expression, NameSyntax name)
        {
            if (expression is NameSyntax ns)
            {
                return SyntaxFactory.AreEquivalent(ns, name);
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
                return CheckExpression(maes.Name, name) || CheckExpression(maes.Expression, name);
            }

            return false;
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