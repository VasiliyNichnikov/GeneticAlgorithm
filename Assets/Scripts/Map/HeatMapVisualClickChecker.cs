using System;
using HandlerClicks;
using UnityEngine;

namespace Map
{
    public class HeatMapVisualClickChecker : MonoBehaviour, IDisposable, IObjectToClick
    {
        private Grid _grid;

        public void Init(Grid gridForClicks)
        {
            _grid = gridForClicks;
        }

        public void Dispose()
        {
            _grid = null;
        }

        public void Clicked(Vector3 position)
        {
            var positions = _grid.GetEmptyPointsAroundSelectedObject(position, (int)MapObjectType.Empty, 2);
            foreach (var p in positions)
            {
                _grid.SetValue(p, 55);
            }

            // _grid.AddValue(position, 5, 2);
        }

        public void OnStartDrag()
        {
            // nothing
        }

        public void OnEndDrag()
        {
            // nothing
        }
    }
}