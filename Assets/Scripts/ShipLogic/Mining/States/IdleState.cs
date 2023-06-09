﻿using StateMachineLogic;

namespace ShipLogic.Mining.States
{
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

            if (_commander.HasPointForMovement)
            {
                Machine.ChangeState(_commander.Movement);
            }
        }
    }
}