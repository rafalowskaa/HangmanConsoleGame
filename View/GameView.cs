using WisielecConsole.Model;
using WisielecConsole.Resources;
using WisielecConsole.Utilities;

namespace WisielecConsole.View
{
    // Klasa odpowiedzialna za interfejs użytkownika w konsoli
    public class GameView : IGameView
    {
        // Właściwość Language do przekazywania języka
        public string Language { get; set; }

        // Konstruktor przyjmujący parametr language
        public GameView(string language)
        {
            Language = language;
        }

        // Graficzne reprezentacje wisielca na różnych etapach
        private readonly string[] hangmanStages = new string[]
        {
            // Etapy wisielca (możesz je dostosować)
            "     \n     \n     \n     \n     \n    \n",
            "     \n     \n     \n     \n      \n_|_ \n",
            "  \n|\n|\n|\n|\n _|_  \n",
            "    _____ \n|\n|\n|\n|\n _|_  \n",
            "    _____ \n    |    |\n|\n|\n|\n _|_  \n",
            "    _____ \n    |    |\n    |    O\n|\n|\n _|_  \n",
            "    _____ \n    |    |\n    |    O\n     |    |\n|\n _|_  \n",
            "    _____ \n    |    |\n    |    O\n     |   /|\n|\n _|_  \n",
            "    _____ \n    |    |\n    |    O\n     |   /|\\\n|\n _|_  \n",
            "    _____ \n    |    |\n    |    O\n     |   /|\\\n    |   /\n _|_  \n",
            "    _____ \n    |    |\n    |    O\n     |   /|\\\n      |   / \\\n  _|_   \n"
        };

        // Układy klawiatury dla języków polskiego i angielskiego
        private readonly string[][] keyboardLayouts = new string[][]
        {
            // Polski układ klawiatury
            new string[]
            {
                "A  Ą  B  C  Ć  D  E  Ę  F  G  H  I  J",
                " ",
                "K  L  Ł  M  N  Ń  O  Ó  P  Q  R  S  Ś  T",
                " ",
                "U  W  X  Y  Z  Ź  Ż"
            },
            
            // Angielski układ klawiatury
            new string[]
            {
                "Q  W  E  R  T  Y  U  I  O  P",
                " ",
                "A  S  D  F  G  H  J  K  L",
                " ",
                "Z  X  C  V  B  N  M"
            }
        };

        // Pobiera odpowiedni układ klawiatury na podstawie języka
        private string[] GetKeyboardLayout()
        {
            return Language switch
            {
                "pl" => keyboardLayouts[0],
                "en" => keyboardLayouts[1],
                _ => keyboardLayouts[1]
            };
        }

        // Wyświetla grę wraz z nazwą gracza
        public void DisplayGameWithPlayerName(GameModel model, string playerName, string language, int currentRound, int totalRounds)
        {
            Language = language;
            // Aktualizacja informacji o rundzie i imieniu gracza
            UpdateRoundAndPlayerInfo(model, currentRound, totalRounds);

            // Aktualizacja tytułu
            UpdateColoredTitle(LanguageResources.GetResource("Title", Language));

            // Aktualizacja grafiki wisielca
            UpdateHangmanGraphic(model.AttemptsLeft[model.CurrentPlayer], model.IsGameOver(model.CurrentPlayer), 5);

            // Aktualizacja pozostałych prób i postępu słowa
            UpdateAttemptsAndWord(model, 13);
        }

