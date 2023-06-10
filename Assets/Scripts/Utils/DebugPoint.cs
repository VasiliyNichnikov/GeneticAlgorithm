using System.Collections.Generic;
using System.Linq;
using Planets;
using UnityEngine;

namespace Utils
{
    public class DebugPoint : MonoBehaviour, ITarget
    {
        public float ThreatLevel => _threatLevel;

        [SerializeField] private float _threatLevel;
        [SerializeField] private Transform[] _pointsInSector;
        
        public Vector3 GetPointToApproximate()
        {
            return transform.position;
        }

        public IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            return _pointsInSector.Select(p => p.position).ToArray();
        }
    }
}