using StateMachineLogic;

namespace ShipLogic.MainStates
{
    /// <summary>
    /// Состояние бездействия
    /// 1) Если находим врага, переходим в режим поиска врага
    /// 2) Если есть точка для движения, переходим в режим движения
    /// </summary>
    public class IdleState : StateBase
    {
        public override string NameState => "Idle";
        private readonly ICommander _commander;


        public IdleState(StateMachine machine, ICommander commander) : base(machine)
        {
            _commander = commander;
        }

        public override void Enter()
        {
            base.Enter();
            _commander.TurnOffEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (_commander.HasEnemy && _commander.SeeOtherEnemyShip())
            {
                Machine.ChangeState(_commander.PrepareAttack);
                return;
            }

            if (_commander.HasPointForMovement)
            {
                Machine.ChangeState(_commander.Movement);
            }
        }
    }
}