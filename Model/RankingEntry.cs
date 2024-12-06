namespace WisielecConsole.Model
{
    // Klasa reprezentująca wpis w rankingu
    public class RankingEntry
    {
        public string? PlayerName { get; set; }
        public int Score { get; set; }
        public Guid GameId { get; set; }
    }
}
