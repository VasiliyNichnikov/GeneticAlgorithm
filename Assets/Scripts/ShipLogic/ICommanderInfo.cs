using CommandsShip;
using UnityEngine;

namespace ShipLogic
{
    /// <summary>
    /// Информация о корабле необхадимая для выполнения действий
    /// </summary>
    public interface ICommanderInfo
    {
        ShipType ShipType { get; }
        Vector3 PositionShip { get; }

        float CalculateWeight();
        void ExecuteCommand(Command command);
    }
}