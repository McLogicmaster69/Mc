using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc
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

        private SyntaxToken Match(SyntaxKind kind)
        {
            if(Current.Kind == kind)
                return NextToken();

            //ERROR
            diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);

        }

        private ExpressionSyntax ParseExpression()
        {
            return ParseTerm();
        }

        //Begin Parse
        public SyntaxTree Parse()
        {
            ExpressionSyntax expression = ParseTerm();
            SyntaxToken EndOfFileToken = Match(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(Diagnostics, expression, EndOfFileToken);
        }

        // + and -
        private ExpressionSyntax ParseTerm()
        {
            ExpressionSyntax Left = ParseFactor();
            while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
            {
                SyntaxToken OperatorToken = NextToken();
                ExpressionSyntax Right = ParseFactor();
                Left = new BinaryExpressionSyntax(Left, OperatorToken, Right);
            }
            return Left;
        }

        // * and /
        private ExpressionSyntax ParseFactor()
        {
            ExpressionSyntax Left = ParsePrimaryExpression();
            while (Current.Kind == SyntaxKind.StarToken || Current.Kind == SyntaxKind.SlashToken)
            {
                SyntaxToken OperatorToken = NextToken();
                ExpressionSyntax Right = ParsePrimaryExpression();
                Left = new BinaryExpressionSyntax(Left, OperatorToken, Right);
            }
            return Left;
        }

        // ( and )
        private ExpressionSyntax ParsePrimaryExpression()
        {
            if(Current.Kind == SyntaxKind.OpenParenthesisToken)
            {
                SyntaxToken left = NextToken();
                ExpressionSyntax expression = ParseExpression();
                SyntaxToken right = Match(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }

            SyntaxToken NumberToken = Match(SyntaxKind.NumberToken);
            return new NumberExpressionSyntax(NumberToken);
        }
    }
}
