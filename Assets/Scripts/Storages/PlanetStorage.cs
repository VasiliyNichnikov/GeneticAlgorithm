using System.Collections.Generic;
using System.Linq;
using Planets.MiningPlanet;
using Planets.PlayerPlanet;
using Players;
using UnityEngine;

namespace Storages
{
    public class PlanetStorage
    {
        private readonly MiningPlanet[] _miningPlanets;
        private readonly PlayerPlanet[] _playerPlanets;

        public PlanetStorage()
        {
            _miningPlanets = Object.FindObjectsOfType<MiningPlanet>();
            _playerPlanets = Object.FindObjectsOfType<PlayerPlanet>();
        }


        public IReadOnlyCollection<IMiningPlanet> GetMiningPlanets()
        {
            return _miningPlanets;
        }

        public IPlayerPlanet GetPlayerPlanet(PlayerType playerType)
        {
            return _playerPlanets.First(p => p.PlayerType == playerType);
        }
    }
}