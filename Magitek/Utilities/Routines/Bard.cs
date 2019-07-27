using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Bard;

namespace Magitek.Utilities.Routines
{
    internal static class Bard
    {
        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;
        public static int SnapShotCheck = 0;

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(15);
            AoeEnemies5Yards = Core.Me.CurrentTarget.EnemiesNearby(5).Count();
            AoeEnemies8Yards = Core.Me.CurrentTarget.EnemiesNearby(8).Count();
        }
        
        public static List<uint> DotsList => Core.Me.ClassLevel >= 64 ?
            new List<uint>() { Auras.StormBite, Auras.CausticBite } :
            new List<uint>() { Auras.Windbite, Auras.VenomousBite };

        public static List<SpellData> OGCDSpells = new List<SpellData>() {Spells.RagingStrikes, Spells.Barrage, Spells.PitchPerfect,
                                                                            Spells.Bloodletter, Spells.EmpyrealArrow, Spells.Sidewinder,
                                                                            Spells.RainofDeath, Spells.Shadowbite, Spells.TheWanderersMinuet,
                                                                            Spells.MagesBallad, Spells.ArmysPaeon, Spells.Troubadour,
                                                                            Spells.HeadGraze, Spells.NaturesMinne, Spells.TheWardensPaean
                                                                            };

        public static int CheckLastSpellsForWeaveing()
        {
            int weaveingCounter = 0;

            if (Casting.SpellCastHistory.Count < 2)
                return 0;

            SpellData lastSpellCast = Casting.SpellCastHistory.ElementAt(0).Spell;
            SpellData secondLastSpellCast = Casting.SpellCastHistory.ElementAt(1).Spell;

            if (OGCDSpells.Contains(lastSpellCast))
                weaveingCounter += 1;
            else
                return 0;

            if (OGCDSpells.Contains(secondLastSpellCast))
                weaveingCounter += 1;

            return weaveingCounter;
        }
        public static bool HealthCheck(GameObject tar)
        {
            if (tar == null)
                return false;

            if (tar.EnglishName.Contains("Dummy"))
                return true;

            if (BardSettings.Instance.DontDotIfEnemyIsDyingSoon)
            {
                // Target doesn't have a combat time left yet
                if (Combat.CurrentTargetCombatTimeLeft < 0)
                    return true;

                return Combat.CurrentTargetCombatTimeLeft > BardSettings.Instance.DontDotIfEnemyIsDyingWithinXSeconds;
            }

            if (tar.IsBoss())
                return true;

            return tar.CurrentHealthPercent > 20;
        }
    }
}