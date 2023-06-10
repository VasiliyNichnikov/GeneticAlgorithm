using StateMachineLogic;

namespace ShipLogic.AircraftCarrier.States
{
    /// <summary>
    /// Запускаем таймер обратного отсчета
    /// По завершению таймера переносимся в точку
    /// После телепорта переходим в состояние Idle
    /// </summary>
    public class JumpOnBattlefieldState :StateBase
    {
        public override string NameState => "JumpOnBattleField";

        private readonly AircraftCarrierCommander _commander;
        
        public JumpOnBattlefieldState(StateMachine machine, AircraftCarrierCommander commander) : base(machine)
        {
            _commander = commander;
        }

        public override void Enter()
        {
            base.Enter();
            _commander.StartJumpTimer(OnJumpTimerEnd);
        }

        private void OnJumpTimerEnd()
        {
            _commander.JumpToPointOfMovement();
            Machine.ChangeState(_commander.Idle);
        }
    }
}