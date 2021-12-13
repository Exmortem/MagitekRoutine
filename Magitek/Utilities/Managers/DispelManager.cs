using ff14bot;
using ff14bot.Enums;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.ViewModels;
using System.Collections.Generic;

namespace Magitek.Utilities.Managers
{
    internal static class DispelManager
    {
        public static readonly HashSet<uint> HighPriorityDispels = new HashSet<uint>();
        public static readonly HashSet<uint> NormalDispels = new HashSet<uint>();

        public static void Reset()
        {
            if (!Core.Me.IsHealer() && Core.Me.CurrentJob != ClassJobType.Bard && Core.Me.CurrentJob != ClassJobType.BlueMage)
                return;

            if (Dispelling.Instance == null)
                return;

            if (Dispelling.Instance.StatusList == null || Dispelling.Instance.StatusList.Count == 0)
                return;

            HighPriorityDispels.Clear();
            NormalDispels.Clear();

            foreach (var dispel in Dispelling.Instance.StatusList)
            {
                // ReSharper disable once SwitchStatementMissingSomeCases
                switch (RotationManager.CurrentRotation)
                {
                    case ClassJobType.Conjurer: if (dispel.WhiteMage) { AddDispel(dispel.Id, dispel.HighPriority); } continue;
                    case ClassJobType.Bard: if (dispel.Bard) { AddDispel(dispel.Id, dispel.HighPriority); } continue;
                    case ClassJobType.WhiteMage: if (dispel.WhiteMage) { AddDispel(dispel.Id, dispel.HighPriority); } continue;
                    case ClassJobType.Scholar: if (dispel.Scholar) { AddDispel(dispel.Id, dispel.HighPriority); } continue;
                    case ClassJobType.Astrologian: if (dispel.Astrologian) { AddDispel(dispel.Id, dispel.HighPriority); } continue;
                    case ClassJobType.BlueMage: if (dispel.BlueMage) { AddDispel(dispel.Id, dispel.HighPriority); } continue;
                    default: continue;
                }
            }

            Logger.Write("Dispel List Reset");

            void AddDispel(uint dispel, bool highPriority)
            {
                if (highPriority) { HighPriorityDispels.Add(dispel); } else { NormalDispels.Add(dispel); }
            }
        }

        public static float GetWeight(Character c)
        {
            if (c.IsHealer())
            {
                return 100;
            }

            if (c.IsTank())
            {
                return 90;
            }

            if (c.IsDps())
            {
                return 80;
            }

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }
    }
}
