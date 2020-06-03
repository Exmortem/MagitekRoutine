using System.Threading.Tasks;

namespace Magitek.Utilities.Routines.StateMachine
{
    public delegate bool ActionTest();
    public delegate Task<bool> ActionExecution();

    public class StateTransition<T>
    {
        private ActionTest ShouldExecuteAction;
        private ActionExecution ExecuteAction;
        public T NextState { get; private set; }

        public StateTransition(ActionTest shouldExecute, ActionExecution execute, T nextState)
        {
            ShouldExecuteAction = shouldExecute;
            ExecuteAction = execute;
            NextState = nextState;
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
