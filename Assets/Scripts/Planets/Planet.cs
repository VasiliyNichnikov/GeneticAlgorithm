using FindingPath;
using SpaceObjects;
using UnityEngine;

namespace Planets
{
    public class Planet : MonoBehaviour, IObjectOnMap
    {
        public MapObjectType TypeObject => MapObjectType.Planet;
        public bool IsStatic => true;
        public Vector3 LeftBottomPosition => _perimeter.LeftBottomPoint;
        public Vector3 RightTopPosition => _perimeter.RightTopPoint;

        [SerializeField] private PerimeterOfObject _perimeter;

        private void Start()
        {
            Main.Instance.Map.AddObjectOnMap(this);
        }
    }
}