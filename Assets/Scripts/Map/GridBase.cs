using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Map
{
    public abstract class GridBase<T> where T: struct 
    {
        private readonly int _width;
        private readonly int _length;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        private readonly T[,] _gridArray;
        private readonly TextMesh[,] _debugTextArray;
        private readonly bool _isDebugMode;

        public event Action<(int x, int z)> OnChangeCellValue;

        protected GridBase(int width, int length, float cellSize, Vector3 originPosition, Transform parent=null, bool isDebug=false)
        {
            _width = width;
            _cellSize = cellSize;
            _length = length;
            _originPosition = originPosition;

            _gridArray = new T[_width, _length];
            _debugTextArray = new TextMesh[_width, _length];
            _isDebugMode = isDebug;
            
            if (isDebug)
            {
                for (var x = 0; x < _gridArray.GetLength(0); x++)
                {
                    for (var z = 0; z < _gridArray.GetLength(1); z++)
                    {
                        var textPosition = GetWorldPosition(x, z) + new Vector3(_cellSize, 0, _cellSize) * 0.5f;
                        _debugTextArray[x, z] = TextMeshUtils.CreateWorldText(_gridArray[x, z].ToString(),
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

        protected void SetValue(int x, int z, T value)
        {
            if (x >= 0 && z >= 0 && x < _width && z < _length)
            {
                // _gridArray[x, z] = Mathf.Clamp(value, HeatMapMinValue, HeatMapMaxValue);
                _gridArray[x, z] = value;
                if (_isDebugMode)
                {
                    _debugTextArray[x, z].text = value.ToString();
                }
                OnChangeCellValue?.Invoke((x, z));
            }
        }

        public void SetValue(Vector3 worldPosition, T value)
        {
            GetXZ(worldPosition,out var x, out var z);
            SetValue(x, z, value);
        }

        public abstract int GetValueInt(int x, int z);
        
        public T GetValue(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < _width && z < _length)
            {
                return _gridArray[x, z];
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