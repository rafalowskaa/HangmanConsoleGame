using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WisielecConsole.View;
using WisielecCore.Utilities;
using WisielecCore.Controller;
using WisielecCore.Model;
using WisielecCore.View;
using WisielecCore.Resources;

namespace WisielecConsole
{
    class Program
    {
        // Aktualny język gry (domyślnie polski)
        static string CurrentLanguage = "pl";
        // Stos do śledzenia historii widoków
        static Stack<Func<Task>> ViewStack = new Stack<Func<Task>>();

        // Zmienne do monitorowania rozmiaru okna
        private static volatile int previousWindowWidth = Console.WindowWidth;
        private static volatile int previousWindowHeight = Console.WindowHeight;
        private static volatile bool needRefresh = false;

        // Token anulowania dla wątku monitorującego rozmiar okna
        private static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            // Uruchomienie muzyki w tle
            BackgroundMusicPlayer.StartBackgroundMusic();

            // Uruchomienie wątku monitorującego rozmiar okna
            Thread windowMonitorThread = new Thread(() => WindowSizeMonitor(cancellationTokenSource.Token));
            windowMonitorThread.IsBackground = true;
            windowMonitorThread.Start();

            try
            {
                CurrentLanguage = SelectLanguage();
                ViewStack.Push(MainMenu);

                await ViewStack.Peek()();

                // Po wyjściu z menu głównego kończymy program
                ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("ThankYou", CurrentLanguage), ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                ConsoleHelper.DisplayCenteredMessage($"{LanguageResources.GetResource("AnErrorOccurred", CurrentLanguage)}: {ex.Message}", ConsoleColor.Red);
                ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("PressToContinue", CurrentLanguage));
                Console.ReadKey();
            }
            finally
            {
                // Zatrzymanie muzyki w tle
                BackgroundMusicPlayer.StopBackgroundMusic();

                // Anulowanie wątku monitorującego rozmiar okna
                cancellationTokenSource.Cancel();
                windowMonitorThread.Join();

                // Przywrócenie widoczności kursora
                Console.CursorVisible = true;
            }
        }

        // Metoda monitorująca zmianę rozmiaru okna
        static void WindowSizeMonitor(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (Console.WindowWidth != previousWindowWidth || Console.WindowHeight != previousWindowHeight)
                    {
                        previousWindowWidth = Console.WindowWidth;
                        previousWindowHeight = Console.WindowHeight;

                        if (Console.WindowWidth < 80 || Console.WindowHeight < 25)
                        {
                            // Okno jest za małe
                            needRefresh = true;
                            ConsoleHelper.ClearSection(0, 30);
                            ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("WindowTooSmall", CurrentLanguage), ConsoleColor.Red);
                            // Czekaj, aż rozmiar okna zostanie zwiększony
                            while (Console.WindowWidth < 80 || Console.WindowHeight < 25)
                            {
                                if (cancellationToken.IsCancellationRequested)
                                    return;
                                Thread.Sleep(500);
                            }
                            needRefresh = true;
                        }
                        else
                        {
                            needRefresh = true;
                        }
                    }
                }
                catch (IOException)
                {
                    // Obsługa wyjątku, gdy konsola jest niedostępna (np. zminimalizowana)
                    Thread.Sleep(500);
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Obsługa wyjątku, gdy rozmiar konsoli jest poza zakresem
                    Thread.Sleep(500);
                }
                Thread.Sleep(100); // Sprawdzaj co 100 ms
            }
        }

        static async Task MainMenu()
        {
            bool exit = false;
            IGameView gameView = new GameView(CurrentLanguage);
            IRankingView rankingView = new RankingView(CurrentLanguage);

            while (!exit)
            {
                if (needRefresh)
                {
                    ConsoleHelper.ClearSection(0, Console.WindowHeight); 
                    needRefresh = false;
                }

                ConsoleHelper.ClearSection(0, 30);
                gameView.DisplayStartScreen(CurrentLanguage);
                string[] mainMenuOptions = new string[]
                {
                    LanguageResources.GetResource("StartGame", CurrentLanguage),
                    LanguageResources.GetResource("ShowRanking", CurrentLanguage),
                    LanguageResources.GetResource("ClearRanking", CurrentLanguage),
                    LanguageResources.GetResource("ExitGame", CurrentLanguage),
                    LanguageResources.GetResource("ChangeLanguage", CurrentLanguage)
                };

                int selectedOption = ConsoleHelper.SelectOption(mainMenuOptions, allowEsc: false);

                switch (selectedOption)
                {
                    case 0:
                        ViewStack.Push(SelectGameMode);
                        await ViewStack.Peek()();
                        break;
                    case 1:
                        rankingView.DisplayRanking(CurrentLanguage);
                        break;
                    case 2:
                        ClearRanking(CurrentLanguage);
                        break;
                    case 3:
                        exit = true;
                        break;
                    case 4:
                        CurrentLanguage = SelectLanguage();
                        break;
                }
            }
        }

        private static void ClearRanking(string language)
        {
            ConsoleHelper.ClearSection(0, 30);
            ConsoleHelper.DisplayPressEscInfo(language);
            ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("ClearRankingConfirm", language), ConsoleColor.Red, 3);
            Console.WriteLine();
            string[] options = {
                LanguageResources.GetResource("Yes", language),
                LanguageResources.GetResource("No", language)
            };
            int choice = ConsoleHelper.SelectOption(options, allowEsc: false);
            if (choice == 0)
            {
                GameModel.ClearRanking(language);
                IGameView gameView = new GameView(language);
                gameView.DisplayMessage(LanguageResources.GetResource("RankingCleared", language), language);
            }
        }

        // Wybór języka gry
        private static string SelectLanguage()
        {
            ConsoleHelper.ClearSection(0, 30);
            Console.WriteLine("\n\n");
            ConsoleHelper.DisplayColoredTitle("WISIELEC / HANGMAN");
            Console.WriteLine("\n\n");
            string[] options = { "Polski", "English" };
            int choice = ConsoleHelper.SelectOption(options, allowEsc: false);
            if (choice == -1)
                return "pl"; // Domyślnie polski
            return choice == 0 ? "pl" : "en";
        }

        // Wybór trybu gry: jednoosobowa czy wieloosobowa
        private static async Task SelectGameMode()
        {
            while (true)
            {
                if (needRefresh)
                {
                    ConsoleHelper.ClearSection(0, 30);
                    needRefresh = false;
                }

                ConsoleHelper.ClearSection(0, 30);
                ConsoleHelper.DisplayPressEscInfo(CurrentLanguage);
                Console.WriteLine("\n\n");
                ConsoleHelper.DisplayColoredTitle(LanguageResources.GetResource("SelectGameMode", CurrentLanguage));
                Console.WriteLine("\n\n");
                string[] options = {
                    LanguageResources.GetResource("SinglePlayer", CurrentLanguage),
                    LanguageResources.GetResource("Multiplayer", CurrentLanguage)
                };
                int choice = ConsoleHelper.SelectOption(options);

                if (choice == -1)
                {
                    // Użytkownik nacisnął Esc – wracamy do menu głównego
                    if (ViewStack.Count > 1)
                    {
                        ViewStack.Pop();
                        await ViewStack.Peek()();
                    }
                    else
                    {
                        ViewStack.Clear();
                        ViewStack.Push(MainMenu);
                        await ViewStack.Peek()();
                    }
                    return;
                }

                if (choice == 0 || choice == 1)
                {
                    ViewStack.Push(() => GetGameSettings(CurrentLanguage, choice == 1));
                    await ViewStack.Peek()();
                    return;
                }
            }
        }

        private enum InputState
        {
            EnteringRounds,
            EnteringPlayerNames
        }

        // Pobranie ustawień gry od użytkownika
        private static async Task GetGameSettings(string language, bool isMultiplayer)
        {
            int rounds = 0;
            List<string> playerNames = new List<string>();
            InputState inputState = InputState.EnteringRounds;

            if (needRefresh)
            {
                ConsoleHelper.ClearSection(0, 30);
                needRefresh = false;
            }

            ConsoleHelper.ClearSection(0, 30);
            ConsoleHelper.DisplayPressEscInfo(CurrentLanguage);
            Console.WriteLine("\n\n");
            ConsoleHelper.DisplayColoredTitle(LanguageResources.GetResource("GameSettings", language), line: 1);
            Console.WriteLine("\n");
            ConsoleHelper.CenterText(LanguageResources.GetResource("EnterNumberOfRoundsAndNames", language), line: 5, newLine: false);
            ConsoleHelper.CenterText($"({LanguageResources.GetResource("MaxRoundsInfo", language)})", line: 7, newLine: false);
            Console.WriteLine();

            int promptLine = 5; // Zapamiętujemy linię, na której zaczynamy wyświetlać prompty

            while (true)
            {
                if (inputState == InputState.EnteringRounds)
                {
                    // Czyszczenie linii z ewentualnych poprzednich komunikatów i promptów
                    ConsoleHelper.ClearSection(promptLine, 7); // Czyścimy 3 linie (prompt, ewentualny błąd, wprowadzane dane)
                    Console.SetCursorPosition(0, promptLine);

                    // Wyświetlanie promptu z kolorem
                    ConsoleHelper.CenterText($"{LanguageResources.GetResource("EnterNumberOfRounds", language)}: ", line: promptLine, newLine: false, color: ConsoleColor.Yellow);
                    string roundsInput = "";
                    ConsoleKeyInfo keyInfo;
                    while (true)
                    {
                        keyInfo = Console.ReadKey(true);
                        if (keyInfo.Key == ConsoleKey.Escape)
                        {
                            // Powrót do poprzedniego widoku
                            if (ViewStack.Count > 1)
                            {
                                ViewStack.Pop();
                                await ViewStack.Peek()();
                            }
                            else
                            {
                                ViewStack.Clear();
                                ViewStack.Push(MainMenu);
                                await ViewStack.Peek()();
                            }
                            return;
                        }
                        else if (keyInfo.Key == ConsoleKey.Enter)
                        {
                            break;
                        }
                        else if (char.IsDigit(keyInfo.KeyChar))
                        {
                            roundsInput += keyInfo.KeyChar;
                            Console.Write(keyInfo.KeyChar);
                        }
                        else if (keyInfo.Key == ConsoleKey.Backspace && roundsInput.Length > 0)
                        {
                            roundsInput = roundsInput.Substring(0, roundsInput.Length - 1);
                            int cursorLeft = Console.CursorLeft;
                            Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
                        }
                    }

                    if (int.TryParse(roundsInput, out rounds) && rounds >= 1 && rounds <= 10)
                    {
                        inputState = InputState.EnteringPlayerNames;
                        Console.WriteLine(); // Przejdź do następnej linii
                    }
                    else
                    {
                        // Wyświetlenie komunikatu o błędzie
                        ConsoleHelper.ClearLine(promptLine + 1);
                        ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("InvalidRoundsNumber", language), ConsoleColor.Red, promptLine + 1);
                        Thread.Sleep(1000);
                        // Czyścimy komunikat o błędzie i prompt
                        ConsoleHelper.ClearLine(promptLine + 1);
                        ConsoleHelper.ClearLine(promptLine);
                        // Przesuwamy kursor do linii z promptem
                        Console.SetCursorPosition(0, promptLine);
                    }
                }
                else if (inputState == InputState.EnteringPlayerNames)
                {
                    // Pobieranie imion graczy
                    int playerCount = isMultiplayer ? 2 : 1;
                    playerNames.Clear();

                    for (int i = 1; i <= playerCount; i++)
                    {
                        while (true)
                        {
                            // Czyszczenie linii z ewentualnych poprzednich komunikatów i promptów
                            ConsoleHelper.ClearSection(promptLine, 7); // Czyścimy 3 linie (prompt, ewentualny błąd, wprowadzane dane)
                            Console.SetCursorPosition(0, promptLine);

                            // Tworzenie promptu dla gracza
                            string playerPrompt = isMultiplayer
                                ? LanguageResources.GetResource("EnterPlayerName", language).Replace("{Player}", i.ToString())
                                : LanguageResources.GetResource("EnterPlayerNameSingle", language);

                            // Wyświetlanie promptu z kolorem
                            ConsoleHelper.CenterText($"{playerPrompt}: ", line: promptLine, newLine: false, color: ConsoleColor.Yellow);

                            // Odczytanie wprowadzonego imienia
                            string playerName = "";
                            ConsoleKeyInfo keyInfo;
                            while (true)
                            {
                                keyInfo = Console.ReadKey(true);
                                if (keyInfo.Key == ConsoleKey.Escape)
                                {
                                    // Cofnięcie się do wprowadzania liczby rund
                                    inputState = InputState.EnteringRounds;
                                    // Czyścimy wcześniej wprowadzone imiona
                                    playerNames.Clear();
                                    break;
                                }
                                else if (keyInfo.Key == ConsoleKey.Enter)
                                {
                                    break;
                                }
                                else if (keyInfo.Key == ConsoleKey.Backspace && playerName.Length > 0)
                                {
                                    playerName = playerName.Substring(0, playerName.Length - 1);
                                    int cursorLeft = Console.CursorLeft;
                                    Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
                                    Console.Write(" ");
                                    Console.SetCursorPosition(cursorLeft - 1, Console.CursorTop);
                                }
                                else if (!char.IsControl(keyInfo.KeyChar))
                                {
                                    playerName += keyInfo.KeyChar;
                                    Console.Write(keyInfo.KeyChar);
                                }
                            }

                            if (inputState == InputState.EnteringRounds)
                            {
                                // Użytkownik nacisnął Esc, wracamy do wprowadzania liczby rund
                                break;
                            }

                            if (!string.IsNullOrWhiteSpace(playerName))
                            {
                                playerNames.Add(playerName);
                                Console.WriteLine(); // Przejdź do następnej linii
                                break;
                            }
                            else
                            {
                                // Wyświetlenie komunikatu o błędzie
                                ConsoleHelper.ClearLine(promptLine + 1);
                                ConsoleHelper.DisplayCenteredMessage(LanguageResources.GetResource("InvalidStringInput", language), ConsoleColor.Red, promptLine + 1);
                                Thread.Sleep(1000);
                                // Czyścimy komunikat o błędzie i prompt
                                ConsoleHelper.ClearLine(promptLine + 1);
                                ConsoleHelper.ClearLine(promptLine);
                                // Przesuwamy kursor do linii z promptem
                                Console.SetCursorPosition(0, promptLine);
                            }
                        }

                        if (inputState == InputState.EnteringRounds)
                        {
                            // Użytkownik nacisnął Esc, przerwij pętlę wprowadzania imion
                            break;
                        }
                    }

                    if (inputState == InputState.EnteringRounds)
                    {
                        // Użytkownik nacisnął Esc, wracamy do wprowadzania liczby rund
                        continue;
                    }

                    // Przechodzimy dalej po pomyślnym wprowadzeniu danych
                    break;
                }
            }

            // Przechodzimy do wyboru poziomu trudności
            ViewStack.Push(() => SelectDifficultyLevel(language, rounds, playerNames, isMultiplayer));
            await ViewStack.Peek()();
        }



        // Wybór poziomu trudności
        private static async Task SelectDifficultyLevel(string language, int rounds, List<string> playerNames, bool isMultiplayer)
        {
            while (true)
            {
                if (needRefresh)
                {
                    ConsoleHelper.ClearSection(0, 30);
                    needRefresh = false;
                }

                ConsoleHelper.ClearSection(0, 30);
                ConsoleHelper.DisplayPressEscInfo(CurrentLanguage);
                Console.WriteLine("\n\n");
                ConsoleHelper.DisplayColoredTitle(LanguageResources.GetResource("SelectDifficulty", language));
                Console.WriteLine("\n\n");
                string[] options = {
                    LanguageResources.GetResource("Easy", language),
                    LanguageResources.GetResource("Medium", language),
                    LanguageResources.GetResource("Hard", language)
                };
                int choice = ConsoleHelper.SelectOption(options);

                if (choice == -1)
                {
                    // Cofnięcie się do ustawień gry
                    ViewStack.Pop();
                    await ViewStack.Peek()();
                    return;
                }

                DifficultyLevel difficulty = choice switch
                {
                    0 => DifficultyLevel.Easy,
                    1 => DifficultyLevel.Medium,
                    2 => DifficultyLevel.Hard,
                    _ => DifficultyLevel.Medium
                };

                GameSettings settings = new GameSettings
                {
                    Rounds = rounds,
                    PlayerNames = playerNames,
                    PlayerCount = playerNames.Count,
                    Difficulty = difficulty
                };

                try
                {
                    GameModel model = await GameModel.CreateAsync(language, settings.PlayerCount, settings.Difficulty);
                    model.SetRounds(settings.Rounds);
                    model.SetPlayerNames(settings.PlayerNames);
                    model.IsMultiplayer = isMultiplayer;
                    IGameView view = new GameView(language);
                    GameController gameController = new GameController(view, model, isMultiplayer, settings.Rounds);
                    bool restartRequested = await gameController.RunGameAsync();

                    if (restartRequested)
                    {
                        // Użytkownik chce ponownie uruchomić ustawienia gry
                        ViewStack.Clear(); // Wyczyść stos widoków
                        ViewStack.Push(MainMenu);
                        ViewStack.Push(SelectGameMode);
                        await ViewStack.Peek()();
                        return;
                    }
                    else
                    {
                        // Powrót do menu głównego
                        ViewStack.Clear();
                        ViewStack.Push(MainMenu);
                        await ViewStack.Peek()();
                        return;
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.HandleException(ex, language);
                    ViewStack.Clear();
                    ViewStack.Push(MainMenu);
                    await ViewStack.Peek()();
                    return;
                }
            }
        }
    }
}
