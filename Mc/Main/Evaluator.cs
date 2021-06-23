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
                switch (u.Op.Kind)
                {
                    case BoundUnaryOperatorKind.Identity:
                        return (int) operand;
                    case BoundUnaryOperatorKind.Negation:
                        return -(int) operand;
                    case BoundUnaryOperatorKind.LogicalNegation:
                        return !(bool)operand;
                    default:
                        throw new Exception($"Unexpected binary operator {u.Op}");
                }
            }
            if(node is BoundBinaryExpression b)
            {
                object left = EvaluateExpression(b.Left);
                object right = EvaluateExpression(b.Right);

                switch (b.Op.Kind)
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
                    case BoundBinaryOperatorKind.Equivalent:
                        return Equals(left, right);
                    case BoundBinaryOperatorKind.NotEquivalent:
                        return !Equals(left, right);
                    default:
                        throw new Exception($"Unexpected binary operator {b.Op}");
                }
            }

            throw new Exception($"Unexpected node {node.Kind}");

        }
    }
}
