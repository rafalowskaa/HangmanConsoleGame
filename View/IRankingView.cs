using WisielecConsole.Model;

namespace WisielecConsole.View
{
    public interface IRankingView
    {
        void DisplayRanking(string language);
        void DisplayRankingWithHighlight(GameModel model, Guid gameId, int startLine);
    }
}
