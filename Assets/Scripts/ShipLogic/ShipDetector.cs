#nullable enable
using System;
using System.Collections.Generic;
using Map;
using Planets;
using UnityEngine;
using Utils.Ship;

namespace ShipLogic
{
    public class ShipDetector : MonoBehaviour, IShipDetector
    {
        public IReadOnlyCollection<ShipBase> Enemies => _foundEnemies;
        public IReadOnlyCollection<ShipBase> Allies => _foundAllies;
        public IReadOnlyCollection<IPlanet> Planets => _foundPlanets;

        [SerializeField] private SphereCollider _collider = null!;

        public float Radius => _collider.radius;

        private ShipBase _ship = null!;

        private readonly List<ShipBase> _foundEnemies = new();
        private readonly List<ShipBase> _foundAllies = new();
        private readonly List<IPlanet> _foundPlanets = new();

        public void Init(ShipBase ship)
        {
            _ship = ship;
            Main.Instance.ShipFactory.OnDestroyShip += TryRemoveFoundShip;
        }

        public void SetRadius(float radius)
        {
            if (radius <= 0)
            {
                Debug.LogError($"Not corrected value radius: {radius}");
                return;
            }

            _collider.radius = radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            var detectedShip = other.GetComponent<IObjectOnMap>();
            if (detectedShip == null)
            {
                return;
            }

            TryAddFoundObject(detectedShip);
        }


        private void OnTriggerExit(Collider other)
        {
            var detectedShip = other.GetComponent<IObjectOnMap>();
            if (detectedShip == null)
            {
                return;
            }

            TryRemoveFoundObject(detectedShip);
        }

        private void TryAddFoundObject(IObjectOnMap detectedObject)
        {
            if (detectedObject.TypeObject == MapObjectType.Ship)
            {
                var shipType = _ship.TryGetOtherShip(detectedObject, out var ship);
                switch (shipType)
                {
                    case UtilsMap.ShipType.None:
                        Debug.LogError($"Not corrected type: {shipType}");
                        break;
                    case UtilsMap.ShipType.Ally:
                        TryAddObjectInList(_foundAllies, ship);
                        break;
                    case UtilsMap.ShipType.Enemy:
                        TryAddObjectInList(_foundEnemies, ship);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (detectedObject.TypeObject == MapObjectType.Planet)
            {
                var state = UtilsMap.TryGetPlanet(detectedObject, out var planet);
                if (state)
                {
                    TryAddObjectInList(_foundPlanets, planet);
                }
            }
            else
            {
                Debug.LogError($"Not corrected type: {detectedObject.TypeObject}");
            }
        }

        public void TryRemoveFoundShip(ShipBase enemy)
        {
            TryRemoveFoundObject(enemy);
        }

        private void TryRemoveFoundObject(IObjectOnMap detectedObject)
        {
            if (detectedObject.TypeObject == MapObjectType.Ship)
            {
                var shipType = _ship.TryGetOtherShip(detectedObject, out var ship);
                switch (shipType)
                {
                    case UtilsMap.ShipType.None:
                        Debug.LogError($"Not corrected type: {shipType}");
                        break;
                    case UtilsMap.ShipType.Ally:
                        TryRemoveObjectFromList(_foundAllies, ship);
                        break;
                    case UtilsMap.ShipType.Enemy:
                        TryRemoveObjectFromList(_foundEnemies, ship);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (detectedObject.TypeObject == MapObjectType.Planet)
            {
                var state = UtilsMap.TryGetPlanet(detectedObject, out var planet);
                if (state)
                {
                    TryRemoveObjectFromList(_foundPlanets, planet);
                }
            }
            else
            {
                Debug.LogError($"Not corrected type: {detectedObject.TypeObject}");
            }
        }

        private void TryAddObjectInList<T>(ICollection<T> objectList, T newObject)
        {
            if (objectList.Contains(newObject))
            {
                Debug.LogError(
                    $"(Ship: {_ship.gameObject.name}) Current detected object is contains in list: {newObject}");
                return;
            }

            objectList.Add(newObject);
        }

        private static void TryRemoveObjectFromList<T>(ICollection<T> objectList, T newObject)
        {
            if (!objectList.Contains(newObject))
            {
                // Это нормальная логика, так как RemoveFoundEnemy - подписка, то
                // уведомление приходит даже тем, у кого корабля нет
                // Debug.LogWarning("Current detected object is not contains in list");
                return;
            }

            objectList.Remove(newObject);
        }

        public void Dispose()
        {
            Main.Instance.ShipFactory.OnDestroyShip -= TryRemoveFoundShip;
            _foundEnemies.Clear();
            _foundAllies.Clear();
            _foundPlanets.Clear();
            _ship = null!;
        }
    }
}