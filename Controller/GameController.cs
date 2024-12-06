using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WisielecConsole.Model;
using WisielecConsole.View;

namespace WisielecConsole.Controller
{
    
    public class GameController
    {
        private readonly IGameView _view;
        private readonly GameModel _model;
        private int _totalRounds;
        private int _currentRound;
        private bool _isMultiplayer;
        private Guid _gameId;
        private bool _isGameExited = false;

        public GameModel Model => _model;

        // Konstruktor kontrolera gry
        public GameController(IGameView view, GameModel model, bool isMultiplayer, int totalRounds)
        {
            _view = view;
            _model = model;
            _isMultiplayer = isMultiplayer;
            _totalRounds = totalRounds > 0 ? totalRounds : model.TotalRounds;
            _gameId = Guid.NewGuid();

            _currentRound = 1;
        }

        // Główna pętla gry
        public async Task<bool> RunGameAsync()
        {
            do
            {
                await PlayRoundAsync();
                if (_isGameExited)
                    break;
                _currentRound++;
                _model.CurrentRound = _currentRound;
            } while (_currentRound <= _totalRounds);

            return await DisplayGameSummaryAsync();
        }

        // Rozgrywanie pojedynczej rundy
        private async Task PlayRoundAsync()
        {
            if (_currentRound > 1)
            {
                await _model.ResetForNewRoundAsync();
            }

            if (_isMultiplayer)
            {
                // Tryb wieloosobowy
                for (int playerIndex = 0; playerIndex < _model.PlayerNames.Count; playerIndex++)
                {
                    _model.CurrentPlayer = playerIndex;

                    bool roundOver = false;

                    // Ustawienie czasu końca rundy dla danego gracza
                    int totalTimeLimit = GetTotalTimeLimit(_model.Difficulty); // Metoda, która zwraca całkowity limit czasu dla rundy
                    _model.RoundEndTime = DateTime.Now.AddSeconds(totalTimeLimit);

                    // Odświeżenie widoku gry przed rozpoczęciem tury gracza
                    _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                    // Wyświetlenie komunikatu rozpoczęcia rundy
                    await _view.DisplayRoundStartAsync(_currentRound, _totalRounds, _model.Language);

                    while (!roundOver)
                    {
                        using (var cts = new CancellationTokenSource())
                        {
                            // Przekazanie tokenu anulowania
                            var guessResultTask = _view.GetPlayerGuessAsync(_model, cts.Token);
                            var timerTask = Task.Delay(TimeSpan.FromSeconds(totalTimeLimit), cts.Token);

                            // Czekanie na którykolwiek z tasków: wprowadzenie litery lub upłynięcie czasu
                            var completedTask = await Task.WhenAny(guessResultTask, timerTask);

                            if (completedTask == guessResultTask)
                            {
                                // Wprowadzenie litery
                                var guessResult = await guessResultTask;

                                if (guessResult.IsEscPressed)
                                {
                                    // Zapytaj użytkownika, czy chce opuścić grę
                                    bool exitConfirmed = _view.PromptToExitGame(_model.Language);
                                    if (exitConfirmed)
                                    {
                                        _isGameExited = true;
                                        return;
                                    }
                                    else
                                    {
                                        // Odśwież widok gry po anulowaniu wyjścia
                                        _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);
                                        continue;
                                    }
                                }

                                if (guessResult.IsTimedOut)
                                {
                                    // Limit czasu przekroczony - zakończ rundę i grę
                                    roundOver = true;
                                    _model.AttemptsLeft[_model.CurrentPlayer]--;
                                    await _view.PlayIncorrectGuessSound();

                                    // Odśwież widok po zmniejszeniu liczby prób
                                    _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                                    if (_model.IsGameOver(_model.CurrentPlayer))
                                    {
                                        _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                                    }

                                    return;
                                }

                                char guess = guessResult.GuessedChar;
                                bool correctGuess = _model.MakeGuess(guess, _model.CurrentPlayer);

                                if (correctGuess)
                                {
                                    await _view.PlayCorrectGuessSound();
                                }
                                else
                                {
                                    await _view.PlayIncorrectGuessSound();
                                }

                                // Odświeżenie widoku gry po wprowadzeniu litery
                                _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                                if (_model.IsGameWon(_model.CurrentPlayer))
                                {
                                    // Bbonus za długość słowa
                                    int wordLengthBonus = _model.WordToGuess[_model.CurrentPlayer].Length * 100;
                                    _model.Scores[_model.CurrentPlayer] += wordLengthBonus;

                                    roundOver = true;
                                    _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                                }
                                else if (_model.IsGameOver(_model.CurrentPlayer))
                                {
                                    roundOver = true;
                                    _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                                }
                            }
                            else
                            {
                                // Upłynięcie czasu - zakończ rundę i grę
                                roundOver = true;
                                _model.AttemptsLeft[_model.CurrentPlayer]--;
                                await _view.PlayIncorrectGuessSound();

                                // Odśwież widok po zmniejszeniu liczby prób
                                _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                                if (_model.IsGameOver(_model.CurrentPlayer))
                                {
                                    _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                                }

                                // Zakończenie całej gry po wygaśnięciu czasu
                                _isGameExited = true;
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                // Tryb jednoosobowy
                _model.CurrentPlayer = 0;

                bool roundOver = false;

                // Ustawienie czasu końca rundy
                int totalTimeLimit = GetTotalTimeLimit(_model.Difficulty); // Metoda, która zwraca całkowity limit czasu dla rundy
                _model.RoundEndTime = DateTime.Now.AddSeconds(totalTimeLimit);

                // Odświeżenie widoku gry przed rozpoczęciem rundy
                _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                // Wyświetlenie komunikatu rozpoczęcia rundy
                await _view.DisplayRoundStartAsync(_currentRound, _totalRounds, _model.Language);

                while (!roundOver)
                {
                    using (var cts = new CancellationTokenSource())
                    {
                        // Przekazanie tokenu anulowania
                        var guessResultTask = _view.GetPlayerGuessAsync(_model, cts.Token);
                        var timerTask = Task.Delay(TimeSpan.FromSeconds(totalTimeLimit), cts.Token);

                        // Czekanie na którykolwiek z tasków: wprowadzenie litery lub upłynięcie czasu
                        var completedTask = await Task.WhenAny(guessResultTask, timerTask);

                        if (completedTask == guessResultTask)
                        {
                            // Wprowadzenie litery
                            var guessResult = await guessResultTask;

                            if (guessResult.IsEscPressed)
                            {
                                // Zapytaj użytkownika, czy chce opuścić grę
                                bool exitConfirmed = _view.PromptToExitGame(_model.Language);
                                if (exitConfirmed)
                                {
                                    _isGameExited = true;
                                    return;
                                }
                                else
                                {
                                    // Odśwież widok gry po anulowaniu wyjścia
                                    _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);
                                    continue;
                                }
                            }

                            if (guessResult.IsTimedOut)
                            {
                                // Limit czasu przekroczony - zakończ rundę i grę
                                roundOver = true;
                                _model.AttemptsLeft[_model.CurrentPlayer]--;
                                await _view.PlayIncorrectGuessSound();

                                // Odśwież widok po zmniejszeniu liczby prób
                                _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                                if (_model.IsGameOver(_model.CurrentPlayer))
                                {
                                    _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                                }

                                // Zakończenie całej gry po wygaśnięciu czasu
                                _isGameExited = true;
                                return;
                            }

                            char guess = guessResult.GuessedChar;
                            bool correctGuess = _model.MakeGuess(guess, _model.CurrentPlayer);

                            if (correctGuess)
                            {
                                await _view.PlayCorrectGuessSound();
                            }
                            else
                            {
                                await _view.PlayIncorrectGuessSound();
                            }

                            // Odświeżenie widoku gry po wprowadzeniu litery
                            _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                            if (_model.IsGameWon(_model.CurrentPlayer))
                            {
                                // Dodaj bonus za długość słowa
                                int wordLengthBonus = _model.WordToGuess[_model.CurrentPlayer].Length * 100;
                                _model.Scores[_model.CurrentPlayer] += wordLengthBonus;

                                roundOver = true;
                                _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                            }
                            else if (_model.IsGameOver(_model.CurrentPlayer))
                            {
                                roundOver = true;
                                _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                            }
                        }
                        else
                        {
                            // Upłynięcie czasu - zakończ rundę i grę
                            roundOver = true;
                            _model.AttemptsLeft[_model.CurrentPlayer]--;
                            await _view.PlayIncorrectGuessSound();

                            // Odśwież widok po zmniejszeniu liczby prób
                            _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);

                            if (_model.IsGameOver(_model.CurrentPlayer))
                            {
                                _view.DisplayWordReveal(_model, _model.CurrentPlayer);
                            }

                            // Zakończenie całej gry po wygaśnięciu czasu
                            _isGameExited = true;
                            return;
                        }
                    }
                }
            }
            
        }

        // Wyświetla podsumowanie gry i pyta o dalsze działania
        private async Task<bool> DisplayGameSummaryAsync()
        {
            for (int i = 0; i < _model.PlayerNames.Count; i++)
            {
                int totalScore = _model.Scores[i];
                int averageScore = _totalRounds > 0 ? totalScore / _totalRounds : _model.MinimumScore;
                _model.Scores[i] = averageScore;
                _model.SaveScore(_model.PlayerNames[i], averageScore, _gameId);
            }

            // Odtworzenie dźwięku aplauzu
            await _view.PlayApplauseSound();

            int choice = _view.DisplayEndGameScreen(_model, _gameId);

            // Zatrzymanie dźwięku aplauzu po wyborze
            _view.StopApplauseSound();

            if (choice == 0)
            {
                // Użytkownik chce ponownie uruchomić grę
                return true;
            }
            else if (choice == 1)
            {
                // Powrót do menu głównego
                _view.RefreshGameView(_model, _model.PlayerNames[_model.CurrentPlayer], _model.Language, _currentRound, _totalRounds);
                return false;
            }

            return false;
        }

        // Pobiera całkowity limit czasu dla rundy na podstawie poziomu trudności
        private int GetTotalTimeLimit(DifficultyLevel difficulty)
        {
            return difficulty switch
            {
                DifficultyLevel.Easy => 90,  
                DifficultyLevel.Medium => 70, 
                DifficultyLevel.Hard => 40,    
                _ => 70
            };
        }
    }
}



