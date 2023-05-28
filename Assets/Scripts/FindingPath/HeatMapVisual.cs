using UnityEngine;
using Utils;

namespace FindingPath
{
    public class HeatMapVisual : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;

        private Mesh _mesh;
        private Grid _grid;

        private void Awake()
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
        }

        public void SetGrid(Grid grid)
        {
            _grid = grid;
        }

        public void UpdateHeatMapVisual()
        {
            MeshUtils.CreateEmptyMeshArrays(
                _grid.GetWidth() * _grid.GetLength(), 
                out Vector3[] vertices,
                out Vector2[] uv, 
                out int[] triangles);

            for (var x = 0; x < _grid.GetWidth(); x++)
            {
                for (var z = 0; z < _grid.GetLength(); z++)
                {
                    var index = x * _grid.GetLength() + z;
                    var quadSize = new Vector3(1, 0, 1) * _grid.GetCellSize();

                    var gridValue = _grid.GetValue(x, z);
                    var gridValueNormalized = (float)gridValue / Grid.HeatMapMaxValue;
                    var gridValueUV = new Vector2(gridValueNormalized, 0);
                    
                    MeshUtils.AddToMeshArrays(
                        vertices, 
                        uv,
                        triangles, 
                        index, 
                        _grid.GetWorldPosition(x, z) + quadSize * 0.5f,
                        0f,
                        quadSize, 
                        gridValueUV,
                        gridValueUV);
                }
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
        }
    }
}