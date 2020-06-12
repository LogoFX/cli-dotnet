using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal sealed partial class FakeProviderSyntaxRewriter : CSharpSyntaxRewriter
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
                    return node;

                node = newClass;
            }

            return base.VisitClassDeclaration(node);
        }

        public override SyntaxNode VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            _checkingMethod = node.Identifier.Text;
            if (_checkingMethod == RegisterModuleMethodName)
            {
                node = NormalizeWhitespaceOnly ? SyntaxHelper.NormalizeWhitespace(node) :
                    RewriteRegisterModuleMethod(node);
            }
            var result = base.VisitMethodDeclaration(node);
            _checkingMethod = null;
            return result;
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

        private SyntaxList<StatementSyntax> CreateRegistrationStatement(SyntaxList<StatementSyntax> statements,
            string registratorIdentifier)
        {
            statements = statements.Add(
                SyntaxFactory.ExpressionStatement(SyntaxHelper.ToInvocationExpression(
                        SyntaxHelper.ToMemberAccessExpression(SyntaxHelper.ToInvocationExpression(
                                SyntaxHelper.ToMemberAccessExpression(registratorIdentifier, AddInstanceName), SyntaxHelper.ArgumentList(SyntaxHelper.ToInvocationExpression(_initializeContainerMethodName))),
                            SyntaxHelper.ToGenericName(AddSingletonName, _providerContractName, _fakeProviderName)))));

            statements = statements.Add(
                SyntaxFactory.ExpressionStatement(SyntaxHelper.ToInvocationExpression(
                        SyntaxHelper.ToMemberAccessExpression(registratorIdentifier, RegisterInstanceName), SyntaxHelper.ArgumentList(SyntaxHelper.ToInvocationExpression(
                                SyntaxHelper.ToMemberAccessExpression(_providerBuilderName, CreateBuilderMethodName))))));

            return statements;
        }

        public override SyntaxNode VisitImplicitArrayCreationExpression(ImplicitArrayCreationExpressionSyntax node)
        {
            if (NormalizeWhitespaceOnly && _checkingMethod != RegisterInstanceName)
            {
                var closeBracket = node.CloseBracketToken.WithTrailingTrivia(SyntaxHelper.EndOfLineTrivia);
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
                var openBrace = node.OpenBraceToken.WithLeadingTrivia(SyntaxHelper.Whitespace(12));
                node = node.WithOpenBraceToken(openBrace);
                var closeBrace = node.CloseBraceToken.WithLeadingTrivia(SyntaxHelper.EndOfLineTrivia, SyntaxHelper.Whitespace(12));
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
                    .WithLeadingTrivia(SyntaxHelper.EndOfLineTrivia, SyntaxHelper.Whitespace(16))
                    .WithTrailingTrivia(SyntaxHelper.Whitespace(1));
                node = node.WithNewKeyword(newKeyword);
                var type = (IdentifierNameSyntax) node.Type;
                
                var identifier = type.Identifier.WithTrailingTrivia(SyntaxHelper.EndOfLineTrivia);
                type = type.WithIdentifier(identifier);
                node = node.WithType(type);

                var initializer = RewriteInitializer(node.Initializer);
                var openBrace = initializer.OpenBraceToken
                    .WithLeadingTrivia(SyntaxHelper.Whitespace(16))
                    .WithTrailingTrivia(SyntaxHelper.EndOfLineTrivia);
                var closeBrace = initializer.CloseBraceToken
                    .WithLeadingTrivia(SyntaxHelper.EndOfLineTrivia, SyntaxHelper.Whitespace(16));
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
                var expr = expression.WithLeadingTrivia(SyntaxHelper.Whitespace(20));
                expressions.Add(expr);
            }

            initializer = initializer.WithExpressions(
                SyntaxFactory.SeparatedList(
                    expressions,
                    Enumerable.Repeat(
                        SyntaxFactory.Token(SyntaxKind.CommaToken).WithTrailingTrivia(SyntaxHelper.EndOfLineTrivia), 
                        expressions.Count - 1)));

            return initializer;
        }

        private ClassDeclarationSyntax AddContainerInitializationMethod(ClassDeclarationSyntax moduleClass)
        {
            var foundMember = moduleClass.Members
                .OfType<MethodDeclarationSyntax>()
                .Any(x => x.Identifier.Text == _initializeContainerMethodName);

            if (foundMember)
                return null;

            var method = SyntaxFactory.MethodDeclaration(
                SyntaxFactory.ParseTypeName(_containerContract),
                _initializeContainerMethodName);
            method = method.NormalizeWhitespace().AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));
            method = method.WithBody(CreateContainerInitializationBody());

            var members = moduleClass.Members.Add(method);
            moduleClass = moduleClass.WithMembers(members);

            return moduleClass;
        }
    }
}