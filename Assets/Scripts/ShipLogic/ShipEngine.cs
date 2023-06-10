using Map;
using UnityEngine;
using UnityEngine.AI;

namespace ShipLogic
{
    public interface IShipEngine
    {
        bool HasPath { get; }
        bool IsStopped { get; }
        void SetTarget(Vector3 target, bool checkTarget = true);
        float TurnToTarget(Vector3 target);
        void Move();
        void TryJumpToLastPoint();

#if UNITY_EDITOR
        Vector3[] PathDebug { get; }
#endif
    }

    public class ShipEngine : IShipEngine
    {
        #region Speed

        private const float CustomRotationSpeed = 2.5f;
        
        private readonly float _movementSpeed;
        private readonly float _rotationSpeed;

        #endregion
        
        private readonly Transform _shipTransform;
        private readonly NavMeshAgent _agent;
        private Vector3 _agentPosition;

        private Vector3 _target;
        private int _pathIteration;
        private Vector3 _destination;
        
        private Vector3[] _pathForMovement;

#if UNITY_EDITOR
        public Vector3[] PathDebug => _pathForMovement;
#endif


        public ShipEngine(
            Transform shipTransform,
            NavMeshAgent agent,
            float speed,
            float rotationSpeed)
        {
            _shipTransform = shipTransform;
            _agent = agent;
            _movementSpeed = speed;
            _rotationSpeed = rotationSpeed;
        }

        public bool HasPath => _pathForMovement is { Length: > 0 };
        public bool IsStopped => _agent.enabled && _agent.isStopped;

        public void SetTarget(Vector3 target, bool checkTarget = true)
        {
            if (_target == target && checkTarget)
            {
                return;
            }

            _pathIteration = 1;
            _target = target;
            _pathForMovement = SpaceMap.Map.TryToFindPath(_shipTransform.position, target, out var isFound);
            _agent.isStopped = false;
        }
        
        public void Move()
        {
            SetAgentPosition();
            
            if (_pathForMovement.Length == 0)
            {
                return;
            }

            if (_pathIteration >= _pathForMovement.Length)
            {
                _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                _agent.isStopped = true;
                return;
            }

            _destination = _pathForMovement[_pathIteration];

            if (_destination.x < float.PositiveInfinity)
            {
                var direction = _destination - _agentPosition;
                direction.y = 0.0f;
                
                var newRotation = Quaternion.LookRotation(direction);
                _shipTransform.rotation = Quaternion.Slerp(_shipTransform.rotation, newRotation,
                    Time.deltaTime * CustomRotationSpeed);

                var distance = Vector3.Distance(_agentPosition, _destination);
                if (distance > _agent.radius)
                {
                    var movement = _shipTransform.forward * Time.deltaTime * _movementSpeed;
                    _agent.Move(movement);
                }
                else
                {
                    ++_pathIteration;
                    if (_pathIteration >= _pathForMovement.Length)
                    {
                        _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity,
                            float.PositiveInfinity);
                        _agent.isStopped = true;
                    }
                }
            }
        }

        public void TryJumpToLastPoint()
        {
            if (_pathForMovement.Length == 0)
            {
                return;
            }

            if (_pathIteration >= _pathForMovement.Length)
            {
                _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
                _agent.isStopped = true;
                return;
            }

            var endPosition = _pathForMovement[^1];
  
            _shipTransform.position = endPosition;
            _destination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            _agent.isStopped = true;
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
            
            var newRotation = Quaternion.LookRotation(direction);
            _shipTransform.rotation = Quaternion.Slerp(_shipTransform.rotation, newRotation,
                Time.deltaTime * CustomRotationSpeed);

            return Vector3.Angle(_shipTransform.forward, direction);
        }
        
    }
}