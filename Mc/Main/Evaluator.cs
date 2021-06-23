using Mc.Binding;
using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.Main
{
    internal sealed class Evaluator
    {
        private readonly BoundExpression Root;

        public Evaluator(BoundExpression root)
        {
            Root = root;
        }

        public object Evaluate()
        {
            return EvaluateExpression(Root);
        }

        private object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression n)
                return n.Value;
            if(node is BoundUnaryExpression u)
            {
                object operand = EvaluateExpression(u.Operand);
                switch (u.OperatorKind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return (int) operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int) operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;
                    default:
                        throw new Exception($"Unexpected binary operator {u.OperatorKind}");
                }
            }
            if(node is BoundBinaryExpression b)
            {
                object left = EvaluateExpression(b.Left);
                object right = EvaluateExpression(b.Right);

                switch (b.OperatorKind)
                {
                    case BoundBinaryOperatorKind.Addition:
                        return (int) left + (int)right;
                    case BoundBinaryOperatorKind.Subtraction:
                        return (int) left - (int) right;
                    case BoundBinaryOperatorKind.Multiplication:
                        return (int) left * (int) right;
                    case BoundBinaryOperatorKind.Division:
                        return (int) left / (int) right;
                    case BoundBinaryOperatorKind.LogicalAnd:
                        return (bool) left && (bool) right;
                    case BoundBinaryOperatorKind.LogicalOr:
                        return (bool) left || (bool) right;
                    default:
                        throw new Exception($"Unexpected binary operator {b.OperatorKind}");
                }
            }

            throw new Exception($"Unexpected node {node.Kind}");

        }
    }
}
