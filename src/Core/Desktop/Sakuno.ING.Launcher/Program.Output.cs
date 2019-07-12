using System;

namespace Sakuno.ING
{
    internal partial class Program
    {
        private static ConsoleColor _defaultConsoleColor;

        private static void Print(string content) => Print(content, _defaultConsoleColor);

        private static void Print(string content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(content);
            Console.ForegroundColor = _defaultConsoleColor;
        }

        private static void PrintLine() => Console.WriteLine();
        private static void PrintLine(string content) => PrintLine(content, _defaultConsoleColor);

        private static void PrintLine(string content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            Console.ForegroundColor = _defaultConsoleColor;
        }

        private static void Print(char content) => Print(content, _defaultConsoleColor);

        private static void Print(char content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(content);
            Console.ForegroundColor = _defaultConsoleColor;
        }

        private static void PrintLine(char content) => PrintLine(content, _defaultConsoleColor);

        private static void PrintLine(char content, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(content);
            Console.ForegroundColor = _defaultConsoleColor;
        }
    }
}
