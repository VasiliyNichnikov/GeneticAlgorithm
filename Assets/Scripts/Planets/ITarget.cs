using UnityEngine;

namespace Planets
{
    public interface ITarget
    {
        /// <summary>
        /// Получаем точку для приближения к объекту
        /// </summary>
        Vector3 GetPointToApproximate();
    }
}