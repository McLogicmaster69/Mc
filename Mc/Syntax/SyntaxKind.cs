using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.Syntax
{
    public enum SyntaxKind
    {

        BadToken,
        EndOfFileToken,

        NumberToken,
        WhiteSpaceToken,

        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,

        OpenParenthesisToken,
        CloseParenthesisToken,

        LiteralExpression,
        BinaryExpression,
        UnaryExpression,
        ParenthesizedExpression

    }
}
