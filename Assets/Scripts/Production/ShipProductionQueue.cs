using System;
using System.Collections.Generic;
using Group;
using JetBrains.Annotations;
using Loaders;
using Players;
using ShipLogic;
using UnityEngine;
using Utils;

namespace Production
{
    public class ShipProductionQueue
    {
        public class Production
        {
            public float MaxTime { get; }

            public float CurrentTime
            {
                get
                {
                    if (_timer == null)
                    {
                        return 0.0f;
                    }

                    return !_timer.IsRunning ? 0.0f : _timer.CurrentTime;
                }
            }

            private readonly Action _onCompleteProduction;

            public ShipType ShipType { get; }
            private Timer _timer;

            public Production(float maxTime, ShipType ship, Action onCompleteProduction)
            {
                ShipType = ship;
                MaxTime = maxTime;
                _onCompleteProduction = onCompleteProduction;
            }

            public void Start()
            {
                if (_timer is { IsRunning: true })
                {
                    Debug.LogWarning("Timer is already running");
                    return;
                }

                _timer = Timer.StartTimer(MaxTime, () => _onCompleteProduction?.Invoke());
            }
        }

        [CanBeNull] public Production CurrentProduction => _currentProduction;

        private readonly PlayerType _player;
        private readonly Transform _startingPoint;
        private readonly PlayerGoldManager _goldManager;

        private Production _currentProduction = null!;
        private readonly Queue<Production> _productions = new();

        public ShipProductionQueue(PlayerType player, Transform startingPoint, PlayerGoldManager goldManager)
        {
            _player = player;
            _startingPoint = startingPoint;
            _goldManager = goldManager;
        }

        public void AddShipToProduction(ShipData data, Action<ShipBase> onCompleteProduction)
        {
            void AddProduction(float time)
            {
                var production = new Production(time, data.ShipType, () =>
                {
                    _currentProduction = null;
                    var createdShip = CreateShip(data);
                    onCompleteProduction?.Invoke(createdShip);
                    CheckNextProduction();
                });
                _productions.Enqueue(production);
            }

            Main.Instance.LoaderManager.LoadAsync<ProductionLoader>(loader =>
            {
                var price = loader.GetPriceForShip(data.ShipType);
                var time = loader.GetTimeForShip(data.ShipType);

                if (_goldManager.CanMakeTransaction(price))
                {
                    AddProduction(time);
                    _goldManager.RemoveGold(price);
                    CheckNextProduction();
                }
            }, false);
        }

        private void CheckNextProduction()
        {
            if (_currentProduction != null || _productions.Count == 0)
            {
                return;
            }

            _currentProduction = _productions.Dequeue();
            _currentProduction.Start();
        }

        private ShipBase CreateShip(ShipData data)
        {
            var ship = Main.Instance.ShipFactory.AddShipOnMap(_player, data);
            ship.transform.position = _startingPoint.position;
            // if (ship.GetCommander() is ISupportedGroup shipInGroup)
            // {
            //     Main.Instance.ShipGroupManager.AddShipInGroup(_player, shipInGroup);
            // }

            return ship;
        }
    }
}