using System.Threading.Tasks;

namespace Magitek.Utilities.Routines
{
    public delegate bool ActionTest();
    public delegate Task<bool> ActionExecution();

    public enum TransitionType
    {
        Immediate,
        NextPulse,
        AfterSpell
    }

    //Represents a state transition. Each state transition comprises four main parts:
    //1. A test to determine if the transition should be attempted (ShouldExecuteAction)
    //2. The action to execute before doing the transition (ExecuteAction)
    //3. The state to move to after executing the action (NextState)
    //4. The manner in which the transition should occur (TransitionType)
    //A state consists of, essentially, an ordered list of transitions. The state machine runs through
    //them all in order. If ShouldExecuteAction is true, it will call ExecuteAction. If that succeeds
    //(i.e., returns true), the state machine transitions to the next state.
    //The transition can occur three ways:
    //1. AfterSpell: AfterSpell is the default transition type. When an AfterSpell transition is specified,
    //   the state machine waits until the spell specified in ExecuteAction has finished casting, and then
    //   it transitions to the given state.
    //2. Immediate: Immediate means that the transition occurs immediately after the action is executed.
    //   This is intended for use when no spell is being cast and you want to redirect to another state to
    //   re-evaluate what needs to be done.
    //   IMPORTANT: Using an Immediate transition must be done carefully. The state machine handles
    //   immediate transitions recursively, so if you set up a scenario where states keep immediately
    //   transitioning back and forth between each other, you'll overflow the stack and crash
    //3. NextPulse: This is similar to the Immediate transition type, but it's non-recursive. Instead, it
    //   executes the transition at the next pulse from the botbase. This incurs a slight delay but
    //   removes the element of recursion
    //See Logic/RedMage/StateMachine.cs for an example of this in action.
    public class StateTransition<T>
    {
        private ActionTest ShouldExecuteAction;
        private ActionExecution ExecuteAction;
        public T NextState { get; private set; }
        public TransitionType TransitionType { get; private set; }

        public StateTransition(ActionTest shouldExecute, ActionExecution execute, T nextState) : this(shouldExecute, execute, nextState, TransitionType.AfterSpell) { }

        public StateTransition(ActionTest shouldExecute, ActionExecution execute, T nextState, TransitionType transitionType)
        {
            ShouldExecuteAction = shouldExecute;
            ExecuteAction = execute;
            NextState = nextState;
            TransitionType = transitionType;
        }

        public async Task<bool> TryDoAction()
        {
            if (ShouldExecuteAction())
            {
                return await ExecuteAction();
            }
            return false;
        }
    }
}
