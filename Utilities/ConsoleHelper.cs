using System;
using WisielecConsole.Resources;

namespace WisielecConsole.Utilities
{
    public static class ConsoleHelper
    {
        private static readonly object consoleLock = new object(); // Obiekt blokady

        // Centrowanie tekstu na określonej linii
        public static void CenterText(string text, int line = -1, bool newLine = true, ConsoleColor? color = null)
        {
            lock (consoleLock)
            {
                if (line >= 0)
                {
                    try
                    {
                        Console.SetCursorPosition(0, line);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        
                    }
                }

                int windowWidth = Console.WindowWidth;
                int textLength = text.Length;
                int leftPadding = (windowWidth - textLength) / 2;
                leftPadding = Math.Max(0, leftPadding);

                if (color.HasValue)
                {
                    Console.ForegroundColor = color.Value;
                }

                string paddedText = new string(' ', leftPadding) + text;

                try
                {
                    Console.SetCursorPosition(leftPadding, line >= 0 ? line : Console.CursorTop);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.SetCursorPosition(0, line >= 0 ? line : Console.CursorTop);
                }

                if (newLine)
                {
                    Console.WriteLine(text);
                }
                else
                {
                    Console.Write(text);
                }

                if (color.HasValue)
                {
                    Console.ResetColor();
                }
            }
        }



        // Metoda do czyszczenia konkretnej linii
        public static void ClearLine(int line)
        {
            lock (consoleLock)
            {
                try
                {
                    Console.SetCursorPosition(0, line);
                    Console.Write(new string(' ', Console.WindowWidth));
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Ignorowanie, jeśli linia jest poza zakresem
                }
            }
        }

        // Metoda do czyszczenia określonej sekcji ekranu
        public static void ClearSection(int startLine, int numberOfLines)
        {
            lock (consoleLock)
            {
                for (int i = 0; i < numberOfLines; i++)
                {
                    ClearLine(startLine + i);
                }
            }
        }

        // Metoda wyboru opcji
        public static int SelectOption(string[] options, bool allowEsc = true, int startLine = -1)
        {
            lock (consoleLock)
            {
                int selectedIndex = 0;
                ConsoleKey key;

                int cursorTop = startLine >= 0 ? startLine : Console.CursorTop;

                // Wyświetlenie wszystkich opcji początkowo
                for (int i = 0; i < options.Length; i++)
                {
                    Console.SetCursorPosition(0, cursorTop + i);
                    Console.Write(new string(' ', Console.WindowWidth));
                    Console.SetCursorPosition(0, cursorTop + i);
                    CenterText(options[i], line: cursorTop + i, newLine: false);
                }

                do
                {
                    // Aktualizacja zaznaczenia
                    for (int i = 0; i < options.Length; i++)
                    {
                        Console.SetCursorPosition(0, cursorTop + i);
                        Console.Write(new string(' ', Console.WindowWidth));
                        Console.SetCursorPosition(0, cursorTop + i);

                        if (i == selectedIndex)
                        {
                            Console.ForegroundColor = ConsoleColor.Blue;
                            CenterText($"> {options[i]} <", cursorTop + i, newLine: false, color: ConsoleColor.Blue);
                            Console.ResetColor();
                        }
                        else
                        {
                            CenterText(options[i], line: cursorTop + i, newLine: false);
                        }
                    }

                    key = Console.ReadKey(true).Key;

                    if (key == ConsoleKey.UpArrow || key == ConsoleKey.LeftArrow)
                    {
                        selectedIndex = selectedIndex == 0 ? options.Length - 1 : selectedIndex - 1;
                    }
                    else if (key == ConsoleKey.DownArrow || key == ConsoleKey.RightArrow)
                    {
                        selectedIndex = (selectedIndex + 1) % options.Length;
                    }
                    else if (key == ConsoleKey.Escape && allowEsc)
                    {
                        return -1;
                    }

                } while (key != ConsoleKey.Enter);

                return selectedIndex;
            }
        }


        // Metoda do wyświetlania komunikatów centrowanych na określonej linii
        public static void DisplayCenteredMessage(string text, ConsoleColor color = ConsoleColor.White, int line = -1)
        {
            lock (consoleLock)
            {
                Console.ForegroundColor = color;
                CenterText(text, line: line, newLine: false, color: color);
                Console.ResetColor();
            }
        }

        // Metoda do wyświetlania tytułu w kolorze na określonej linii
        public static void DisplayColoredTitle(string text, int line = 1)
        {
            lock (consoleLock)
            {
                ConsoleColor[] colors = {
                    ConsoleColor.DarkMagenta,
                    ConsoleColor.DarkBlue,
                    ConsoleColor.Cyan,
                    ConsoleColor.DarkGreen,
                    ConsoleColor.Yellow,
                    ConsoleColor.DarkYellow,
                    ConsoleColor.Red,
                    ConsoleColor.DarkRed
                };

                int colorIndex = 0;
                int windowWidth = Console.WindowWidth;
                int leftPadding = (windowWidth - text.Length) / 2;
                leftPadding = Math.Max(0, leftPadding);

                try
                {
                    Console.SetCursorPosition(leftPadding, line);
                }
                catch (ArgumentOutOfRangeException)
                {
                    Console.SetCursorPosition(0, line);
                }

                foreach (char c in text)
                {
                    Console.ForegroundColor = colors[colorIndex % colors.Length];
                    Console.Write(c);
                    colorIndex++;
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        // Metoda do wyświetlania informacji o naciśnięciu Esc w prawym górnym rogu
        public static void DisplayPressEscInfo(string language)
        {
            lock (consoleLock)
            {
                string info = LanguageResources.GetResource("PressEscToGoBack", language);
                int windowWidth = Console.WindowWidth;
                int infoLength = info.Length;
                int leftPadding = windowWidth - infoLength - 1;
                leftPadding = Math.Max(0, leftPadding); 

                try
                {
                    Console.SetCursorPosition(leftPadding, 0);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(info);
                    Console.ResetColor();
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Jeśli obliczona pozycja jest poza buforem, wyświetlamy na początku linii
                    Console.SetCursorPosition(0, 0);
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(info);
                    Console.ResetColor();
                }
            }
        }

        // Metoda do pisania tekstu w określonej pozycji
        public static void WriteAtPosition(string text, int column, int line, ConsoleColor? color = null)
        {
            lock (consoleLock)
            {
                try
                {
                    // Upewnienie się, że kolumna nie przekracza szerokości konsoli
                    if (column >= Console.WindowWidth)
                        column = Console.WindowWidth - 1;
                    if (line >= Console.WindowHeight)
                        line = Console.WindowHeight - 1;

                    Console.SetCursorPosition(column, line);
                    if (color.HasValue)
                    {
                        Console.ForegroundColor = color.Value;
                    }
                    Console.Write(text);
                    if (color.HasValue)
                    {
                        Console.ResetColor();
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Ignorowanie, jeśli pozycja jest poza zakresem konsoli
                }
            }
        }


    }
}
