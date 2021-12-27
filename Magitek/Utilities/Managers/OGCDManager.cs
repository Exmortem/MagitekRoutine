using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Models.Account;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Managers
{
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

        public static void ManualAdditions()
        {



        }

        public static void RemoveFalsePositives()
        {

            OGCDAbilities.Remove(Spells.Flamethrower); // MCH FlameThrower
            OGCDAbilities.Remove(Spells.PhantomFlurry); // BLU PhantomFlurry

        }

        public static bool CanWeave(SpellData spell, int maxWeaveCount = 2)
        {
            //700 MS = typical animation lock, with Alexander triple weave should be possible
            if (spell.Cooldown.TotalMilliseconds < 700 + BaseSettings.Instance.UserLatencyOffset)
                return false;

            maxWeaveCount -= Casting.SpellCastHistory.FindIndex(
                x => !OGCDAbilities.Where(
                    p => p.JobTypes.Contains(Core.Me.CurrentJob)).Contains(x.Spell));

            return maxWeaveCount > 0;
        }

        public static bool IsWeaveWindow(SpellData spell, int targetWindow = 1, bool timeBased = false)
        {
            //700 MS = typical animation lock, with Alexander triple weave should be possible
            if (spell.Cooldown.TotalMilliseconds < 700 + BaseSettings.Instance.UserLatencyOffset)
                return false;

            targetWindow--;
            targetWindow -= Casting.SpellCastHistory.FindIndex(
                x => !OGCDAbilities.Where(
                    p => p.JobTypes.Contains(Core.Me.CurrentJob)).Contains(x.Spell));

            var targetWindowIsZero = targetWindow == 0;
            var spellFitsInWindow = spell.Cooldown.TotalMilliseconds <= spell.AdjustedCooldown.TotalMilliseconds - (targetWindow * 700 + BaseSettings.Instance.UserLatencyOffset);
            return targetWindowIsZero || (timeBased && spellFitsInWindow);
        }

    }
}
