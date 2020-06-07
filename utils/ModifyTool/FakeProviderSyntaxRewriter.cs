using System;
using System.Collections.Generic;
using System.Linq;
using Common.Infra;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal sealed class FakeProviderSyntaxRewriter : CSharpSyntaxRewriter
    {
        private const string ModuleClassName = "Module";
        private const string RegisterModuleMethodName = "RegisterModule";

        private readonly string _type;
        private readonly string _name;
        private readonly string _dtoName;
        private readonly string _initializeContainerMethodName;

        private string _checkingMethod;

        public FakeProviderSyntaxRewriter(string entityName)
        {
            _type = $"{entityName}Container";
            _name = _type.ToCamelCase();
            _dtoName = $"{entityName}Dto";
            _initializeContainerMethodName = $"Initialize{entityName}Container";

        }

        public bool NormalizeWhitespaceOnly { get; set; }

        public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (!NormalizeWhitespaceOnly && node.Identifier.Text == ModuleClassName)
            {
                var newClass = AddContainerInitializationMethod(node);
                if (newClass == null)
                {
                    return node;
                }

                node = newClass;
            }

            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            _checkingMethod = node.Identifier.Text;
            var result = base.VisitMethodDeclaration(node);
            _checkingMethod = null;
            return result;
        }

        public override SyntaxNode VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            if (NormalizeWhitespaceOnly && _checkingMethod == _initializeContainerMethodName)
            {
                var closeBracket = node.CloseBracketToken.WithTrailingTrivia(EndOfLineTrivia);
                node = node.WithCloseBracketToken(closeBracket);
                node = node.WithInitializer(RewriteInitializer(node.Initializer));
            }

            return base.VisitImplicitArrayCreationExpression(node);
        }

        public override SyntaxNode VisitInitializerExpression(InitializerExpressionSyntax node)
        {
            if (NormalizeWhitespaceOnly &&
                node.IsKind(SyntaxKind.ArrayInitializerExpression) &&
                _checkingMethod == _initializeContainerMethodName)
            {
                var openBrace = node.OpenBraceToken
                    .WithLeadingTrivia(Whitespace(12))
                    .WithTrailingTrivia(EndOfLineTrivia);
                node = node.WithOpenBraceToken(openBrace);
                var closeBrace = node.CloseBraceToken.WithLeadingTrivia(Whitespace(12));
                node = node.WithCloseBraceToken(closeBrace);
            }

            return base.VisitInitializerExpression(node);
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (NormalizeWhitespaceOnly &&
                node.Parent is InitializerExpressionSyntax initializerExpression &&
                initializerExpression.IsKind(SyntaxKind.ArrayInitializerExpression) &&
                _checkingMethod == _initializeContainerMethodName)
            {
                var newKeyword = node.NewKeyword.WithLeadingTrivia(Whitespace(16)).WithTrailingTrivia(Whitespace(1));
                node = node.WithNewKeyword(newKeyword);
                var type = (IdentifierNameSyntax) node.Type;
                
                var identifier = type.Identifier.WithTrailingTrivia(EndOfLineTrivia);
                type = type.WithIdentifier(identifier);
                node = node.WithType(type);

                var initializer = RewriteInitializer(node.Initializer);
                var openBrace = initializer.OpenBraceToken
                    .WithLeadingTrivia(Whitespace(16))
                    .WithTrailingTrivia(EndOfLineTrivia);
                var closeBrace = initializer.CloseBraceToken
                    .WithLeadingTrivia(EndOfLineTrivia, Whitespace(16));
                initializer = initializer.WithOpenBraceToken(openBrace).WithCloseBraceToken(closeBrace);
                node = node.WithInitializer(initializer);
            }

            return base.VisitObjectCreationExpression(node);
        }

        private InitializerExpressionSyntax RewriteInitializer(InitializerExpressionSyntax initializer)
        {
            var expressions = new List<ExpressionSyntax>();
            foreach (var expression in initializer.Expressions)
            {
                var expr = expression.WithLeadingTrivia(Whitespace(20));
                expressions.Add(expr);
            }

            initializer = initializer.WithExpressions(
                SyntaxFactory.SeparatedList(
                    expressions,
                    Enumerable.Repeat(
                        SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(EndOfLineTrivia), 
                        expressions.Count - 1)));

            return initializer;
        }

        private ClassDeclarationSyntax AddContainerInitializationMethod(ClassDeclarationSyntax moduleClass)
        {
            var foundMember = moduleClass.Members
                .OfType<MethodDeclarationSyntax>()
                .Any(x => x.Identifier.Text == _initializeContainerMethodName);

            if (foundMember)
            {
                return null;
            }

            var method = SyntaxFactory.MethodDeclaration(SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.VoidKeyword)), _initializeContainerMethodName);
            method = method.NormalizeWhitespace().AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            method = method.WithBody(CreateContainerInitializationBody());

            var members = moduleClass.Members.Add(method);
            moduleClass = moduleClass.WithMembers(members);

            return moduleClass;
        }

        private BlockSyntax CreateContainerInitializationBody()
        {
            var block = SyntaxFactory.Block(
                CreateContainerCreationStatement(),
                CreateUpdateContainerStatement(),
                CreateReturnContainerStatement());
            return block;
        }

        private StatementSyntax CreateUpdateContainerStatement()
        {
            var updateItemsMemberAccessExpr = SyntaxFactory.MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName(_name),
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
            //SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal("PC"))
            var list = SyntaxFactory.SeparatedList(new[]
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
            var initializer = SyntaxFactory.InitializerExpression(SyntaxKind.ObjectInitializerExpression, list);
            var result = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(dtoName), null, initializer);
            return result;
        }

        private ExpressionSyntax CreateDtoAssignment(string name, ExpressionSyntax value)
        {
            var result = SyntaxFactory.AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                SyntaxFactory.IdentifierName(name),
                value);
            return result;
        }

        private StatementSyntax CreateReturnContainerStatement()
        {
            var statement = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(_name));
            return statement;
        }

        private StatementSyntax CreateContainerCreationStatement()
        {
            var varId = SyntaxFactory.IdentifierName("var");
            var declarator = SyntaxFactory.VariableDeclarator(_name);
            var objCreationExpr = SyntaxFactory.ObjectCreationExpression(SyntaxFactory.ParseTypeName(_type), SyntaxFactory.ArgumentList(), null);
            var initializer = SyntaxFactory.EqualsValueClause(objCreationExpr);
            declarator = declarator.WithInitializer(initializer);
            var variables = SyntaxFactory.SeparatedList(new[] { declarator });
            var declaration = SyntaxFactory.VariableDeclaration(varId, variables);

            var result = SyntaxFactory.LocalDeclarationStatement(declaration);

            return result;
        }

        private SyntaxTrivia EndOfLineTrivia => SyntaxFactory.EndOfLine(Environment.NewLine);

        private SyntaxTrivia Whitespace(int count) => SyntaxFactory.Whitespace(new string(' ', count));
    }
}