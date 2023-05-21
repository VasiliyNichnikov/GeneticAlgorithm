﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace FindingPath
{
    public class Grid
    {
        private readonly int _width;
        private readonly int _length;
        private readonly float _cellSize;
        private readonly Vector3 _originPosition;
        private readonly int[,] _gridArray;
        private readonly TextMesh[,] _debugTextArray;

        public Grid(int width, int length, float cellSize, Vector3 originPosition, Transform parent=null)
        {
            _width = width;
            _cellSize = cellSize;
            _length = length;
            _originPosition = originPosition;

            _gridArray = new int[_width, _length];
            _debugTextArray = new TextMesh[_width, _length];
            
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

            SetValue(4, 5, 14);
        }

        private Vector3 GetWorldPosition(int x, int z)
        {
            return new Vector3(x, 0, z) * _cellSize + _originPosition;
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
                _gridArray[x, z] = value;
                _debugTextArray[x, z].text = value.ToString();
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

        /// <summary>
        /// Проверка идет с точками на гриде
        /// todo нужно доработать и проверить
        /// </summary>
        public bool CheckPointGridInPerimeter(Vector3Int a, Vector3Int b, Vector3Int pointToCheck)
        {
            return (pointToCheck.x - a.x) * (pointToCheck.x - b.x) <= 0 && (pointToCheck.y - a.y) * (pointToCheck.y - b.y) <= 0;
        }
        
        /// <summary>
        /// Выделяем все ячейки в выбранном периметре
        /// </summary>
        public void SetValuesAroundPerimeter(Vector3 pointOne, Vector3 pointTwo, int value)
        {
            var pointOneLocal = GetXZ(pointOne);
            var pointTwoLocal = GetXZ(pointTwo);

            var points = GetRightTopAndLeftBottom(pointOneLocal, pointTwoLocal);
            
            /*for (var x = startX; x <= endX; x++)
            {
                for (var z = startZ; z <= endZ; z++)
                {
                    var currentValue = GetValue(x, z);
                    if (currentValue != (int)MapObjectType.Empty && value != (int)MapObjectType.Empty)
                    {
                        continue;
                    }
                    SetValue(x, z, value);
                }
            }*/
            
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