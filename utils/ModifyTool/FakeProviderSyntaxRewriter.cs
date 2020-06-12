using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal sealed class FakeProviderSyntaxRewriter : CSharpSyntaxRewriter
    {
        private const string ModuleClassName = "Module";
        private const string RegisterModuleMethodName = "RegisterModule";
        private const string CreateBuilderMethodName = "CreateBuilder";
        private const string RegisterInstanceName = "RegisterInstance";
        private const string AddSingletonName = "AddSingleton";
        private const string AddInstanceName = "AddInstance";

        private readonly string _containerContract;
        private readonly string _containerType;
        private readonly string _containerName;
        private readonly string _dtoName;
        private readonly string _initializeContainerMethodName;
        private readonly string _providerBuilderName;
        private readonly string _providerContractName;
        private readonly string _fakeProviderName;

        private string _checkingMethod;

        public FakeProviderSyntaxRewriter(string entityName)
        {
            _containerContract = $"I{entityName}DataContainer";
            _containerType = $"{entityName}DataContainer";
            _containerName = "container";
            _dtoName = $"{entityName}Dto";
            _initializeContainerMethodName = $"Initialize{entityName}Container";
            _providerBuilderName = $"{entityName}ProviderBuilder";
            _providerContractName = $"I{entityName}DataProvider";
            _fakeProviderName = $"Fake{entityName}DataProvider";
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
            if (_checkingMethod == RegisterModuleMethodName)
            {
                node = NormalizeWhitespaceOnly ? 
                    NormalizeWhitespace(node) :
                    RewriteRegisterModuleMethod(node);
            }
            var result = base.VisitMethodDeclaration(node);
            _checkingMethod = null;
            return result;
        }

        private MethodDeclarationSyntax NormalizeWhitespace(MethodDeclarationSyntax node)
        {
            var body = node.Body;
            var statements = new SyntaxList<StatementSyntax>();
            // ReSharper disable once PossibleNullReferenceException
            statements = body.Statements.OfType<ExpressionStatementSyntax>()
                .Select(AlignExpression)
                .Aggregate(statements, (current, newEss) => current.Add(newEss));
            body = body.WithStatements(statements);
            node = node.WithBody(body);
            return node;
        }

        private ExpressionStatementSyntax AlignExpression(ExpressionStatementSyntax ess)
        {
            if (ess.ToFullString().Length <= 120)
            {
                return ess;
            }

            return ess.WithExpression(AlignExpression(ess.Expression));
        }

        private ExpressionSyntax AlignExpression(ExpressionSyntax expression)
        {
            if (expression is InvocationExpressionSyntax invocationExpression)
            {
                return invocationExpression.WithExpression(AlignExpression(invocationExpression.Expression));
            }

            if (expression is MemberAccessExpressionSyntax memberAccessExpression)
            {
                return memberAccessExpression
                    .WithExpression(
                        AlignExpression(memberAccessExpression.Expression))
                    .WithOperatorToken(
                        memberAccessExpression.OperatorToken
                            .WithLeadingTrivia(EndOfLineTrivia, Whitespace(16)));
            }

            return expression;
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private MethodDeclarationSyntax RewriteRegisterModuleMethod(MethodDeclarationSyntax method)
        {
            var dependencyRegistratorParam = method.ParameterList.ChildNodes().OfType<ParameterSyntax>().First();
            var body = method.Body;

            var stList = CreateRegistrationStatement(body.Statements, dependencyRegistratorParam.Identifier.Text);
            body = body.WithStatements(stList);
            method = method.WithBody(body);
            return method;
        }

        private SyntaxList<StatementSyntax> CreateRegistrationStatement(SyntaxList<StatementSyntax> statements, string registratorIdentifier)
        {
            statements = statements.Add(
                SyntaxFactory.ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(
                            InvocationExpression(
                                MemberAccessExpression(registratorIdentifier, AddInstanceName),
                                ArgumentList(
                                    InvocationExpression(_initializeContainerMethodName))),
                            GenericName(AddSingletonName, _providerContractName, _fakeProviderName)))));

            statements = statements.Add(
                SyntaxFactory.ExpressionStatement(
                    InvocationExpression(
                        MemberAccessExpression(registratorIdentifier, RegisterInstanceName),
                        ArgumentList(
                            InvocationExpression(
                                MemberAccessExpression(_providerBuilderName, CreateBuilderMethodName))))));


            return statements;
        }

        private GenericNameSyntax GenericName(string identifier, params string[] typeNames)
        {
            return SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(identifier),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList(typeNames.Select(x => SyntaxFactory.ParseTypeName(x)))));
        }

        private MemberAccessExpressionSyntax MemberAccessExpression(string identifier, string name)
        {
            return MemberAccessExpression(SyntaxFactory.IdentifierName(identifier), name);
        }

        private MemberAccessExpressionSyntax MemberAccessExpression(ExpressionSyntax expression, string name)
        {
            return MemberAccessExpression(expression, SyntaxFactory.IdentifierName(name));
        }

        private MemberAccessExpressionSyntax MemberAccessExpression(ExpressionSyntax expression, SimpleNameSyntax name)
        {
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
        }

        private ArgumentListSyntax ArgumentList(ExpressionSyntax argument)
        {
            return SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(new[]
                {
                    SyntaxFactory.Argument(argument)
                }));
        }

        private ExpressionSyntax InvocationExpression(string identifier, ArgumentListSyntax argumentList = null)
        {
            return InvocationExpression(SyntaxFactory.IdentifierName(identifier), argumentList);
        }

        private ExpressionSyntax InvocationExpression(ExpressionSyntax expression, ArgumentListSyntax argumentList = null)
        {
            return SyntaxFactory.InvocationExpression(expression, argumentList ?? SyntaxFactory.ArgumentList());
        }

        public override SyntaxNode VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            if (NormalizeWhitespaceOnly && _checkingMethod != RegisterInstanceName)
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
                _checkingMethod != RegisterInstanceName)
            {
                var openBrace = node.OpenBraceToken.WithLeadingTrivia(Whitespace(12));
                node = node.WithOpenBraceToken(openBrace);
                var closeBrace = node.CloseBraceToken.WithLeadingTrivia(EndOfLineTrivia, Whitespace(12));
                node = node.WithCloseBraceToken(closeBrace);
            }

            return base.VisitInitializerExpression(node!);
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            if (NormalizeWhitespaceOnly &&
                node.Parent is InitializerExpressionSyntax initializerExpression &&
                initializerExpression.IsKind(SyntaxKind.ArrayInitializerExpression) &&
                _checkingMethod != RegisterInstanceName)
            {
                var newKeyword = node.NewKeyword
                    .WithLeadingTrivia(EndOfLineTrivia, Whitespace(16))
                    .WithTrailingTrivia(Whitespace(1));
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

            var method = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseTypeName(_containerContract),
                _initializeContainerMethodName);
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
            var statement = SyntaxFactory.ReturnStatement(SyntaxFactory.IdentifierName(_containerName));
            return statement;
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

        private SyntaxTrivia EndOfLineTrivia => SyntaxFactory.EndOfLine(Environment.NewLine);

        private SyntaxTrivia Whitespace(int count) => SyntaxFactory.Whitespace(new string(' ', count));
    }
}