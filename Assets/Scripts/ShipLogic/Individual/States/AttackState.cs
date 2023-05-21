using StateMachineLogic;

namespace ShipLogic.Individual.States
{
    public class AttackState : StateBase
    {
        public override string NameState => "Attack";
        
        private readonly IShipCommander _commander;


        public AttackState(StateMachine machine, IShipCommander commander) : base(machine)
        {
            _commander = commander;
        }

        public override void Enter()
        {
            base.Enter();
            _commander.StartShoot();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            
            if (!_commander.SeeOtherEnemyShip())
            {
                Machine.ChangeState(_commander.PrepareAttack);
                return;
            }

            _commander.ShootInEnemy();
        }

        public override void Exit()
        {
            base.Exit();
            _commander.FinishShoot();
        }
    }
}