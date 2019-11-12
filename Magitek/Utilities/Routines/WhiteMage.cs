using System;
using System.Collections.Generic;
using System.Linq;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities.Managers;

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
        
        private static bool NeedCure2TankBuster
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

        public static bool NeedToInterruptCast()
        {
            if (Casting.CastingTankBuster)
                return false;

            if (Casting.CastingSpell != Spells.Raise && Casting.SpellTarget?.CurrentHealth < 1)
            {
                Logger.Error($@"Stopped Cast: Unit Died");
                return true;
            }

            // Scalebound Extreme Rathalos
            if (Core.Me.HasAura(1495))
                return false;

            if (!Casting.CastingTankBuster && WhiteMageSettings.Instance.InterruptHealing && Casting.DoHealthChecks && Casting.SpellTarget?.CurrentHealthPercent >= WhiteMageSettings.Instance.InterruptHealingHealthPercent)
            {
                Logger.Error($@"Stopped Healing: Target's Health Too High");
                return true;
            }

            if (!Casting.CastingTankBuster && WhiteMageSettings.Instance.StopDpsIfPartyMemberBelow && DamageSpells.Contains(Core.Me.CastingSpellId))
            {
                if (Group.CastableAlliesWithin30.Any(r => r.CurrentHealthPercent < WhiteMageSettings.Instance.StopDpsIfPartyMemberBelowHealthPercent))
                {
                    Logger.Error($@"Stopped Casting Damage Spell: Ally Below Setting Health");
                    return true;
                }
            }

            if (!WhiteMageSettings.Instance.UseTankBusters || !WhiteMageSettings.Instance.PrioritizeTankBusters)
                return false;

            if (!Globals.InParty || !Globals.PartyInCombat)
                return false;

            if (Core.Me.CurrentManaPercent < WhiteMageSettings.Instance.TankBusterMinimumMpPercent)
                return false;

            if (!NeedCure2TankBuster && !NeedMedicaTankBuster && !NeedMedica2TankBuster && !NeedDivineBenisonTankBuster &&!NeedAfflatusRaptureTankBuster)
                return false;
            
            Logger.Error($@"Stopping Cast: Need To Use A Tank Buster");
            return true;
        }
        
        public static void GroupExtension()
        {
            // Should we be ignoring our alliance? Check to see if we're even in an instance
            if (!WhiteMageSettings.Instance.IgnoreAlliance && (Globals.InActiveDuty || WorldManager.InPvP))
            {
                // Create a list of alliance members that we need to check
                if (WhiteMageSettings.Instance.HealAllianceDps || WhiteMageSettings.Instance.HealAllianceHealers || WhiteMageSettings.Instance.HealAllianceTanks)
                {
                    var allianceToHeal =  Group.AllianceMembers.Where(a => !a.CanAttack && !a.HasAura(Auras.MountedPvp) && (WhiteMageSettings.Instance.HealAllianceDps && a.IsDps() ||
                                                                           WhiteMageSettings.Instance.HealAllianceTanks && a.IsTank() ||
                                                                           WhiteMageSettings.Instance.HealAllianceHealers && a.IsDps()));

                    // If all we're going to do with the alliance is Physick them, then simply use this list
                    if (WhiteMageSettings.Instance.HealAllianceOnlyCure)
                    {
                        AllianceCureOnly = allianceToHeal.ToList();
                    }
                    else
                    {
                        // If not, then sort the alliance members into the appropriate lists
                        foreach (var ally in allianceToHeal)
                        {
                            var distance = ally.Distance(Core.Me);

                            if (distance <= 30)
                            {
                                Group.CastableAlliesWithin30.Add(ally);
                            }

                            if (distance <= 15)
                            {
                                Group.CastableAlliesWithin15.Add(ally);
                            }

                            if (distance <= 10)
                            {
                                Group.CastableAlliesWithin10.Add(ally);
                            }
                        }
                    }
                }

                if (WhiteMageSettings.Instance.ResAllianceDps || WhiteMageSettings.Instance.ResAllianceHealers || WhiteMageSettings.Instance.ResAllianceTanks)
                {
                    var allianceToRes = Group.AllianceMembers.Where(a => a.CurrentHealth <= 0 &&
                                                                         (WhiteMageSettings.Instance.ResAllianceDps && a.IsDps() ||
                                                                          WhiteMageSettings.Instance.ResAllianceTanks && a.IsTank() ||
                                                                          WhiteMageSettings.Instance.ResAllianceHealers && a.IsDps()));

                    foreach (var ally in allianceToRes)
                    {
                        Group.DeadAllies.Add(ally);
                    }
                }
            }

            // Heal Pets
            if (!WhiteMageSettings.Instance.HealPartyMembersPets)
                return;

            var pets = WhiteMageSettings.Instance.HealPartyMembersPetsTitanOnly ? Group.Pets.Where(r => r.EnglishName.Contains("Titan")).ToArray() : Group.Pets.ToArray();
            Group.CastableAlliesWithin30.AddRange(pets);
        }
    }
}
