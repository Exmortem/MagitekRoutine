using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    //Represents a state. Each state consists simply of an ordered list of state transitions.
    //See StateTransition.cs for details on how the state machine operates.
    //See Logic/RedMage/StateMachine.cs for an example of this in action.
    public class State<T>
    {
        public List<StateTransition<T>> Transitions;

        public State(List<StateTransition<T>> transitions)
        {
            Transitions = transitions;
        }
    }
}
