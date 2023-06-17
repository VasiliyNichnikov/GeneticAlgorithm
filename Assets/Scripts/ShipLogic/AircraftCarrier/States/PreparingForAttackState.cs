using StateMachineLogic;

namespace ShipLogic.AircraftCarrier.States
{
    public class PreparingForAttackState : StateBase
    {
        public override string NameState => "Prepare for attack";
        private readonly ICommanderCommander _commanderCommander;


        public PreparingForAttackState(StateMachine machine, ICommanderCommander commanderCommander) : base(machine)
        {
            _commanderCommander = commanderCommander;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            
            if (!_commanderCommander.HasEnemy)
            {
                Machine.ChangeState(_commanderCommander.Movement);
                return;   
            }
            
            if (!_commanderCommander.CanAttackOtherEnemyShip())
            {
                // todo нужно тогда и двигаться к цели
                // _commander.MoveToSelectedPoint();
                _commanderCommander.TurnToEnemyShip();
                return;
            }

            Machine.ChangeState(_commanderCommander.Attack);
        }
    }
}