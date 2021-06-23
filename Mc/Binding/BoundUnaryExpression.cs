using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.Binding
{
    internal sealed class BoundUnaryExpression : BoundExpression
    {
        public BoundUnaryExpression(BoundUnaryOperator op,BoundExpression operand)
        {
            Op = op;
            Operand = operand;
        }

        public BoundUnaryOperator Op { get; }
        public BoundExpression Operand { get; }
        public override Type Type => Op.Type;
        public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    }
}
