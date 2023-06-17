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
        private readonly ICommanderCommander _commanderCommander;


        public IdleState(StateMachine machine, ICommanderCommander commanderCommander) : base(machine)
        {
            _commanderCommander = commanderCommander;
        }

        public override void Enter()
        {
            base.Enter();
            _commanderCommander.TurnOffEngine();
        }

        public override void UpdateLogic()
        {
            base.UpdateLogic();

            if (_commanderCommander.HasEnemy && _commanderCommander.SeeOtherEnemyShip())
            {
                Machine.ChangeState(_commanderCommander.PrepareAttack);
                return;
            }

            if (_commanderCommander.HasPointForMovement)
            {
                Machine.ChangeState(_commanderCommander.Movement);
            }
        }
    }
}