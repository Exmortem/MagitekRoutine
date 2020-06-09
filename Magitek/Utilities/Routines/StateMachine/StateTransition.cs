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
