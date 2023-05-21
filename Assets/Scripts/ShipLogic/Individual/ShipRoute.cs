using System.Collections.Generic;
using UnityEngine;

namespace ShipLogic.Individual
{
    public class ShipRoute
    {
        private readonly Queue<Vector3> _pointsForMovement = new Queue<Vector3>();
        
        private Vector3? _highPriorityPoint = null;
        private Vector3? _currentPointForMovement;

        public bool IsMovement => _highPriorityPoint != null || _currentPointForMovement != null;  
        
        public void SetHighPriorityPoint(Vector3? highPriorityPoint)
        {
            _highPriorityPoint = highPriorityPoint;
        }

        public void AddPointForMovement(Vector3 point)
        {
            if (_pointsForMovement.Contains(point))
            {
                return;
            }

            _pointsForMovement.Enqueue(point);
        }

        public Vector3 GetPointForMovement()
        {
            if (_highPriorityPoint != null)
            {
                return _highPriorityPoint.Value;
            }

            if (_currentPointForMovement != null)
            {
                return _currentPointForMovement.Value;
            }
            
            return Vector3.zero;
        }
        
        public Vector3 ChangeAndGetPointForMovement()
        {
            if (_pointsForMovement.Count == 0)
            {
                _currentPointForMovement = null;
            }
            else
            {
                _currentPointForMovement = _pointsForMovement.Dequeue();
            }
            return GetPointForMovement();
        }
    }
}