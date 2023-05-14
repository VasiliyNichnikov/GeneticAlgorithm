using UnityEngine;

namespace FindingPath
{
    public interface IObjectOnMap
    {
        /// <summary>
        /// Значение берется из GridCon
        /// </summary>
        MapObjectType TypeObject { get; }
        
        bool IsStatic { get; }
        
        Vector3 LeftBottomPosition { get; }
        Vector3 RightTopPosition { get; }
    }
}