        // Metody pomocnicze do aktualizacji ekranu
        private void UpdateRoundAndPlayerInfo(GameModel model, int currentRound, int totalRounds)
        {
            string roundInfo = $"{LanguageResources.GetResource("Round", Language)}: {currentRound}/{totalRounds}";
            string playerInfo = $"{LanguageResources.GetResource("NameLabel", Language)}: {model.PlayerNames[model.CurrentPlayer]}";
            string scoreInfo = model.Scores.Count > model.CurrentPlayer
            ? $"{LanguageResources.GetResource("ScoreLabel", Language)}: {model.Scores[model.CurrentPlayer]}"
            : $"{LanguageResources.GetResource("ScoreLabel", Language)}: 0";

            // Czyszczenie linii
            ConsoleHelper.ClearLine(1);
            ConsoleHelper.ClearLine(2);
            ConsoleHelper.ClearLine(3);

            // Wyświetlenie rundy
            Console.SetCursorPosition(0, 1);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(roundInfo.PadRight(Console.WindowWidth));
            Console.ResetColor();

            // Wyświetlenie imienia gracza

            Console.SetCursorPosition(0, 2);
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(playerInfo.PadRight(Console.WindowWidth));
            Console.ResetColor();

            // Wyświetlenie punktacji gracza

            Console.SetCursorPosition(0, 3);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(scoreInfo.PadRight(Console.WindowWidth));
            Console.ResetColor();
        }

        private void UpdateColoredTitle(string text)
        {
            ConsoleHelper.ClearLine(4);
            Console.SetCursorPosition(0, 4);
            ConsoleHelper.DisplayColoredTitle(text);
        }

        public void UpdateHangmanGraphic(int attemptsLeft, bool isGameOver, int startLine)
        {
            int index = isGameOver ? hangmanStages.Length - 1 : Math.Max(0, hangmanStages.Length - attemptsLeft - 1);
            string graphic = hangmanStages[index];

            string[] lines = graphic.Split('\n');
            int currentLine = startLine;

            foreach (var line in lines)
            {
                ConsoleHelper.ClearLine(currentLine);
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                ConsoleHelper.CenterText(line, currentLine);
                currentLine++;
            }
        }


        // Odświeża widok gry
        public void RefreshGameView(GameModel model, string playerName, string language, int currentRound, int totalRounds)
        {
            Language = language;

            // Aktualizacja informacji o rundzie i graczu
            UpdateRoundAndPlayerInfo(model, currentRound, totalRounds);

            // Aktualizacja tytułu (jeśli jest to konieczne)
            UpdateColoredTitle(LanguageResources.GetResource("Title", Language));

            // Aktualizacja grafiki wisielca
            UpdateHangmanGraphic(model.AttemptsLeft[model.CurrentPlayer], model.IsGameOver(model.CurrentPlayer), 5);

            // Aktualizacja prób i postępu słowa
            UpdateAttemptsAndWord(model, 13);
        }


        // Wyświetla postęp słowa i licznik prób
        private void UpdateAttemptsAndWord(GameModel model, int startLine)
        {
            string attemptsInfo = $"{LanguageResources.GetResource("AttemptsLeft", Language)}: {model.AttemptsLeft[model.CurrentPlayer]}";
            string wordProgress = $"{LanguageResources.GetResource("Word", Language)}: {GetWordProgress(model, model.CurrentPlayer)}";

            // Wyświetlenie liczby prób
            ConsoleHelper.ClearLine(startLine);
            Console.SetCursorPosition(0, startLine);
            Console.ForegroundColor = ConsoleColor.Red;
            ConsoleHelper.CenterText(attemptsInfo, newLine: false);
            Console.ResetColor();

            // Wyświetlenie postępu słowa
            ConsoleHelper.ClearLine(startLine + 2);
            Console.SetCursorPosition(0, startLine + 2);
            Console.ForegroundColor = ConsoleColor.Magenta;
            ConsoleHelper.CenterText(wordProgress, newLine: false);
            Console.ResetColor();

            // Wyświetlenie układu klawiatury
            DisplayKeyboardLayout(model, model.CurrentPlayer, startLine + 4);
        }


        // Wyświetla odkryte słowo po zakończeniu rundy
        public void DisplayWordReveal(GameModel model, int playerId)
        {
            int line = 15; // Linia, na której wyświetlamy słowo

            // Czyszczenie tylko tej linii
            ConsoleHelper.ClearLine(line);

            // Wyświetlenie tytułu (jeśli konieczne)
            ConsoleHelper.DisplayColoredTitle(LanguageResources.GetResource("Title", Language));

            ConsoleHelper.DisplayCenteredMessage($"{LanguageResources.GetResource("WordWas", Language)}: {model.WordToGuess[playerId]}", ConsoleColor.Green, line);

            // Krótkie wyświetlenie słowa i automatyczne przejście dalej
            Thread.Sleep(2000);
        }


