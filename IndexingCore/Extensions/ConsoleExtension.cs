using System;

namespace IndexerCore.Extensions
{
    static public class ConsoleExtension
    {
        public static void WriteLine(this ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Write(color, text + "\n");
            Console.ResetColor();
        }

        public static void Write(this ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}
