using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using static ff14bot.Managers.ActionResourceManager.Sage;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Sage
{
    internal static class Heal
    {
        private static async Task<bool> UseEukrasia()
        {
            if (!SageSettings.Instance.Eukrasia)
                return false;
            if (!await Spells.Eukrasia.Cast(Core.Me))
                return false;
            if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Eukrasia)))
                return false;
            return await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.EukrasianDiagnosis.Id, Core.Me));
        }

        public static async Task<bool> Diagnosis()
        {
            if (!SageSettings.Instance.Diagnosis)
                return false;

            if (Globals.InParty)
            {
                var DiagnosisTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent < SageSettings.Instance.DiagnosisHpPercent || r.HasAura(Auras.Doom));

                if (DiagnosisTarget != null)
                    return await Spells.Diagnosis.Heal(DiagnosisTarget);

                if (!SageSettings.Instance.HealAllianceOnlyDiagnosis)
                    return false;

                DiagnosisTarget = Utilities.Routines.Sage.AllianceDiagnosisOnly.FirstOrDefault(r => r.CurrentHealthPercent < SageSettings.Instance.DiagnosisHpPercent);

                if (DiagnosisTarget == null)
                    return false;

                return await Spells.Diagnosis.Heal(DiagnosisTarget);

            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.DiagnosisHpPercent)
                return false;

            return await Spells.Diagnosis.Heal(Core.Me);
        }

        public static async Task<bool> EukrasianDiagnosis()
        {
            if (!SageSettings.Instance.EukrasianDiagnosis)
                return false;

            if (Core.Me.ClassLevel < Spells.Eukrasia.LevelAcquired)
                return false;

            if (Globals.InParty)
            {
                // If the lowest heal target is higher than EukrasianDiagnosis health, check to see if the user wants us to shield the tank
                if (SageSettings.Instance.EukrasianDiagnosisTankForBuff && Globals.HealTarget?.CurrentHealthPercent > SageSettings.Instance.EukrasianDiagnosisHpPercent)
                {
                    // Pick any tank who doesn't have shield on them
                    var tankEukrasianDiagnosisTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.IsTank() && !r.HasAura(Auras.EukrasianDiagnosis));

                    if (tankEukrasianDiagnosisTarget == null)
                        return false;

                    await UseEukrasia();

                    return await Spells.EukrasianDiagnosis.HealAura(tankEukrasianDiagnosisTarget, Auras.EukrasianDiagnosis, false);
                }

                var EukrasianDiagnosisTarget = Group.CastableAlliesWithin30.FirstOrDefault(CanEukrasianDiagnosis);

                if (EukrasianDiagnosisTarget == null)
                    return false;

                await UseEukrasia();

                return await Spells.EukrasianDiagnosis.HealAura(EukrasianDiagnosisTarget, Auras.EukrasianDiagnosis);

                bool CanEukrasianDiagnosis(Character unit)
                {
                    if (unit == null)
                        return false;

                    if (unit.CurrentHealthPercent > SageSettings.Instance.EukrasianDiagnosisHpPercent)
                        return false;

                    if (unit.HasAura(Auras.EukrasianDiagnosis))
                        return false;

                    if (unit.HasAura(Auras.Galvanize))
                        return false;

                    if (!SageSettings.Instance.EukrasianDiagnosisOnlyHealer && !SageSettings.Instance.EukrasianDiagnosisOnlyTank)
                        return true;

                    if (SageSettings.Instance.EukrasianDiagnosisOnlyHealer && unit.IsHealer())
                        return true;

                    return SageSettings.Instance.EukrasianDiagnosisOnlyTank && unit.IsTank();
                }
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.EukrasianDiagnosisHpPercent || Core.Me.HasAura(Auras.EukrasianDiagnosis))
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(Core.Me, Auras.EukrasianDiagnosis);
        }
        private static async Task<bool> ShieldHealers()
        {
            if (!SageSettings.Instance.ShieldOnHealers)
                return false;

            var shieldTarget = SageSettings.Instance.ShieldKeepUpOnHealers
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && r.IsHealer() && !r.HasAura(Auras.EukrasianDiagnosis, true))
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && r.IsHealer() && r.CurrentHealthPercent <= SageSettings.Instance.ShieldHealthPercent && !r.HasAura(Auras.EukrasianDiagnosis, true));

            if (shieldTarget == null)
                return false;

            if (!await UseEukrasia())
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(shieldTarget, Auras.EukrasianDiagnosis);
        }
        private static async Task<bool> ShieldTanks()
        {
            if (!SageSettings.Instance.ShieldOnTanks)
                return false;

            var shieldTarget = SageSettings.Instance.ShieldKeepUpOnTanks ?
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.HasAura(Auras.EukrasianDiagnosis, true)) :
                Group.CastableTanks.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.HasAura(Auras.EukrasianDiagnosis, true) && r.CurrentHealthPercent <= SageSettings.Instance.ShieldHealthPercent);

            if (!MovementManager.IsMoving && SageSettings.Instance.OnlyShieldWhileMoving)
                return false;

            if (shieldTarget == null)
                return false;

            if (!await UseEukrasia())
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(shieldTarget, Auras.EukrasianDiagnosis); ;
        }

        private static async Task<bool> ShieldDps()
        {
            if (!SageSettings.Instance.ShieldOnDps)
                return false;

            var shieldTarget = SageSettings.Instance.ShieldKeepUpOnDps
                ? Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.IsTank() && !r.IsHealer() && !r.HasAura(Auras.EukrasianDiagnosis, true))
                : Group.CastableAlliesWithin30.FirstOrDefault(r => !Utilities.Routines.Sage.DontShield.Contains(r.Name) && r.CurrentHealth > 0 && !r.IsTank() && !r.IsHealer() && !r.HasAura(Auras.EukrasianDiagnosis, true) && r.CurrentHealthPercent <= SageSettings.Instance.ShieldHealthPercent);

            if (shieldTarget == null)
                return false;

            if (!await UseEukrasia())
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(shieldTarget, Auras.EukrasianDiagnosis);
        }

        public static async Task<bool> Shield()
        {
            if (!SageSettings.Instance.Shield)
                return false;

            if (Globals.InParty)
            {
                if (await ShieldTanks()) return true;
                if (await ShieldHealers()) return true;
                return await ShieldDps();
            }
            else
            {
                if (Core.Me.HasAura(Auras.EukrasianDiagnosis))
                    return false;

                if (!SageSettings.Instance.ShieldKeepUpOnHealers && Core.Me.CurrentHealthPercent > SageSettings.Instance.ShieldHealthPercent)
                    return false;

                if (!await UseEukrasia())
                    return false;

                return await Spells.EukrasianDiagnosis.HealAura(Core.Me, Auras.EukrasianDiagnosis);
            }
        }
        public static async Task<bool> Prognosis()
        {
            if (!SageSettings.Instance.Prognosis)
                return false;

            if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PrognosisHpPercent) < SageSettings.Instance.PrognosisNeedHealing)
                return false;

            return await Spells.Prognosis.Cast(Core.Me);
        }
        public static async Task<bool> EukrasianPrognosis()
        {
            if (!SageSettings.Instance.EukrasianPrognosis)
                return false;

            if (Core.Me.ClassLevel < Spells.Eukrasia.LevelAcquired)
                return false;

            var needEukrasianPrognosis = Group.CastableAlliesWithin15.Count(r => r.IsAlive &&
                                                                     r.CurrentHealthPercent <= SageSettings.Instance.EukrasianPrognosisHpPercent &&
                                                                     !r.HasAura(Auras.EukrasianPrognosis) && !r.HasAura(Auras.Galvanize)) >= SageSettings.Instance.EukrasianPrognosisNeedHealing;

            if (!needEukrasianPrognosis)
                return false;

            if (!await UseEukrasia())
                return false;

            return await Spells.EukrasianPrognosis.Heal(Core.Me);
        }
        public static async Task<bool> Physis()
        {
            if (!SageSettings.Instance.Physis)
                return false;

            if (Core.Me.ClassLevel < Spells.Physis.LevelAcquired)
                return false;

            if (Spells.Physis.Cooldown != TimeSpan.Zero)
                return false;

            if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PhysisHpPercent) < SageSettings.Instance.PhysisNeedHealing)
                return false;

            return await Spells.Physis.Cast(Core.Me);
        }
        public static async Task<bool> Druochole()
        {
            if (!SageSettings.Instance.Druochole)
                return false;

            if (Addersgall == 0)
                return false;

            if (Core.Me.ClassLevel < Spells.Druochole.LevelAcquired)
                return false;

            if (Globals.InParty)
            {
                var DruocholeTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent < SageSettings.Instance.DruocholeHpPercent);

                if (DruocholeTarget == null)
                    return false;

                return await Spells.Diagnosis.Heal(DruocholeTarget);
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.DruocholeHpPercent)
                return false;

            return await Spells.Druochole.Heal(Core.Me);
        }
        public static async Task<bool> Ixochole()
        {
            if (!SageSettings.Instance.Ixochole)
                return false;

            if (Addersgall == 0)
                return false;

            if (Core.Me.ClassLevel < Spells.Ixochole.LevelAcquired)
                return false;

            if (Spells.Ixochole.Cooldown != TimeSpan.Zero)
                return false;

            if (Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.IxocholeHpPercent) < SageSettings.Instance.IxocholeNeedHealing)
                return false;

            return await Spells.Ixochole.Cast(Core.Me);
        }
        public static async Task<bool> Pepsis()
        {
            if (!SageSettings.Instance.Pepsis)
                return false;

            if (Core.Me.ClassLevel < Spells.Eukrasia.LevelAcquired)
                return false;

            var needPepsis = Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PepsisHpPercent &&
                                                                     (r.HasAura(Auras.EukrasianPrognosis, true) || r.HasAura(Auras.EukrasianDiagnosis, true))) >= SageSettings.Instance.PepsisNeedHealing;

            if (!needPepsis)
                return false;

            return await Spells.Pepsis.Cast(Core.Me);

        }
        public static async Task<bool> PepsisEukrasianPrognosis()
        {
            if (!SageSettings.Instance.PepsisEukrasianPrognosis)
                return false;

            if (Core.Me.ClassLevel < Spells.Eukrasia.LevelAcquired)
                return false;

            var needPepsis = Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PepsisEukrasianPrognosisHpPercent) >= SageSettings.Instance.PepsisEukrasianPrognosisNeedHealing;

            if (!needPepsis)
                return false;

            if (!await UseEukrasia())
                return false;

            if (!await Spells.EukrasianPrognosis.Cast(Core.Me))
                return false;

            return await Spells.Pepsis.Cast(Core.Me);
        }
        public static async Task<bool> Taurochole()
        {
            if (!SageSettings.Instance.Taurochole)
                return false;

            if (Addersgall == 0)
                return false;

            if (Core.Me.HasAura(Auras.Kerachole))
                return false;

            if (Core.Me.ClassLevel < Spells.Taurochole.LevelAcquired)
                return false;

            if (Spells.Taurochole.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var taurocholeCandidates = Group.CastableAlliesWithin30.Where(r => r.CurrentHealthPercent < SageSettings.Instance.TaurocholeHpPercent);

                if (SageSettings.Instance.TaurocholeTankOnly)
                    taurocholeCandidates = taurocholeCandidates.Where(r => r.IsTank(SageSettings.Instance.TaurocholeMainTankOnly) || r.CurrentHealthPercent <= SageSettings.Instance.TaurocholeOthersHpPercent);

                var taurocholeTarget = taurocholeCandidates.FirstOrDefault();

                if (taurocholeTarget == null)
                    return false;

                return await Spells.Taurochole.Heal(taurocholeTarget);
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.TaurocholeHpPercent)
                return false;

            return await Spells.Taurochole.Cast(Core.Me);
        }
        public static async Task<bool> Haima()
        {
            if (!SageSettings.Instance.Haima)
                return false;

            if (Core.Me.ClassLevel < Spells.Haima.LevelAcquired)
                return false;

            if (Spells.Haima.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var haimaCandidates = Group.CastableAlliesWithin30.Where(r => r.CurrentHealthPercent < SageSettings.Instance.HaimaHpPercent
                                                                         && !r.HasAura(Auras.Weakness));

                if (SageSettings.Instance.HaimaTankForBuff)
                    haimaCandidates = haimaCandidates.Where(r => r.IsTank(SageSettings.Instance.HaimaMainTankForBuff));

                var haimaTarget = haimaCandidates.FirstOrDefault();

                if (haimaTarget == null)
                    return false;

                return await Spells.Haima.Heal(haimaTarget);
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.HaimaHpPercent)
                return false;

            return await Spells.Haima.Cast(Core.Me);
        }
        public static async Task<bool> Panhaima()
        {
            if (!SageSettings.Instance.Panhaima)
                return false;

            if (Core.Me.ClassLevel < Spells.Panhaima.LevelAcquired)
                return false;

            var needPanhaima = Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PanhaimaHpPercent) >= SageSettings.Instance.PanhaimaNeedHealing;

            if (!needPanhaima)
                return false;

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.PanhaimaHpPercent)
                return false;

            if (Globals.InParty)
            {
                var CanPanhaimaTargets = Group.CastableAlliesWithin15.Where(CanPanhaima).ToList();

                if (CanPanhaimaTargets.Count < SageSettings.Instance.PanhaimaNeedHealing)
                    return false;

                if (SageSettings.Instance.PanhaimaOnlyWithTank && !CanPanhaimaTargets.Any(r => r.IsTank(SageSettings.Instance.PanhaimaOnlyWithMainTank)))
                    return false;

                return await Spells.Panhaima.Cast(Core.Me);
            }

            return await Spells.Panhaima.Cast(Core.Me);

            bool CanPanhaima(Character unit)
            {
                if (unit == null)
                    return false;

                if (unit.CurrentHealthPercent > SageSettings.Instance.PanhaimaHpPercent)
                    return false;

                return unit.Distance(Core.Me) <= 15;
            }

        }
        public static async Task<bool> Egeiro()
        {
            if (!Globals.InParty)
                return false;

            var deadList = Group.DeadAllies.Where(u => u.CurrentHealth == 0 &&
                                                       !u.HasAura(Auras.Raise) &&
                                                       u.Distance(Core.Me) <= 30 &&
                                                       u.IsVisible &&
                                                       u.InLineOfSight() &&
                                                       u.IsTargetable)
                .OrderByDescending(r => r.GetResurrectionWeight());

            var deadTarget = deadList.FirstOrDefault();

            if (deadTarget == null)
                return false;

            if (Globals.PartyInCombat)
            {
                if (SageSettings.Instance.SwiftcastRes && Spells.Swiftcast.Cooldown == TimeSpan.Zero)
                {
                    // Prevent burning switftcast if no mana to actually rez.
                    if (!ActionManager.CanCast(Spells.Egeiro, deadTarget))
                        return false;

                    if (await Buff.Swiftcast())
                    {
                        while (Core.Me.HasAura(Auras.Swiftcast))
                        {
                            if (await Spells.Egeiro.CastAura(deadTarget, Auras.Raise))
                                return true;
                            await Coroutine.Yield();
                        }
                    }
                }
            }

            if (Globals.PartyInCombat && SageSettings.Instance.SlowcastRes || !Globals.PartyInCombat && SageSettings.Instance.ResOutOfCombat)
            {
                return await Spells.Egeiro.CastAura(deadTarget, Auras.Raise);
            }

            return false;
        }
        public static async Task<bool> Pneuma()
        {
            if (!SageSettings.Instance.Pneuma)
                return false;

            if (SageSettings.Instance.OnlyZoePneuma)
                return false;

            if (Core.Me.ClassLevel < Spells.Pneuma.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Spells.Pneuma.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var pneumaTarget = Group.CastableAlliesWithin25.Count(r => r.IsAlive &&
                                                                     r.CurrentHealthPercent <= SageSettings.Instance.PneumaHpPercent) >= SageSettings.Instance.PneumaNeedHealing;

                if (!pneumaTarget)
                    return false;

                return await Spells.Pneuma.Cast(Core.Me.CurrentTarget);
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.PneumaHpPercent)
                return false;

            return await Spells.Pneuma.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> ZoePneuma()
        {
            if (!SageSettings.Instance.Pneuma)
                return false;

            if (!SageSettings.Instance.OnlyZoePneuma)
                return false;

            if (Core.Me.ClassLevel < Spells.Pneuma.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Spells.Pneuma.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var pneumaTarget = Group.CastableAlliesWithin20.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PneumaHpPercent) >= SageSettings.Instance.PneumaNeedHealing;

                if (!pneumaTarget)
                    return false;

                if (!await UseZoe())
                    return false;

                return await Spells.Pneuma.Cast(Core.Me.CurrentTarget);
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.PneumaHpPercent)
                return false;

            if (!await UseZoe())
                return false;

            return await Spells.Pneuma.Cast(Core.Me.CurrentTarget);

            async Task<bool> UseZoe()
            {
                if (!SageSettings.Instance.OnlyZoePneuma)
                    return false;

                if (Spells.Zoe.Cooldown != TimeSpan.Zero)
                    return false;

                if (!await Spells.Zoe.Cast(Core.Me))
                    return false;

                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Zoe)))
                    return false;

                return await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Pneuma.Id, Core.Me.CurrentTarget));
            }
        }
    }
}
