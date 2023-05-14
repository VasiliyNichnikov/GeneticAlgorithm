namespace StateMachineLogic
{
    public abstract class StateBase
    {
        protected StateMachineLogic.StateMachine Machine { get; private set; }

        protected StateBase(StateMachineLogic.StateMachine machine)
        {
            Machine = machine;
        }
        
        public virtual void Enter()
        {
        }

        public virtual void UpdateLogic()
        {
        }

        public virtual void Exit()
        {
        }
    }
}