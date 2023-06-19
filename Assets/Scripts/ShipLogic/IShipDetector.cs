using System;
using System.Collections.Generic;
using Planets;

namespace ShipLogic
{
    public interface IShipDetector : IDisposable
    {
        float Radius { get; }
        
        IReadOnlyCollection<ShipBase> Enemies { get; }
        IReadOnlyCollection<ShipBase> Allies { get; }
        IReadOnlyCollection<IPlanet> Planets { get; }

        void TryRemoveFoundShip(ShipBase enemy);
    }
}