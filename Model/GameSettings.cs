namespace WisielecConsole.Model
{
    //klasa przechowująca ustawienia gry
    public class GameSettings
    {
        public int Rounds { get; set; }
        public List<string>? PlayerNames { get; set; }
        public int PlayerCount { get; set; }
        public DifficultyLevel Difficulty { get; set; }
    }
}
