namespace WisielecConsole.Resources
{
    public static class LanguageResources
    {
        // Zasoby języka polskiego
        public static Dictionary<string, string> Polish = new Dictionary<string, string>
        {
            {"Title", "WISIELEC"},
            {"Polish", "Polski"},
            {"English", "Angielski"},
            {"SelectLanguage", "Wybierz język"},
            {"StartGame", "Zacznij grę"},
            {"ShowRanking", "Wyświetl ranking"},
            {"ClearRanking", "Wyczyść ranking"},
            {"ExitGame", "Zakończ grę"},
            {"ChangeLanguage", "Zmień język"},
            {"ThankYou", "Dziękujemy za grę! Do zobaczenia!"},
            {"EnterLetter", "Podaj literę"},
            {"Congratulations", "Gratulacje, zgadłeś poprawnie!"},
            {"Sorry", "Przykro mi, nie zgadłeś słowa."},
            {"SaveScore", "Podaj swoje imię, aby zapisać wynik:"},
            {"Word", "Słowo"},
            {"AttemptsLeft", "Pozostało prób"},
            {"ScoreSaved", "Wynik zapisany!"},
            {"NoResults", "Brak wyników."},
            {"ClearRankingConfirm", "Czy na pewno chcesz wyczyścić ranking?"},
            {"Yes", "Tak"},
            {"No", "Nie"},
            {"SinglePlayer", "Gra jednoosobowa"},
            {"Multiplayer", "Gra wieloosobowa"},
            {"EnterNumberOfRounds", "Podaj liczbę rund"},
            {"MaxRoundsInfo", "Maksymalna liczba rund to 10."},
            {"InvalidRoundsNumber", "Nieprawidłowa liczba rund. Podaj liczbę od 1 do 10."},
            {"InvalidInput", "Nieprawidłowe dane. Wprowadź literę lub naciśnij Esc."},
            {"PressToContinue", "Naciśnij dowolny klawisz, aby kontynuować..."},
            {"EnterPlayerName", "Graczu {Player}, podaj swoje imię"},
            {"EnterPlayerNameSingle", "Podaj swoje imię"},
            {"Points", "punktów"},
            {"YourTurn", "Twoja kolej"},
            {"ReturnToMenu", "Powrót do menu"},
            {"EnterNumberOfRoundsAndNames", "Podaj liczbę rund i imiona graczy"},
            {"RankingCleared", "Ranking został wyczyszczony."},
            {"RestartGame", "Zacznij od nowa"},
            {"NameLabel", "Imię"},
            {"WordWas", "Zgadywane słowo to"},
            {"YourScore", "Twój wynik"},
            {"ViewRanking", "Zobacz ranking"},
            {"Ranking", "Ranking"},
            {"CongratulationsWord", "Gratulacje, zgadłeś hasło: {Word}"},
            {"SorryWord", "Przykro mi, poprawnym hasłem było: {Word}"},
            {"Round", "Runda"},
            {"SelectGameMode", "Wybierz tryb gry"},
            {"GameSettings", "Ustawienia gry"},
            {"SelectDifficulty", "Wybierz poziom trudności"},
            {"Easy", "Łatwy"},
            {"Medium", "Średni"},
            {"Hard", "Trudny"},
            {"SaveGamePrompt", "Czy chcesz zapisać grę?"},
            {"GameSaved", "Gra została zapisana."},
            {"LoadingGame", "Wczytywanie gry..."},
            {"NoSavedGame", "Brak zapisanego stanu gry."},
            {"PressEscToExit", "Naciśnij Esc, aby zakończyć grę"},
            {"PressEscToGoBack", "Naciśnij Esc, aby wrócić"},
            {"TimeLeft", "Pozostały czas"},
            {"ScoreLabel", "Punkty"},
            {"SelectSavedGame", "Wybierz zapis gry:"},
            {"FailedToLoadGame", "Nie udało się wczytać gry."},
            {"AnErrorOccurred", "Wystąpił błąd"},
            {"InvalidNumberInput", "Nieprawidłowa liczba. Spróbuj ponownie."},
            {"InvalidStringInput", "Nieprawidłowy ciąg znaków. Spróbuj ponownie."},
            {"SoundFileNotFound", "Plik dźwiękowy nie został znaleziony: {FileName}"},
            {"SoundPlayError", "Błąd podczas odtwarzania dźwięku '{FileName}': {Error}"},
            {"ErrorLoadingRanking", "Błąd podczas wczytywania rankingu: {Error}"},
            {"ErrorClearingRanking", "Błąd podczas czyszczenia rankingu: {Error}"},
            {"ErrorFetchingRandomWord", "Błąd podczas pobierania losowego słowa"},
            {"WordListNotFound", "Plik z listą słów nie został znaleziony: {FileName}"},
            {"NoWordsAvailable", "Brak słów dostępnych dla wybranego poziomu trudności."},
            {"WindowTooSmall", "Okno konsoli jest za małe. Zwiększ rozmiar okna."},
            {"GameOver", "Koniec gry!"},
            {"InvalidOption", "Nieprawidłowa opcja. Spróbuj ponownie."},
            {"NewGame", "Nowa gra"},
            {"LoadingError", "Błąd podczas wczytywania danych: {Error}"},
            {"BackgroundMusicError", "Błąd podczas odtwarzania muzyki w tle: {Error}"},
            {"ConfirmExitGame", "Czy na pewno chcesz opuścić grę?"},
            {"Player", "Gracz"},
            { "NoWordsAvailableFromApi", "Brak dostępnych słów z API." },
            { "ApiRequestError", "Błąd żądania API: {Error}" },
            { "ApiResponseError", "Błąd odpowiedzi API: {Error}" },

        };

