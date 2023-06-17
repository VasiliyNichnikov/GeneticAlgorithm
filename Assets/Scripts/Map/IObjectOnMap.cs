using Players;
using UnityEngine;
using Utils;

namespace Map
{
    public interface IObjectOnMap : IPosition
    {
        /// <summary>
        /// Значение берется из GridCon
        /// </summary>
        MapObjectType TypeObject { get; }
        PlayerType PlayerType { get; }
        
        bool IsStatic { get; }
        
        Vector3 LeftBottomPosition { get; }
        Vector3 RightTopPosition { get; }
    }
}