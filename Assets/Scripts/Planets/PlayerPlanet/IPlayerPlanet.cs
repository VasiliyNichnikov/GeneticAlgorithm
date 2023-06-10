using Map;

namespace Planets.PlayerPlanet
{
    public interface IPlayerPlanet : IObjectOnMap, ITarget
    {
        float CurrentGold { get; }
        void CreateRandomShipDebug();
    }
}