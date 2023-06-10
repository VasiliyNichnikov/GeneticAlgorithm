using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Map
{
    public class GridViewing : IDisposable
    {
        private readonly GridSettings _settings;
        private readonly HeatMapVisual _heatMapVisualPrefab;

        private readonly Transform _heatMapParent;
        private HeatMapVisual _createdHeatMapVisual = null!;

        private Grid _previewActiveGrid = null!;
        
        public GridViewing(GridSettings settings, Transform heatMapParent, HeatMapVisual heatMapVisualPrefab)
        {
            _settings = settings;
            _heatMapParent = heatMapParent;
            _heatMapVisualPrefab = heatMapVisualPrefab;
        }

        public void SelectGridToDisplayForMovement()
        {
            CheckHeatMapVisual();
            CheckPreviewActiveGrid();
            
            _settings.GridForMovement.OnChangeCellValue += UpdateVisualGrid;
            
            _createdHeatMapVisual.SetGrid(_settings.GridForMovement);
            _createdHeatMapVisual.UpdateHeatMapVisual(true);

            _previewActiveGrid = _settings.GridForMovement;
        }

        public void SelectGridToDisplayForSector()
        {
            CheckHeatMapVisual();
            CheckPreviewActiveGrid();
            
            _settings.GridForSector.OnChangeCellValue += UpdateVisualGrid;

            _createdHeatMapVisual.SetGrid(_settings.GridForSector);
            _createdHeatMapVisual.UpdateHeatMapVisual(true);

            _previewActiveGrid = _settings.GridForSector;
        }
        
        public void SelectGridToDisplayForClick()
        {
            CheckHeatMapVisual();
            CheckPreviewActiveGrid();
            
            _settings.GridForClick.OnChangeCellValue += UpdateVisualGrid;

            var checkerClicks = _createdHeatMapVisual.GetComponent<HeatMapVisualClickChecker>();
            _createdHeatMapVisual.SetGrid(_settings.GridForClick);
            _createdHeatMapVisual.UpdateHeatMapVisual(true);
            checkerClicks.Init(_settings.GridForClick);
            
            _previewActiveGrid = _settings.GridForClick;
        }

        public void ChangeVisibilityHeatMapVisual(bool state)
        {
            if (_createdHeatMapVisual == null)
            {
                return;
            }

            _createdHeatMapVisual.gameObject.SetActive(state);
        }

        private void CheckHeatMapVisual()
        {
            if (_createdHeatMapVisual == null)
            {
                _createdHeatMapVisual = Object.Instantiate(_heatMapVisualPrefab, _heatMapParent, false);
            }
        }

        private void CheckPreviewActiveGrid()
        {
            if (_previewActiveGrid != null)
            {
                _previewActiveGrid.OnChangeCellValue -= UpdateVisualGrid;
            }
        }
        
        private void UpdateVisualGrid((int x, int z) position)
        {
            if (_createdHeatMapVisual == null)
            {
                return;
            }
            
            _createdHeatMapVisual.UpdateHeatMapVisual();
        }


        public void Dispose()
        {
            if (_previewActiveGrid != null)
            {
                _previewActiveGrid.OnChangeCellValue -= UpdateVisualGrid;
            }
        }
    }
}