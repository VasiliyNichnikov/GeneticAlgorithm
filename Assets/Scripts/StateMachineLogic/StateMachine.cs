namespace StateMachineLogic
{
    public class StateMachine
    {
        public StateBase CurrentState { get; private set; }
        
        public void Init(StateBase startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
        }
        
        public void ChangeState(StateBase state)
        {
            CurrentState.Exit();
            CurrentState = state;
            CurrentState.Enter();
        }
    }
}