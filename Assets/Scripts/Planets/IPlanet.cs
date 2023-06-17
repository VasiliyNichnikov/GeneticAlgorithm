using Map;

namespace Planets
{
    public interface IPlanet : ITarget, IObjectOnMap
    {
        PlanetType Type { get; }
    }
}