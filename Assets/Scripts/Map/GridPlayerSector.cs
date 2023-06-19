using System.Collections.Generic;
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
        
        public Vector3Int[] GetAllSectors()
        {
            var result = new List<Vector3Int>();
            for (var x = 0; x < GridArray.GetLength(0); x++)
            {
                for (var z = 0; z < GridArray.GetLength(1); z++)
                {
                    result.Add(new Vector3Int(x, 0, z));
                }
            }

            return result.ToArray();
        }
        
        /// <summary>
        /// Возвращает локальные позиции секторов вокруг выбранного объекта
        /// </summary>
        public Vector3Int[] GetSectorsAroundSelectedObject(Vector3 worldPosition, int range)
        {
            void CheckAndAddPointOnList(int x, int z, List<Vector3Int> points)
            {
                if (x >= 0 && z >= 0 && x < GetWidth() && z < GetLength())
                {
                    points.Add(new Vector3Int(x, 0, z));
                }
            }

            return GetPointsAroundSelectedObject<Vector3Int>(worldPosition, range, CheckAndAddPointOnList);
        }

        public override float GetValueFloat(int x, int z)
        {
            return GetValue(x, z);
        }
    }
}