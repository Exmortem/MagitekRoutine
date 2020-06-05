using System.Threading.Tasks;

namespace Magitek.Utilities.Routines
{
    public delegate bool ActionTest();
    public delegate Task<bool> ActionExecution();

    public class StateTransition<T>
    {
        private ActionTest ShouldExecuteAction;
        private ActionExecution ExecuteAction;
        public T NextState { get; private set; }
        public bool ImmediateTransition { get; private set; }

        public StateTransition(ActionTest shouldExecute, ActionExecution execute, T nextState) : this(shouldExecute, execute, nextState, false) { }

        public StateTransition(ActionTest shouldExecute, ActionExecution execute, T nextState, bool immediateTransition)
        {
            ShouldExecuteAction = shouldExecute;
            ExecuteAction = execute;
            NextState = nextState;
            ImmediateTransition = immediateTransition;
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
