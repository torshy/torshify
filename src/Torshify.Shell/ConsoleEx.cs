using System;

namespace Torshify.Shell
{
    public class ConsoleExtensions
    {
        
    }

    public static class ConsoleEx
    {
        public static string Indent(int count)
        {
            return "".PadLeft(count);
        }

        public static string Truncate(string source, int length)
        {
            if (string.IsNullOrEmpty(source))
                return "Unknown playlist";

            if (source.Length > length)
            {
                source = source.Substring(0, length - 2).Trim() + "..";
            }
            return source;
        }

        public static ConsoleExtensions WriteLine(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            using(BeginColorBlock(color))
            {
                Console.WriteLine(text);
            }

            return new ConsoleExtensions();
        }

        public static ConsoleExtensions WriteLine(string format, ConsoleColor color = ConsoleColor.Gray, params object[] args)
        {
            using (BeginColorBlock(color))
            {
                Console.WriteLine(format, args);
            }

            return new ConsoleExtensions();
        }

        public static ConsoleExtensions Write(string text, ConsoleColor color = ConsoleColor.Gray)
        {
            using(BeginColorBlock(color))
            {
                Console.Write(text);
            }

            return new ConsoleExtensions();
        }
        public static ConsoleExtensions Write(string format, ConsoleColor color = ConsoleColor.Gray, params object[] args)
        {
            using (BeginColorBlock(color))
            {
                Console.Write(format, args);
            }

            return new ConsoleExtensions();
        }

        public static string GetPassword()
        {
            string password = string.Empty;
            ConsoleKeyInfo info = Console.ReadKey(true);
            while (info.Key != ConsoleKey.Enter)
            {
                if (info.Key != ConsoleKey.Backspace)
                {
                    password += info.KeyChar;
                    info = Console.ReadKey(true);
                }
                else if (info.Key == ConsoleKey.Backspace)
                {
                    if (!string.IsNullOrEmpty(password))
                    {
                        password = password.Substring
                            (0, password.Length - 1);
                    }
                    info = Console.ReadKey(true);
                }
            }

            return password;
        }

        public static IDisposable BeginColorBlock(ConsoleColor color)
        {
            return new ColorBlock(color);
        }

        public class ColorBlock : IDisposable
        {
            private readonly ConsoleColor _previousColor;

            public ColorBlock(ConsoleColor foreground)
            {
                _previousColor = Console.ForegroundColor;
                Console.ForegroundColor = foreground;
            }

            public void Dispose()
            {
                Console.ForegroundColor = _previousColor;
            }
        }
    }
}