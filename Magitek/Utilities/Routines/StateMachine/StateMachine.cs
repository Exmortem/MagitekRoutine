using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Magitek.Utilities.Routines
{
    //This class can be used to implement combat rotations.
    //See StateTransition.cs for details on how the state machine operates.
    //See Logic/RedMage/StateMachine.cs for an example implementation
    public class StateMachine<T> : IStateMachine where T : IComparable
    {
        private Dictionary<T, State<T>> mStateDict;
        private T mNextState;
        private T mDefaultState;
        private TransitionType mTransitionType;

        public T CurrentState { get; private set; }

        public void ResetToDefaultState()
        {
            Logger.WriteInfo("Resetting State Machine");
            CurrentState = mDefaultState;
            mNextState = mDefaultState;
        }

        private void LogStateChange(T current, T next)
        {
            if (current.CompareTo(next) != 0)
            {
                Logger.WriteInfo($"State transition: {CurrentState} -> {mNextState}");
            }
        }

        public async Task<bool> Pulse()
        {
            if (mTransitionType == TransitionType.AfterSpell && Casting.LastSpellSucceeded)
            {
                LogStateChange(CurrentState, mNextState);
                CurrentState = mNextState;
            }
            else if (mTransitionType == TransitionType.NextPulse)
            {
                LogStateChange(CurrentState, mNextState);
                CurrentState = mNextState;
            }

            List<StateTransition<T>> transitions = mStateDict[CurrentState].Transitions;
            foreach (StateTransition<T> st in transitions)
            {
                if (await st.TryDoAction())
                {
                    mNextState = st.NextState;
                    mTransitionType = st.TransitionType;
                    if (st.TransitionType == TransitionType.Immediate)
                    {
                        LogStateChange(CurrentState, mNextState);
                        CurrentState = mNextState;
                        return await Pulse();
                    }
                    return true; 
                }
            }

            return false;
        }

        public StateMachine(T defaultState, Dictionary<T, State<T>> stateDict)
        {
            mDefaultState = defaultState;
            CurrentState = defaultState;
            mStateDict = stateDict;
        }
    }
}
