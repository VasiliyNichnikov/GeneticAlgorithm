using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Map
{
    public abstract class GridBase<T>
    {
        public const int HeatMapMaxValue = 100;
        public const int HeatMapMinValue = 0;
        
        private readonly int _width;
        private readonly int _length;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        
        protected readonly TextMesh[,] DebugTextArray;
        protected readonly bool IsDebugMode;
        protected readonly T[,] GridArray;

        public event Action<(int x, int z)> OnChangeCellValue;

        protected GridBase(int width, int length, float cellSize, Vector3 originPosition, Transform parent=null, bool isDebug=false)
        {
            _width = width;
            _cellSize = cellSize;
            _length = length;
            _originPosition = originPosition;

            GridArray = new T[_width, _length];
            DebugTextArray = new TextMesh[_width, _length];
            IsDebugMode = isDebug;
            
            if (isDebug)
            {
                for (var x = 0; x < GridArray.GetLength(0); x++)
                {
                    for (var z = 0; z < GridArray.GetLength(1); z++)
                    {
                        var textPosition = GetWorldPosition(x, z) + new Vector3(_cellSize, 0, _cellSize) * 0.5f;
                        DebugTextArray[x, z] = TextMeshUtils.CreateWorldText(GridArray[x, z].ToString(),
                            parent, 
                            textPosition,
                            20, Color.
                                white);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x, z + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, z), GetWorldPosition(x + 1, z), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, _length), GetWorldPosition(_width, _length), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _length), Color.white, 100f);
            }
        }

        public int GetWidth()
        {
            return _width;
        }

        public int GetLength()
        {
            return _length;
        }

        public float GetCellSize()
        {
            return _cellSize;
        }

        public Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * _cellSize + _originPosition;
        }

        public Vector3 GetWorldPositionCenterCell(Vector3Int gridPosition)
        {
            var cellWorldPosition = GetWorldPosition(gridPosition);
            return cellWorldPosition + new Vector3(1, 0, 1) * _cellSize * 0.5f;
        }

        private Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            return GetWorldPosition(gridPosition.x, gridPosition.z);
        }

        public void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            z = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
        }

        public Vector3Int GetXZ(Vector3 worldPosition)
        {
            GetXZ(worldPosition, out var x, out var z);
            return new Vector3Int(x, 0, z);
        }

        protected bool CanAddValue(int x, int z, T value)
        {
            return x >= 0 && z >= 0 && x < _width && z < _length;
        }

        protected abstract void SetValue(int x, int z, T value);

        public void SetValue(Vector3 worldPosition, T value)
        {
            GetXZ(worldPosition,out var x, out var z);
            SetValue(x, z, value);
        }

        public void SetValue(Vector3Int gridPosition, T value)
        {
            SetValue(gridPosition.x, gridPosition.z, value);
        }

        public abstract float GetValueFloat(int x, int z);
        
        public T GetValue(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < _width && z < _length)
            {
                return GridArray[x, z];
            }

            return default(T);
        }
        
        /// <summary>
        /// Выделяем все ячейки в выбранном периметре
        /// </summary>
        protected void SetValuesAroundPerimeter(Vector3 pointOne, Vector3 pointTwo, T value, Func<T, bool> additionalCheck)
        {
            var pointOneLocal = GetXZ(pointOne);
            var pointTwoLocal = GetXZ(pointTwo);

            var points = GetRightTopAndLeftBottom(pointOneLocal, pointTwoLocal);

            for (var x = points.leftBottom.x; x <= points.rightTop.x; x++)
            {
                for (var z = points.leftBottom.z; z <= points.rightTop.z; z++)
                {
                    var currentValue = GetValue(x, z);
                    if (additionalCheck(currentValue))
                    {
                        SetValue(x, z, value);
                    }
                }
            }
        }

        public IReadOnlyCollection<Vector3Int> GetPositionsOfCellsAroundPerimeter(Vector3 pointOne, Vector3 pointTwo)
        {
            var positionsOnGrid = new List<Vector3Int>();
            var pointOneLocal = GetXZ(pointOne);
            var pointTwoLocal = GetXZ(pointTwo);

            var points = GetRightTopAndLeftBottom(pointOneLocal, pointTwoLocal);
            
            for (var x = points.leftBottom.x; x <= points.rightTop.x; x++)
            {
                for (var z = points.leftBottom.z; z <= points.rightTop.z; z++)
                {
                    positionsOnGrid.Add(new Vector3Int(x, 0, z));
                }
            }

            return positionsOnGrid;
        }

        protected void CallOnChangeCellValue(int x, int z)
        {
            OnChangeCellValue?.Invoke((x, z));
        }
        
        protected TPoint[] GetPointsAroundSelectedObject<TPoint>(Vector3 worldPosition, int range, Action<int, int, List<TPoint>> checkAndAndPointOnList) where TPoint: struct
        {
            var result = new List<TPoint>();
            
            GetXZ(worldPosition, out var originX, out var originZ);
            for (var x = 0; x < range; x++)
            {
                for (var z = 0; z < range - x; z++)
                {
                    checkAndAndPointOnList(originX + x, originZ + z, result);
                    if (x != 0)
                    {
                        checkAndAndPointOnList(originX - x, originZ + z, result);
                    }

                    if (z != 0)
                    {
                        checkAndAndPointOnList(originX + x, originZ - z, result);
                        if (x != 0)
                        {
                            checkAndAndPointOnList(originX - x, originZ - z, result);
                        }
                    }
                }
            }

            return result.ToArray();
        }
        
        private (Vector3Int leftBottom, Vector3Int rightTop) GetRightTopAndLeftBottom(Vector3Int pointOne, Vector3Int pointTwo)
        {
            const int y = 0;
            
            int startX, endX;
            if (pointOne.x > pointTwo.x)
            {
                startX = pointTwo.x;
                endX = pointOne.x;
            }
            else
            {
                startX = pointOne.x;
                endX = pointTwo.x;
            }
            
            int startZ, endZ;
            if (pointOne.z > pointTwo.z)
            {
                startZ = pointTwo.z;
                endZ = pointOne.z;
            }
            else
            {
                startZ = pointOne.z;
                endZ = pointTwo.z;
            }

            return (new Vector3Int(startX, y, startZ), new Vector3Int(endX, y, endZ));
        }

        public T GetValue(Vector3 worldPosition)
        {
            GetXZ(worldPosition,out var x, out var z);
            return GetValue(x, z);
        }
    }
}