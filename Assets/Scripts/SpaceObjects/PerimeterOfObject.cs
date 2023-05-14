using UnityEngine;
using Utils;

namespace SpaceObjects
{
    public class PerimeterOfObject : MonoBehaviour
    {
        public Vector3 RightTopPoint => transform.GetRightTopPoint(_collider);
        public Vector3 LeftBottomPoint => transform.GetLeftBottomPoint(_collider);
        
        [SerializeField] private Collider _collider;

        
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
            {
                return;
            }

            Gizmos.color = Color.red;
            foreach (var point in transform.GetPointsAlongPerimeter(_collider))
            {
                Gizmos.DrawSphere(point, 0.5f);
            }
        }
#endif
    }
}