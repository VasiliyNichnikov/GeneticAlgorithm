using System;
using System.Collections.Generic;
using Planets;
using UnityEngine;

namespace Group
{
    public class PointForMovement : ITarget
    {
        public float ThreatLevel => throw new Exception("Need supported");
        private readonly Vector3 _position;


        public PointForMovement(Vector3 position)
        {
            _position = position;
        }

        public Vector3 GetPointToApproximate()
        {
            return _position;
        }

        public IReadOnlyCollection<Vector3> GetPointsInSector()
        {
            Debug.LogError("GetPointsInSector don't supported");
            return ArraySegment<Vector3>.Empty;
        }
    }
}