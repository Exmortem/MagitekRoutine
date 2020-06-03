using System.Collections.Generic;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;

namespace Magitek.Utilities.Routines.StateMachine
{
    public class StateMachine<T>
    {
        private Dictionary<T, State<T>> mStateDict;
        private T mCurrentState;
        private T mNextState;

        public void Transition()
        {
            Logger.WriteInfo($"STATE TRANSITION: {mCurrentState} -> {mNextState}");
            mCurrentState = mNextState;
        }

        public async Task<bool> Pulse()
        {
            List<StateTransition<T>> transitions = mStateDict[mCurrentState].Transitions;
            foreach (StateTransition<T> st in transitions)
            {
                if (await st.TryDoAction())
                {
                    mNextState = st.NextState;
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
