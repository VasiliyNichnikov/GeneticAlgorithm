using System;
using System.Collections.Generic;
using Planets.MiningPlayer;

namespace Players
{
    public class PlayerBrains : IDisposable
    {
        private readonly Dictionary<PlayerType, PlayerBrain> _brains;

        public PlayerBrains(IMiningPlanet[] planets)
        {
            _brains = new Dictionary<PlayerType, PlayerBrain>
            {
                { PlayerType.Player1, new PlayerBrain(PlayerType.Player1, planets) },
                { PlayerType.Player2, new PlayerBrain(PlayerType.Player2, planets) }
            };
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