        // Wyświetla ekran końca gry
        public int DisplayEndGameScreen(GameModel model, Guid gameId)
        {
            ConsoleHelper.ClearSection(0, 30);
            ConsoleHelper.DisplayColoredTitle(LanguageResources.GetResource("Title", Language));

            int currentLine = 3;

            IRankingView rankingView = new RankingView(Language);
            rankingView.DisplayRankingWithHighlight(model, gameId, currentLine);

            currentLine += Math.Min(GameModel.MaxRankingEntries, GameModel.LoadRanking(Language).Count) + 5;


            // Czekamy na wybór użytkownika
            string[] options = {
                LanguageResources.GetResource("RestartGame", Language),
                LanguageResources.GetResource("ReturnToMenu", Language)
            };
            int choice = ConsoleHelper.SelectOption(options, allowEsc: false, startLine: currentLine);


            ConsoleHelper.ClearSection(0, 30);
            return choice;
        }


        // Pobiera postęp odgadywanego słowa
        private string GetWordProgress(GameModel model, int playerId)
        {
            return string.Join(" ", model.WordToGuess[playerId].Select(c =>
                char.IsWhiteSpace(c) ? " " : (model.GuessedLetters[playerId].Contains(c) ? c.ToString() : "_")));
        }

        // Wyświetla układ klawiatury
        private void DisplayKeyboardLayout(GameModel model, int playerId, int startLine)
        {
            var keyboardLayout = GetKeyboardLayout();
            int currentLine = startLine;

            foreach (var row in keyboardLayout)
            {
                var letters = row.Split(' ');
                string rowString = "";

                foreach (string letter in letters)
                {
                    if (string.IsNullOrWhiteSpace(letter))
                    {
                        rowString += "   "; // Trzy spacje
                        continue;
                    }

                    char upperLetter = char.ToUpper(letter[0]);
                    // Dodajemy spacje przed i po literze dla lepszego rozmieszczenia
                    rowString += $" {upperLetter} ";
                }

                // Wyświetlamy cały wiersz scentralizowany z kolorowaniem liter
                DisplayColoredRow(rowString, currentLine, model, playerId);
                currentLine++;
            }
        }
        // Pomoc dla klawiatury
        private void DisplayColoredRow(string rowString, int line, GameModel model, int playerId)
        {
            int windowWidth = Console.WindowWidth;
            int textLength = rowString.Length;
            int leftPadding = (windowWidth - textLength) / 2;
            leftPadding = Math.Max(0, leftPadding);

            // Ustawienie kursora i czyszczenie linii tylko raz
            try
            {
                Console.SetCursorPosition(leftPadding, line);
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.SetCursorPosition(0, line);
            }

            // Budowanie całej linii z kolorami
            for (int i = 0; i < rowString.Length; i++)
            {
                char c = rowString[i];
                if (char.IsLetter(c))
                {
                    char upperLetter = char.ToUpper(c);
                    if (model.GuessedLetters[playerId].Contains(upperLetter))
                    {
                        if (model.WordToGuess[playerId].Contains(upperLetter))
                        {
                            Console.ForegroundColor = ConsoleColor.Green; // Poprawna litera
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red; // Błędna litera
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray; // Litera jeszcze nie zgadywana
                    }
                    Console.Write(upperLetter);
                    Console.ResetColor();
                }
                else
                {
                    // Jeśli to nie litera, wypisujemy znak bez zmian
                    Console.Write(c);
                }
            }

            // Wypełnienie reszty linii spacjami, jeśli konieczne
            int remainingSpace = windowWidth - leftPadding - textLength;
            if (remainingSpace > 0)
            {
                Console.Write(new string(' ', remainingSpace));
            }
        }

        // Wyświetla graficzną reprezentację wisielca
        public void DisplayHangmanGraphic(int attemptsLeft, bool isGameOver, int startLine)
        {
            int index = isGameOver ? hangmanStages.Length - 1 : Math.Max(0, hangmanStages.Length - attemptsLeft - 1);
            string graphic = hangmanStages[index];

            string[] lines = graphic.Split('\n');
            int currentLine = startLine;

            foreach (var line in lines)
            {
                ConsoleHelper.ClearLine(currentLine);
                ConsoleHelper.CenterText(line, currentLine);
                currentLine++;
            }
        }


        // Wyświetla komunikat na ekranie
        public void DisplayMessage(string message, string currentLanguage)
        {
            ConsoleHelper.ClearSection(0, Console.WindowHeight);
            ConsoleHelper.DisplayPressEscInfo(currentLanguage);
            Console.WriteLine("\n\n");
            ConsoleHelper.DisplayCenteredMessage(message, ConsoleColor.White, 3);
            Console.WriteLine();
            ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("PressToContinue", currentLanguage), ConsoleColor.White, 5);
            Console.ReadKey();
        }

