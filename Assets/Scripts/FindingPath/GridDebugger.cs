using UnityEngine;

namespace FindingPath
{
    public class GridDebugger : MonoBehaviour
    {
        [SerializeField] private int _weight;
        [SerializeField] private int _length;
        [SerializeField] private float _cellSize;
        [SerializeField] private Vector3 _originPosition;
        [SerializeField] private Transform _parentForText;

        [SerializeField] private HeatMapVisual _heatMapVisual;

        private Grid _grid;
        
        private void Start()
        {
            _grid = new Grid(_weight, _length, _cellSize, _originPosition, _parentForText);
            _grid.SetValue(0,0, 40);
            _grid.SetValue(5,0, 20);
            _grid.SetValue(9,4, 100);
            _grid.SetValue(4,4, 65);
            _heatMapVisual.SetGrid(_grid);
        }
    }
}