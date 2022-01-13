using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Toggles;
using Magitek.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.WhiteMage
{
    public class Heal
    {
        public static async Task<bool> Cure()
        {
            if (!WhiteMageSettings.Instance.Cure)
                return false;

            if (Globals.InParty)
            {
                foreach (var ally in Group.CastableAlliesWithin30)
                {
                    if (Utilities.Routines.WhiteMage.DontCure.Contains(ally.Name))
                        continue;

                    if (ally.CurrentHealthPercent > WhiteMageSettings.Instance.CureHealthPercent || ally.CurrentHealth <= 0)
                        continue;

                    if (!ally.HasAura(Auras.Regen))
                    {
                        return await CastCure(ally);
                    }

                    if (!WhiteMageSettings.Instance.RegenDontCureUnlessUnderDps && ally.IsDps())
                    {
                        return await CastCure(ally);
                    }

                    if (!WhiteMageSettings.Instance.RegenDontCureUnlessUnderHealer && ally.IsHealer())
                    {
                        return await CastCure(ally);
                    }

                    if (!WhiteMageSettings.Instance.RegenDontCureUnlessUnderTank && ally.IsTank())
                    {
                        return await CastCure(ally);
                    }

                    if (WhiteMageSettings.Instance.RegenDontCureUnlessUnderDps && ally.IsDps() && ally.CurrentHealthPercent < WhiteMageSettings.Instance.RegenDontCureUnlessUnderHealth)
                    {
                        return await CastCure(ally);
                    }

                    if (WhiteMageSettings.Instance.RegenDontCureUnlessUnderHealer && ally.IsHealer() && ally.CurrentHealthPercent < WhiteMageSettings.Instance.RegenDontCureUnlessUnderHealth)
                    {
                        return await CastCure(ally);
                    }

                    if (WhiteMageSettings.Instance.RegenDontCureUnlessUnderTank && ally.IsTank() && ally.CurrentHealthPercent < WhiteMageSettings.Instance.RegenDontCureUnlessUnderHealth)
                    {
                        return await CastCure(ally);
                    }
                }

                return false;
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.CureHealthPercent)
                    return false;

                if (WhiteMageSettings.Instance.Cure2 && Core.Me.HasAura(Auras.Freecure))
                {
                    return await Spells.Cure2.Heal(Core.Me);
                }

                if (Core.Me.ClassLevel >= 30 && WhiteMageSettings.Instance.Cure2 && Core.Me.CurrentHealthPercent <= WhiteMageSettings.Instance.Cure2HealthPercent)
                {
                    return await Spells.Cure2.Heal(Core.Me);
                }

                return await Spells.Cure.Heal(Core.Me);
            }

            async Task<bool> CastCure(GameObject ally)
            {
                return await Spells.Cure.Heal(ally);
            }
        }

        public static async Task<bool> Cure2()
        {
            if (!WhiteMageSettings.Instance.Cure2)
                return false;

            if (Core.Me.ClassLevel < Spells.Cure2.LevelAcquired)
                return false;

            if (Globals.InParty)
            {
                if (Core.Me.HasAura(Auras.Freecure) && Globals.HealTarget?.CurrentHealthPercent <= WhiteMageSettings.Instance.CureHealthPercent)
                {
                    return await Spells.Cure2.Heal(Globals.HealTarget);
                }

                var cure2Target = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontCure2.Contains(r.Name) && r.CurrentHealth > 0 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.Cure2HealthPercent);

                if (cure2Target == null)
                    return false;

                return await Spells.Cure2.Heal(cure2Target);
            }
            else
            {
                if (Core.Me.HasAura(Auras.Freecure))
                {
                    return await Spells.Cure2.Heal(Globals.HealTarget);
                }

                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.Cure2HealthPercent)
                    return false;

                return await Spells.Cure2.Heal(Core.Me);
            }
        }

        public static async Task<bool> Cure3()
        {
            if (!WhiteMageSettings.Instance.Cure3)
                return false;

            if (Core.Me.ClassLevel < Spells.Cure3.LevelAcquired)
                return false;

            if (!Globals.InParty)
                return false;

            if (Casting.LastSpell == Spells.Cure3)
                return false;


            //Choose the target that will hit the most in-need recipients. If there's a tie, pick the one where the recipients are missing the most health
            Character cure3Target = null;
            int bestTargetCount = 0;
            long mostHealthDown = 0;
            foreach (Character ally in Group.CastableAlliesWithin30.Where(r => r.IsAlive))
            {
                List<Character> nearby = Group.CastableAlliesWithin30.Where(r => r.IsAlive && ally.Distance(r) <= 6).ToList();
                int nearbyTargets = nearby.Count(r => r.CurrentHealthPercent <= WhiteMageSettings.Instance.Cure3HealthPercent);
                long totalHealthDown = nearby.Sum(r => r.MaxHealth - r.CurrentHealth);
                if (nearbyTargets >= WhiteMageSettings.Instance.Cure3Allies
                    && (nearbyTargets > bestTargetCount
                        || (nearbyTargets == bestTargetCount
                            && totalHealthDown > mostHealthDown)))
                {
                    bestTargetCount = nearbyTargets;
                    mostHealthDown = totalHealthDown;
                    cure3Target = ally;
                }
            }

            if (cure3Target == null)
                return false;

            if (WhiteMageSettings.Instance.ThinAirBeforeCure3 && await Buff.ThinAir(true))
            {
                await Coroutine.Wait(3000, () => Core.Me.HasAura(Auras.ThinAir));
            }

            return await Spells.Cure3.Heal(cure3Target, false);
        }

        public static async Task<bool> Benediction()
        {
            if (!WhiteMageSettings.Instance.Benediction)
                return false;

            if (Core.Me.ClassLevel < Spells.Benediction.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Globals.InParty)
            {
                if (WhiteMageSettings.Instance.BenedictionTankOnly)
                {
                    var tar = Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontBenediction.Contains(r.Name) && r.IsAlive && r.CurrentHealthPercent <= WhiteMageSettings.Instance.BenedictionHealthPercent);

                    if (tar == null)
                        return false;

                    if (Casting.LastSpell == Spells.Tetragrammaton && Casting.LastSpellTarget == tar)
                        return false;

                    return await Spells.Benediction.Heal(tar, false);
                }

                var benedictionTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontBenediction.Contains(r.Name) && r.CurrentHealth > 0 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.BenedictionHealthPercent);

                if (benedictionTarget == null)
                    return false;

                if (Casting.LastSpell == Spells.Tetragrammaton && Casting.LastSpellTarget == benedictionTarget)
                    return false;

                return await Spells.Benediction.Heal(benedictionTarget);
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.BenedictionHealthPercent)
                    return false;

                return await Spells.Benediction.Heal(Core.Me, false);
            }
        }

        public static async Task<bool> Tetragrammaton()
        {
            if (!WhiteMageSettings.Instance.Tetragrammaton)
                return false;

            if (Core.Me.ClassLevel < Spells.Tetragrammaton.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Globals.InParty)
            {
                if (WhiteMageSettings.Instance.TetragrammatonTankOnly)
                {
                    var tar = Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontTetraGrammaton.Contains(r.Name) && r.IsAlive && r.CurrentHealthPercent <= WhiteMageSettings.Instance.TetragrammatonHealthPercent);

                    if (tar == null)
                        return false;

                    if (Casting.LastSpell == Spells.Benediction && Casting.LastSpellTarget == tar)
                        return false;

                    return await Spells.Tetragrammaton.Heal(tar, false);
                }

                var tetragrammatonTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontTetraGrammaton.Contains(r.Name) && r.CurrentHealth > 0 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.TetragrammatonHealthPercent);

                if (tetragrammatonTarget == null)
                    return false;

                if (Casting.LastSpell == Spells.Benediction && Casting.LastSpellTarget == tetragrammatonTarget)
                    return false;

                return await Spells.Tetragrammaton.Heal(tetragrammatonTarget);
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.TetragrammatonHealthPercent)
                    return false;

                return await Spells.Tetragrammaton.Heal(Core.Me, false);
            }
        }

        public static async Task<bool> Medica()
        {
            if (!WhiteMageSettings.Instance.Medica)
                return false;

            if (Casting.LastSpell == Spells.Medica)
                return false;

            if (Casting.LastSpell == Spells.Medica2)
                return false;

            if (Casting.LastSpell == Spells.Assize)
                return false;

            if (Casting.LastSpell == Spells.AfflatusRapture)
                return false;

            var medicaCount = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0 && r.Distance(Core.Me) <= 15 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.MedicaHealthPercent);

            if (medicaCount < WhiteMageSettings.Instance.MedicaAllies)
                return false;

            return await Spells.Medica.Heal(Core.Me, false);
        }

        //public static async Task<bool> AssizeHeal()
        //{
        // if (!WhiteMageSettings.Instance.Assize)
        //return false;

        // if (!Core.Me.InCombat)
        //  return false;

        // if (Casting.LastSpell == Spells.Medica)
        //  return false;

        // if (Casting.LastSpell == Spells.Medica2)
        //   return false;

        // if (Casting.LastSpell == Spells.AfflatusRapture)
        //    return false;

        // var assizeCount = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0 && r.Distance(Core.Me) <= 15 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.AssizeHealthPercent);

        // if (assizeCount < WhiteMageSettings.Instance.AssizeAllies)
        //   return false;

        // return await Spells.Assize.Heal(Core.Me, false);
        //  }

        public static async Task<bool> Asylum()
        {
            if (!WhiteMageSettings.Instance.Asylum)
                return false;

            if (Core.Me.ClassLevel < Spells.Asylum.LevelAcquired)
                return false;

            if (!Globals.InParty)
                return false;

            if (!Globals.PartyInCombat)
                return false;

            if (MovementManager.IsMoving)
                return false;

            if (Combat.CombatTotalTimeLeft < 20)
                return false;

            var asylumTarget = Group.CastableTanks.FirstOrDefault(r => r.CurrentHealth > 0 &&
                                                                       r.CurrentHealthPercent <= WhiteMageSettings.Instance.AsylumHealthPercent &&
                                                                       Group.CastableAlliesWithin30.Count(x => x.CurrentHealth > 0 && x.Distance(r) <= 7 && x.CurrentHealthPercent <= WhiteMageSettings.Instance.AsylumHealthPercent) >= WhiteMageSettings.Instance.AsylumAllies - 1);

            if (asylumTarget == null)
                return false;

            return await Spells.Asylum.Cast(asylumTarget);
        }

        public static async Task<bool> LiturgyOfTheBell()
        {
            if (!WhiteMageSettings.Instance.LiturgyOfTheBell)
                return false;

            if (!Spells.LiturgyOfTheBell.IsKnownAndReady())
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Combat.CombatTotalTimeLeft < 20)
                return false;

            if (!Core.Me.HasTarget)
                return false;

            if (Globals.InParty)
            {
                var liturgyTargets = Group.CastableAlliesWithin30.Where(r => r.CurrentHealthPercent <= WhiteMageSettings.Instance.LiturgyOfTheBellHealthPercent);

                if (liturgyTargets.Count() < WhiteMageSettings.Instance.LiturgyOfTheBellAllies)
                    return false;

                Character target = liturgyTargets.FirstOrDefault();

                if (WhiteMageSettings.Instance.LiturgyOfTheBellCenterParty)
                {
                    var targets = Group.CastableAlliesWithin30.OrderBy(r =>
                        Group.CastableAlliesWithin30.Sum(ot => r.Distance(ot.Location))
                    ).ThenBy(t => Core.Me.Distance(t.Location));

                    target = targets.FirstOrDefault();
                }

                if (target == null)
                    return false;

                return await Spells.LiturgyOfTheBell.Cast(target);
            }

            if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.LiturgyOfTheBellHealthPercent)
                return false;

            return await Spells.LiturgyOfTheBell.Cast(Core.Me);
        }

        public static async Task<bool> Medica2()
        {
            if (!WhiteMageSettings.Instance.Medica2)
                return false;

            if (Core.Me.ClassLevel < Spells.Medica2.LevelAcquired)
                return false;

            if (Casting.LastSpell == Spells.Medica2)
                return false;

            if (Group.CastableAlliesWithin30.Count(CanMedica2) < WhiteMageSettings.Instance.Medica2Allies)
                return false;

            if (!await Spells.Medica2.Heal(Core.Me, false))
            {
                return false;
            }

            return await Coroutine.Wait(5000, () => MovementManager.IsMoving || Group.CastableAlliesWithin30.Count(CanMedica2) < WhiteMageSettings.Instance.Medica2Allies);

            bool CanMedica2(GameObject unit)
            {
                if (unit.Distance(Core.Me) > 15)
                    return false;

                if (unit.CurrentHealthPercent > WhiteMageSettings.Instance.Medica2HealthPercent)
                    return false;

                if (unit.HasAura(Auras.Medica2, true))
                    return false;

                return true;
            }
        }

        private static async Task<bool> RegenHealers()
        {
            if (!WhiteMageSettings.Instance.RegenOnHealers)
                return false;

            var regenTarget = WhiteMageSettings.Instance.RegenKeepUpOnHealers
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontRegen.Contains(r.Name) && r.CurrentHealth > 0 && r.IsHealer() && !r.HasMyRegen())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontRegen.Contains(r.Name) && r.CurrentHealth > 0 && r.IsHealer() && r.CurrentHealthPercent <= WhiteMageSettings.Instance.RegenHealthPercent && !r.HasMyRegen());

            if (regenTarget == null)
                return false;

            return await Spells.Regen.HealAura(regenTarget, Auras.Regen);
        }

        private static async Task<bool> RegenTanks()
        {
            if (!WhiteMageSettings.Instance.RegenOnTanks)
                return false;

            var regenTarget = WhiteMageSettings.Instance.RegenKeepUpOnTanks ?
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontRegen.Contains(r.Name) && r.CurrentHealth > 0 && !r.HasAura(Auras.Regen) && !r.HasMyRegen()) :
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontRegen.Contains(r.Name) && r.CurrentHealth > 0 && !r.HasAura(Auras.Regen) && r.CurrentHealthPercent <= WhiteMageSettings.Instance.RegenHealthPercent && !r.HasMyRegen());

            if (regenTarget == null)
                return false;

            if (!MovementManager.IsMoving && WhiteMageSettings.Instance.OnlyRegenWhileMoving)
                return false;

            return await Spells.Regen.HealAura(regenTarget, Auras.Regen);
        }

        private static async Task<bool> RegenDps()
        {
            if (!WhiteMageSettings.Instance.RegenOnDps)
                return false;

            var regenTarget = WhiteMageSettings.Instance.RegenKeepUpOnDps
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontRegen.Contains(r.Name) && r.CurrentHealth > 0 && !r.IsTank() && !r.IsHealer() && !r.HasMyRegen())
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontRegen.Contains(r.Name) && r.CurrentHealth > 0 && !r.IsTank() && !r.IsHealer() && !r.HasMyRegen() && r.CurrentHealthPercent <= WhiteMageSettings.Instance.RegenHealthPercent);

            if (regenTarget == null)
                return false;

            return await Spells.Regen.HealAura(regenTarget, Auras.Regen);
        }

        public static async Task<bool> Regen()
        {
            if (!WhiteMageSettings.Instance.Regen)
                return false;

            if (Core.Me.ClassLevel < Spells.Regen.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Globals.InParty)
            {
                if (await RegenTanks()) return true;
                if (await RegenHealers()) return true;
                return await RegenDps();
            }
            else
            {
                if (Core.Me.HasAura(Auras.Regen))
                    return false;

                if (!WhiteMageSettings.Instance.RegenKeepUpOnHealers && Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.RegenHealthPercent)
                    return false;

                return await Spells.Regen.HealAura(Core.Me, Auras.Regen);
            }
        }

        public static async Task<bool> Raise()
        {
            return await Roles.Healer.Raise(
                Spells.Raise,
                WhiteMageSettings.Instance.RaiseSwiftcast,
                WhiteMageSettings.Instance.Raise,
                WhiteMageSettings.Instance.Raise,
                ThinAir
            );

            async Task<bool> ThinAir(bool isSwiftcast)
            {
                if (isSwiftcast)
                {
                    if (WhiteMageSettings.Instance.ThinAirBeforeSwiftcastRaise && Spells.ThinAir.IsKnownAndReady() && await Buff.ThinAir(true))
                    {
                        return await Coroutine.Wait(3000, () => Core.Me.HasAura(Auras.ThinAir));
                    }
                }
                return true;
            }
        }

        public static async Task<bool> PlenaryIndulgence()
        {
            if (!WhiteMageSettings.Instance.PlenaryIndulgence)
                return false;

            var canPlenaryIndulgence = Group.CastableAlliesWithin30.Count(r => r.CurrentHealthPercent < WhiteMageSettings.Instance.PlenaryIndulgenceHealthPercent);

            if (canPlenaryIndulgence < WhiteMageSettings.Instance.PlenaryIndulgenceAllies)
                return false;

            if (await Spells.PlenaryIndulgence.Cast(Core.Me))
                if (!await Cure3())
                    if (!await AfflatusRapture())
                        if (!await Medica2())
                            return await Spells.Medica.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> AfflatusSolace()
        {
            if (!WhiteMageSettings.Instance.AfflatusSolace)
                return false;

            if (Core.Me.ClassLevel < Spells.AfflatusSolace.LevelAcquired)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (ActionResourceManager.WhiteMage.Lily < 1)
                return false;

            if (Globals.InParty)
            {
                if (WhiteMageSettings.Instance.AfflatusSolaceTankOnly)
                {
                    var afflatusSolaceTankTarget = Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontAfflatusSolace.Contains(r.Name) && r.IsAlive && r.CurrentHealthPercent <= WhiteMageSettings.Instance.AfflatusSolaceHealthPercent);

                    if (afflatusSolaceTankTarget == null)
                        return false;

                    if ((Casting.LastSpell == Spells.Tetragrammaton || Casting.LastSpell == Spells.Benediction) && Casting.LastSpellTarget == afflatusSolaceTankTarget)
                        return false;

                    return await Spells.AfflatusSolace.Heal(afflatusSolaceTankTarget, false);
                }

                var afflatusSolaceTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontAfflatusSolace.Contains(r.Name) && r.CurrentHealth > 0 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.AfflatusSolaceHealthPercent);

                if (afflatusSolaceTarget == null)
                    return false;

                if ((Casting.LastSpell == Spells.Tetragrammaton || Casting.LastSpell == Spells.Benediction) && Casting.LastSpellTarget == afflatusSolaceTarget)
                    return false;

                return await Spells.AfflatusSolace.Heal(afflatusSolaceTarget, false);
            }
            else
            {
                if (Core.Me.CurrentHealthPercent > WhiteMageSettings.Instance.AfflatusSolaceHealthPercent)
                    return false;

                return await Spells.AfflatusSolace.Heal(Core.Me, false);
            }
        }

        public static async Task<bool> ForceMedica()
        {
            if (!WhiteMageSettings.Instance.ForceMedica)
                return false;

            if (!await Spells.Medica.Heal(Core.Me)) return false;
            WhiteMageSettings.Instance.ForceMedica = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceMedicaII()
        {
            if (!WhiteMageSettings.Instance.ForceMedicaII)
                return false;

            if (!await Spells.Medica2.HealAura(Core.Me, Auras.Medica2)) return false;
            WhiteMageSettings.Instance.ForceMedicaII = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceAfflatusSolace()
        {
            if (!WhiteMageSettings.Instance.ForceAfflatusSolace)
                return false;

            if (!await Spells.AfflatusSolace.Heal(Core.Me.CurrentTarget, false)) return false;
            WhiteMageSettings.Instance.ForceAfflatusSolace = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceAfflatusRapture()
        {
            if (!WhiteMageSettings.Instance.ForceAfflatusRapture)
                return false;

            if (!await Spells.AfflatusRapture.Heal(Core.Me, false)) return false;
            WhiteMageSettings.Instance.ForceAfflatusRapture = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceCureII()
        {
            if (!WhiteMageSettings.Instance.ForceCureII)
                return false;

            if (!await Spells.Cure2.Heal(Core.Me.CurrentTarget, false)) return false;
            WhiteMageSettings.Instance.ForceCureII = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceCureIII()
        {
            if (!WhiteMageSettings.Instance.ForceCureIII)
                return false;

            if (!await Spells.Cure3.Heal(Core.Me.CurrentTarget, false)) return false;
            WhiteMageSettings.Instance.ForceCureIII = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceBenediction()
        {
            if (!WhiteMageSettings.Instance.ForceBenediction)
                return false;

            if (!await Spells.Benediction.Heal(Core.Me.CurrentTarget, false)) return false;
            WhiteMageSettings.Instance.ForceBenediction = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceRegen()
        {
            if (!WhiteMageSettings.Instance.ForceRegen)
                return false;

            if (!await Spells.Regen.HealAura(Core.Me.CurrentTarget, Auras.Regen)) return false;
            WhiteMageSettings.Instance.ForceRegen = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> ForceTetra()
        {
            if (!WhiteMageSettings.Instance.ForceTetra)
                return false;

            if (!await Spells.Tetragrammaton.Heal(Core.Me.CurrentTarget)) return false;
            WhiteMageSettings.Instance.ForceTetra = false;
            TogglesManager.ResetToggles();
            return true;
        }

        public static async Task<bool> AfflatusRapture()
        {
            if (!WhiteMageSettings.Instance.AfflatusRapture)
                return false;

            if (Core.Me.ClassLevel < Spells.AfflatusRapture.LevelAcquired)
                return false;

            if (Casting.LastSpell == Spells.Medica)
                return false;

            if (Casting.LastSpell == Spells.Medica2)
                return false;

            if (Casting.LastSpell == Spells.AfflatusRapture)
                return false;

            if (Casting.LastSpell == Spells.Assize)
                return false;

            var afflatusRaptureCount = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0 && r.Distance(Core.Me) <= 15 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.AfflatusRaptureHealthPercent);

            if (afflatusRaptureCount < WhiteMageSettings.Instance.AfflatusRaptureAllies)
                return false;

            return await Spells.AfflatusRapture.Heal(Core.Me, false);
        }
    }
}
