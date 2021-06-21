﻿using Mc.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc
{
    class Program
    {
        static void Main(string[] args)
        {
            bool ShowTree = false;
            while (true)
            {

                Console.Write("Mc: > ");
                string Line = Console.ReadLine();
                if (String.IsNullOrEmpty(Line))
                    return;
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

                SyntaxTree syntaxTree = SyntaxTree.Parse(Line);

                if (ShowTree)
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ForegroundColor = color;
                }

                if (!syntaxTree.Diagnostics.Any())
                {
                    Evaluator e = new Evaluator(syntaxTree.Root);
                    int Result = e.Evaluate();
                    Console.WriteLine(Result);
                }
                else
                {
                    var color = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;

                    foreach (var diagnostic in syntaxTree.Diagnostics)
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