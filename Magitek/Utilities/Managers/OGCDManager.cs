using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Managers
{

    class OgcdManager
    {

        private SpellData _gcd;
        private ClassJobType _job;
        private List<SpellData> _ogcds;

        public OgcdManager(ClassJobType job, SpellData gcd, List<SpellData> removeActions = null, List<SpellData> addActions = null)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="targetWindow"></param>
        /// <param name="timeBased"></param>
        /// <returns></returns>
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

    /*
    internal static class OGCDManager
    {
        public static List<ClassJobType> CombatJobs = new List<ClassJobType>()
        {
            //Melee
            ClassJobType.Pugilist, ClassJobType.Monk,
            ClassJobType.Lancer, ClassJobType.Dragoon,
            ClassJobType.Rogue, ClassJobType.Ninja,
            ClassJobType.Samurai,
            ClassJobType.Reaper,

            //Physical Range
            ClassJobType.Archer, ClassJobType.Bard,
            ClassJobType.Machinist,
            ClassJobType.Dancer,

            //Magical Range
            ClassJobType.Arcanist, ClassJobType.Summoner,
            ClassJobType.Thaumaturge, ClassJobType.BlackMage,
            ClassJobType.RedMage,
            ClassJobType.BlueMage,

            //Tank
            ClassJobType.Gladiator, ClassJobType.Paladin,
            ClassJobType.Marauder, ClassJobType.Warrior,
            ClassJobType.DarkKnight,
            ClassJobType.Gunbreaker,

            //Healer
            ClassJobType.Conjurer, ClassJobType.WhiteMage,
            ClassJobType.Scholar,
            ClassJobType.Astrologian,
            ClassJobType.Sage
        };

        public static List<SpellData> OGCDAbilities;

        static OGCDManager()
        {
            OGCDAbilities = DataManager.SpellCache.Values.Where(
                x =>
                (x.IsPlayerAction
                    && x.SpellType == SpellType.Ability
                    && x.JobTypes.Any(CombatJobs.Contains))
                || (x.SpellType == SpellType.System
                    && x.Job == ClassJobType.Adventurer))
                .ToList();

            RemoveFalsePositives();
            ManualAdditions();
        }

        /// <summary>
        /// Some Actions that are labeled as "WeaponSkills", but wont trigger a GCD.
        /// This function is to add those to be recognized as OGCDs
        /// </summary>
        public static void ManualAdditions()
        {



        }

        /// <summary>
        /// Some Actions are labeled as an "Ability", but will trigger a GCD.
        /// This function is to remove those false positive oGCDs.
        /// </summary>
        public static void RemoveFalsePositives()
        {

            OGCDAbilities.Remove(Spells.Flamethrower); // MCH FlameThrower
            OGCDAbilities.Remove(Spells.PhantomFlurry); // BLU PhantomFlurry

        }

        /// <summary>
        /// Checks for running GCD and oGCD possibilities
        /// </summary>
        /// <param name="spell">SpellData for a typical 123 GCD Action like Slice from RPR</param>
        /// <param name="maxWeaveCount">Determines how many oGCD-Weaves should be allowed during one GCD</param>
        /// <returns>true = acceptable to weave, false = we shouldn't try to weave</returns>
        public static bool CanWeave(SpellData spell, int maxWeaveCount = 2)
        {
            //700 MS = typical animation lock, with Alexander triple weave should be possible
            if (spell.IsReady(700 + BaseSettings.Instance.UserLatencyOffset))
                return false;

            maxWeaveCount -= Casting.SpellCastHistory.FindIndex(
                x => !OGCDAbilities.Where(
                    p => p.JobTypes.Contains(Core.Me.CurrentJob)).Contains(x.Spell));

            return maxWeaveCount > 0;
        }

        /// <summary>
        /// Simple function to get the amount of oGCDs used during current GCD
        /// </summary>
        /// <param name="spell">SpellData for a typical 123 GCD Action like Slice from RPR</param>
        /// <returns>Amount of oGCDs used since the last GCD was used</returns>
        public static int CountOGCDs(this SpellData spell)
        {
            if (spell.IsReady(700 + BaseSettings.Instance.UserLatencyOffset))
                return 0;

            return Casting.SpellCastHistory.FindIndex(
                x => !OGCDAbilities.Where(
                    p => p.JobTypes.Contains(Core.Me.CurrentJob)).Contains(x.Spell));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell"></param>
        /// <param name="targetWindow"></param>
        /// <param name="timeBased"></param>
        /// <returns></returns>
        public static bool IsWeaveWindow(this SpellData spell, int targetWindow = 1, bool timeBased = false)
        {
            //700 MS = typical animation lock, with Alexander triple weave should be possible
            if (spell.IsReady(700 + BaseSettings.Instance.UserLatencyOffset))
                return false;

            targetWindow--;
            targetWindow -= Casting.SpellCastHistory.FindIndex(
                x => !OGCDAbilities.Where(
                    p => p.JobTypes.Contains(Core.Me.CurrentJob)).Contains(x.Spell));

            bool targetWindowIsZero = targetWindow == 0;
            bool spellFitsInWindow = spell.Cooldown.TotalMilliseconds <= spell.AdjustedCooldown.TotalMilliseconds - (targetWindow * 700 + BaseSettings.Instance.UserLatencyOffset);
            return targetWindowIsZero || (timeBased && spellFitsInWindow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spell">SpellData for a typical 123 GCD Action like Slice from RPR</param>
        /// <param name="ogcdPlacement"></param>
        /// <returns></returns>
        public static bool CanWeaveLate(this SpellData spell, int ogcdPlacement = 1)
        {
            if (!CanWeave(spell, ogcdPlacement))
                return false;

            switch (ogcdPlacement)
            {
                case 1:
                    return (Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset) * 2 > spell.Cooldown.TotalMilliseconds;
                case 2:
                    return (Globals.AnimationLockMs + BaseSettings.Instance.UserLatencyOffset) > spell.Cooldown.TotalMilliseconds;
                default:
                    return false;
            }

        }

    }
    */
    }
