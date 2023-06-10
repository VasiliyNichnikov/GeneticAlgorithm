using UnityEngine;

namespace Map
{
    public class GridSector : GridBase<Sector>
    {
        public GridSector(int width, int length, float cellSize, Vector3 originPosition, Transform parent = null, bool isDebug = false) : base(width, length, cellSize, originPosition, parent, isDebug)
        {
        }

        public void SetValuesAroundPerimeter(Vector3 pointOne, Vector3 pointTwo, Sector value)
        {
            
        }

        public override int GetValueInt(int x, int z)
        {
            return GetValue(x, z).Value;
        }
    }
}