using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc.Main
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

        private char Current => Peek(0);
        private char LookAhead => Peek(1);

        private char Peek(int offset)
        {
            int index = Position + offset;
            if (index >= Text.Length)
                return '\0';
            return Text[Position];
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
                return new SyntaxToken(SyntaxKind.WhiteSpaceToken, Start, text);
            }

            if (char.IsLetter(Current))
            {
                int Start = Position;
                while (char.IsLetter(Current))
                    Next();

                int length = Position - Start;
                string text = Text.Substring(Start, length);
                SyntaxKind kind = SyntaxFacts.GetKeywordKind(text);
                return new SyntaxToken(kind, Start, text);
            }

            //Check for symbols
            switch (Current)
            {
                case '+':
                    return new SyntaxToken(SyntaxKind.PlusToken, Position++, "+");
                case '-':
                    return new SyntaxToken(SyntaxKind.MinusToken, Position++, "-");
                case '*':
                    return new SyntaxToken(SyntaxKind.StarToken, Position++, "*");
                case '/':
                    return new SyntaxToken(SyntaxKind.SlashToken, Position++, "/");
                case '(':
                    return new SyntaxToken(SyntaxKind.OpenParenthesisToken, Position++, "*");
                case ')':
                    return new SyntaxToken(SyntaxKind.CloseParenthesisToken, Position++, "/");
                case '!':
                    return new SyntaxToken(SyntaxKind.BangToken, Position++, "!");
                case '&':
                    if (LookAhead == '&')
                        return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, Position += 2, "&&");
                    break;
                case '|':
                    if (LookAhead == '|')
                        return new SyntaxToken(SyntaxKind.PipePipeToken, Position += 2, "||");
                    break;
            }

            //ERROR
            diagnostics.Add($"ERROR: bad character input: '{Current}'");
            return new SyntaxToken(SyntaxKind.BadToken, Position++, Text.Substring(Position - 1, 1));

        }

    }
}
