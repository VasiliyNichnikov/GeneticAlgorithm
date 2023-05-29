using FindingPath;

namespace Planets.PlayerPlanet
{
    public interface IPlayerPlanet : IObjectOnMap
    {
        float CurrentGold { get; }
        void CreateRandomShipDebug();
    }
}