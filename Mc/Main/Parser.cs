using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.Main
{
    internal sealed class Parser
    {
        private readonly SyntaxToken[] Tokens;
        private List<string> diagnostics = new List<string>();
        private int Position;

        //Generate Tokens
        public Parser(string Text)
        {
            List<SyntaxToken> tokens = new List<SyntaxToken>();
            Lexer lexer = new Lexer(Text);
            SyntaxToken token;

            //Get the token list
            do
            {
                token = lexer.NextToken();
                if(token.Kind != SyntaxKind.WhiteSpaceToken && token.Kind != SyntaxKind.BadToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);

            Tokens = tokens.ToArray();
            diagnostics.AddRange(lexer.Diagnostics);

        }

        public IEnumerable<string> Diagnostics => diagnostics;

        private SyntaxToken Peek(int Offset)
        {
            int Index = Position + Offset;
            if (Index >= Tokens.Length)
                return Tokens[Tokens.Length - 1];
            return Tokens[Index];
        }

        private SyntaxToken Current => Peek(0);

        private SyntaxToken NextToken()
        {
            SyntaxToken current = Current;
            Position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if(Current.Kind == kind)
                return NextToken();

            //ERROR
            diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);

        }

        //Begin Parse
        public SyntaxTree Parse()
        {
            ExpressionSyntax expression = ParseExpression();
            SyntaxToken EndOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(Diagnostics, expression, EndOfFileToken);
        }

        private ExpressionSyntax ParseExpression(int ParentPrecedence = 0)
        {
            ExpressionSyntax left;
            int UnaryOP = Current.Kind.GetBinaryOperatorPrecedence();
            if(UnaryOP != 0 && UnaryOP >= ParentPrecedence)
            {
                SyntaxToken OT = NextToken();
                ExpressionSyntax operand = ParseExpression(UnaryOP);
                left = new UnaryExpressionSyntax(OT, operand);
            }
            else
            {
                left = ParsePrimaryExpression();
            }

            while (true)
            {
                int Precedence = Current.Kind.GetBinaryOperatorPrecedence();
                if (Precedence == 0 || Precedence <= ParentPrecedence)
                    break;

                SyntaxToken OperatorToken = NextToken();
                ExpressionSyntax right = ParseExpression(Precedence);
                left = new BinaryExpressionSyntax(left, OperatorToken, right);
            }

            return left;
        }

        // ( and )
        private ExpressionSyntax ParsePrimaryExpression()
        {
            switch (Current.Kind)
            {
                case SyntaxKind.OpenParenthesisToken:
                {
                        SyntaxToken left = NextToken();
                        ExpressionSyntax expression = ParseExpression();
                        SyntaxToken right = MatchToken(SyntaxKind.CloseParenthesisToken);
                        return new ParenthesizedExpressionSyntax(left, expression, right);
                }

                case SyntaxKind.TrueKeyword:
                case SyntaxKind.FalseKeyword:
                {
                        SyntaxToken KeywordToken = NextToken();
                        bool value = KeywordToken.Kind == SyntaxKind.TrueKeyword;
                        return new LiteralExpressionSyntax(KeywordToken, value);
                }

                default:
                {

                        SyntaxToken NumberToken = MatchToken(SyntaxKind.NumberToken);
                        return new LiteralExpressionSyntax(NumberToken);

                }
            }
        }
    }
}
