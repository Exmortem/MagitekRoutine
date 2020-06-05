using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magitek.Utilities.Routines.StateMachine
{
    public class StateMachine<T>
    {
        private Dictionary<T, State<T>> mStateDict;
        private T mCurrentState;
        private T mNextState;

        public void SetState(T state)
        {
            if (state.ToString() != mCurrentState.ToString())
                Logger.WriteInfo("Resetting State Machine");
            mCurrentState = state;
            mNextState = state;
        }
        
        public async Task<bool> Pulse()
        {
            if (Casting.LastSpellSucceeded)
            {
                if (mNextState.ToString() != mCurrentState.ToString())
                {
                    Logger.WriteInfo($"State transition: {mCurrentState} -> {mNextState}");
                }
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
                        if (mNextState.ToString() != mCurrentState.ToString())
                        {
                            Logger.WriteInfo($"State transition: {mCurrentState} -> {mNextState}");
                        }
                        mCurrentState = mNextState;
                        return await Pulse();
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
