using System;
using System.IO;
using WisielecConsole.Resources;

namespace WisielecConsole.Utilities
{
    public static class ExceptionHandler
    {
        // Metoda do obsługi wyjątków
        public static void HandleException(Exception ex, string language)
        {
            string errorMessage = $"{LanguageResources.GetResource("AnErrorOccurred", language)}: {ex.Message}";
            LogError(errorMessage);
        }

        // Metoda do logowania błędów do pliku
        public static void LogError(string message)
        {
            try
            {
                string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Logs");
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                string logFile = Path.Combine(logDirectory, $"error_{DateTime.Now:yyyyMMdd}.log");
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
                File.AppendAllLines(logFile, new[] { logEntry });
            }
            catch
            {
                // Jeśli logowanie się nie powiedzie, nic nie rób
            }
        }
    }
}
