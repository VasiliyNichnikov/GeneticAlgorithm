using System;
using Map;
using Players;
using SpaceObjects;
using UnityEngine;

namespace Planets
{
    public class Sun : MonoBehaviour, IObjectOnMap
    {
        public Vector3 ObjectPosition => transform.position;
        public MapObjectType TypeObject => MapObjectType.Planet;
        public PlayerType PlayerType => PlayerType.None;
        public bool IsStatic => true;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;

        [SerializeField] private PerimeterOfObject _perimeter;

        private void Start()
        {
            SpaceMap.Map.AddObjectOnMap(this);
        }
    }
}