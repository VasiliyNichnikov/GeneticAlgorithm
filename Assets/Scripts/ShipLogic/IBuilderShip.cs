using Players;

namespace ShipLogic
{
    public interface IBuilderShip
    {
        PlayerType PlayerType { get; }
        IShipCommander Commander { get; }
        ShipData Data { get; }
    }
}