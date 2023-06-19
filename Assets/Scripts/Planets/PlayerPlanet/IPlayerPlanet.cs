using System;
using JetBrains.Annotations;
using Production;
using ShipLogic;

namespace Planets.PlayerPlanet
{
    public interface IPlayerPlanet : IPlanet
    {
        void Init();
        float CurrentGold { get; }
        [CanBeNull] ShipProductionQueue.Production Production { get; }

        void AddExtractedGold(float gold);
        void AddShipToProduction(ShipType type, Action<ShipBase> onCompleteProduction);
        void CreateRandomShipDebug();
    }
}