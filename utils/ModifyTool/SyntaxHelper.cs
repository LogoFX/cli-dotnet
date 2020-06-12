using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace ModifyTool
{
    internal static class SyntaxHelper
    {
        internal static GenericNameSyntax ToGenericName(string identifier, params string[] typeNames)
        {
            return SyntaxFactory.GenericName(
                SyntaxFactory.Identifier(identifier),
                SyntaxFactory.TypeArgumentList(
                    SyntaxFactory.SeparatedList(typeNames.Select(x => SyntaxFactory.ParseTypeName(x)))));
        }

        internal static MemberAccessExpressionSyntax ToMemberAccessExpression(string identifier, string name)
        {
            return ToMemberAccessExpression(SyntaxFactory.IdentifierName(identifier), name);
        }

        internal static MemberAccessExpressionSyntax ToMemberAccessExpression(ExpressionSyntax expression, string name)
        {
            return ToMemberAccessExpression(expression, SyntaxFactory.IdentifierName(name));
        }

        internal static MemberAccessExpressionSyntax ToMemberAccessExpression(ExpressionSyntax expression,
            SimpleNameSyntax name)
        {
            return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, expression, name);
        }

        internal static ArgumentListSyntax ArgumentList(ExpressionSyntax argument)
        {
            return SyntaxFactory.ArgumentList(
                SyntaxFactory.SeparatedList(new[]
                {
                    SyntaxFactory.Argument(argument)
                }));
        }

        internal static ExpressionSyntax ToInvocationExpression(string identifier,
            ArgumentListSyntax argumentList = null)
        {
            return ToInvocationExpression(SyntaxFactory.IdentifierName(identifier), argumentList);
        }

        internal static ExpressionSyntax ToInvocationExpression(ExpressionSyntax expression,
            ArgumentListSyntax argumentList = null)
        {
            return SyntaxFactory.InvocationExpression(expression, argumentList ?? SyntaxFactory.ArgumentList());
        }

        internal static ExpressionStatementSyntax AlignExpression(ExpressionStatementSyntax ess)
        {
            if (ess.ToFullString().Length <= 120)
            {
                return ess;
            }

            return ess.WithExpression(AlignExpression(ess.Expression));
        }

        internal static ExpressionSyntax AlignExpression(ExpressionSyntax expression)
        {
            if (expression is InvocationExpressionSyntax invocationExpression)
            {
                return invocationExpression.WithExpression(AlignExpression(invocationExpression.Expression));
            }

            if (expression is MemberAccessExpressionSyntax memberAccessExpression)
            {
                return memberAccessExpression
                    .WithExpression(AlignExpression(memberAccessExpression.Expression))
                    .WithOperatorToken(
                        memberAccessExpression.OperatorToken
                            .WithLeadingTrivia(EndOfLineTrivia, Whitespace(16)));
            }

            return expression;
        }

        internal static MethodDeclarationSyntax NormalizeWhitespace(MethodDeclarationSyntax node)
        {
            var body = node.Body;
            var statements = new SyntaxList<StatementSyntax>();
            // ReSharper disable once PossibleNullReferenceException
            statements = body.Statements.OfType<ExpressionStatementSyntax>()
                .Select(SyntaxHelper.AlignExpression)
                .Aggregate(statements, (current, newEss) => current.Add(newEss));
            body = body.WithStatements(statements);
            node = node.WithBody(body);
            return node;
        }

        internal static SyntaxTrivia EndOfLineTrivia => SyntaxFactory.EndOfLine(Environment.NewLine);

        internal static SyntaxTrivia Whitespace(int count) => SyntaxFactory.Whitespace(new string(' ', count));
    }
}
