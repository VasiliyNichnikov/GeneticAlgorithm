using Players;

namespace Utils
{
    public static class PlayerUtils
    {
        public static string GetPlayerName(PlayerType player)
        {
            return player switch
            {
                PlayerType.None => string.Empty,
                PlayerType.Player1 => "Player One",
                PlayerType.Player2 => "Player Two",
                _ => string.Empty
            };
        }
    }
}