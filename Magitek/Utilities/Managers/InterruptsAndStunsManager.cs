using ff14bot.Enums;
using Magitek.Extensions;
using Magitek.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Managers
{
    internal static class InterruptsAndStunsManager
    {
        public static readonly HashSet<uint> HighPriorityInterrupts = new HashSet<uint>();
        public static readonly HashSet<uint> NormalInterrupts = new HashSet<uint>();
        public static readonly HashSet<uint> HighPriorityStuns = new HashSet<uint>();
        public static readonly HashSet<uint> NormalStuns = new HashSet<uint>();

        public static HashSet<uint> AllInterrupts => HighPriorityInterrupts.Union(NormalInterrupts).ToHashSet();
        public static HashSet<uint> AllStuns => HighPriorityStuns.Union(NormalStuns).ToHashSet();

        public static void Reset()
        {
            if (InterruptsAndStuns.Instance == null)
                return;

            if (InterruptsAndStuns.Instance.ActionList == null || InterruptsAndStuns.Instance.ActionList.Count == 0)
                return;

            HighPriorityInterrupts.Clear();
            NormalInterrupts.Clear();
            HighPriorityStuns.Clear();
            NormalStuns.Clear();

            foreach (var action in InterruptsAndStuns.Instance.ActionList)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (RotationManager.CurrentRotation)
                {
                    case ClassJobType.Bard:
                        if (action.Bard) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.Machinist:
                        if (action.Machinist) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.Scholar:
                        if (action.Scholar) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.Dragoon:
                        if (action.Dragoon) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.Lancer:
                        if (action.Dragoon) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.Paladin:
                        if (action.Paladin) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.Warrior:
                        if (action.Warrior) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.DarkKnight:
                        if (action.DarkKnight) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    case ClassJobType.BlueMage:
                        if (action.BlueMage) { AddAction(action.Id, action.Stun, action.Interrupt, action.HighPriority); }
                        continue;

                    default: continue;
                }
            }

            Logger.Write("Interrupts And Stuns List Reset");

            void AddAction(uint action, bool stun, bool interrupt, bool highPriority)
            {
                if (stun)
                {
                    if (highPriority) { HighPriorityStuns.Add(action); } else { NormalStuns.Add(action); }
                }

                if (!interrupt)
                    return;

                if (highPriority) { HighPriorityInterrupts.Add(action); } else { NormalInterrupts.Add(action); }
            }
        }
    }
}
