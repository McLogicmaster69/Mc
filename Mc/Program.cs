using Mc.Binding;
using Mc.Main;
using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc
{
    internal static class Program
    {
        static void Main()
        {
            bool ShowTree = false;
            while (true)
            {

                Console.Write("Mc: > ");
                string Line = Console.ReadLine();
                if (String.IsNullOrEmpty(Line))
                    continue;
                if(Line == "#ShowTree")
                {
                    ShowTree = !ShowTree;
                    Console.WriteLine(ShowTree ? "Showing parse trees" : "Hiding parse trees");
                    continue;
                }
                else if (Line == "#Clear" || Line == "#cls")
                {
                    Console.Clear();
                    continue;
                }
                else if(Line == "#End")
                {
                    return;
                }
                else if(Line == "#Mc")
                {
                    Console.WriteLine("Mc PROGRAMMING LANGUAGE");
                    Console.WriteLine("Built using C#");
                    Console.WriteLine($"Current Version: {McInfo.Version}");
                    continue;
                }

                SyntaxTree syntaxTree = SyntaxTree.Parse(Line);
                Binder binder = new Binder();
                BoundExpression boundExpression = binder.BindExpression(syntaxTree.Root);
                IReadOnlyList<string> diagnostics = syntaxTree.Diagnostics;

                if (ShowTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ForegroundColor = color;
                }

                if (!diagnostics.Any())
                {
                    Evaluator e = new Evaluator(boundExpression);
                    int Result = e.Evaluate();
                    Console.WriteLine(Result);
                }
                else
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach (var diagnostic in diagnostics)
                        Console.WriteLine(diagnostic);

                    Console.ForegroundColor = color;
                }

            }
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool IsLast = true)
        {

            //└──
            //├──
            //│  

            var Marker = IsLast ? "└──" : "├──";

            Console.Write(indent);
            Console.Write(Marker);
            Console.Write(node.Kind);

            if(node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }

            Console.WriteLine();

            indent += IsLast ? "   " : "│  ";

            var last = node.GetChildren().LastOrDefault();

            foreach (var child in node.GetChildren())
            {
                PrettyPrint(child, indent, child == last);
            }
        }
    }
}
