using UnityEngine;

namespace Map
{
    public class GridPlayerSector : GridBase<float>
    {
        public GridPlayerSector(int width, int length, float cellSize, Vector3 originPosition, Transform parent = null, bool isDebug = false) : base(width, length, cellSize, originPosition, parent, isDebug)
        {
        }

        protected override void SetValue(int x, int z, float value)
        {
            if (!CanAddValue(x, z, value))
            {
                return;
            }
            
            GridArray[x, z] = Mathf.Clamp(value, HeatMapMinValue, HeatMapMaxValue);;
            if (IsDebugMode)
            {
                DebugTextArray[x, z].text = value.ToString();
            }
            
            CallOnChangeCellValue(x, z);
        }

        public override float GetValueFloat(int x, int z)
        {
            return GetValue(x, z);
        }
    }
}