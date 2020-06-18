using System.Collections.Generic;

namespace Magitek.Utilities.Routines
{
    class StateMachineManager
    {
        private static List<IStateMachine> mRegisteredStateMachines = new List<IStateMachine>();

        public static void RegisterStateMachine(IStateMachine stateMachine)
        {
            mRegisteredStateMachines.Add(stateMachine);
        }

        public static void UnregisterStatemachine(IStateMachine stateMachine)
        {
            mRegisteredStateMachines.Remove(stateMachine);
        }

        public static void ResetRegisteredStateMachines()
        {
            foreach (IStateMachine stateMachine in mRegisteredStateMachines)
            {
                stateMachine.ResetToDefaultState();
            }
        }
    }
}
