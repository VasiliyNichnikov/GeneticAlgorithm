using System;
using System.Collections.Generic;
using Map;
using Planets;

namespace ShipLogic
{
    public interface IShipDetector : IDisposable
    {
        IReadOnlyCollection<ShipBase> Enemies { get; }
        IReadOnlyCollection<ShipBase> Allies { get; }
        IReadOnlyCollection<IPlanet> Planets { get; }

        void TryRemoveFoundShip(IObjectOnMap enemy);
    }
}