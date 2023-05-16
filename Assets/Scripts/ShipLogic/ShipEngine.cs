using System;
using FindingPath;
using UnityEngine;

namespace ShipLogic
{
    public interface IShipEngine
    {
        void SetTarget(Vector3 target);
        float TurnToTarget(Vector3 target);
        void Move();
    }

    public class ShipEngine : IShipEngine
    {
        #region Speed

        private readonly float _currentSpeed;
        private readonly float _rotationSpeed;

        #endregion

        private readonly IFindingPath _findingPath;
        private readonly Transform _shipTransform;
        private readonly float _radiusShip;

        private Vector3 _target;
        private int _pathIteration;
        private Vector3 _destination;

        private Vector3[] _currentPath;
        
        #if UNITY_EDITOR
        public Vector3[] PathDebug = Array.Empty<Vector3>();
        #endif
        

        public ShipEngine(
            Transform shipTransform,
            IFindingPath findingPath,
            float radiusShip,
            float speed,
            float rotationSpeed)
        {
            _findingPath = findingPath;
            _shipTransform = shipTransform;
            _radiusShip = radiusShip;
            _currentSpeed = speed;
            _rotationSpeed = rotationSpeed;
        }

        public void SetTarget(Vector3 target)
        {
            if (_target == target)
            {
                return;
            }
            
            _pathIteration = 0;
            _target = target;
            _currentPath = CreateRoute();
            PathDebug = _currentPath;
        }

        public void Move()
        {
            if (_currentPath.Length == 0)
            {
                return;
            }


            if (_pathIteration >= _currentPath.Length)
            {
                _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                return;
            }

            _destination = _currentPath[_pathIteration];

            if (_destination.x < float.PositiveInfinity)
            {
                var direction = _destination - _shipTransform.position;
                direction.y = 0.0f;
                var newDirection = Vector3.RotateTowards(_shipTransform.forward, direction,
                    _rotationSpeed * Time.deltaTime, 0.0f);

                var newRotation = Quaternion.LookRotation(newDirection);

                _shipTransform.rotation = Quaternion.Slerp(_shipTransform.rotation, newRotation,
                    Time.deltaTime * _rotationSpeed);

                var distance = Vector3.Distance(_shipTransform.position, _destination);
                if (distance > _radiusShip)
                {
                    var movement = _shipTransform.forward * Time.deltaTime * _currentSpeed;
                    _shipTransform.Translate(movement, Space.World);
                }
                else
                {
                    ++_pathIteration;
                    if (_pathIteration >= _currentPath.Length)
                    {
                        _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity,
                            float.PositiveInfinity);
                    }
                }
            }
        }

        public float TurnToTarget(Vector3 target)
        {
            var direction = target - _shipTransform.position;
            direction.y = 0.0f;
            var newDirection =
                Vector3.RotateTowards(_shipTransform.forward, direction, _rotationSpeed * Time.deltaTime, 0.0f);
            var newRotation = Quaternion.LookRotation(newDirection);

            _shipTransform.rotation =
                Quaternion.Slerp(_shipTransform.rotation, newRotation, Time.deltaTime * _rotationSpeed);

            return Vector3.Angle(direction, newDirection);
        }


        /// <summary>
        /// Составление маршрута
        /// </summary>
        private Vector3[] CreateRoute()
        {
            var path = _findingPath.TryToFindPath(_shipTransform.position, _target, out var isFound);
            if (!isFound)
            {
                Debug.LogWarning("Path not found");
                return new[] { _target };
            }
            return path;
        }
    }
}