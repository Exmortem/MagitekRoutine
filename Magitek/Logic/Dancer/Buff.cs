using System;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Managers;
using Magitek.Extensions;
using Magitek.Models.Dancer;
using Magitek.Models.Samurai;
using Magitek.Utilities;

namespace Magitek.Logic.Dancer
{
    internal static class Buff
    {
        public static async Task<bool> DancePartner()
        {
            if (!DancerSettings.Instance.DancePartnerChocobo)
                return false;

            if (Core.Me.HasAura(Auras.ClosedPosition))
                return false;

            if (!ChocoboManager.Summoned)
                return false;

            return await Spells.ClosedPosition.Cast(ChocoboManager.Object);
        }

        public static async Task<bool> Devilment()
        {
            if (!DancerSettings.Instance.UseDevilment)
                return false;

            if (DancerSettings.Instance.DevilmentWithTechnicalStep)
                return false;

            if (DancerSettings.Instance.DevilmentWithFlourish && ActionManager.HasSpell(Spells.Flourish.Id) && Spells.Flourish.Cooldown < TimeSpan.FromSeconds(52))
                return false;

            return await Spells.Devilment.Cast(Core.Me);
        }

        public static async Task<bool> PreTechnicalDevilment()
        {
            if (!DancerSettings.Instance.UseDevilment)
                return false;

            if (DancerSettings.Instance.DevilmentWithFlourish)
                return false;

            if (DancerSettings.Instance.DevilmentWithTechnicalStep && ActionManager.HasSpell(Spells.TechnicalStep.Id) && Spells.TechnicalStep.Cooldown > TimeSpan.FromMilliseconds(1000))
                return false;

            if (DancerSettings.Instance.DevilmentWithTechnicalStep && !Core.Me.HasAura(Auras.StandardFinish))
                return false;
            
            return await Spells.Devilment.Cast(Core.Me);
        }

        public static async Task<bool> CuringWaltz()
        {
            if (!DancerSettings.Instance.UseCuringWaltz)
                return false;

            var cureTargets = PartyManager.AllMembers.Count(x => x.IsValid && x.BattleCharacter.CurrentHealthPercent < DancerSettings.Instance.CuringWaltzHP && x.BattleCharacter.Distance(Core.Me) < 5);

            if (Core.Me.HasAura(Auras.ClosedPosition))
            {
                var DancePartner = PartyManager.AllMembers.FirstOrDefault(x => x.BattleCharacter.HasMyAura(Auras.DancePartner));

                if (DancePartner != null)
                    cureTargets += PartyManager.AllMembers.Count(x => x.IsValid && x.BattleCharacter.CurrentHealthPercent < DancerSettings.Instance.CuringWaltzHP && x.BattleCharacter.Distance(DancePartner.BattleCharacter) < 5);
            }

            if (cureTargets < (PartyManager.IsInParty ? DancerSettings.Instance.CuringWaltzCount : 1))
                return false;

            return await Spells.CuringWaltz.Cast(Core.Me);
        }

        public static async Task<bool> Improvisation()
        {
            if (!DancerSettings.Instance.UseImprovisation)
                return false;

            if (ActionResourceManager.Dancer.Esprit > 80)
                return false;

            return await Spells.Improvisation.Cast(Core.Me);
        }

        private static uint[] FlourishingAuras = { Auras.FlourshingCascade, Auras.FlourshingFountain, Auras.FlourshingShower, Auras.FlourshingWindmill, Auras.FlourishingFanDance };

        public static async Task<bool> Flourish()
        {
            if (!DancerSettings.Instance.UseFlourish)
                return false;

            //if (Spells.TechnicalStep.Cooldown < TimeSpan.FromSeconds(3))
            //    return false;

            if (!Core.Me.HasAura(Auras.StandardFinish))
                return false;

            if (Core.Me.HasAnyAura(FlourishingAuras))
                return false;

            return await Spells.Flourish.Cast(Core.Me);
        }
    }
}