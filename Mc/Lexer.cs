using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc
{
    internal sealed class Lexer
    {
        private readonly string Text;
        private int Position;
        private List<string> diagnostics = new List<string>();

        public Lexer(string text)
        {
            Text = text;
        }

        public IEnumerable<string> Diagnostics => diagnostics;

        private char Current
        {
            get
            {
                if(Position >= Text.Length)
                    return '\0';
                return Text[Position];
            }
        }

        private void Next()
        {
            Position++;
        }

        //Check the next type of token
        public SyntaxToken NextToken()
        {

            //Check if not at the end of the text
            if(Position >= Text.Length)
                return new SyntaxToken(SyntaxKind.EndOfFileToken, Position, "\0", null);

            //Check if the character is a number
            if (char.IsDigit(Current))
            {
                int Start = Position;
                while (char.IsDigit(Current))
                    Next();

                int length = Position - Start;
                string text = Text.Substring(Start, length);
                if(!int.TryParse(text, out var value))
                    diagnostics.Add($"The number {text} is not a valid Int32");
                return new SyntaxToken(SyntaxKind.NumberToken, Start, text, value);
            }

            //Check if there is whitespace
            if (char.IsWhiteSpace(Current))
            {
                int Start = Position;
                while (char.IsWhiteSpace(Current))
                    Next();

                int length = Position - Start;
                string text = Text.Substring(Start, length);
                int.TryParse(text, out var value);
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, Start, text);
            }

            //Check for symbols
            if (Current == '+')
                return new SyntaxToken(SyntaxKind.PlusToken, Position++, "+");
            if (Current == '-')
                return new SyntaxToken(SyntaxKind.MinusToken, Position++, "-");
            if (Current == '*')
                return new SyntaxToken(SyntaxKind.StarToken, Position++, "*");
            if (Current == '/')
                return new SyntaxToken(SyntaxKind.SlashToken, Position++, "/");
            if (Current == '(')
                return new SyntaxToken(SyntaxKind.OpenParenthesisToken, Position++, "*");
            if (Current == ')')
                return new SyntaxToken(SyntaxKind.CloseParenthesisToken, Position++, "/");

            //ERROR
            diagnostics.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, Position++, Text.Substring(Position - 1, 1));

        }

    }
}
