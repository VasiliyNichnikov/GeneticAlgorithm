using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Mono.Collections.Generic;
using SpaceObjects;
using UnityEngine;
using Utils.Ship;

namespace ShipLogic.Strategy.Attack
{
    public class ShipAttackLogicBase : IDisposable
    {
        public ShipBase SelectedEnemy;
        public ICommanderCommander CommanderCommander => _commanderCommanderCache ??= _ship.GetCommander();
        public IReadOnlyCollection<ShipBase> FoundEnemies => _foundEnemies;
        public IReadOnlyCollection<ShipBase> FoundAllies => _foundAllies;
        

        private ICommanderCommander _commanderCommanderCache;
        
        private readonly ShipBase _ship;

        private readonly List<ShipBase> _foundEnemies = new();
        private readonly List<ShipBase> _foundAllies = new();

        public ShipAttackLogicBase(ShipBase ship)
        {
            _ship = ship;
        }

        /// <summary>
        /// Определяем тип найденного корабля и добавляем его
        /// Или в врагов или союзников
        /// </summary>
        /// <param name="detectedObject"></param>
        public void TryAddFoundShip(IDetectedObject detectedObject)
        {
            var shipType = _ship.TryGetOtherShip(detectedObject, out var ship);
            switch (shipType)
            {
                case UtilsEnemy.ShipType.None:
                    break;
                case UtilsEnemy.ShipType.Ally:
                    TryAddShipInList(_foundAllies, ship);
                    break;
                case UtilsEnemy.ShipType.Enemy:
                    TryAddShipInList(_foundEnemies, ship);
                    break;
            }
        }

        /// <summary>
        /// Определяем тип найденного корабля и удаляем его
        /// Если он есть среди врагов или союзников
        /// </summary>
        public void TryRemoveFoundShip(IDetectedObject detectedObject)
        {
            var shipType = _ship.TryGetOtherShip(detectedObject, out var ship);
            switch (shipType)
            {
                case UtilsEnemy.ShipType.None:
                    break;
                case UtilsEnemy.ShipType.Ally:
                    TryRemoveShipFromList(_foundAllies, ship);
                    break;
                case UtilsEnemy.ShipType.Enemy:
                    if (SelectedEnemy != null && SelectedEnemy.Equals(ship))
                    {
                        LosingSelectedEnemy();
                    }
                    
                    TryRemoveShipFromList(_foundEnemies, ship);
                    break;
            }
        }

        private void TryAddShipInList(ICollection<ShipBase> shipList, ShipBase additionalShip)
        {
            if (shipList.Contains(additionalShip))
            {
                Debug.LogError($"(Ship: {_ship.gameObject.name}) Current detected object is contains in list: {additionalShip.gameObject.name}");
                return;
            }
            
            shipList.Add(additionalShip);
        }

        private void TryRemoveShipFromList(ICollection<ShipBase> shipList, ShipBase removingShip)
        {
            if (!shipList.Contains(removingShip))
            {
                // Это нормальная логика, так как RemoveFoundEnemy - подписка, то
                // уведомление приходит даже тем, у кого корабля нет
                // Debug.LogWarning("Current detected object is not contains in list");
                return;
            }
            
            shipList.Remove(removingShip);
        }
        
        
        public void LosingSelectedEnemy()
        {
            if (SelectedEnemy == null)
            {
                Debug.LogError("Enemy is already null");
                return;
            }

            CommanderCommander.ChangeStateToIdle();
            SelectedEnemy = null;
            // Commander.SetPointForMovementToEnemy();
        }
        
        [CanBeNull]
        public ShipBase SortAndGetFirstEnemyShip(Comparison<ShipBase> sortingFunc)
        {
            if (_foundEnemies.Count == 0)
            {
                return null;
            }
            _foundEnemies.Sort(sortingFunc);
            return _foundEnemies[0];
        }

        public void Dispose()
        {
            _commanderCommanderCache = null;
            SelectedEnemy = null;
            _foundEnemies.Clear();
            _foundAllies.Clear();
        }
    }
}