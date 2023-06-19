namespace StateMachineLogic
{
    public abstract class StateBase
    {
        public abstract string NameState { get; }
        protected StateMachine Machine { get; private set; }

        protected StateBase(StateMachine machine)
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