using StateMachineLogic;

namespace ShipLogic.MainStates
{
    public class PreparingForAttackState : StateBase
    {
        public override string NameState => "Preparing for attack";

        private readonly ICommanderCommander _commanderCommander;
        
        public PreparingForAttackState(StateMachine machine, ICommanderCommander commanderCommander) : base(machine)
        {
            _commanderCommander = commanderCommander;
        }

        public override void Enter()
        {
            base.Enter();
            _commanderCommander.TurnOnEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (!_commanderCommander.HasEnemy)
            {
                Machine.ChangeState(_commanderCommander.Movement);
                return;
            }

            if (!_commanderCommander.IsDistanceToAttack())
            {
                _commanderCommander.MoveToSelectedPoint();
            }
            
            if (!_commanderCommander.CanAttackOtherEnemyShip())
            {
                _commanderCommander.TurnToEnemyShip();
                return;
            }

            Machine.ChangeState(_commanderCommander.Attack);
        }
    }
}