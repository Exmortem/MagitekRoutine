using Buddy.Coroutines;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Sage;
using Magitek.Toggles;
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
        public static async Task<bool> UseEukrasia(uint spellId = 24291, GameObject targetObject = null)
        {
            if (Core.Me.HasAura(Auras.Eukrasia, true))
                return true;
            if (!SageSettings.Instance.Eukrasia)
                return false;
            if (Casting.LastSpell == Spells.Eukrasia || Casting.CastingSpell == Spells.Eukrasia)
                return false;
            if (!await Spells.Eukrasia.Cast(Core.Me))
                return false;
            if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Eukrasia, true)))
                return false;
            var target = targetObject == null ? Core.Me : targetObject;
            return await Coroutine.Wait(1000, () => ActionManager.CanCast(spellId, target));
        }
        private static async Task<bool> UseZoe()
        {
            if (Core.Me.HasAura(Auras.Zoe))
                return true;

            if (!Spells.Zoe.IsKnownAndReady())
                return false;

            if (!await Spells.Zoe.Cast(Core.Me))
                return false;

            return await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.Zoe));
        }

        public static async Task<bool> Diagnosis()
        {
            if (!SageSettings.Instance.Diagnosis)
                return false;

            if (Globals.InParty)
            {
                var DiagnosisTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent < SageSettings.Instance.DiagnosisHpPercent || r.HasAura(Auras.Doom));

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
                var target = Group.CastableAlliesWithin30.FirstOrDefault(CanEukrasianDiagnosis);

                if (target == null)
                    return false;

                if (SageSettings.Instance.Zoe && SageSettings.Instance.ZoeEukrasianDiagnosis && !SageSettings.Instance.OnlyZoePneuma)
                    if (SageSettings.Instance.ZoeHealer && target.IsHealer()
                        || SageSettings.Instance.ZoeTank && target.IsTank(SageSettings.Instance.ZoeMainTank))
                        if (target.CurrentHealthPercent <= SageSettings.Instance.ZoeHealthPercent)
                            await UseZoe(); // intentionally ignore failures

                if (!await UseEukrasia(targetObject: target))
                    return false;

                return await Spells.EukrasianDiagnosis.HealAura(target, Auras.EukrasianDiagnosis);

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

                    return SageSettings.Instance.EukrasianDiagnosisOnlyTank && unit.IsTank(SageSettings.Instance.EukrasianDiagnosisOnlyMainTank);
                }
            }

            if (Core.Me.CurrentHealthPercent > SageSettings.Instance.EukrasianDiagnosisHpPercent || Core.Me.HasAura(Auras.EukrasianDiagnosis))
                return false;

            if (!await UseEukrasia())
                return false;

            return await Spells.EukrasianDiagnosis.HealAura(Core.Me, Auras.EukrasianDiagnosis);
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

            var targets = Group.CastableAlliesWithin15.Where(r => r.CurrentHealthPercent <= SageSettings.Instance.EukrasianPrognosisHealthPercent &&
                                                                !r.HasAura(Auras.EukrasianDiagnosis) &&
                                                                !r.HasAura(Auras.EukrasianPrognosis) &&
                                                                !r.HasAura(Auras.Galvanize));

            var needEukrasianPrognosis = targets.Count() >= SageSettings.Instance.EukrasianPrognosisNeedHealing;

            if (!needEukrasianPrognosis)
                return false;

            if (SageSettings.Instance.Zoe && SageSettings.Instance.ZoeEukrasianPrognosis && !SageSettings.Instance.OnlyZoePneuma)
                if (SageSettings.Instance.ZoeHealer && targets.Any(r => r.IsHealer())
                    || SageSettings.Instance.ZoeTank && targets.Any(r => r.IsTank(SageSettings.Instance.ZoeMainTank)))
                    if (targets.Any(r => r.CurrentHealthPercent <= SageSettings.Instance.ZoeHealthPercent))
                        await UseZoe(); // intentionally ignore failures

            if (!await UseEukrasia(Spells.EukrasianPrognosis.Id))
                return false;

            return await Spells.EukrasianPrognosis.Heal(Core.Me);
        }
        public static async Task<bool> ForceEukrasianPrognosis()
        {
            if (!SageSettings.Instance.ForceEukrasianPrognosis)
                return false;

            if (Core.Me.ClassLevel < Spells.Eukrasia.LevelAcquired)
                return false;

            if (!await UseEukrasia(Spells.EukrasianPrognosis.Id))
                return false;

            if (!await Spells.EukrasianPrognosis.Heal(Core.Me))
                return false;

            SageSettings.Instance.ForceEukrasianPrognosis = false;
            TogglesManager.ResetToggles();
            return true;
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
                var DruocholeTarget = Group.CastableAlliesWithin30.FirstOrDefault(r => r.CurrentHealthPercent <= SageSettings.Instance.DruocholeHpPercent);

                if (DruocholeTarget == null)
                    return false;

                return await Spells.Druochole.Heal(DruocholeTarget);
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

            if (Spells.Pepsis.Cooldown != TimeSpan.Zero)
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

            if (Spells.Pepsis.Cooldown != TimeSpan.Zero)
                return false;

            var needPepsis = Group.CastableAlliesWithin15.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PepsisEukrasianPrognosisHealthPercent) >= SageSettings.Instance.PepsisEukrasianPrognosisNeedHealing;

            if (!needPepsis)
                return false;

            if (!await UseEukrasianPrognosisIfNeeded(Group.CastableAlliesWithin15.Count(), Spells.Pepsis, Core.Me))
                return false;

            return await Spells.Pepsis.Cast(Core.Me);
        }
        public static async Task<bool> ForcePepsisEukrasianPrognosis()
        {
            if (!SageSettings.Instance.ForcePepsisEukrasianPrognosis)
                return false;

            if (Core.Me.ClassLevel < Spells.Eukrasia.LevelAcquired)
                return false;

            if (!Spells.Pepsis.IsKnownAndReady())
                return false;

            if (!await UseEukrasianPrognosisIfNeeded(Group.CastableAlliesWithin15.Count(), Spells.Pepsis, Core.Me))
                return false;

            if (!await Spells.Pepsis.Cast(Core.Me))
                return false;

            SageSettings.Instance.ForcePepsisEukrasianPrognosis = false;
            TogglesManager.ResetToggles();
            return true;
        }

        private static async Task<bool> UseEukrasianPrognosisIfNeeded(int NeedShields, SpellData forSpell, Character target)
        {
            var needPrognosis = Group.CastableAlliesWithin15.Count(r => r.HasAura(Auras.EukrasianPrognosis, true) || r.HasAura(Auras.EukrasianDiagnosis, true)) < NeedShields;

            if (needPrognosis)
            {
                if (!await UseEukrasia(Spells.EukrasianPrognosis.Id))
                    return false;

                if (!await Spells.EukrasianPrognosis.Cast(Core.Me))
                    return false;

                if (!await Coroutine.Wait(1000, () => Core.Me.HasAura(Auras.EukrasianPrognosis, true)))
                    return false;

                if (!await Coroutine.Wait(1000, () => ActionManager.CanCast(forSpell, target)))
                    return false;
            }

            return true;
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
        public static async Task<bool> ForceHaima()
        {
            if (!SageSettings.Instance.ForceHaima)
                return false;

            if (Core.Me.ClassLevel < Spells.Haima.LevelAcquired)
                return false;

            if (Spells.Haima.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var haimaCandidates = Group.CastableAlliesWithin30.Where(r => !r.HasAura(Auras.Weakness));

                if (SageSettings.Instance.HaimaTankForBuff)
                    haimaCandidates = haimaCandidates.Where(r => r.IsTank(SageSettings.Instance.HaimaMainTankForBuff));

                var haimaTarget = haimaCandidates.FirstOrDefault();

                if (haimaTarget == null)
                    return false;

                if (!await Spells.Haima.Heal(haimaTarget))
                    return false;
            }
            else
            {
                if (!await Spells.Haima.Cast(Core.Me))
                    return false;
            }

            SageSettings.Instance.ForceHaima = false;
            TogglesManager.ResetToggles();
            return true;
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
        public static async Task<bool> ForcePanhaima()
        {
            if (!SageSettings.Instance.ForcePanhaima)
                return false;

            if (Core.Me.ClassLevel < Spells.Panhaima.LevelAcquired)
                return false;

            if (!await Spells.Panhaima.Cast(Core.Me))
                return false;

            SageSettings.Instance.ForcePanhaima = false;
            TogglesManager.ResetToggles();
            return true;
        }
        public static async Task<bool> Egeiro()
        {
            return await Roles.Healer.Raise(
                Spells.Egeiro,
                SageSettings.Instance.SwiftcastRes,
                SageSettings.Instance.SlowcastRes,
                SageSettings.Instance.ResOutOfCombat
            );
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
                var pneumaTarget = Group.CastableAlliesWithin25.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PneumaHpPercent) >= SageSettings.Instance.PneumaNeedHealing;

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

            if (SageSettings.Instance.Zoe)
            {
                if (!SageSettings.Instance.ZoePneuma)
                    return false;
            }
            else if (!SageSettings.Instance.OnlyZoePneuma)
                return false;

            if (Core.Me.ClassLevel < Spells.Pneuma.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Spells.Pneuma.Cooldown != TimeSpan.Zero)
                return false;

            if (Globals.InParty)
            {
                var pneumaTarget = Group.CastableAlliesWithin25.Count(r => r.CurrentHealthPercent <= SageSettings.Instance.PneumaHpPercent) >= SageSettings.Instance.PneumaNeedHealing;

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

            if (!await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Pneuma.Id, Core.Me.CurrentTarget)))
                return false;

            return await Spells.Pneuma.Cast(Core.Me.CurrentTarget);
        }


        public static async Task<bool> ForceZoePneuma()
        {
            if (!SageSettings.Instance.ForceZoePneuma)
                return false;

            if (Core.Me.ClassLevel < Spells.Pneuma.LevelAcquired)
                return false;

            if (Core.Me.CurrentTarget == null)
                return false;

            if (Spells.Pneuma.Cooldown != TimeSpan.Zero)
                return false;

            if (!await UseZoe())
                return false;

            if (!await Coroutine.Wait(1000, () => ActionManager.CanCast(Spells.Pneuma.Id, Core.Me.CurrentTarget)))
                return false;

            if (!await Spells.Pneuma.Cast(Core.Me.CurrentTarget))
                return false;

            SageSettings.Instance.ForceZoePneuma = false;
            TogglesManager.ResetToggles();
            return true;
        }
    }
}
