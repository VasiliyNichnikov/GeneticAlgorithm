using UnityEngine;

namespace Utils
{
    public static class CameraUtils
    {
        public static Vector3 GetMouseWorldPosition()
        {
            var vector = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
            vector.z = 0f;
            return vector;
        }

        public static Vector3 GetMouseWorldPositionWithZ()
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
        {
            return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
        }

        public static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
        {
            var worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
            return worldPosition;
        }
    }
}