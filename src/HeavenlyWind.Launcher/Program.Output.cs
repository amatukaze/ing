using System;

namespace HeavenlyWind
{
    partial class Program
    {
        static ConsoleColor _defaultConsoleColor;

        static void Print(string content) => Print(content, _defaultConsoleColor);
        static void Print(string content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(content);
            Console.ForegroundColor = _defaultConsoleColor;
        }
        static void PrintLine() => Console.WriteLine();
        static void PrintLine(string content) => PrintLine(content, _defaultConsoleColor);
        static void PrintLine(string content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            Console.ForegroundColor = _defaultConsoleColor;
        }
    }
}
