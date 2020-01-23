using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Extensions;
using Magitek.Models.Dancer;
using Magitek.Models.QueueSpell;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Dancer
{
    internal static class Dances
    {
        public static async Task<bool> StartStandardDance()
        {
            if (!DancerSettings.Instance.UseStandardStep)
                return false;

            if (Core.Me.HasAura(Auras.StandardStep))
                return false;

            if (Core.Me.HasAura(Auras.StandardFinish) && ActionManager.HasSpell(Spells.Flourish.Id) && Spells.Flourish.Cooldown < TimeSpan.FromSeconds(4))
                return false;

            if (Core.Me.HasAura(Auras.TechnicalFinish, true) && !Core.Me.HasAura(Auras.TechnicalFinish, true, 4000))
                return false;

            var procs = Core.Me.Auras.AuraList.Where(x => x.Caster == Core.Me && (x.Id == Auras.FlourshingCascade || x.Id == Auras.FlourshingFountain || x.Id == Auras.FlourshingShower || x.Id == Auras.FlourshingWindmill));

            if (procs.Any())
            {
                if (2500 + (Spells.Cascade.AdjustedCooldown.TotalMilliseconds * procs.Count()) < procs.Min(x => x.TimeLeft))
                    return false;
            }

            return await Spells.StandardStep.Cast(Core.Me);
        }

        public static bool StandardStep()
        {
            if (Core.Me.ClassLevel < 15)
                return false;

            if (Casting.LastSpell == Spells.DoubleStandardFinish)
                return false;

            if (!Core.Me.HasAura(Auras.StandardStep))
                return false;

            //if (Core.Me.CurrentTarget.Distance(Core.Me) > 40)
            //    return false;

            Logger.Write("Starting Dance Queue-up...");


            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.CancelSpellQueue = () => !Core.Me.HasAura(Auras.StandardStep);

            foreach (var step in ActionResourceManager.Dancer.Steps)
            {
                SpellData danceStep;

                Logger.Write($@"[Magitek] Dance Log {step}");

                switch (step)
                {
                    case ActionResourceManager.Dancer.DanceStep.Finish:
                        danceStep = Spells.DoubleStandardFinish;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Emboite:
                        danceStep = Spells.Emboite;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Entrechat:
                        danceStep = Spells.Entrechat;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Jete:
                        danceStep = Spells.Jete;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Pirouette:
                        danceStep = Spells.Pirouette;
                        break;

                    default:
                        danceStep = Spells.DoubleStandardFinish;
                        break;
                }

                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell
                {
                    Spell = danceStep,
                    TargetSelf = true,
                    Wait = new QueueSpellWait() { Check = () => Spells.Jete.Cooldown == TimeSpan.Zero, Name = "Next Dance Step",WaitTime = 3000 },

                } );
            }

            foreach (var spell in SpellQueueLogic.SpellQueue)
            {
                Logger.Write($"Queueing: {spell.Spell.Name}");
            }
            return true;
        }

        public static async Task<bool> StartTechnicalDance()
        {
            if (!DancerSettings.Instance.UseTechnicalStep)
                return false;

            if (Core.Me.HasAura(Auras.TechnicalFinish, true))
                return false;

            if (Core.Me.HasAura(Auras.TechnicalStep, true))
                return false;

            if (!Core.Me.HasAura(Auras.StandardFinish))
                return false;

            if (DancerSettings.Instance.DevilmentWithTechnicalStep && !Core.Me.HasAura(Auras.Devilment))
                return false;

            return await Spells.TechnicalStep.Cast(Core.Me);
        }

        public static bool TechnicalStep()
        {
            if (Core.Me.ClassLevel < 15)
                return false;

            if (Casting.LastSpell == Spells.QuadrupleTechnicalFinish)
                return false;

            if (!Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            //if (Core.Me.CurrentTarget.Distance(Core.Me) > 40)
            //    return false;

            Logger.Write("Starting Dance Queue-up...");
            SpellQueueLogic.SpellQueue.Clear();
            SpellQueueLogic.CancelSpellQueue = () => !Core.Me.HasAura(Auras.TechnicalStep);

            foreach (var step in ActionResourceManager.Dancer.Steps)
            {
                SpellData danceStep;

                switch (step)
                {
                    case ActionResourceManager.Dancer.DanceStep.Finish:
                        danceStep = Spells.QuadrupleTechnicalFinish;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Emboite:
                        danceStep = Spells.Emboite;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Entrechat:
                        danceStep = Spells.Entrechat;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Jete:
                        danceStep = Spells.Jete;
                        break;

                    case ActionResourceManager.Dancer.DanceStep.Pirouette:
                        danceStep = Spells.Pirouette;
                        break;

                    default:
                        danceStep = Spells.QuadrupleTechnicalFinish;
                        break;
                }

                SpellQueueLogic.SpellQueue.Enqueue(new QueueSpell
                {
                    Spell = danceStep,
                    TargetSelf = true,
                    Wait = new QueueSpellWait() { Check = () => Spells.Jete.Cooldown == TimeSpan.Zero, Name = "Next Dance Step", WaitTime = 3000 },

                });
            }

            foreach (var spell in SpellQueueLogic.SpellQueue)
            {
                Logger.Write($"Queueing: {spell.Spell.Name}");
            }
            return true;
        }

    }
}
