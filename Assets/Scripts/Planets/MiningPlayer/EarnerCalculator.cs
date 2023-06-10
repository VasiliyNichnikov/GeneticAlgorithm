using System;
using System.Collections.Generic;
using Loaders;
using Players;
using UnityEngine;

namespace Planets.MiningPlayer
{
    /// <summary>
    /// Рассчитывает сколько золота должен получить игрок
    /// </summary>
    public class EarnerCalculator
    {
        /// <summary>
        /// Данные о добычи
        /// </summary>
        private class MiningLogic
        {
            public float TimeLeft => _timeLeft;

            private float _timeLeft;
            private float _timeLeftWithoutPercentage;

            private readonly float _minimumTimeLeft;
            private readonly float _maximumTimeLeft;
            private readonly float _percentageForNumberShipsInZone;

            private float _percentageMining;
            private int _numberShipsInZone;
            private readonly Action<float> _onGoldCollected;

            public MiningLogic(float minimumTimeLeft, float maximumTimeLeft, float percentageForNumberShipsInZone,
                int startingNumberShipsInZone, Action<float> onGoldCollected)
            {
                if (startingNumberShipsInZone <= 0)
                {
                    Debug.LogError($"Number ships must be more zero (Current: {startingNumberShipsInZone})");
                    _numberShipsInZone = 1;
                }
                else
                {
                    _numberShipsInZone = startingNumberShipsInZone;
                }

                _minimumTimeLeft = minimumTimeLeft;
                _maximumTimeLeft = maximumTimeLeft;
                _percentageForNumberShipsInZone = percentageForNumberShipsInZone;
                _timeLeft = _maximumTimeLeft;
                _timeLeftWithoutPercentage = _maximumTimeLeft;
                _onGoldCollected = onGoldCollected;
                _percentageMining = RecalculatePercentage();

                Main.Instance.OnUpdateGame += CustomUpdate;
            }

            public void AddShip()
            {
                _numberShipsInZone += 1;
                _percentageMining = RecalculatePercentage();
            }

            public bool CanRemoveShip()
            {
                return _numberShipsInZone - 1 > 0;
            }

            private void CustomUpdate()
            {
                _timeLeftWithoutPercentage -= Time.deltaTime;
                _timeLeft = _timeLeftWithoutPercentage - (_maximumTimeLeft - _minimumTimeLeft) * _percentageMining;
                if (_timeLeft <= 0)
                {
                    _onGoldCollected?.Invoke(10);
                    _timeLeft = _maximumTimeLeft;
                    _timeLeftWithoutPercentage = _maximumTimeLeft;
                }
            }

            public void RemoveShip()
            {
                if (_numberShipsInZone - 1 <= 0)
                {
                    Debug.LogError($"Number ships must be more or equal zero (Current: {_numberShipsInZone - 1})");
                    return;
                }

                _numberShipsInZone -= 1;
                _percentageMining = RecalculatePercentage();
            }


            public void StopMining()
            {
                _timeLeft = 0.0f;
                Main.Instance.OnUpdateGame -= CustomUpdate;
            }

            private float RecalculatePercentage()
            {
                var currentPercentage = _numberShipsInZone * _percentageForNumberShipsInZone;
                if (currentPercentage >= 100)
                {
                    currentPercentage = 99.9f / 100f;
                }
                else if (currentPercentage == 0)
                {
                    currentPercentage = 1;
                }
                else
                {
                    currentPercentage /= 100;
                }

                return currentPercentage;
            }
        }

        public event Action<PlayerType, float> OnGoldCollected;

        public float? GetTimeLeftForSelectedPlayer(PlayerType player)
        {
            if (!_playersMiningData.ContainsKey(player))
            {
                return null;
            }

            return _playersMiningData[player].TimeLeft;
        }

        private readonly Dictionary<PlayerType, MiningLogic> _playersMiningData = new ();

        public void AddShip(PlayerType player)
        {
            if (!_playersMiningData.ContainsKey(player))
            {
                StartMining(player);
                return;
            }

            _playersMiningData[player].AddShip();
        }

        public void RemoveShip(PlayerType player)
        {
            if (!_playersMiningData.ContainsKey(player))
            {
                Debug.LogWarning($"Player {player} has not started mining yet");
                return;
            }

            var miningData = _playersMiningData[player];
            if (miningData.CanRemoveShip())
            {
                miningData.RemoveShip();
            }
            else
            {
                StopMining(player);
            }
        }

        private void StartMining(PlayerType player, int numberShipsInZone = 1)
        {
            if (_playersMiningData.ContainsKey(player))
            {
                Debug.LogWarning($"Player {player} can't start mining because mining is already start");
                return;
            }

            var loader = Main.Instance.LoaderManager.Get<MiningPlanetLoader>();
            if (loader != null)
            {
                var miningLogic = new MiningLogic(
                    loader.GetMinimumTimeMining(),
                    loader.GetMaximumTimeMining(),
                    loader.AccelerationPercentageForEachShip(),
                    numberShipsInZone,
                    gold => OnGoldCollectedAction(player, gold));
                _playersMiningData[player] = miningLogic;
            }
        }

        private void StopMining(PlayerType player)
        {
            _playersMiningData[player].StopMining();
            _playersMiningData.Remove(player);
        }

        private void OnGoldCollectedAction(PlayerType player, float gold)
        {
            OnGoldCollected?.Invoke(player, gold);
        }
    }
}