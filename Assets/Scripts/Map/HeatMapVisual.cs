using UnityEngine;
using Utils;

namespace Map
{
    public class HeatMapVisual: MonoBehaviour
    {
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private BoxCollider _collider;
        
        private Mesh _mesh;
        
        
        private GridWrapper _grid;

        private void Awake()
        {
            RecreateMesh();
        }

        public void SetGrid(GridWrapper grid)
        {
            _grid = grid;
        }

        public void UpdateHeatMapVisual(bool needRecreateMesh = false)
        {
            if (_grid.HasGridInt)
            {
                UpdateHeatMapVisual(_grid.GridInt, needRecreateMesh);
                return;
            }
            
            if (_grid.HasGridSector)
            {
                UpdateHeatMapVisual(_grid.GridPlayerSector, needRecreateMesh);
                return;
            }
        }
        
        private void UpdateHeatMapVisual<T>(GridBase<T> grid, bool needRecreateMesh = false)
        {
            if (needRecreateMesh)
            {
                RecreateMesh();
            }
            
            MeshUtils.CreateEmptyMeshArrays(
                grid.GetWidth() * grid.GetLength(), 
                out Vector3[] vertices,
                out Vector2[] uv, 
                out int[] triangles);

            for (var x = 0; x < grid.GetWidth(); x++)
            {
                for (var z = 0; z < grid.GetLength(); z++)
                {
                    var index = x * grid.GetLength() + z;
                    var quadSize = new Vector3(1, 0, 1) * grid.GetCellSize();

                    var gridValue = grid.GetValueFloat(x, z);
                    var gridValueNormalized = gridValue / GridInt.HeatMapMaxValue;
                    var gridValueUV = new Vector2(gridValueNormalized, 0);
                    
                    MeshUtils.AddToMeshArrays(
                        vertices, 
                        uv,
                        triangles, 
                        index, 
                        grid.GetWorldPosition(x, z) + quadSize * 0.5f,
                        0f,
                        quadSize, 
                        gridValueUV,
                        gridValueUV);
                }
            }

            _mesh.vertices = vertices;
            _mesh.uv = uv;
            _mesh.triangles = triangles;
            _collider.size = new Vector3(grid.GetWidth() * grid.GetCellSize(), 1, grid.GetLength() * grid.GetCellSize());
        }

        private void RecreateMesh()
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;
        }
    }
}