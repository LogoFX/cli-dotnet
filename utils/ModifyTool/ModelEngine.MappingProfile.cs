using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal sealed partial class ModelEngine
    {
        private const string MappersFolderName = "Mappers";
        private const string MappingProfileFileName = "MappingProfile.cs";
        private const string MappingProfileClassName = "MappingProfile";

        /// <summary>
        /// Create or update MappingProfile.cs in .Model\Mappers
        /// </summary>
        /// <param name="entityName"></param>
        public void CreateMapping(string entityName)
        {
            var filePath = Path.Combine(Path.Combine(GetProjectFolder(), MappersFolderName), MappingProfileFileName);

            if (!File.Exists(filePath))
            {
                var helper = new ResourceHelper("Model", MappingProfileFileName);

                using var stream = helper.GetResourceStream();
                using (var fileStream = File.Create(filePath))
                {
                    stream.CopyTo(fileStream);
                }

                ReplaceSolutionName(filePath);
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
            var ctorDecl = (ConstructorDeclarationSyntax)members[ctorIndex];
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

    }
}