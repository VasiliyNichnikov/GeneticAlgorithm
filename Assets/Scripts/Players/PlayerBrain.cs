using System;
using Planets.MiningPlayer;
using Planets.PlayerPlanet;
using UnityEngine;

namespace Players
{
    public class PlayerBrain : MonoBehaviour, IPlayerBrain
    {
        [SerializeField] private PlayerType _player;
        [SerializeField] private PlayerPlanet _planet;
        // todo в будущем будут две планеты
        [SerializeField] private MiningPlanet _miningPlanets;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _miningPlanets.OnPlayerCollectedGold += UpdateGoldValue;
            _planet.Init(_player, this);
        }
        
        public Vector3 GetPointForMovement()
        {
            return _miningPlanets.GetPointToApproximate();
        }

        private void OnDestroy()
        {
            _miningPlanets.OnPlayerCollectedGold -= UpdateGoldValue;
        }

        private void UpdateGoldValue(PlayerType player, float addedGold)
        {
            if (player != _player)
            {
                return;
            }
            
            _planet.AddExtractedGold(addedGold);
        }
    }
}