using UnityEngine;

namespace Utils
{
    public static class TextMeshUtils
    {
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default,
            int fontSize = 40, Color color = default, TextAnchor anchor = TextAnchor.MiddleCenter,
            TextAlignment alignment = TextAlignment.Center, int sortingOrder = 1)
        {
            if (color == default)
            {
                color = Color.white;
            }

            return CreateWorldText(parent, text, localPosition, fontSize, color, anchor, alignment, sortingOrder);
        }

        public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPosition, int fontSize,
            Color color, TextAnchor anchor, TextAlignment alignment, int sortingOrder)
        {
            var gameObject = new GameObject("World_Text", typeof(TextMesh));
            var transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;

            var textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = anchor;
            textMesh.alignment = alignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;
            return textMesh;
        }
    }
}