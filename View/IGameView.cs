using WisielecConsole.Model;

namespace WisielecConsole.View
{
    public interface IGameView
    {
        void DisplayStartScreen(string language);
        void DisplayGameWithPlayerName(GameModel model, string playerName, string language, int currentRound, int totalRounds);
        void DisplayWordReveal(GameModel model, int playerId);
        void DisplayHangmanGraphic(int attemptsLeft, bool isGameOver, int startLine);
        Task<(char GuessedChar, bool IsEscPressed, bool IsTimedOut)> GetPlayerGuessAsync(GameModel model, CancellationToken cancellationToken);
        Task PlayCorrectGuessSound();
        Task PlayIncorrectGuessSound();
        Task PlayApplauseSound();
        void StopApplauseSound();
        void DisplayMessage(string message, string currentLanguage);
        void RefreshGameView(GameModel model, string playerName, string language, int currentRound, int totalRounds);
        int DisplayEndGameScreen(GameModel model, Guid gameId);
        Task DisplayRoundStartAsync(int currentRound, int totalRounds, string language);
        void DisplayRoundStartScreen(int currentRound, int totalRounds, string language);
        bool PromptToExitGame(string language);

    }
}
