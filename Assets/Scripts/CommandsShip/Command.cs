using System;
using JetBrains.Annotations;
using Planets;
using Players;
using UnityEngine;

namespace CommandsShip
{
    public struct Command
    {
        public enum CommandType
        {
            Movement, // Двигаться к точке
            Protection, // Защищать точку
            Help,  // Попросить помощь
            EscapeFromBattle, // Покинуть бой,
            ToHelp // Прийти на помощь
        }

        public enum PriorityType
        {
            Low = 0,
            Medium = 1,
            High = 2
        }
        
        public readonly PlayerType PlayerType;
        [CanBeNull] public readonly ITarget Target;
        public readonly CommandType SelectedCommand;
        public readonly PriorityType SelectedPriorityType;

        public static Command Movement(ITarget pointForMovement)
        {
            return new Command(PlayerType.None, PriorityType.Low, CommandType.Movement, pointForMovement);
        }

        public static Command MovementToHelp(ITarget pointForMovement)
        {
            return new Command(PlayerType.None, PriorityType.High, CommandType.ToHelp, pointForMovement);
        }
        
        public static Command Help(PlayerType playerType, ITarget pointForHelp)
        {
            return new Command(playerType, PriorityType.Medium, CommandType.Help, pointForHelp);
        }

        public static Command EscapeFromBattle(PlayerType playerType)
        {
            Debug.LogWarning($"Escape: {playerType}");
            return new Command(playerType, PriorityType.High, CommandType.EscapeFromBattle, Main.Instance.PlanetStorage.GetPlayerPlanet(playerType));
        }
        
        private Command(PlayerType playerType, PriorityType priority, CommandType command, ITarget target)
        {
            PlayerType = playerType;
            SelectedPriorityType = priority;
            SelectedCommand = command;
            Target = target;
        }
    }
}