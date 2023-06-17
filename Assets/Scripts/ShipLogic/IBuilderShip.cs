using Players;

namespace ShipLogic
{
    public interface IBuilderShip
    {
        ICommander Commander { get; }
        PlayerType PlayerType { get; }
        ShipData Data { get; }
    }
}