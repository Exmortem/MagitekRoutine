using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dancer;
using Magitek.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Dancer
{
    internal static class Dances
    {
        public static async Task<bool> Tillana()
        {
            if (Core.Me.ClassLevel < Spells.Tillana.LevelAcquired) return false;

            if (!Core.Me.HasAura(Auras.FlourishingFinish)) return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) return false;

            //if (Core.Me.CurrentTarget.Distance(Core.Me) > 15) return false;

            return await Spells.Tillana.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> StandardStep()
        {
            if (!DancerSettings.Instance.UseStandardStep) return false;

            if (!Spells.StandardStep.IsKnownAndReady()) return false;

            if (Core.Me.HasAura(Auras.StandardStep)) return false;

            //if (Core.Me.HasAura(Auras.StandardFinish) && ActionManager.HasSpell(Spells.Flourish.Id) && Spells.Flourish.Cooldown < TimeSpan.FromSeconds(4)) return false;

            if (Core.Me.HasAura(Auras.FlourishingStarfall, true)) return false;

            if (Core.Me.HasAura(Auras.TechnicalFinish, true) && !Core.Me.HasAura(Auras.TechnicalFinish, true, 4000)) return false;

            if (DancerSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= DancerSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            var procs = Core.Me.Auras.AuraList.Where(x => x.Caster == Core.Me && (
                x.Id == Auras.FlourshingCascade || x.Id == Auras.FlourshingFountain || x.Id == Auras.FlourshingShower ||
                x.Id == Auras.FlourshingWindmill || x.Id == Auras.FlourshingFlow || x.Id == Auras.FlourishingSymmetry ||
                x.Id == Auras.ThreefoldFanDance || x.Id == Auras.FourfoldFanDance || x.Id == Auras.FlourishingFinish
            ));

            if (procs.Any())
            {
                if (2500 + (Spells.Cascade.AdjustedCooldown.TotalMilliseconds * procs.Count()) < procs.Min(x => x.TimeLeft))
                    return false;
            }

            return await Spells.StandardStep.Cast(Core.Me);
        }


        public static async Task<bool> TechnicalStep()
        {
            if (!DancerSettings.Instance.UseTechnicalStep)
                return false;

            if (!Spells.TechnicalStep.IsKnownAndReady()) return false;

            if (Core.Me.HasAura(Auras.TechnicalFinish, true))
                return false;

            if (Core.Me.HasAura(Auras.TechnicalStep, true))
                return false;

            if (!Core.Me.HasAura(Auras.StandardFinish))
                return false;

            if (DancerSettings.Instance.DontDotIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= DancerSettings.Instance.DontDotIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            if (DancerSettings.Instance.DevilmentWithTechnicalStep && !Core.Me.HasAura(Auras.Devilment))
                return false;

            if (Core.Me.HasAura(Auras.FlourishingStarfall, true)) return false;

            var procs = Core.Me.Auras.AuraList.Where(x => x.Caster == Core.Me && (
                x.Id == Auras.FlourshingCascade || x.Id == Auras.FlourshingFountain || x.Id == Auras.FlourshingShower ||
                x.Id == Auras.FlourshingWindmill || x.Id == Auras.FlourshingFlow || x.Id == Auras.FlourishingSymmetry ||
                x.Id == Auras.FourfoldFanDance
            ));

            if (procs.Any())
            {
                if (6500 + (Spells.Cascade.AdjustedCooldown.TotalMilliseconds * procs.Count()) < procs.Min(x => x.TimeLeft))
                    return false;

            }

            return await Spells.TechnicalStep.Cast(Core.Me);
        }

        public static async Task<bool> DanceFinish() // Just for Gambit Readablity
        {
            return await DanceStep();
        }

        public static async Task<bool> DanceStep()
        {
            if (!Core.Me.HasAura(Auras.StandardStep) && !Core.Me.HasAura(Auras.TechnicalStep)) return false;

            if (Casting.LastSpell == Spells.DoubleStandardFinish) return false;

            if (Casting.LastSpell == Spells.QuadrupleTechnicalFinish) return false;

            if (DancerSettings.Instance.UseRangeAndFacingChecks)
            {
                if (Core.Me.CurrentTarget.Distance(Core.Me) > (15 + Core.Me.CurrentTarget.CombatReach))
                    return false;
            }

            try
            {
                Logger.Write($@"[Magitek] Dance Log {ActionResourceManager.Dancer.CurrentStep}");
                switch (ActionResourceManager.Dancer.CurrentStep)
                {
                    case ActionResourceManager.Dancer.DanceStep.Finish:
                        if (Core.Me.HasAura(Auras.StandardStep))
                            return await Spells.DoubleStandardFinish.Cast(Core.Me);
                        else if (Core.Me.HasAura(Auras.TechnicalStep))
                            return await Spells.QuadrupleTechnicalFinish.Cast(Core.Me);
                        else
                            return false;

                    case ActionResourceManager.Dancer.DanceStep.Emboite:
                        return await Spells.Emboite.Cast(Core.Me);

                    case ActionResourceManager.Dancer.DanceStep.Entrechat:
                        return await Spells.Entrechat.Cast(Core.Me);

                    case ActionResourceManager.Dancer.DanceStep.Jete:
                        return await Spells.Jete.Cast(Core.Me);

                    case ActionResourceManager.Dancer.DanceStep.Pirouette:
                        return await Spells.Pirouette.Cast(Core.Me);
                }
            }
            catch
            {
                // This is a safty. If CurrentStep is checked and your not dancing you get a memory read error.
                return false;
            }

            return false;
        }

    }
}
