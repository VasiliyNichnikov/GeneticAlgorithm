using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace ShipLogic
{
    /// <summary>
    /// todo
    /// Тут сейчас есть ошибка из-за чего появляется точка в нулевой координате
    /// </summary>
    public class ShipRoute
    {
        private enum PriorityPoint
        {
            Movement = 0,
            Enemy = 1,
            EscapeFromBattle = 2,
            ToHelp = 3
        }

        private readonly struct PointData
        {
            public readonly Vector3 Position;
            public readonly PriorityPoint Priority;

            public PointData(Vector3 position, PriorityPoint priority)
            {
                Position = position;
                Priority = priority;
            }
        }

        public event Action OnEndPointsForMovement;

        private readonly Dictionary<PriorityPoint, Queue<Vector3>> _pointsForMovement = new();

        private readonly IPosition _shipPosition;
        private PointData? _currentPointForMovement;
        private readonly int[] _cachePriorities;

        public bool IsMovement => _currentPointForMovement != null;


        public ShipRoute(IPosition shipPosition)
        {
            _shipPosition = shipPosition;
            var values = Enum.GetValues(typeof(PriorityPoint));

            var valuesInt = values.Cast<int>().ToList();
            valuesInt.Sort();
            valuesInt.Reverse();
            
            _cachePriorities = valuesInt.ToArray();
        }


        public void AddPointForMovement(Vector3 pointPosition)
        {
            TryAddNewPoint(pointPosition, PriorityPoint.Movement);
            UpdateCurrentPointForMovement();
        }

        public void AddPointForMovementToEnemy(Vector3 pointPosition)
        {
            TryAddNewPoint(pointPosition, PriorityPoint.Enemy);
            UpdateCurrentPointForMovement();
        }

        public void AddPointForEscapeFromBattle(Vector3 pointPosition)
        {
            TryAddNewPoint(pointPosition, PriorityPoint.EscapeFromBattle);
            UpdateCurrentPointForMovement();
        }

        public void AddPointToHelp(Vector3 pointPosition)
        {
            TryAddNewPoint(pointPosition, PriorityPoint.ToHelp);
            UpdateCurrentPointForMovement();
        }

        public void AddPointForMovementRange(IEnumerable<Vector3> points)
        {
            foreach (var point in points)
            {
                TryAddNewPoint(point, PriorityPoint.Movement);
            }

            UpdateCurrentPointForMovement();
        }

        public Vector3 GetPointForMovement()
        {
            return _currentPointForMovement != null ? _currentPointForMovement.Value.Position : Vector3.zero;
        }

        public void ChangePointForMovement()
        {
            _currentPointForMovement = null;
            UpdateCurrentPointForMovement();
        }
        
        private void TryAddNewPoint(Vector3 newPoint, PriorityPoint priority)
        {
            var points = GetPointsByType(priority);
            if (points.Contains(newPoint))
            {
                return;
            }
            
            _pointsForMovement[priority].Enqueue(newPoint);
        }
        
        private Queue<Vector3> GetPointsByType(PriorityPoint priority)
        {
            if (_pointsForMovement.TryGetValue(priority, out var points))
            {
                return points;
            }

            _pointsForMovement.Add(priority, new Queue<Vector3>());
            return _pointsForMovement[priority];
        }
        
        private void UpdateCurrentPointForMovement()
        {
            foreach (var priority in _cachePriorities)
            {
                var priorityType = (PriorityPoint)priority;
                var points = GetPointsByType(priorityType);
                if (points.Count == 0)
                {
                    continue;
                }
                
                if (_currentPointForMovement == null || _currentPointForMovement.Value.Priority <= priorityType)
                {
                    // todo нормально не работает
                    // SaveCurrentPositionBeforeChangePoint(priorityType);
                    
                    _currentPointForMovement = new PointData(points.Dequeue(), priorityType);
                    return;
                }
            }

            _currentPointForMovement = null;
            OnEndPointsForMovement?.Invoke();
        }

        public void ClearAllPoints()
        {
            _pointsForMovement.Clear();
            _currentPointForMovement = null;
        }
        
        // todo не работает
        private void SaveCurrentPositionBeforeChangePoint(PriorityPoint nextPointPriority)
        {
            if (nextPointPriority != PriorityPoint.Enemy)
            {
                return;
            }
            
            if (_currentPointForMovement is { Priority: PriorityPoint.Movement })
            {
                TryAddNewPoint(_currentPointForMovement.Value.Position, PriorityPoint.Movement);
            }
            else if(_currentPointForMovement == null)
            {
                TryAddNewPoint(_shipPosition.ObjectPosition, PriorityPoint.Movement);
            }
        }
    }
}