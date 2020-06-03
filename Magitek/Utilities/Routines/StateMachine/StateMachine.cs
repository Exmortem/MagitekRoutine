using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magitek.Utilities.Routines.StateMachine
{
    public class StateMachine<T>
    {
        private Dictionary<T, State<T>> mStateDict;
        private T mCurrentState;

        public async Task<bool> Pulse()
        {
            List<StateTransition<T>> transitions = mStateDict[mCurrentState].Transitions;
            foreach (StateTransition<T> st in transitions)
            {
                if (await st.TryDoAction())
                {
                    Logger.WriteInfo($"STATE TRANSITION: {mCurrentState} -> {st.NextState}");
                    mCurrentState = st.NextState;
                    return true;
                }
            }

            return false;
        }

        public StateMachine(T startState, Dictionary<T, State<T>> stateDict)
        {
            mCurrentState = startState;
            mStateDict = stateDict;
        }
    }
}
