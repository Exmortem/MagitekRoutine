using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magitek.Utilities.Routines.StateMachine
{
    public class StateMachine<T>
    {
        private Dictionary<T, State<T>> mStateDict;
        private T mCurrentState;
        private T mNextState;

        public async Task<bool> Pulse()
        {
            if (Casting.LastSpellSucceeded)
            {
                mCurrentState = mNextState;
            }

            List<StateTransition<T>> transitions = mStateDict[mCurrentState].Transitions;
            foreach (StateTransition<T> st in transitions)
            {
                if (await st.TryDoAction())
                {
                    mNextState = st.NextState;
                    if (st.ImmediateTransition)
                    {
                        mCurrentState = mNextState;
                    }
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
