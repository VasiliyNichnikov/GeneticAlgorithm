using System.Collections.Generic;
using System.Data;
using NavigatorShip;
using ShipLogic;
using UnityEngine;

namespace CommandsShip
{
    public class TeamManager
    {
        private static readonly Dictionary<Command.CommandType, List<ShipType>> CommandExecutionPermissions = new()
        {
            { Command.CommandType.EscapeFromBattle, new List<ShipType> { ShipType.Stealth } },
            { Command.CommandType.Help, new List<ShipType> { ShipType.AircraftCarrier }}
        };
        
        public bool IsEscapeFromBattle
        {
            get
            {
                if (_currentCommand == null)
                {
                    return false;
                }

                return _currentCommand.Value.SelectedCommand == Command.CommandType.EscapeFromBattle;
            }
        }

        private readonly Dictionary<Command.CommandType, ICommandLogic> _commandLogics;

        private Command? _currentCommand;

        private readonly IShipCommander _commander;

        public TeamManager(IShipCommander commander)
        {
            _commander = commander;
            _commandLogics = new Dictionary<Command.CommandType, ICommandLogic>
            {
                { Command.CommandType.Movement, new MovementCommand(commander) },
                { Command.CommandType.Help, new HelpCommand() },
                { Command.CommandType.EscapeFromBattle, new EscapeFromBattleCommand(commander) }
            };
        }

        public void GiveCommand(Command command)
        {
            // todo сейчас нет нормального перехода между командами
            if (CanGiveCommand(command))
            {
                _currentCommand = command;
            }
            else
            {
                return;
            }

            if (!_commandLogics.ContainsKey(_currentCommand.Value.SelectedCommand))
            {
                Debug.LogError($"Signal type: {_currentCommand.Value.SelectedCommand} is not in dictionary");
                return;
            }

            _commandLogics[_currentCommand.Value.SelectedCommand].ExecuteRequest(command);
        }

        private bool CanGiveCommand(Command command)
        {
            if (_currentCommand == null)
            {
                return true;
            }

            var checkPriority = _currentCommand.Value.SelectedPriorityType < command.SelectedPriorityType;
            var checkPermission = (CommandExecutionPermissions.ContainsKey(command.SelectedCommand) && 
                                   CommandExecutionPermissions[command.SelectedCommand].Contains(_commander.ShipType)) || 
                                  !CommandExecutionPermissions.ContainsKey(command.SelectedCommand);
            return checkPriority && checkPermission;
        }
    }
}