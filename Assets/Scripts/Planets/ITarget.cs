using System.Collections.Generic;
using UnityEngine;

namespace Planets
{
    public interface ITarget
    {
        /// <summary>
        /// Уровень угрозы на этой точки (от 0 до 1)
        /// </summary>
        float ThreatLevel { get; }

        /// <summary>
        /// Получаем точку для приближения к объекту
        /// </summary>
        Vector3 GetPointToApproximate();
        
        /// <summary>
        /// Находим сектор в котором находится объект
        /// И возвращаем все точки, в которые можно двигаться
        /// </summary>
        IReadOnlyCollection<Vector3> GetPointsInSector(); // todo подумать над именим и сделать чтобы он возвращал ближайшие несколько точек
    }
}