using UnityEngine;

namespace HandlerClicks
{
    public interface IObjectToClick
    {
        void Clicked(Vector3 position);

        void OnStartDrag();
        void OnEndDrag();
    }
}