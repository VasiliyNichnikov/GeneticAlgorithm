using Map;

namespace Planets.PlayerPlanet
{
    public interface IPlayerPlanet : IPlanet
    {
        float CurrentGold { get; }
        void CreateRandomShipDebug();
    }
}