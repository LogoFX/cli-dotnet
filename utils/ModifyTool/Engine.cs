using System;
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

        private readonly string _solutionFolder;

        public Engine(string solutionFolder)
        {
            _solutionFolder = Path.GetFullPath(solutionFolder);
        }

        private string GetProjectFolder()
        {
            return $"{_solutionFolder}.Model";
        }

        public void CreateMapping(string entityName)
        {
            var filePath = Path.Combine(Path.Combine(GetProjectFolder(), MappersFolderName), MappingProfileFileName);

            if (!File.Exists(filePath))
            {
                var helper = new ResourceHelper(MappingProfileFileName);

                using var stream = helper.GetResourceStream();
                using (var fileStream = File.Create(filePath))
                {
                    stream.CopyTo(fileStream);
                }

                var solutionName = Path.GetFileName(_solutionFolder);
                ReplaceInFile(filePath, solutionName);
            }

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
            var ctorDecl = (ConstructorDeclarationSyntax) members[ctorIndex];
            // ReSharper disable once PossibleNullReferenceException
            var body = ctorDecl.Body.AddStatements(
                SyntaxFactory.ExpressionStatement(
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.IdentifierName(method.Identifier))));
            ctorDecl = ctorDecl.WithBody(body);
            members = members.RemoveAt(ctorIndex);
            members = members.Insert(ctorIndex, ctorDecl);
            var newClass = @class.WithMembers(members);

            node = node.ReplaceNode(@class, newClass);
            File.WriteAllText(filePath, node.NormalizeWhitespace().ToFullString());
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public void RegisterMappers(string entityName)
        {
            var moduleFilePath = Path.Combine(GetProjectFolder(), ModuleFileName);

            if (!File.Exists(moduleFilePath))
            {
                var helper = new ResourceHelper(ModuleFileName);

                using var stream = helper.GetResourceStream();
                using (var fileStream = File.Create(moduleFilePath))
                {
                    stream.CopyTo(fileStream);
                }

                var solutionName = Path.GetFileName(_solutionFolder);
                ReplaceInFile(moduleFilePath, solutionName);
            }

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

        private void ReplaceInFile(string filePath, string solutionName)
        {
            var lines = File.ReadAllText(filePath);
            var index1 = lines.IndexOf("namespace", StringComparison.Ordinal) + "namespace".Length;
            var index2 = lines.IndexOf(".Model", index1, StringComparison.Ordinal);
            var oldSolutionName = lines.Substring(index1, index2 - index1).Trim();
            lines = lines.Replace(oldSolutionName, solutionName);
            File.WriteAllText(filePath, lines);
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