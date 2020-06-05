using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    public class State<T>
    {
        public List<StateTransition<T>> Transitions;

        public State(List<StateTransition<T>> transitions)
        {
            Transitions = transitions;
        }
    }
}
