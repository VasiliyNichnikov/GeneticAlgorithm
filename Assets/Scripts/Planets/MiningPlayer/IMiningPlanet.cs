using System;
using FindingPath;
using Players;
using SpaceObjects;

namespace Planets.MiningPlayer
{
    public interface IMiningPlanet : IObjectOnMap, ITarget
    {
        /// <summary>
        /// Обновление времени сколько осталось до захвата
        /// </summary>
        event Action<float> OnUpdateRemainingTimeCatch;
        /// <summary>
        /// Игрок, который сейчас захватывает точку
        /// </summary>
        event Action<PlayerType> OnUpdatePlayerType;

        /// <summary>
        /// Обновление времени сколько осталось до получения монет заданного игрока
        /// </summary>
        event Action<PlayerType, float> OnUpdateRemainingTimeExtraction;
        
        float CaptureTime { get; }

        void AddFoundShip(IDetectedObject detectedObject);
        void RemoveFoundShip(IDetectedObject detectedObject);
    }
}