using WisielecConsole.Resources;

namespace WisielecConsole.Utilities
{
    public static class ValidationHelper
    {
        public static int GetValidIntegerInput(string prompt, int min, int max, string language)
        {
            int result;
            while (true)
            {
                ConsoleHelper.ClearLine(Console.CursorTop);
                Console.Write(prompt);

                string input = Console.ReadLine();
                if (int.TryParse(input, out result) && result >= min && result <= max)
                {
                    return result;
                }
                else
                {
                    ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("InvalidNumberInput", language), ConsoleColor.Red);
                    Thread.Sleep(1000);
                    ConsoleHelper.ClearLine(Console.CursorTop - 1); // Czyścimy komunikat o błędzie
                }
            }
        }

        public static string GetValidStringInput(string prompt, string language)
        {
            while (true)
            {
                ConsoleHelper.ClearLine(Console.CursorTop);
                Console.Write(prompt); // Używamy Console.Write zamiast Console.WriteLine

                string input = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input;
                }
                else
                {
                    ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("InvalidStringInput", language), ConsoleColor.Red);
                    Thread.Sleep(1000);
                    ConsoleHelper.ClearLine(Console.CursorTop - 1); // Czyścimy komunikat o błędzie
                }
            }
        }
    }
}
