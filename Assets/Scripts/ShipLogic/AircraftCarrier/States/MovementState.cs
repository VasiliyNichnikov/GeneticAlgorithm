using StateMachineLogic;

namespace ShipLogic.AircraftCarrier.States
{
    /// <summary>
    /// Отвечает за движение корабля
    /// Логика следующая
    /// 1) Если обнаружен враг, переходим в режим подготовки к атаке
    /// 2) Если есть точка для движения следуем к ней, пока корабль не остановится
    /// 3) После остановки, переходим в режим бездействия
    /// </summary>
    public class MovementState : StateBase
    {
        public override string NameState => "Movement";
        private readonly AircraftCarrierCommander _commander;


        public MovementState(StateMachine machine, AircraftCarrierCommander commander) : base(machine)
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

            if (_commander.HasEnemy && _commander.SeeOtherEnemyShip())
            {
                Machine.ChangeState(_commander.PrepareAttack);
                return;
            }

            if (_commander.HasPointForMovement && _commander.NeedJumpInPointForMovement &&
                _commander.CanJumpToPointOfMovement())
            {
                Machine.ChangeState(_commander.PreparingForJump);
                return;
            }


            if (!_commander.HasPointForMovement || _commander.IsNeedStop)
            {
                Machine.ChangeState(_commander.Idle);
                return;
            }

            _commander.MoveToSelectedPoint();
        }
    }
}