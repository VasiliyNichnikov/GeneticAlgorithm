using System;
using Players;
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

        public void SelectedGridToDisplayForSectorPlayer1()
        {
            SelectedGridToDisplayForSector(PlayerType.Player1);
        }
        
        public void SelectedGridToDisplayForSectorPlayer2()
        {
            SelectedGridToDisplayForSector(PlayerType.Player2);
        }
        
        private void SelectedGridToDisplayForSector(PlayerType player)
        {
            CheckHeatMapVisual();
            CheckPreviewActiveGrid();

            var grid = _settings.GetGridWrapperPlayerForSector(player);
            if (!grid.HasGridSector)
            {
                Debug.LogError("Grid for sector is null");
                return;
            }
            
            grid.GridPlayerSector.OnChangeCellValue += UpdateVisualGrid;
            
            _createdHeatMapVisual.SetGrid(grid);
            _createdHeatMapVisual.UpdateHeatMapVisual(true);
            
            _previewActiveGrid = grid;
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
                _previewActiveGrid.GridInt!.OnChangeCellValue -= UpdateVisualGrid;
                return;
            }
            
            if (_previewActiveGrid.HasGridSector)
            {
                _previewActiveGrid.GridPlayerSector!.OnChangeCellValue -= UpdateVisualGrid;
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