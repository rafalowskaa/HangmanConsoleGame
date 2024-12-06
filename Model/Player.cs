namespace WisielecConsole.Model
{

    public class Player
    {

        public string PlayerName { get; set; }


        public Guid GameId { get; set; }


        public Player(string playerName)
        {
            PlayerName = playerName;
            GameId = Guid.NewGuid(); // Generuje unikalny GameId dla każdego gracza
        }
    }
}
