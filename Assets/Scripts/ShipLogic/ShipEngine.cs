using System;
using FindingPath;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.AI;

namespace ShipLogic
{
    public interface IShipEngine
    {
        void SetTarget(Vector3 target);
        float TurnToTarget(Vector3 target);
        void Move();

#if UNITY_EDITOR
        NavMeshPath PathDebug { get; }
#endif
    }

    public class ShipEngine : IShipEngine
    {
        #region Speed

        private readonly float _movementSpeed;
        private readonly float _rotationSpeed;

        #endregion

        private readonly IFindingPath _findingPath;
        private readonly Transform _shipTransform;
        private readonly NavMeshAgent _agent;
        private Vector3 _agentPosition;
        private readonly float _radiusShip;

        private Vector3 _target;
        private int _pathIteration;
        private Vector3 _destination;

        private NavMeshPath _path = null;

#if UNITY_EDITOR
        [CanBeNull] public NavMeshPath PathDebug => _path;
#endif


        public ShipEngine(
            Transform shipTransform,
            NavMeshAgent agent,
            float radiusShip,
            float speed,
            float rotationSpeed)
        {
            _shipTransform = shipTransform;
            _agent = agent;
            _radiusShip = radiusShip;
            _movementSpeed = speed;
            _rotationSpeed = rotationSpeed;
        }

        public void SetTarget(Vector3 target)
        {
            if (_target == target)
            {
                return;
            }

            _pathIteration = 1;
            _target = target;
            _path = new NavMeshPath();
            _agent.CalculatePath(_target, _path);
            _agent.isStopped = false;
        }

        public void Move()
        {
            SetAgentPosition();
            
            if (_path.corners.Length == 0)
            {
                return;
            }

            if (_pathIteration >= _path.corners.Length)
            {
                _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                _agent.isStopped = true;
                return;
            }

            _destination = _path.corners[_pathIteration];

            if (_destination.x < float.PositiveInfinity)
            {
                var direction = _destination - _agentPosition;
                direction.y = 0.0f;

                var newDirection = Vector3.RotateTowards(_shipTransform.forward, direction,
                    _rotationSpeed * Time.deltaTime, 0.0f);
            
                var angleRotation = Vector3.Angle(_shipTransform.forward, direction);
            
                var newRotation = Quaternion.LookRotation(newDirection);
                _shipTransform.rotation = Quaternion.RotateTowards(_shipTransform.rotation, newRotation,
                    Time.deltaTime * _rotationSpeed);
                
                if (angleRotation >= 0.5f)
                {
                    return;
                }
                
                var distance = Vector3.Distance(_agentPosition, _destination);
                if (distance > _agent.radius + 0.1f)
                {
                    var movement = _shipTransform.forward * Time.deltaTime * _movementSpeed;
                    _agent.Move(movement);
                }
                else
                {
                    ++_pathIteration;
                    if (_pathIteration >= _path.corners.Length)
                    {
                        _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity,
                            float.PositiveInfinity);
                        _agent.isStopped = true;
                    }
                }
            }
        }

        private void SetAgentPosition()
        {
            if (NavMesh.SamplePosition(_shipTransform.position, out var hit, 6.0f, NavMesh.AllAreas))
            {
                _agentPosition = hit.position;
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
                Quaternion.RotateTowards(_shipTransform.rotation, newRotation, Time.deltaTime * _rotationSpeed);

            return Vector3.Angle(_shipTransform.forward, newDirection);
        }
        
    }
}