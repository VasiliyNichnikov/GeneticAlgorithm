using System;
using JetBrains.Annotations;
using UnityEngine;

namespace HandlerClicks
{
    public class MainClickHandler : MonoBehaviour, IDisposable
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private LayerMask _mask;

        private IObjectToClick _previewObjectToClick;
        
        private void Start()
        {
            Main.Instance.OnUpdateGame += CheckClicks;
            Main.Instance.OnUpdateGame += CheckDirection;
        }

        private void CheckClicks()
        {
            if (!Input.GetMouseButtonDown(0))
            {
                return;
            }
            
            var objToClick = GetObjectToClick();
            objToClick?.Clicked();
        }

        /// <summary>
        /// Проверяем наводится мышь на объект или нет
        /// </summary>
        private void CheckDirection()
        {
            var objToClick = GetObjectToClick();
            if (_previewObjectToClick == null && objToClick != null)
            {
                _previewObjectToClick = objToClick;
            }
            else if (_previewObjectToClick != null && objToClick == null)
            {
                _previewObjectToClick?.OnEndDrag();
                _previewObjectToClick = null;
                return;
            }
            objToClick?.OnStartDrag();
        }

        [CanBeNull]
        private IObjectToClick GetObjectToClick()
        {
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _mask))
            {
                return null;
            }
            
            var obj = hit.collider.gameObject;
            var objToClick = obj.GetComponent<IObjectToClick>();
            return objToClick;
        }

        private void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            Main.Instance.OnUpdateGame -= CheckClicks;
            Main.Instance.OnUpdateGame -= CheckDirection;
        }
    }
}