        // Wyświetla ekran startowy
        public void DisplayStartScreen(string language)
        {
            ConsoleHelper.ClearSection(0, 30);
            Console.WriteLine("\n\n");
            ConsoleHelper.DisplayColoredTitle(LanguageResources.GetResource("Title", language));
            Console.WriteLine("\n\n");
        }

        // Wyświetla komunikat o rozpoczęciu rundy
        public async Task DisplayRoundStartAsync(int currentRound, int totalRounds, string language)
        {
            string roundInfo = $"{LanguageResources.GetResource("Round", language)} {currentRound}/{totalRounds}";
            int line = 4; // Linia, na której będzie wyświetlany komunikat

            for (int i = 0; i < 6; i++)
            {
                if (i % 2 == 0)
                {
                    // Wyświetlenie komunikatu
                    ConsoleHelper.CenterText(roundInfo, line: line, newLine: false, color: ConsoleColor.Yellow);
                }
                else
                {
                    // Czyszczenie linii
                    ConsoleHelper.ClearLine(line);
                }
                await Task.Delay(250); // Skrócenie czasu migania do 250 ms
            }

            // Po zakończeniu migania, pozostaw komunikat na ekranie
            ConsoleHelper.CenterText(roundInfo, line: line, newLine: false, color: ConsoleColor.Yellow);
        }


        // Wywołuje dźwięk poprawnego odgadnięcia
        public Task PlayCorrectGuessSound()
        {
            SoundManager.PlaySound("correct.mp3", Language);
            return Task.CompletedTask;
        }

        // Wywołuje dźwięk błędnego odgadnięcia
        public Task PlayIncorrectGuessSound()
        {
            SoundManager.PlaySound("incorrect.mp3", Language);
            return Task.CompletedTask;
        }

        public Task PlayApplauseSound()
        {
            SoundManager.PlaySound("applause.mp3", Language);
            return Task.CompletedTask;
        }

        public void StopApplauseSound()
        {
            SoundManager.StopSound("applause.mp3");
        }



        public bool PromptToExitGame(string language)
        {
            int totalLines = Console.WindowHeight;
            int promptStartLine = 15;
            int optionsStartLine = promptStartLine + 2;

            // Wyświetlenie nakładki na całą konsolę
            for (int i = 15; i < totalLines - 5; i++)
            {
                ConsoleHelper.ClearLine(i);
            }

            // Ustawienie kursora na środek
            Console.SetCursorPosition(0, promptStartLine);

            // Wyświetl pytanie
            ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("ConfirmExitGame", language), ConsoleColor.Yellow, promptStartLine);

            // Wyświetl opcje
            string[] options = {
                LanguageResources.GetResource("Yes", language),
                LanguageResources.GetResource("No", language)
            };
            int choice = ConsoleHelper.SelectOption(options, allowEsc: false, startLine: optionsStartLine);

            // Usunięcie nakładki po wyborze
            for (int i = 15; i < totalLines - 5; i++)
            {
                ConsoleHelper.ClearLine(i);
            }

            return choice == 0;
        }



        public void DisplayRoundStartScreen(int currentRound, int totalRounds, string language)
        {
            ConsoleHelper.ClearSection(0, 30);
            DisplayRoundStartAsync(currentRound, totalRounds, language).Wait(); // Czekamy na zakończenie asynchronicznej metody
        }


