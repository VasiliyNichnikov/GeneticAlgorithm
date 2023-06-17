using StateMachineLogic;

namespace ShipLogic.MainStates
{
    public class PreparingForAttackState : StateBase
    {
        public override string NameState => "Preparing for attack";

        private readonly ICommander _commander;
        
        public PreparingForAttackState(StateMachine machine, ICommander commander) : base(machine)
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

            if (!_commander.HasEnemy)
            {
                Machine.ChangeState(_commander.Movement);
                return;
            }

            if (!_commander.IsDistanceToAttack())
            {
                _commander.MoveToSelectedPoint();
            }
            
            if (!_commander.CanAttackOtherEnemyShip())
            {
                _commander.TurnToEnemyShip();
                return;
            }

            Machine.ChangeState(_commander.Attack);
        }
    }
}