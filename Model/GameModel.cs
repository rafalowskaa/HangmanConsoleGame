using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WisielecConsole.Resources;
using WisielecConsole.Utilities;

namespace WisielecConsole.Model
{
    // Enum reprezentujący poziomy trudności
    public enum DifficultyLevel
    {
        None,
        Easy,   // Łatwy
        Medium, // Średni
        Hard    // Trudny
    }

    // Klasa reprezentująca model gry
    public class GameModel
    {
        public static int MaxRankingEntries { get; } = 10;
        public List<string> WordToGuess { get; private set; }
        public List<HashSet<char>> GuessedLetters { get; private set; }
        private static readonly HttpClient httpClient = new HttpClient();

        public List<int> AttemptsLeft { get; private set; }
        public List<int> Scores { get; private set; }
        public List<string> PlayerNames { get; private set; }
        public int TotalRounds { get; private set; }
        public int CurrentRound { get; set; }
        public int CurrentPlayer { get; set; }
        public DifficultyLevel Difficulty { get; private set; }
        public bool IsMultiplayer { get; set; }
        public int MinimumScore { get; private set; } = 0;
        public string Language { get; set; }
        public DateTime RoundEndTime { get; set; }


        // Konstruktor publiczny
        public GameModel()
        {
            WordToGuess = new List<string>();
            GuessedLetters = new List<HashSet<char>>();
            AttemptsLeft = new List<int>();
            Scores = new List<int>();
            PlayerNames = new List<string>();
        }

        // Tworzy nowy model gry
        public static async Task<GameModel> CreateAsync(string language, int playerCount, DifficultyLevel difficulty)
        {
            GameModel model = new GameModel
            {
                Language = language,
                Difficulty = difficulty
            };

            for (int i = 0; i < playerCount; i++)
            {
                string word = await model.GetRandomWordAsync(difficulty);
                model.WordToGuess.Add(word.ToUpper());
                model.GuessedLetters.Add(new HashSet<char>());
                model.AttemptsLeft.Add(10);
                model.Scores.Add(0);
            }

            return model;
        }

        // Resetuje model gry dla nowej rundy
        public async Task ResetForNewRoundAsync()
        {
            for (int i = 0; i < PlayerNames.Count; i++)
            {
                string word = await GetRandomWordAsync(Difficulty);
                WordToGuess[i] = word.ToUpper();
                GuessedLetters[i] = new HashSet<char>();
                AttemptsLeft[i] = 10;
            }
        }

        // Wykonuje ruch gracza
        public bool MakeGuess(char letter, int playerId)
        {
            letter = char.ToUpper(letter);

            if (GuessedLetters[playerId].Contains(letter))
            {
                return false;
            }

            GuessedLetters[playerId].Add(letter);

            if (WordToGuess[playerId].Contains(letter))
            {
                // Mnożnik punktowy w zależności od trudności
                int scoreMultiplier = Difficulty switch
                {
                    DifficultyLevel.Easy => 73,
                    DifficultyLevel.Medium => 144,
                    DifficultyLevel.Hard => 195,
                    _ => 100
                };
                Scores[playerId] += scoreMultiplier;
                return true;
            }
            else
            {
                // Kary zależne od trudności
                int penalty = Difficulty switch
                {
                    DifficultyLevel.Easy => 21,
                    DifficultyLevel.Medium => 54,
                    DifficultyLevel.Hard => 107,
                    _ => 20
                };
                Scores[playerId] = Math.Max(0, Scores[playerId] - penalty);
                AttemptsLeft[playerId]--;
                return false;
            }
        }

        // Sprawdza, czy gracz wygrał
        public bool IsGameWon(int playerId)
        {
            foreach (char c in WordToGuess[playerId])
            {
                if (!char.IsWhiteSpace(c) && !GuessedLetters[playerId].Contains(c))
                {
                    return false;
                }
            }
            return true;
        }

        // Sprawdza, czy gracz przegrał
        public bool IsGameOver(int playerId)
        {
            return AttemptsLeft[playerId] <= 0;
        }

