using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal partial class FakeProviderSyntaxRewriter
    {
        private BlockSyntax CreateContainerInitializationBody()
        {
            var block = SyntaxFactory.Block(
                CreateContainerCreationStatement(),
                CreateUpdateContainerStatement(),
                CreateReturnContainerStatement());
            return block;
        }

        private StatementSyntax CreateContainerCreationStatement()
        {
            var varId = SyntaxFactory.IdentifierName("var");
            var declarator = SyntaxFactory.VariableDeclarator(_containerName);
            var objCreationExpr = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(_containerType), SyntaxFactory.ArgumentList(), null);
            var initializer = SyntaxFactory.EqualsValueClause(objCreationExpr);
            declarator = declarator.WithInitializer(initializer);
            var variables = SyntaxFactory.SeparatedList(new[] { declarator });
            var declaration = SyntaxFactory.VariableDeclaration(varId, variables);

            var result = SyntaxFactory.LocalDeclarationStatement(declaration);

            return result;
        }

        private StatementSyntax CreateUpdateContainerStatement()
        {
            var updateItemsMemberAccessExpr = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(_containerName),
                SyntaxFactory.IdentifierName("UpdateItems"));
            var updateItemsExpr = SyntaxFactory.InvocationExpression(
                updateItemsMemberAccessExpr,
                CreateUpdateContainerArgumentList(_dtoName));

            var statement = SyntaxFactory.ExpressionStatement(updateItemsExpr);
            return statement;
        }

        private ArgumentListSyntax CreateUpdateContainerArgumentList(string dtoName)
        {
            var initializer = SyntaxFactory.InitializerExpression(
                SyntaxKind.ArrayInitializerExpression,
                CreateUpdateContainerArrayInitializerList(dtoName));
            var argument = SyntaxFactory.ImplicitArrayCreationExpression(initializer);
            var result = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(new[]
            {
                SyntaxFactory.Argument(argument)
            }));

            return result;
        }

        private SeparatedSyntaxList<ExpressionSyntax> CreateUpdateContainerArrayInitializerList(string dtoName)
        {
            return SyntaxFactory.SeparatedList<ExpressionSyntax>(new[]
            {
                CreateDtoCreation(dtoName, "PC", 8),
                CreateDtoCreation(dtoName, "Acme", 10),
                CreateDtoCreation(dtoName, "Bacme", 3),
                CreateDtoCreation(dtoName, "Exceed", 100),
                CreateDtoCreation(dtoName, "Acme2", 10)
            });
        }

        private ObjectCreationExpressionSyntax CreateDtoCreation(string dtoName, string displayName, int value)
        {
            var list = CreateDtoAssignments(displayName, value);
            var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, list);
            var result = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(dtoName), null, initializer);
            return result;
        }

        private SeparatedSyntaxList<ExpressionSyntax> CreateDtoAssignments(string displayName, int value)
        {
            return SyntaxFactory.SeparatedList(new[]
            {
                CreateDtoAssignment(
                    "Id",
                    SyntaxFactory.InvocationExpression(
                        SyntaxFactory.MemberAccessExpression(
                            SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.IdentifierName("Guid"),
                            SyntaxFactory.IdentifierName("NewGuid")),
                        SyntaxFactory.ArgumentList())),
                CreateDtoAssignment(
                    "DisplayName",
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.StringLiteralExpression,
                        SyntaxFactory.Literal(displayName))),
                CreateDtoAssignment(
                    "Value",
                    SyntaxFactory.LiteralExpression(
                        SyntaxKind.NumericLiteralExpression,
                        SyntaxFactory.Literal(value)))
            });
        }

        private static ExpressionSyntax CreateDtoAssignment(string name, ExpressionSyntax value)
        {
            var result = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(name),
                value);
            return result;
        }

        private StatementSyntax CreateReturnContainerStatement()
        {
            var statement = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(_containerName));
            return statement;
        }
    }
}
