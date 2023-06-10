using System;
using Players;
using Pool;

namespace ShipLogic
{
    public class ShipFactory : IDisposable
    {
        private class BuilderShip : IBuilderShip
        {
            public PlayerType PlayerType { get; }
            public ShipData Data { get; }

            private BuilderShip(PlayerType type, ShipData data)
            {
                PlayerType = type;
                Data = data;
            }

            public static BuilderShip CreateForPlayer1(ShipData data)
            {
                return new BuilderShip(PlayerType.Player1, data);
            }

            public static BuilderShip CreateForPlayer2(ShipData data)
            {
                return new BuilderShip(PlayerType.Player2, data);
            }
        }

        public event Action<ShipBase> OnDestroyShip;
        
        private readonly ShipPool _pool = new ();

        public ShipFactory()
        {
            _pool.OnHideShip += CallOnDestroyShip;
        }
        
        public ShipBase AddShipOnMap(PlayerType type, ShipData data)
        {
            return type == PlayerType.Player1 ? AddShipOnMapPlayer1(data) : AddShipOnMapPlayer2(data);
        }
        
        private ShipBase AddShipOnMapPlayer1(ShipData data)
        {
            var ship =  _pool.GetOrCreateShip(data.ShipType);
            var builder = BuilderShip.CreateForPlayer1(data);
            ship.Init(builder);
            return ship;
        }

        private ShipBase AddShipOnMapPlayer2(ShipData data)
        {
            var ship =  _pool.GetOrCreateShip(data.ShipType);
            var builder = BuilderShip.CreateForPlayer2(data);
            ship.Init(builder);
            return ship;
        }

        private void CallOnDestroyShip(ShipBase ship)
        {
            OnDestroyShip?.Invoke(ship);
        }

        public void Dispose()
        {
            _pool.OnHideShip -= CallOnDestroyShip;
        }
    }
}