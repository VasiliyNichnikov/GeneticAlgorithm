using StateMachineLogic;
using UnityEngine;

namespace ShipLogic.MainStates
{
    public class AttackState : StateBase
    {
        public override string NameState => "Attack";
        
        private readonly ICommanderCommander _commanderCommander;


        public AttackState(StateMachine machine, ICommanderCommander commanderCommander) : base(machine)
        {
            _commanderCommander = commanderCommander;
        }

        public override void Enter()
        {
            base.Enter();
            _commanderCommander.StartShoot();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();
            
            if (_commanderCommander.NeedEscapeFromBattle())
            {
                Machine.ChangeState(_commanderCommander.EscapeFromBattle);
                return;
            }
            
            if (!_commanderCommander.CanAttackOtherEnemyShip())
            {
                Machine.ChangeState(_commanderCommander.PrepareAttack);
                return;
            }

            _commanderCommander.ShootInEnemy();
        }

        public override void Exit()
        {
            base.Exit();
            _commanderCommander.FinishShoot();
        }
    }
}