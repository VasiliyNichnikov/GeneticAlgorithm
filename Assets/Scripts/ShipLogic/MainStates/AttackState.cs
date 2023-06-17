using StateMachineLogic;
using UnityEngine;

namespace ShipLogic.MainStates
{
    public class AttackState : StateBase
    {
        public override string NameState => "Attack";
        
        private readonly ICommander _commander;


        public AttackState(StateMachine machine, ICommander commander) : base(machine)
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
            
            if (_commander.NeedEscapeFromBattle())
            {
                Machine.ChangeState(_commander.EscapeFromBattle);
                return;
            }
            
            if (!_commander.CanAttackOtherEnemyShip())
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