using ShipLogic;
using SpaceObjects;
using UnityEngine;

namespace Utils.Ship
{
    public static class UtilsEnemy
    {
        public enum ShipType
        {
            None,
            Ally,
            Enemy
        }
        
        public static ShipType TryGetOtherShip(this ShipBase myShip, IDetectedObject detectedObject, out ShipBase otherShip)
        {
            otherShip = null;
            if (detectedObject.ObjectType != DetectedObjectType.Ship)
            {
                return ShipType.None;
            }

            if (detectedObject is not ShipBase ship)
            {
                return ShipType.None;
            }

            // Найден союзник
            if (IsAlly(myShip, ship))
            {
                otherShip = ship;
                return ShipType.Ally;
            }

            // Корабль мертвый
            // Эта херня все ломает
            // Из-за нее мертвые корабли не могут быть удалены из списка
            /*if (shipEnemy.IsDead)
            {
                return false;
            }*/

            otherShip = ship;
            return ShipType.Enemy;
        }
        
        private static bool IsAlly(IDetectedObject enemy, IDetectedObject ship)
        {
            return enemy.PlayerType == ship.PlayerType;
        }
    }
}