        // Zasoby języka angielskiego
        public static Dictionary<string, string> English = new Dictionary<string, string>
        {
            {"Title", "HANGMAN"},
            {"Polish", "Polish"},
            {"English", "English"},
            {"SelectLanguage", "Select language"},
            {"StartGame", "Start game"},
            {"ShowRanking", "Show ranking"},
            {"ClearRanking", "Clear ranking"},
            {"ExitGame", "Exit game"},
            {"ChangeLanguage", "Change language"},
            {"ThankYou", "Thank you for playing! See you next time!"},
            {"EnterLetter", "Enter a letter"},
            {"Congratulations", "Congratulations, you guessed correctly!"},
            {"Sorry", "Sorry, you didn't guess the word."},
            {"SaveScore", "Enter your name to save the score:"},
            {"Word", "Word"},
            {"AttemptsLeft", "Attempts left"},
            {"ScoreSaved", "Score saved!"},
            {"NoResults", "No results."},
            {"ClearRankingConfirm", "Are you sure you want to clear the ranking?"},
            {"Yes", "Yes"},
            {"No", "No"},
            {"SinglePlayer", "Single Player"},
            {"Multiplayer", "Multiplayer"},
            {"EnterNumberOfRounds", "Enter number of rounds"},
            {"MaxRoundsInfo", "The maximum number of rounds is 10."},
            {"InvalidRoundsNumber", "Invalid number of rounds. Enter a number between 1 and 10."},
            {"InvalidInput", "Invalid input. Please enter a letter or press Esc."},
            {"PressToContinue", "Press any key to continue..."},
            {"EnterPlayerName", "Player {Player}, enter your name"},
            {"EnterPlayerNameSingle", "Enter your name"},
            {"Points", "points"},
            {"YourTurn", "Your turn"},
            {"ReturnToMenu", "Return to menu"},
            {"EnterNumberOfRoundsAndNames", "Enter number of rounds and player names"},
            {"RankingCleared", "The ranking has been cleared."},
            {"RestartGame", "Restart game"},
            {"NameLabel", "Name"},
            {"WordWas", "The word was"},
            {"YourScore", "Your score"},
            {"ViewRanking", "View ranking"},
            {"Ranking", "Ranking"},
            {"CongratulationsWord", "Congratulations, you guessed the word: {Word}"},
            {"SorryWord", "Sorry, the correct word was: {Word}"},
            {"Round", "Round"},
            {"SelectGameMode", "Select game mode"},
            {"GameSettings", "Game Settings"},
            {"SelectDifficulty", "Select difficulty level"},
            {"Easy", "Easy"},
            {"Medium", "Medium"},
            {"Hard", "Hard"},
            {"LoadingGame", "Loading game..."},
            {"PressEscToExit", "Press Esc to exit the game"},
            {"PressEscToGoBack", "Press Esc to go back"},
            {"TimeLeft", "Time left"},
            {"ScoreLabel", "Score"},
            {"FailedToLoadGame", "Failed to load the game."},
            {"AnErrorOccurred", "An error occurred"},
            {"InvalidNumberInput", "Invalid number. Please try again."},
            {"InvalidStringInput", "Invalid input. Please try again."},
            {"SoundFileNotFound", "Sound file not found: {FileName}"},
            {"SoundPlayError", "Error playing sound '{FileName}': {Error}"},
            {"ErrorLoadingRanking", "Error loading ranking: {Error}"},
            {"ErrorClearingRanking", "Error clearing ranking: {Error}"},
            {"ErrorFetchingRandomWord", "Error fetching random word"},
            {"WordListNotFound", "Word list file not found: {FileName}"},
            {"NoWordsAvailable", "No words available for the selected difficulty level."},
            {"WindowTooSmall", "Console window is too small. Please enlarge the window."},
            {"GameOver", "Game Over!"},
            {"InvalidOption", "Invalid option. Please try again."},
            {"NewGame", "New Game"},
            {"LoadingError", "Error loading data: {Error}"},
            {"BackgroundMusicError", "Error playing background music: {Error}"},
            {"ConfirmExitGame", "Are you sure you want to exit the game?"},
            {"Player", "Player"},
            { "NoWordsAvailableFromApi", "No words available from API." },
            { "ApiRequestError", "API request error: {Error}" },
            { "ApiResponseError", "API response error: {Error}" },


        };

        // Metoda pobierająca zasób tekstowy na podstawie klucza i języka
        public static string GetResource(string key, string language)
        {
            string lang = string.IsNullOrEmpty(language) ? CurrentLanguage : language;

            Dictionary<string, string> resources = lang switch
            {
                "pl" => Polish,
                "en" => English,
                _ => English
            };

            if (resources.TryGetValue(key, out string value))
            {
                return value;
            }
            else
            {
                Console.WriteLine($"Brak zasobu dla klucza: {key} w języku: {lang}");
                return $"[Brak tłumaczenia: {key}]";
            }
        }
        public static string CurrentLanguage { get; private set; } = "en";

        // Metoda ustawiająca aktualny język
        public static void SetCurrentLanguage(string language)
        {
            if (language.Equals("pl", StringComparison.OrdinalIgnoreCase) || language.Equals("en", StringComparison.OrdinalIgnoreCase))
            {
                CurrentLanguage = language.ToLower();
            }
            else
            {
                CurrentLanguage = "en"; // domyślny język
            }
        }


    }
}
