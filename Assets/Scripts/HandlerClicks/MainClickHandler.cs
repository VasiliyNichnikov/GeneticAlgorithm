using System;
using UnityEngine;

namespace HandlerClicks
{
    public class MainClickHandler : MonoBehaviour, IDisposable
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _mask;

        private void Start()
        {
            Main.Instance.OnUpdateGame += CheckClicks;
        }

        private void CheckClicks()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _mask))
            {
                return;
            }
            
            var obj = hit.collider.gameObject;
            var objToClick = obj.GetComponent<IObjectToClick>();
            objToClick?.Clicked();
        }
        
        public void Dispose()
        {
            Main.Instance.OnUpdateGame -= CheckClicks;
        }
    }
}