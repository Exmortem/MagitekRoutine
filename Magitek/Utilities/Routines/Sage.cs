using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Account;
using Magitek.Models.Sage;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class Sage
    {
        public static bool OnGcd => Spells.Dosis.Cooldown.TotalMilliseconds > 100;

        public static HashSet<string> DontShield = new HashSet<string>();
        public static HashSet<string> DontEukrasianDiagnosis = new HashSet<string>();
        public static HashSet<string> DontEukrasianPrognosis = new HashSet<string>();

        public static List<Character> AllianceDiagnosisOnly = new List<Character>();

        public static WeaveWindow GlobalCooldown = new WeaveWindow(ClassJobType.Sage, Spells.Diagnosis);

        public static bool CanWeave()
        {
            if (SageSettings.Instance.WeaveOGCDHeals
                && Core.Me.CurrentMana >= SageSettings.Instance.WeaveOGCDHealsManaPercent)
            {
                if (GlobalCooldown.CanWeave(1))
                    return true;
                else if (Casting.LastSpellTimeFinishAge.ElapsedMilliseconds > 1750 + BaseSettings.Instance.UserLatencyOffset)
                    return true;
            }
            else
            {
                if (Casting.LastSpellTimeFinishAge.ElapsedMilliseconds > 750 + BaseSettings.Instance.UserLatencyOffset)
                    return true;
            }

            return false;
        }

        public static bool NeedToInterruptCast()
        {
            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (Casting.CastingSpell != Spells.Egeiro && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped cast: Unit Died");
                return true;
            }

            if (Casting.CastingSpell == Spells.Egeiro && (Casting.SpellTarget?.HasAura(Auras.Raise) == true || Casting.SpellTarget?.CurrentHealth > 0))
            {
                Logger.Error($@"Stopped Resurrection: Unit has raise aura");
                return true;
            }

            if (SageSettings.Instance.InterruptHealing && Casting.DoHealthChecks &&
                Casting.SpellTarget?.CurrentHealthPercent >= SageSettings.Instance.InterruptHealingHealthPercent)
            {
                if (Casting.CastingSpell == Spells.Prognosis && PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Count(r =>
                        r.CurrentHealth > 0 && r.Distance(Core.Me) <= Spells.Prognosis.Radius && r.CurrentHealthPercent <=
                        SageSettings.Instance.PrognosisHpPercent) <
                    Logic.Sage.Heal.AoeNeedHealing)
                {
                    Logger.Error($@"Stopped Healing: Party's Health Too High");
                    return true;
                }
                else
                {
                    Logger.Error($@"Stopped Healing: Target's Health Too High");
                    return true;
                }
            }

            if (SageSettings.Instance.InterruptDamageToHeal && !Core.Me.HasAura(1495))
            {
                if (Casting.CastingSpell == Spells.Dosis || Casting.CastingSpell == Spells.DosisII ||
                    Casting.CastingSpell == Spells.DosisIII || Casting.CastingSpell == Spells.Dyskrasia ||
                    Casting.CastingSpell == Spells.DyskrasiaII)
                {
                    if (PartyManager.VisibleMembers.Select(r => r.BattleCharacter).Any(r => r.CurrentHealth > 0 &&
                        r.CurrentHealthPercent <= SageSettings.Instance.InterruptDamageHealthPercent && r.Distance() < 30 && r.InLineOfSight()))
                    {
                        Logger.Error($@"Stopping Cast: Need To Heal Someone In The Party");
                        return true;
                    }
                }
            }

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            return false;
        }
        public static void GroupExtension()
        {
            Group.UpdateAlliance(
                SageSettings.Instance.IgnoreAlliance,
                SageSettings.Instance.HealAllianceDps,
                SageSettings.Instance.HealAllianceHealers,
                SageSettings.Instance.HealAllianceTanks,
                SageSettings.Instance.ResAllianceDps,
                SageSettings.Instance.ResAllianceHealers,
                SageSettings.Instance.ResAllianceTanks
            );
        }

        public static readonly uint[] ShieldAuraList = {
            Auras.NocturnalField,
            Auras.Galvanize,
            Auras.EukrasianDiagnosis,
            Auras.EukrasianPrognosis
        };

    }
}
