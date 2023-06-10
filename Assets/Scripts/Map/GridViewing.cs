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

        private GridWrapper _previewActiveGrid;

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

            if (_settings.GridForMovement == null)
            {
                Debug.LogError("Grid for movement is null");
                return;
            }

            _settings.GridForMovement.OnChangeCellValue += UpdateVisualGrid;

            _createdHeatMapVisual.SetGrid(_settings.GridWrapperForMovement);
            _createdHeatMapVisual.UpdateHeatMapVisual(true);

            _previewActiveGrid = _settings.GridWrapperForMovement;
        }

        public void SelectGridToDisplayForSector()
        {
            CheckHeatMapVisual();
            CheckPreviewActiveGrid();

            if (_settings.GridForSector == null)
            {
                Debug.LogError("Grid for sector is null");
                return;
            }
            
            _settings.GridForSector.OnChangeCellValue += UpdateVisualGrid;

            _createdHeatMapVisual.SetGrid(_settings.GridWrapperForSector);
            _createdHeatMapVisual.UpdateHeatMapVisual(true);

            _previewActiveGrid = _settings.GridWrapperForSector;
        }

        public void SelectGridToDisplayForClick()
        {
            // CheckHeatMapVisual();
            // CheckPreviewActiveGrid();
            //
            // if (_settings.GridForClick == null)
            // {
            //     Debug.LogError("Grid for visual is null");
            //     return;
            // }
            //
            // _settings.GridForClick.OnChangeCellValue += UpdateVisualGrid;
            //
            // var checkerClicks = _createdHeatMapVisual.GetComponent<HeatMapVisualClickChecker>();
            // _createdHeatMapVisual.SetGrid(_settings.GridForClick);
            // _createdHeatMapVisual.UpdateHeatMapVisual(true);
            // checkerClicks.Init(_settings.GridForClick);
            //
            // _previewActiveGrid = _settings.GridForClick;
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
            if (_previewActiveGrid.HasGridInt)
            {
                _previewActiveGrid.GridInt.OnChangeCellValue -= UpdateVisualGrid;
                return;
            }
            
            if (_previewActiveGrid.HasGridSector)
            {
                _previewActiveGrid.GridSector.OnChangeCellValue -= UpdateVisualGrid;
                return;
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
            CheckPreviewActiveGrid();
        }
    }
}