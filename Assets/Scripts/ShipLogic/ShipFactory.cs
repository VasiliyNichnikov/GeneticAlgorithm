using System;
using Players;
using Pool;
using ShipLogic.AircraftCarrier;
using ShipLogic.Fighter;
using ShipLogic.Mining;
using ShipLogic.Stealth;

namespace ShipLogic
{
    public class ShipFactory : IDisposable
    {
        private class BuilderShip : IBuilderShip
        {
            public ICommander Commander { get; }
            public PlayerType PlayerType { get; }
            public ShipData Data { get; }

            private BuilderShip(PlayerType type, ICommander commander, ShipData data)
            {
                PlayerType = type;
                Commander = commander;
                Data = data;
            }

            public static BuilderShip CreateForPlayer1(ICommander commander, ShipData data)
            {
                return new BuilderShip(PlayerType.Player1, commander, data);
            }

            public static BuilderShip CreateForPlayer2(ICommander commander, ShipData data)
            {
                return new BuilderShip(PlayerType.Player2, commander, data);
            }
        }

        public event Action<ShipBase> OnDestroyShip;

        private readonly ShipPool _pool = new();

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
            var ship = _pool.GetOrCreateShip(data.ShipType);
            var commander = GetCommander(ship, data.ShipType);
            var builder = BuilderShip.CreateForPlayer1(commander, data);
            ship.Init(builder);
            return ship;
        }

        private ShipBase AddShipOnMapPlayer2(ShipData data)
        {
            var ship = _pool.GetOrCreateShip(data.ShipType);
            var commander = GetCommander(ship, data.ShipType);
            var builder = BuilderShip.CreateForPlayer2(commander, data);
            ship.Init(builder);
            return ship;
        }

        private ICommander GetCommander(ShipBase ship, ShipType shipType)
        {
            switch (shipType)
            {
                case ShipType.Stealth:
                    return new StealthCommander(ship);
                case ShipType.Fighter:
                    return new FighterCommander(ship);
                case ShipType.AircraftCarrier:
                    return new AircraftCarrierCommander(ship);
                case ShipType.Mining:
                    return new MiningCommander(ship);
                default:
                    throw new ArgumentOutOfRangeException();
            }
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