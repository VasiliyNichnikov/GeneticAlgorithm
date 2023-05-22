using Players;
using Pool;
using ShipLogic.Individual;

namespace ShipLogic
{
    public class ShipManager
    {
        private class BuilderShip : IBuilderShip
        {
            public PlayerType PlayerType { get; }
            public IShipCommander Commander { get; }
            public ShipData Data { get; }

            private BuilderShip(PlayerType type, IShipCommander commander, ShipData data)
            {
                PlayerType = type;
                Commander = commander;
                Data = data;
            }

            public static BuilderShip CreateForPlayer1(IShipCommander commander, ShipData data)
            {
                return new BuilderShip(PlayerType.Player1, commander, data);
            }

            public static BuilderShip CreateForPlayer2(IShipCommander commander, ShipData data)
            {
                return new BuilderShip(PlayerType.Player2, commander, data);
            }
        }
        
        
        private readonly ShipPool _pool = new ShipPool();

        public ShipBase AddShipOnMap(PlayerType type, ShipData data)
        {
            return type == PlayerType.Player1 ? AddShipOnMapPlayer1(data) : AddShipOnMapPlayer2(data);
        }
        
        public ShipBase AddShipOnMapPlayer1(ShipData data)
        {
            var readyShip = GetReadyShipWithCommander();
            var builder = BuilderShip.CreateForPlayer1(readyShip.commander, data);
            readyShip.ship.Init(builder);
            return readyShip.ship;
        }

        public ShipBase AddShipOnMapPlayer2(ShipData data)
        {
            var readyShip = GetReadyShipWithCommander();
            var builder = BuilderShip.CreateForPlayer2(readyShip.commander, data);
            readyShip.ship.Init(builder);
            return readyShip.ship;
        }

        private (ShipBase ship, IShipCommander commander) GetReadyShipWithCommander()
        {
            var ship = _pool.GetOrCreateShip();
            var commander = new IndividualCommander(ship);
            return (ship, commander);
        }
    }
}