using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.Syntax
{
    sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openToken, ExpressionSyntax expression, SyntaxToken closeToken)
        {
            OpenToken = openToken;
            Expression = expression;
            CloseToken = closeToken;
        }

        public SyntaxToken OpenToken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseToken { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenToken;
            yield return Expression;
            yield return CloseToken;
        }
    }
}
