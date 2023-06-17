using CommandsShip;
using Planets;
using UnityEngine;
using Utils;

namespace ShipLogic
{
    /// <summary>
    /// Информация о корабле необхадимая для выполнения действий
    /// </summary>
    public interface ICommanderInfo
    {
        float HealthPercentages { get; } // Здоровье в процентах 0-1
        bool IsInBattle { get; } // Находится в сражениях
        int NumberOfEnemiesNearby { get; } // Кол-во врагом поблизости (В зоне видимости)
        int NumberOfAlliesNearby { get; }  // Кол-во союзников поблизости (В зоне видимости)
        ITarget ShipTarget { get; }
        Vector3 PositionShip { get; }

        float CalculateWeight();
        void ExecuteCommand(Command command);
    }
}