using UnityEngine;

namespace Utils
{
    public static class RectTransformExtension
    {
        public static void PinUIObjectToObjectOnScene(this RectTransform obj, Vector3 position, Vector2? shift = null)
        {
            var camera = Camera.main;
            var anchoredPosition = camera.GetPositionOfObjectOnSceneInUI(
                Main.Instance.LocationCanvas.GetComponent<RectTransform>().sizeDelta,
                position);

            if (shift != null)
            {
                anchoredPosition.x += shift.Value.x;
                anchoredPosition.y += shift.Value.y;
            }

            obj.anchoredPosition = anchoredPosition;
        }
    }
}