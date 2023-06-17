using System;
using Map;
using Players;

namespace Planets.MiningPlanet
{
    public interface IMiningPlanet : IPlanet
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

        void AddFoundShip(IObjectOnMap detectedObject);
        void RemoveFoundShip(IObjectOnMap detectedObject);
    }
}