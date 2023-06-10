using StateMachineLogic;

namespace ShipLogic.AircraftCarrier.States
{
    /// <summary>
    /// Нужно повернуться на цель
    /// При этом игнорируем все остальные внешние действия
    /// </summary>
    public class PreparingForJumpState : StateBase
    {
        public override string NameState => "PreparingForJump";
        private readonly AircraftCarrierCommander _commander;
        
        public PreparingForJumpState(StateMachine machine, AircraftCarrierCommander commander) : base(machine)
        {
            _commander = commander;
        }
        
        public override void Enter()
        {
            base.Enter();
            _commander.TurnOnEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (_commander.CompletedTurnToPointOfMovement())
            {
                Machine.ChangeState(_commander.JumpOnBattlefield);
                return;
            }

            _commander.TurnToPointOfMovement();
        }
    }
}