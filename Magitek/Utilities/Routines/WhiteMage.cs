using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using System.Collections.Generic;
using System.Linq;

namespace Magitek.Utilities.Routines
{
    internal static class WhiteMage
    {
        public static bool OnGcd => Spells.Stone.Cooldown.TotalMilliseconds > 100;

        public static HashSet<string> DontCure = new HashSet<string>();
        public static HashSet<string> DontCure2 = new HashSet<string>();
        public static HashSet<string> DontRegen = new HashSet<string>();
        public static HashSet<string> DontTetraGrammaton = new HashSet<string>();
        public static HashSet<string> DontBenediction = new HashSet<string>();
        public static HashSet<string> DontAfflatusSolace = new HashSet<string>();
        public static List<Character> AllianceCureOnly = new List<Character>();

        private static HashSet<uint> DamageSpells = new HashSet<uint>()
        {
            Spells.Stone.Id,
            Spells.Stone2.Id,
            Spells.Stone3.Id,
            Spells.Stone4.Id,
            Spells.Holy.Id,
            Spells.Stone4.Id,
        };

        /*private static bool NeedCure2TankBuster
        {
            get
            {
                var a = Combat.Enemies.FirstOrDefault(r => r.IsCasting && TankBusterManager.Cure2List.Contains(r.CastingSpellId))?.TargetCharacter;

                if (a == null)
                    return false;

                return Casting.LastSpell != Spells.Cure2 || Casting.LastSpellTarget != a || DateTime.Now >= Casting.LastTankBusterTime.AddSeconds(5);
            }   
        }

        private static bool NeedMedicaTankBuster
        {
            get
            {
                if (Casting.LastSpell == Spells.Medica && Casting.LastSpellTarget == Core.Me && DateTime.Now < Casting.LastTankBusterTime.AddSeconds(5))
                    return false;

                return Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.MedicaList.Contains(r.CastingSpellId));
            }
        }
        
        private static bool NeedMedica2TankBuster
        {
            get
            {
                var enemyCasting = Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.Medica2List.Contains(r.CastingSpellId));
                return enemyCasting && Group.CastableAlliesWithin15.Count(r => !r.HasAura(Auras.Medica2)) < WhiteMageSettings.Instance.Medica2Allies;
            }
        }

        private static bool NeedDivineBenisonTankBuster
        {
            get
            {
                return ActionResourceManager.WhiteMage.Lily != 0 && Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.DivineBenison.Contains(r.CastingSpellId) && r.HasTarget &&
                                                                                            !r.TargetGameObject.HasAura(Auras.DivineBenison) && !r.TargetGameObject.HasAura(Auras.DivineBenison2));
            }
        }

        private static bool NeedAfflatusRaptureTankBuster
        {
            get
            {
                if (Casting.LastSpell == Spells.AfflatusRapture && Casting.LastSpellTarget == Core.Me && DateTime.Now < Casting.LastTankBusterTime.AddSeconds(5))
                    return false;

                return Combat.Enemies.Any(r => r.IsCasting && TankBusterManager.AfflatusRaptureList.Contains(r.CastingSpellId));
            }
        }
        */
        public static bool NeedToInterruptCast()
        {
            //if (Casting.CastingTankBuster)
            //    return false;

            if (Casting.CastingSpell != Spells.Raise && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Cast: Unit Died");
                return true;
            }

            if (Casting.CastingSpell == Spells.Raise && Casting.SpellTarget?.HasAura(Auras.Raise) == true)
            {
                Logger.Error($@"Stopped Resurrection: Unit has raise aura");
                return true;
            }

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (WhiteMageSettings.Instance.InterruptHealing && Casting.DoHealthChecks && Casting.SpellTarget?.CurrentHealthPercent >= WhiteMageSettings.Instance.InterruptHealingHealthPercent)
            {
                Logger.Error($@"Stopped Healing: Target's Health Too High");
                return true;
            }

            if (WhiteMageSettings.Instance.StopDpsIfPartyMemberBelow && DamageSpells.Contains(Core.Me.CastingSpellId))
            {
                if (Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < WhiteMageSettings.Instance.StopDpsIfPartyMemberBelowHealthPercent))
                {
                    Logger.Error($@"Stopped Casting Damage Spell: Ally Below Setting Health");
                    return true;
                }
            }

            //if (!WhiteMageSettings.Instance.UseTankBusters || !WhiteMageSettings.Instance.PrioritizeTankBusters)
            //    return false;

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            //if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.TankBusterMinimumMpPercent)
            //    return false;

            //if (!NeedCure2TankBuster && !NeedMedicaTankBuster && !NeedMedica2TankBuster && !NeedDivineBenisonTankBuster &&!NeedAfflatusRaptureTankBuster)
            //    return false;

            //Logger.Error($@"Stopping Cast: Need To Use A Tank Buster");
            return false;
        }

        public static void GroupExtension()
        {
            Group.UpdateAlliance(
                WhiteMageSettings.Instance.IgnoreAlliance,
                WhiteMageSettings.Instance.HealAllianceDps,
                WhiteMageSettings.Instance.HealAllianceHealers,
                WhiteMageSettings.Instance.HealAllianceTanks,
                WhiteMageSettings.Instance.ResAllianceDps,
                WhiteMageSettings.Instance.ResAllianceHealers,
                WhiteMageSettings.Instance.ResAllianceTanks
            );
        }
    }
}
