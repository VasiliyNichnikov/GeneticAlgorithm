using Players;

namespace ShipLogic
{
    public interface IBuilderShip
    {
        PlayerType PlayerType { get; }
        ShipData Data { get; }
    }
}