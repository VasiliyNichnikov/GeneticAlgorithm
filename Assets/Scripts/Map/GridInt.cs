﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    public class GridInt : GridBase<int>
    {
        public const int HeatMapMaxValue = 100;
        public const int HeatMapMinValue = 0;
        
        public GridInt(int width, int length, float cellSize, Vector3 originPosition, Transform parent = null, bool isDebug = false) : 
            base(width, length, cellSize, originPosition, parent, isDebug)
        {
        }

        public IReadOnlyCollection<Vector3> GetPositionsAroundOfSelectedPoint(Vector3 center, int positionCount)
        {
            var result = new List<Vector3>();
            for (var i = 0; i < positionCount; i++)
            {
                var angle = i * (360f / positionCount);
                var directory = Quaternion.Euler(0, angle, 0) * new Vector3(1, 0);
                var position = center + directory * GetCellSize();
                result.Add(position);
            }

            return result;
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
        
        
        public void SetValuesAroundPerimeter(Vector3 pointOne, Vector3 pointTwo, int value)
        {
            if (value == (int)MapObjectType.Empty)
            {
                return;
            }
            SetValuesAroundPerimeter(pointOne, pointTwo, value, currentValue => currentValue == (int)MapObjectType.Empty);
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

        public override int GetValueInt(int x, int z)
        {
            return GetValue(x, z);
        }
    }
}