        // Pobiera literę od gracza lub obsługuje klawisz Esc
        public async Task<(char GuessedChar, bool IsEscPressed, bool IsTimedOut)> GetPlayerGuessAsync(GameModel model, CancellationToken cancellationToken)
        {
            int inputLine = 25;
            int timeLine = 24;
            ConsoleHelper.ClearLine(inputLine);
            ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("EnterLetter", Language), ConsoleColor.Cyan, inputLine);

            char guessedChar = '\0';
            bool isEscPressed = false;
            bool isTimedOut = false;

            // Ustawienie stałego komunikatu
            string timeLabel = $"{LanguageResources.GetResource("TimeLeft", Language)}: ";

            // Ustawienie kursora i wyświetlenie stałego tekstu "Pozostały czas: "
            ConsoleHelper.ClearLine(timeLine);
            ConsoleHelper.WriteAtPosition(timeLabel, (Console.WindowWidth - timeLabel.Length) / 2, timeLine, ConsoleColor.Yellow);

            // Obliczenie początkowego czasu pozostałego
            TimeSpan remainingTimeInitial = model.RoundEndTime - DateTime.Now;
            int remainingSecondsInitial = (int)Math.Ceiling(remainingTimeInitial.TotalSeconds);
            string initialTimeDisplay = remainingSecondsInitial >= 0 ? $"{remainingSecondsInitial}s" : "0s";
            ConsoleHelper.WriteAtPosition(initialTimeDisplay.PadRight(5), (Console.WindowWidth + timeLabel.Length) / 2, timeLine, ConsoleColor.Yellow);

            // Inicjalizacja timer'a
            var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var timerTask = Task.Run(async () =>
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    await Task.Delay(1000, cts.Token); // Aktualizacja co sekundę
                    if (cts.Token.IsCancellationRequested)
                        break;

                    // Obliczenie pozostałego czasu
                    TimeSpan remainingTime = model.RoundEndTime - DateTime.Now;
                    int remainingSeconds = (int)Math.Ceiling(remainingTime.TotalSeconds);

                    if (remainingSeconds >= 0)
                    {
                        // Aktualizacja tylko sekcji z czasem bez PadRight
                        string timeDisplay = $"{remainingSeconds}s";
                        ConsoleHelper.WriteAtPosition(timeDisplay.PadRight(5), (Console.WindowWidth + timeLabel.Length) / 2, timeLine, ConsoleColor.Yellow);
                    }
                    else
                    {
                        isTimedOut = true;
                        cts.Cancel(); // Zakończenie timera
                    }
                }
            }, cancellationToken);

            // Pętla oczekiwania na input
            while (!isTimedOut && !cancellationToken.IsCancellationRequested)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.Escape)
                    {
                        isEscPressed = true;
                        cts.Cancel();
                        break;
                    }
                    else if (char.IsLetter(keyInfo.KeyChar))
                    {
                        guessedChar = char.ToUpper(keyInfo.KeyChar);
                        // Zwrócenie zgadywanej litery; kontroler powinien odświeżyć widok
                        cts.Cancel(); // Zakończenie timera po wprowadzeniu litery
                        break;
                    }
                    else
                    {
                        // Wyświetlenie komunikatu o błędzie na linii inputLine + 1
                        ConsoleHelper.ClearLine(inputLine + 1);
                        ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("InvalidInput", Language), ConsoleColor.Red, inputLine + 1);
                        await Task.Delay(200, cancellationToken); // Skrócenie czasu wyświetlania komunikatu
                        ConsoleHelper.ClearLine(inputLine + 1);
                    }
                }
                else
                {
                    await Task.Delay(50, cancellationToken); // Skrócenie interwału sprawdzania do 50 ms
                }
            }

            // Zakończenie timer'a
            cts.Cancel();
            try
            {
                await timerTask;
            }
            catch (TaskCanceledException)
            {
                // Ignorowanie wyjątku TaskCanceledException
            }

            return (guessedChar, isEscPressed, isTimedOut);
        }
    }
}
