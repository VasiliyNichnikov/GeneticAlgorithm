using UnityEngine;

namespace Utils
{
    public static class CameraExtension
    {
        /// <summary>
        /// Возвращает позицию объекта 3d в UI пространстве, в качестве sizeDelta передается Canvas.sizeDelta
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetPositionOfObjectOnSceneInUI(this Camera camera, Vector2 sizeDelta, Vector3 position)
        {
            var viewportPosition = camera.WorldToViewportPoint(position);
            var viewportRelative = new Vector2(viewportPosition.x, viewportPosition.y) - new Vector2(0.5f, 0.5f);
            var screenPosition = new Vector2(viewportRelative.x * sizeDelta.x, viewportRelative.y * sizeDelta.y);

            return screenPosition;
        }
    }
}