using System;
using System.Collections.Generic;
using Planets.MiningPlanet;

namespace Players
{
    public class PlayerBrains : IDisposable
    {
        private readonly Dictionary<PlayerType, PlayerBrain> _brains;

        public PlayerBrains(IMiningPlanet[] planets)
        {
            var planetForPlanet1 = Main.Instance.PlanetStorage.GetPlayerPlanet(PlayerType.Player1);
            var planetForPlanet2 = Main.Instance.PlanetStorage.GetPlayerPlanet(PlayerType.Player2);

            _brains = new Dictionary<PlayerType, PlayerBrain>
            {
                { PlayerType.Player1, new PlayerBrain(planetForPlanet1, planets) },
                { PlayerType.Player2, new PlayerBrain(planetForPlanet2, planets) }
            };
        }

        public void Start()
        {
            foreach (var kvp in _brains)
            {
                kvp.Value.InitPlanet();
                kvp.Value.CreateFirstShips();
            }
        }

        public void Dispose()
        {
            foreach (var kvp in _brains)
            {
                _brains[kvp.Key].Dispose();
            }

            _brains.Clear();
        }
    }
}