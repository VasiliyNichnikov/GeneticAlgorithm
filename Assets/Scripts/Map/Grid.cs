using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Map
{
    public class Grid
    {
        public const int HeatMapMaxValue = 100;
        public const int HeatMapMinValue = 0;
        
        private readonly int _width;
        private readonly int _length;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        private readonly int[,] _gridArray;
        private readonly TextMesh[,] _debugTextArray;
        private readonly bool _isDebugMode;

        public event Action<(int x, int z)> OnChangeCellValue; 

        public Grid(int width, int length, float cellSize, Vector3 originPosition, Transform parent=null, bool isDebug=false)
        {
            _width = width;
            _cellSize = cellSize;
            _length = length;
            _originPosition = originPosition;

            _gridArray = new int[_width, _length];
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

        public Vector3 GetWorldPosition(Vector3Int gridPosition)
        {
            return GetWorldPosition(gridPosition.x, gridPosition.z);
        }

        private void GetXZ(Vector3 worldPosition, out int x, out int z)
        {
            x = Mathf.FloorToInt((worldPosition - _originPosition).x / _cellSize);
            z = Mathf.FloorToInt((worldPosition - _originPosition).z / _cellSize);
        }

        public Vector3Int GetXZ(Vector3 worldPosition)
        {
            GetXZ(worldPosition, out var x, out var z);
            return new Vector3Int(x, 0, z);
        }
        
        public void SetValue(int x, int z, int value)
        {
            if (x >= 0 && z >= 0 && x < _width && z < _length)
            {
                _gridArray[x, z] = Mathf.Clamp(value, HeatMapMinValue, HeatMapMaxValue);
                if (_isDebugMode)
                {
                    _debugTextArray[x, z].text = value.ToString();
                }
                OnChangeCellValue?.Invoke((x, z));
            }
        }

        public void SetValue(Vector3 worldPosition, int value)
        {
            GetXZ(worldPosition,out var x, out var z);
            SetValue(x, z, value);
        }

        public int GetValue(int x, int z)
        {
            if (x >= 0 && z >= 0 && x < _width && z < _length)
            {
                return _gridArray[x, z];
            }

            return -1;
        }

        public int GetValue(Vector3 worldPosition)
        {
            GetXZ(worldPosition,out var x, out var z);
            return GetValue(x, z);
        }

        public Vector3[] GetEmptyPointsAroundSelectedObject(Vector3 worldPosition, int emptyValue, int range)
        {
            void CheckAndAddPointOnList(int x, int z, List<Vector3> points)
            {
                var value = GetValue(x, z);
                if (value == emptyValue)
                {
                    var pointWorldPosition = GetWorldPositionCenterCell(new Vector3Int(x, 0, z));
                    points.Add(pointWorldPosition);
                }
            }
            var result = new List<Vector3>();
            
            GetXZ(worldPosition, out var originX, out var originZ);
            for (var x = 0; x < range; x++)
            {
                for (var z = 0; z < range - x; z++)
                {
                    CheckAndAddPointOnList(originX + x, originZ + z, result);
                    if (x != 0)
                    {
                        CheckAndAddPointOnList(originX - x, originZ + z, result);
                    }

                    if (z != 0)
                    {
                        CheckAndAddPointOnList(originX + x, originZ - z, result);
                        if (x != 0)
                        {
                            CheckAndAddPointOnList(originX - x, originZ - z, result);
                        }
                    }
                }
            }

            return result.ToArray();
        }
        
        public void AddValue(Vector3 worldPosition, int value, int range)
        {
            GetXZ(worldPosition, out var originX, out var originZ);
            for (var x = 0; x < range; x++)
            {
                for (var z = 0; z < range - x; z++)
                {
                    AddValue(originX + x, originZ + z, value);
                    if (x != 0)
                    {
                        AddValue(originX - x, originZ + z, value);
                    }

                    if (z != 0)
                    {
                        AddValue(originX + x, originZ - z, value);
                        if (x != 0)
                        {
                            AddValue(originX - x, originZ - z, value);
                        }
                    }
                }
            }
        }
        
        private void AddValue(int x, int z, int value)
        {
            SetValue(x, z, GetValue(x, z) + value);
        }

        /// <summary>
        /// Выделяем все ячейки в выбранном периметре
        /// </summary>
        public void SetValuesAroundPerimeter(Vector3 pointOne, Vector3 pointTwo, int value)
        {
            var pointOneLocal = GetXZ(pointOne);
            var pointTwoLocal = GetXZ(pointTwo);

            var points = GetRightTopAndLeftBottom(pointOneLocal, pointTwoLocal);

            for (var x = points.leftBottom.x; x <= points.rightTop.x; x++)
            {
                for (var z = points.leftBottom.z; z <= points.rightTop.z; z++)
                {
                    var currentValue = GetValue(x, z);
                    if (currentValue != (int)MapObjectType.Empty && value != (int)MapObjectType.Empty)
                    {
                        continue;
                    }
                    SetValue(x, z, value);
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

        public IReadOnlyCollection<Vector3> GetPositionsAroundOfSelectedPoint(Vector3 center, int positionCount)
        {
            var result = new List<Vector3>();
            for (var i = 0; i < positionCount; i++)
            {
                var angle = i * (360f / positionCount);
                var directory = Quaternion.Euler(0, angle, 0) * new Vector3(1, 0);
                var position = center + directory * _cellSize;
                result.Add(position);
            }

            return result;
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
    }
}