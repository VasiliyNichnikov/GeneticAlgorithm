using UnityEngine;

namespace Utils
{
    public static class TransformUtils
    {
        private const float BigValue = 10000f;


        public static Vector3 GetRightTopPoint(this Transform transform, Collider collider)
        {
            return collider.ClosestPoint(transform.TransformPoint((Vector3.right + Vector3.forward) * BigValue));
        }

        public static Vector3 GetLeftBottomPoint(this Transform transform, Collider collider)
        {
            return collider.ClosestPoint(transform.TransformPoint((Vector3.left + Vector3.back) * BigValue));
        }

        public static Vector3[] GetPointsAlongPerimeter(this Transform transform, Collider collider)
        {
            var cornerTopRight = GetRightTopPoint(transform, collider);
            var cornerTopLeft =
                collider.ClosestPoint(transform.TransformPoint((Vector3.left + Vector3.forward) * BigValue));
            var cornerBottomRight =
                collider.ClosestPoint(transform.TransformPoint((Vector3.right + Vector3.back) * BigValue));
            var cornerBottomLeft = GetLeftBottomPoint(transform, collider);


            return new[] { cornerTopRight, cornerTopLeft, cornerBottomRight, cornerBottomLeft };
        }
    }
}