using Players;
using ShipLogic;

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

        public static string GetShipName(ShipType ship)
        {
            return ship switch
            {
                ShipType.Stealth => "Разведчик",
                ShipType.Fighter => "Истребитель",
                ShipType.AircraftCarrier => "Авианосец",
                ShipType.Mining => "Бур",
                _ => string.Empty
            };
        }
    }
}