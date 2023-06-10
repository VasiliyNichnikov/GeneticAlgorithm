using StateMachineLogic;

namespace ShipLogic.AircraftCarrier.States
{
    public class PreparingForAttackState : StateBase
    {
        public override string NameState => "Prepare for attack";
        private readonly IShipCommander _commander;


        public PreparingForAttackState(StateMachine machine, IShipCommander commander) : base(machine)
        {
            _commander = commander;
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            
            if (!_commander.HasEnemy)
            {
                Machine.ChangeState(_commander.Movement);
                return;   
            }
            
            if (!_commander.CanAttackOtherEnemyShip())
            {
                // todo нужно тогда и двигаться к цели
                // _commander.MoveToSelectedPoint();
                _commander.TurnToEnemyShip();
                return;
            }

            Machine.ChangeState(_commander.Attack);
        }
    }
}