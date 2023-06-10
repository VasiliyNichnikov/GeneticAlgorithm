using UnityEngine;
using Utils;

namespace Map
{
    public class HeatMapVisual : MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private BoxCollider _collider;
        
        private Mesh _mesh;
        private Grid _grid;

        private void Awake()
        {
            RecreateMesh();
        }

        public void SetGrid(Grid grid)
        {
            _grid = grid;
        }

        public void UpdateHeatMapVisual(bool needRecreateMesh = false)
        {
            if (needRecreateMesh)
            {
                RecreateMesh();
            }
            
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
            _collider.size = new Vector3(_grid.GetWidth() * _grid.GetCellSize(), 1, _grid.GetLength() * _grid.GetCellSize());
        }

        private void RecreateMesh()
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
        }
    }
}