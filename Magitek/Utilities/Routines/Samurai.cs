using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Samurai;
using System.Collections.Generic;
using System.Linq;
using static ff14bot.Managers.ActionResourceManager.Samurai;

namespace Magitek.Utilities.Routines
{
    internal static class Samurai
    {
        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Samurai, Spells.Hakaze, new List<SpellData>() { Spells.KaeshiGoken, Spells.KaeshiHiganbana, Spells.KaeshiNamikiri, Spells.KaeshiSetsugekka, });


        public static SpellData Fuko => Core.Me.ClassLevel < 86
                                                    ? Spells.Fuga
                                                    : Spells.Fuko;

        public static bool CanContinueComboAfter(SpellData LastSpellExecuted)
        {
            if (ActionManager.ComboTimeLeft <= 0)
                return false;

            if (ActionManager.LastSpell.Id != LastSpellExecuted.Id)
                return false;

            return true;
        }

        public static bool prepareFillerRotation = false;
        public static bool isReadyFillerRotation = false;

        public static void InitializeFillerVar(bool prepareFiller, bool readyFiller)
        {
            if (SamuraiFillerStrategy.None.Equals(SamuraiSettings.Instance.SamuraiFillerStrategy))
            {
                prepareFillerRotation = false;
                isReadyFillerRotation = false;
            } else
            {
                prepareFillerRotation = prepareFiller;
                isReadyFillerRotation = readyFiller;
            }
        }

        public static int SenCount
        {
            get
            {
                var senCount = 0;
                if (Sen.HasFlag(Iaijutsu.Getsu)) senCount++;
                if (Sen.HasFlag(Iaijutsu.Ka)) senCount++;
                if (Sen.HasFlag(Iaijutsu.Setsu)) senCount++;
                return senCount;
            }
        }

        public static Queue<SpellData> CastDuringMeikyo = new Queue<SpellData>();

        public static int EnemiesInCone;
        public static int AoeEnemies5Yards;
        public static int AoeEnemies8Yards;

        public static void RefreshVars()
        {
            if (!Core.Me.InCombat || !Core.Me.HasTarget)
                return;

            EnemiesInCone = Core.Me.EnemiesInCone(8);
            AoeEnemies5Yards = Combat.Enemies.Count(x => x.WithinSpellRange(5) && x.IsTargetable && x.IsValid && !x.HasAnyAura(Auras.Invincibility) && x.NotInvulnerable());
            AoeEnemies8Yards = Combat.Enemies.Count(x => x.WithinSpellRange(8) && x.IsTargetable && x.IsValid && !x.HasAnyAura(Auras.Invincibility) && x.NotInvulnerable());
        }

    }
}
