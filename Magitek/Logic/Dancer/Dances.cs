using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
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
            if (Core.Me.ClassLevel < Spells.Tillana.LevelAcquired) 
                return false;

            if (!Core.Me.HasAura(Auras.FlourishingFinish)) 
                return false;

            if (!Core.Me.HasAura(Auras.Devilment)) 
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep)) 
                return false;

            if (ActionResourceManager.Dancer.Esprit >= 100)
                return false;

            return await Spells.Tillana.Cast(Core.Me.CurrentTarget);
        }

        public static async Task<bool> StandardStep()
        {
            if (!DancerSettings.Instance.UseStandardStep) 
                return false;

            if (!Spells.StandardStep.IsKnownAndReady()) 
                return false;

            //Do not Standard Step if there 3+ Ennemies unless StandardStep Auras is finishing
            if (DancerSettings.Instance.UseAoe && Combat.Enemies.Count(r => r.Distance(Core.Me) <= Spells.StandardFinish.Radius + r.CombatReach) >= 3)
            {
                Aura StandardStepAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.CasterId == Core.Player.ObjectId && x.Id == Auras.StandardStep);
                if (Core.Me.HasAura(Auras.StandardStep, true) && StandardStepAura.TimespanLeft.TotalMilliseconds > 5000)
                    return false;
            }

            if (Core.Me.HasAura(Auras.StandardStep)) 
                return false;

            if (Core.Me.HasAura(Auras.FlourishingStarfall, true)) 
                return false;

            Aura TechnicalFinishAura = (Core.Me as Character).Auras.FirstOrDefault(x => x.CasterId == Core.Player.ObjectId && x.Id == Auras.TechnicalFinish);
            if (Core.Me.HasAura(Auras.TechnicalFinish, true))
            {
                if (Core.Me.HasAura(Auras.SilkenSymmetry, true) || Core.Me.HasAura(Auras.SilkenFlow, true) || ActionResourceManager.Dancer.Esprit >= 50)
                    return false;

                if (TechnicalFinishAura.TimespanLeft.TotalMilliseconds >= 4000)
                    return false;
            }

            if (DancerSettings.Instance.DontDanceIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= DancerSettings.Instance.DontDanceIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            var procs = Core.Me.Auras.AuraList.Where(x => x.Caster == Core.Me && (
                x.Id == Auras.FlourshingCascade || x.Id == Auras.FlourishingFountain || x.Id == Auras.FlourshingShower ||
                x.Id == Auras.FlourshingWindmill || x.Id == Auras.FlourishingFlow || x.Id == Auras.FlourishingSymmetry ||
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

            if (DancerSettings.Instance.DontDanceIfCurrentTargetIsDyingSoon && Core.Me.CurrentTarget.CombatTimeLeft() <= DancerSettings.Instance.DontDanceIfCurrentTargetIsDyingWithinXSeconds)
                return false;

            if (DancerSettings.Instance.DevilmentWithTechnicalStep && !Core.Me.HasAura(Auras.Devilment))
                return false;

            if (Core.Me.HasAura(Auras.FlourishingStarfall, true)) return false;

            var procs = Core.Me.Auras.AuraList.Where(x => x.Caster == Core.Me && (
                x.Id == Auras.FlourshingCascade || x.Id == Auras.FlourishingFountain || x.Id == Auras.FlourshingShower ||
                x.Id == Auras.FlourshingWindmill || x.Id == Auras.FlourishingFlow || x.Id == Auras.FlourishingSymmetry ||
                x.Id == Auras.FourfoldFanDance
            ));

            if (procs.Any())
            {
                if (6500 + (Spells.Cascade.AdjustedCooldown.TotalMilliseconds * procs.Count()) < procs.Min(x => x.TimeLeft))
                    return false;

            }

            return await Spells.TechnicalStep.Cast(Core.Me);
        }

        public static async Task<bool> DanceFinish() // Just for Gambit Readability
        {
            return await DanceStep();
        }

        public static async Task<bool> DanceStep()
        {
            if (!Core.Me.HasAura(Auras.StandardStep) && !Core.Me.HasAura(Auras.TechnicalStep)) return false;

            if (Casting.LastSpell == Spells.DoubleStandardFinish) return false;

            if (Casting.LastSpell == Spells.QuadrupleTechnicalFinish) return false;

            try
            {
                Logger.Write($@"[Magitek] Current Step Dance To execute {ActionResourceManager.Dancer.CurrentStep}");
                switch (ActionResourceManager.Dancer.CurrentStep)
                {
                    case ActionResourceManager.Dancer.DanceStep.Finish:
                        if (DancerSettings.Instance.OnlyFinishStepInRange && Core.Me.CurrentTarget.Distance(Core.Me) > 15 + Core.Me.CurrentTarget.CombatReach)
                            return false;

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
