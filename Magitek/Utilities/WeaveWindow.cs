using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities
{

    class WeaveWindow
    {

        private SpellData _gcd;
        private ClassJobType _job;
        private List<SpellData> _ogcds;

        public WeaveWindow(ClassJobType job, SpellData gcd, List<SpellData> removeActions = null, List<SpellData> addActions = null)
        {
            _job = job;
            _gcd = gcd;

            _ogcds = DataManager.SpellCache.Values.Where(
                    x =>
                        (x.IsPlayerAction
                         && x.SpellType == SpellType.Ability
                         && x.JobTypes.Contains(_job))
                        || (x.SpellType == SpellType.System
                            && x.Job == ClassJobType.Adventurer))
                .ToList();

            if (removeActions != null)
                foreach (SpellData rAction in removeActions)
                {
                    RemoveFalsePositives(rAction);
                }

            if (addActions != null)
                foreach (SpellData aAction in addActions)
                {
                    ManualAdditions(aAction);
                }


        }

        public void ManualAdditions(SpellData addAction)
        {

            _ogcds.Add(addAction);

        }

        public void RemoveFalsePositives(SpellData removeAction)
        {
            _ogcds.Remove(removeAction);

        }

        public int CountOGCDs()
        {
            if (_gcd.IsReady(Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset))
                return 0;

            return Casting.SpellCastHistory.FindIndex(x => !_ogcds.Contains(x.Spell));

        }

        public bool CanWeave(int maxWeaveCount = 2)
        {
            if (_gcd.IsReady(Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset))
                return false;

            maxWeaveCount -= Casting.SpellCastHistory.FindIndex(x => !_ogcds.Contains(x.Spell));

            return maxWeaveCount > 0;
        }

        public bool CanWeaveLate(int ogcdPlacement = 1)
        {
            if (!CanWeave(ogcdPlacement))
                return false;

            switch (ogcdPlacement)
            {
                case 1:
                    return (Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset) * 2 >
                           _gcd.Cooldown.TotalMilliseconds;
                case 2:
                    return (Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset) >
                           _gcd.Cooldown.TotalMilliseconds;
                default:
                    return false;
            }

        }

        public bool IsWeaveWindow(int targetWindow = 1, bool timeBased = false)
        {
            //700 MS = typical animation lock, with Alexander triple weave should be possible
            if (_gcd.IsReady(700 + BaseSettings.Instance.UserLatencyOffset))
                return false;

            targetWindow--;
            targetWindow -= Casting.SpellCastHistory.FindIndex(
                x => !_ogcds.Contains(x.Spell));

            bool targetWindowIsZero = targetWindow == 0;
            bool spellFitsInWindow = _gcd.Cooldown.TotalMilliseconds <= _gcd.AdjustedCooldown.TotalMilliseconds -
                (targetWindow * Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset);
            return targetWindowIsZero || (timeBased && spellFitsInWindow);
        }
    }

}
