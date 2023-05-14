using System;
using UnityEngine;

namespace ShipLogic
{
    public interface IShipEngine
    {
        void SetTarget(Vector3 target);
        float TurnToTarget(Vector3 target);
        bool Move();
    }

    public class ShipEngine : IShipEngine
    {
        #region Speed

        private float _currentSpeed;
        private readonly float _minSpeed;
        private readonly float _rotationSpeed;

        private readonly Func<float, float> _boostSpeed;
        private readonly Func<float, float> _slowingDownSpeed;

        #endregion

        private readonly Transform _shipTransform;
        private readonly float _radiusShip;

        private Vector3 _target;
        private int _pathIteration;
        private Vector3 _destination;

        private Vector3[] _currentPath;

        private readonly float _minAngleRotation;

        public ShipEngine(Transform shipTransform,
            float radiusShip,
            Func<float, float> boostSpeed,
            Func<float, float> slowingDownSpeed,
            float minAngleRotation,
            float minSpeed,
            float rotationSpeed)
        {
            _shipTransform = shipTransform;
            _radiusShip = radiusShip;
            _minAngleRotation = minAngleRotation;
            _boostSpeed = boostSpeed;
            _slowingDownSpeed = slowingDownSpeed;
            _currentSpeed = _minSpeed;
            _rotationSpeed = rotationSpeed;
            _minSpeed = minSpeed;
        }

        public void SetTarget(Vector3 target)
        {
            _pathIteration = 0;
            _target = target;
            _currentPath = CreateRoute();
        }

        public bool Move()
        {
            if (_currentPath.Length == 0)
            {
                return false;
            }


            if (_pathIteration >= _currentPath.Length)
            {
                _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                return false;
            }

            _destination = _currentPath[_pathIteration];

            if (_destination.x < float.PositiveInfinity)
            {
                var direction = _destination - _shipTransform.position;
                direction.y = 0.0f;
                var newDirection = Vector3.RotateTowards(_shipTransform.forward, direction,
                    _rotationSpeed * Time.deltaTime, 0.0f);

                var angleRotation = Vector3.Angle(direction, newDirection);

                _currentSpeed = angleRotation > _minAngleRotation
                    ? _slowingDownSpeed.Invoke(_currentSpeed)
                    : _boostSpeed.Invoke(_currentSpeed);

                if (angleRotation > _minAngleRotation)
                {
                    var newRotation = Quaternion.LookRotation(newDirection);

                    _shipTransform.rotation = Quaternion.Slerp(_shipTransform.rotation, newRotation,
                        Time.deltaTime * _rotationSpeed);
                }

                var distance = Vector3.Distance(_shipTransform.position, _destination);
                if (distance > _radiusShip)
                {
                    var movement = _shipTransform.forward * Time.deltaTime * _currentSpeed;
                    _shipTransform.Translate(movement, Space.World);
                    return true;
                }
                else
                {
                    ++_pathIteration;
                    if (_pathIteration >= _currentPath.Length)
                    {
                        _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity,
                            float.PositiveInfinity);
                    }

                    return false;
                }
            }

            return false;
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
            // todo доделать
            return new[] { _target };
        }
    }
}