        // Ustawia liczbę rund
        public void SetRounds(int rounds)
        {
            TotalRounds = rounds;
        }

        // Ustawia imiona graczy
        public void SetPlayerNames(List<string> names)
        {
            PlayerNames = names;
        }

        // Zapisuje wynik gracza
        public void SaveScore(string playerName, int score, Guid gameId)
        {
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "scores.txt");
            string entry = $"{playerName}:{score}:{gameId}";
            File.AppendAllLines(filePath, new[] { entry });
        }

        // Wczytuje ranking z pliku
        public static List<RankingEntry> LoadRanking(string language)
        {
            var ranking = new List<RankingEntry>();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "scores.txt");

            try
            {
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        var parts = line.Split(':');
                        if (parts.Length == 3 && int.TryParse(parts[1], out int score) && Guid.TryParse(parts[2], out Guid gameId))
                        {
                            ranking.Add(new RankingEntry
                            {
                                PlayerName = parts[0],
                                Score = score,
                                GameId = gameId
                            });
                        }
                    }
                }

                ranking = ranking.OrderByDescending(entry => entry.Score).Take(MaxRankingEntries).ToList();
            }
            catch (Exception ex)
            {
                string errorMessage = LanguageResources.GetResource("ErrorLoadingRanking", language)
                    .Replace("{Error}", ex.Message);
                ExceptionHandler.LogError(errorMessage);
            }

            return ranking;
        }

        // Czyści ranking
        public static void ClearRanking(string language)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "scores.txt");
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = LanguageResources.GetResource("ErrorClearingRanking", language)
                    .Replace("{Error}", ex.Message);
                ExceptionHandler.LogError(errorMessage);
            }
        }

        private async Task<string> GetWordFromApi()
        {
            var response = await httpClient.GetStringAsync("https://random-word-api.herokuapp.com/word?number=1");
            var words = JsonConvert.DeserializeObject<List<string>>(response);
            return words.FirstOrDefault() ?? throw new Exception("Failed to retrieve a word.");
        }

        // Pobiera losowe słowo na podstawie długości z jednego pliku
        private async Task<string> GetRandomWordAsync(DifficultyLevel difficulty)
        {
            try
            {
                string word = string.Empty;

                if (Language.Equals("en", StringComparison.OrdinalIgnoreCase))
                {
                    // Pobieranie słowa z API dla języka angielskiego
                    word = await GetWordFromApi();
                }
                else
                {
                    // Pobieranie słowa z lokalnego pliku
                    string wordListFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", $"words_{Language}.txt");

                    if (!File.Exists(wordListFile))
                    {
                        string message = LanguageResources.GetResource("WordListNotFound", Language).Replace("{FileName}", wordListFile);
                        throw new FileNotFoundException(message);
                    }

                    var words = await File.ReadAllLinesAsync(wordListFile);

                    // Klasyfikacja słów na podstawie długości dla poziomów trudności
                    IEnumerable<string> filteredWords = difficulty switch
                    {
                        DifficultyLevel.Easy => words.Where(w => w.Length <= 5),
                        DifficultyLevel.Medium => words.Where(w => w.Length > 5 && w.Length <= 9),
                        DifficultyLevel.Hard => words.Where(w => w.Length > 9),
                        _ => words
                    };

                    if (!filteredWords.Any())
                    {
                        string message = LanguageResources.GetResource("NoWordsAvailable", Language);
                        throw new InvalidOperationException(message);
                    }

                    Random random = new Random();
                    word = filteredWords.ElementAt(random.Next(filteredWords.Count()));
                }

                return word.ToUpper();
            }
            catch (Exception ex)
            {
                string errorMessage = LanguageResources.GetResource("ErrorFetchingRandomWord", Language)
                    .Replace("{Error}", ex.Message);
                ExceptionHandler.LogError($"{errorMessage}: {ex.Message}");
                throw;
            }
        }

    }
}
