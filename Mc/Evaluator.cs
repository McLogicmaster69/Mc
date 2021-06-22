using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc
{
    public sealed class Evaluator
    {
        private readonly ExpressionSyntax Root;

        public Evaluator(ExpressionSyntax root)
        {
            this.Root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(Root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is NumberExpressionSyntax n)
                return (int) n.NumberToken.Value;
            if(node is BinaryExpressionSyntax b)
            {
                int left = EvaluateExpression(b.Left);
                int right = EvaluateExpression(b.Right);

                if (b.OperatorToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                if (b.OperatorToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                if (b.OperatorToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                if (b.OperatorToken.Kind == SyntaxKind.SlashToken)
                    return left / right;
                else
                    throw new Exception($"Unexpected binary operator {b.OperatorToken.Kind}");
            }

            if (node is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);

            throw new Exception($"Unexpected node {node.Kind}");

        }
    }
}
