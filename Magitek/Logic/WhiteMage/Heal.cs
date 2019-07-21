using System;
using System.Linq;
using System.Threading.Tasks;
using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.WhiteMage;
using Magitek.Utilities;
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

                if (Core.Me.HasAura(Auras.Freecure))
                {
                    return await Spells.Cure2.Heal(Core.Me);
                }

                if (Core.Me.CurrentHealthPercent <= WhiteMageSettings.Instance.Cure2HealthPercent)
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

            if (!Globals.InParty)
                return false;

            if (Casting.LastSpell == Spells.Cure3)
                return false;

            var cure3Target = Group.CastableAlliesWithin30.FirstOrDefault(r => r.IsAlive && r.CurrentHealthPercent <= WhiteMageSettings.Instance.Cure3HealthPercent &&
                                                                               Group.CastableAlliesWithin30.Count( x => x.Distance(r) <= 6 &&
                                                                                                                        x.CurrentHealthPercent <= WhiteMageSettings.Instance.Cure3HealthPercent)
                                                                               >= WhiteMageSettings.Instance.Cure3Allies);

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

            if (Casting.LastSpell == Spells.Assize)
                return false;

            if (Casting.LastSpell == Spells.AfflatusRapture)
                return false;

            var medicaCount = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0 && r.Distance(Core.Me) <= 15 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.MedicaHealthPercent);

            if (medicaCount < WhiteMageSettings.Instance.MedicaAllies)
                return false;

            return await Spells.Medica.Heal(Core.Me, false);
        }
        
        public static async Task<bool> AssizeHeal()
        {
            if (!WhiteMageSettings.Instance.Assize)
                return false;

            if (!Core.Me.InCombat)
                return false;

            if (Casting.LastSpell == Spells.Medica)
                return false;

            if (Casting.LastSpell == Spells.Medica2)
                return false;

            if (Casting.LastSpell == Spells.AfflatusRapture)
                return false;

            var assizeCount = Group.CastableAlliesWithin30.Count(r => r.CurrentHealth > 0 && r.Distance(Core.Me) <= 15 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.AssizeHealthPercent);

            if (assizeCount < WhiteMageSettings.Instance.AssizeAllies)
                return false;

            return await Spells.Assize.Heal(Core.Me, false);
        }
        
        public static async Task<bool> Asylum()
        {
            if (!WhiteMageSettings.Instance.Asylum)
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
        
        public static async Task<bool> Medica2()
        {
            if (!WhiteMageSettings.Instance.Medica2)
                return false;

            if (Casting.LastSpell == Spells.Medica2)
                return false;

            if (Casting.LastSpell == Spells.Medica)
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

            return await Spells.Regen.Cast(regenTarget);
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

            return await Spells.Regen.Cast(regenTarget);
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

            return await Spells.Regen.Cast(regenTarget);
        }

        public static async Task<bool> Regen()
        {
            if (!WhiteMageSettings.Instance.Regen)
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

                return await Spells.Regen.Cast(Core.Me);
            }
        }
        
        public static async Task<bool> Raise()
        {
            if (!WhiteMageSettings.Instance.Raise)
                return false;

            if (!Globals.InParty)
                return false;

            if (Core.Me.CurrentMana < Spells.Raise.Cost)
                return false;

            var deadList = Group.DeadAllies.Where(u => !u.HasAura(Auras.Raise) &&
                                                       u.Distance(Core.Me) <= 30 &&
                                                       u.InLineOfSight() &&
                                                       u.IsTargetable)
                                           .OrderByDescending(r => r.GetResurrectionWeight());

            var deadTarget = deadList.FirstOrDefault();

            if (deadTarget == null)
                return false;

            if (!deadTarget.IsVisible)
                return false;

            if (!deadTarget.IsTargetable)
                return false;

            if (Core.Me.InCombat || Core.Me.OnPvpMap())
            {
                if (Core.Me.ClassLevel < 28)
                    return false;

                if (!WhiteMageSettings.Instance.RaiseSwiftcast)
                    return false;

                if (!ActionManager.HasSpell(Spells.Swiftcast.Id))
                    return false;

                if (Spells.Swiftcast.Cooldown != TimeSpan.Zero)
                    return false;

                if (await Buff.Swiftcast())
                {
                    if (WhiteMageSettings.Instance.ThinAirBeforeSwiftcastRaise && await Buff.ThinAir(true))
                    {
                        await Coroutine.Wait(3000, () => Core.Me.HasAura(Auras.ThinAir));
                    }

                    while (Core.Me.HasAura(Auras.Swiftcast))
                    {
                        if (await Spells.Raise.Cast(deadTarget)) return true;
                        await Coroutine.Yield();
                    }
                }
            }

            if (Core.Me.InCombat)
                return false;

            return await Spells.Raise.HealAura(deadTarget, Auras.Raise);
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
                    if (!await Medica2())
                        return await Spells.Medica.Cast(Core.Me);

            return false;
        }

        public static async Task<bool> AfflatusSolace()
        {
            if (!WhiteMageSettings.Instance.AfflatusSolace)
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

                    if (Casting.LastSpell == Spells.Tetragrammaton && Casting.LastSpellTarget == afflatusSolaceTankTarget)
                        return false;

                    return await Spells.AfflatusSolace.Heal(afflatusSolaceTankTarget, false);
                }

                var afflatusSolaceTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.WhiteMage.DontAfflatusSolace.Contains(r.Name) && r.CurrentHealth > 0 && r.CurrentHealthPercent <= WhiteMageSettings.Instance.AfflatusSolaceHealthPercent);

                if (afflatusSolaceTarget == null)
                    return false;

                if (Casting.LastSpell == Spells.Tetragrammaton && Casting.LastSpellTarget == afflatusSolaceTarget)
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

        public static async Task<bool> AfflatusRapture()
        {
            if (!WhiteMageSettings.Instance.AfflatusRapture)
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