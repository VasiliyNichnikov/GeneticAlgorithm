using System;
using System.Collections.Generic;
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
            private int _numberShipsInZone;
            private readonly Action<float> _onGoldCollected;

            public MiningLogic(float timeLeft, int numberShipsInZone, Action<float> onGoldCollected)
            {
                if (numberShipsInZone <= 0)
                {
                    Debug.LogError($"Number ships must be more zero (Current: {numberShipsInZone})");
                    _numberShipsInZone = 1;
                    _timeLeft = timeLeft;
                    return;
                }
                
                _timeLeft = timeLeft;
                _numberShipsInZone = numberShipsInZone;
                _onGoldCollected = onGoldCollected;

                Main.Instance.OnUpdateGame += CustomUpdate;
            }

            public void AddShip()
            {
                _numberShipsInZone += 1;
                _timeLeft = RecalculateMiningTime();
            }

            public bool CanRemoveShip()
            {
                return _numberShipsInZone - 1 > 0;
            }

            private void CustomUpdate()
            {
                _timeLeft -= Time.deltaTime;
                if (_timeLeft <= 0)
                {
                    // todo нужно будет внести число добытых монет
                    _onGoldCollected?.Invoke(10);
                    _timeLeft = RecalculateMiningTime();
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
                _timeLeft = RecalculateMiningTime();
            }


            public void StopMining()
            {
                _timeLeft = 0.0f;
                Main.Instance.OnUpdateGame -= CustomUpdate;
            }

            private float RecalculateMiningTime()
            {
                // todo доделать
                // Логика следующая: чем больше кораблей, тем быстрей идет добыча
                // Так же надо учитывать после добавления нового корабля
                return 1.0f;
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

        private readonly Dictionary<PlayerType, MiningLogic> _playersMiningData = new Dictionary<PlayerType, MiningLogic>();

        public void AddShip(PlayerType player)
        {
            if(!_playersMiningData.ContainsKey(player))
            {
                StartMining(player);
                return;
            }

            _playersMiningData[player].AddShip();
        }

        public void RemoveShip(PlayerType player)
        {
            if(!_playersMiningData.ContainsKey(player))
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

            // todo доделать
            _playersMiningData[player] = new MiningLogic(1.0f, numberShipsInZone, gold => OnGoldCollectedAction(player, gold));
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