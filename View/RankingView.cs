using System;
using WisielecCore.Model;
using WisielecCore.Resources;
using WisielecCore.Utilities;
using WisielecCore.View;

namespace WisielecConsole.View
{
    public class RankingView : IRankingView
    {
        private string Language { get; set; }

        public RankingView(string language)
        {
            Language = language;
        }

        public void DisplayRanking(string language)
        {

            ConsoleHelper.ClearSection(0, 30);

            ConsoleHelper.DisplayPressEscInfo(language);
            ConsoleHelper.DisplayColoredTitle(LanguageResources.GetResource("Title", language), line: 1);

            int currentLine = 4;

            var ranking = GameModel.LoadRanking(language);

            if (ranking.Count == 0)
            {
                ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("NoResults", language), ConsoleColor.Red, currentLine);
                currentLine++;
                currentLine++;
                // Czekamy na naciśnięcie Esc
                WaitForEsc();
                return;
            }

            ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("Ranking", language), ConsoleColor.Yellow, currentLine);
            currentLine++;
            currentLine++;

            int position = 1;
            foreach (var entry in ranking)
            {
                string trophy = position switch
                {
                    1 => "🏆",
                    2 => "🥈",
                    3 => "🥉",
                    _ => "  "
                };

                string line = $"{position,3}. {trophy} {entry.PlayerName,-15} {entry.Score,5} {LanguageResources.GetResource("Points", language)}";
                ConsoleHelper.DisplayCenteredMessage(line, ConsoleColor.White, currentLine);
                currentLine++;
                position++;
            }

            Console.WriteLine("\n");
            // Czekamy na naciśnięcie Esc
            WaitForEsc();
            // Czyścimy powyższe linie
            ConsoleHelper.ClearSection(0, 30);
        }

        public void DisplayRankingWithHighlight(GameModel model, Guid gameId, int startLine)
        {
            var ranking = GameModel.LoadRanking(model.Language);

            if (ranking.Count == 0)
            {
                ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("NoResults", model.Language), ConsoleColor.Red, startLine);
                Console.WriteLine();
                Console.WriteLine();
                return;
            }

            ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("Ranking", model.Language), ConsoleColor.Yellow, startLine);
            startLine++;
            startLine++;

            int position = 1;
            foreach (var entry in ranking)
            {
                string trophy = position switch
                {
                    1 => "🏆",
                    2 => "🥈",
                    3 => "🥉",
                    _ => "  "
                };

                bool isFromCurrentGame = entry.GameId == gameId;

                if (isFromCurrentGame)
                {
                    Console.BackgroundColor = ConsoleColor.DarkYellow;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                string line = $"{position,3}. {trophy} {entry.PlayerName,-15} {entry.Score,5} {LanguageResources.GetResource("Points", model.Language)}";
                ConsoleHelper.DisplayCenteredMessage(line, ConsoleColor.White, startLine);

                if (isFromCurrentGame)
                {
                    Console.ResetColor();
                }

                startLine++;
                position++;
            }

            Console.WriteLine("\n");
        }

        private void WaitForEsc()
        {
            ConsoleKeyInfo keyInfo;
            do
            {
                keyInfo = Console.ReadKey(true);
            } while (keyInfo.Key != ConsoleKey.Escape);
        }
    }
}
