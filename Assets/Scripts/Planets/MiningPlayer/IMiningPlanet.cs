using System;
using Players;
using SpaceObjects;

namespace Planets.MiningPlayer
{
    public interface IMiningPlanet
    {
        event Action<float> OnUpdateRemainingTime;
        event Action<PlayerType> OnUpdatePlayerType;
        float CaptureTime { get; }

        void AddFoundShip(IDetectedObject detectedObject);
        void RemoveFoundShip(IDetectedObject detectedObject);
    }
}