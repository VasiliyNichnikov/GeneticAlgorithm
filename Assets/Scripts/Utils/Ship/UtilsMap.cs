using Map;
using Planets;
using ShipLogic;

namespace Utils.Ship
{
    public static class UtilsMap
    {
        public enum ShipType
        {
            None,
            Ally,
            Enemy
        }

        public static ShipType TryGetOtherShip(this ShipBase myShip, IObjectOnMap detectedObject,
            out ShipBase otherShip)
        {
            otherShip = null;
            if (detectedObject.TypeObject != MapObjectType.Ship)
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

        public static bool TryGetPlanet(IObjectOnMap detectedObject, out IPlanet planet)
        {
            planet = null;
            if (detectedObject.TypeObject != MapObjectType.Planet)
            {
                return false;
            }

            if (detectedObject is IPlanet detectedPlanet)
            {
                planet = detectedPlanet;
                return true;
            }

            return false;
        }

        private static bool IsAlly(IObjectOnMap enemy, IObjectOnMap ship)
        {
            return enemy.PlayerType == ship.PlayerType;
        }
    }
}