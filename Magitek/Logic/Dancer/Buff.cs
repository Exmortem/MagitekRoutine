using Buddy.Coroutines;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Logic.Roles;
using Magitek.Models.Dancer;
using Magitek.Utilities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Dancer
{
    internal static class Buff
    {
        private static uint[] FlourishingAuras = { Auras.FlourishingSymmetry, Auras.FlourishingFlow, Auras.ThreefoldFanDance, Auras.FourfoldFanDance };

        /************************************************************************************************************************
         *                                                 Damage
         * **********************************************************************************************************************/
        public static async Task<bool> Devilment()
        {
            if (!DancerSettings.Instance.UseDevilment)
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            if (Casting.LastSpell != Spells.QuadrupleTechnicalFinish)
                return false;

            return await Spells.Devilment.Cast(Core.Me);
        }

        public static async Task<bool> Flourish()
        {
            if (!DancerSettings.Instance.UseFlourish)
                return false;

            if (!Core.Me.HasAura(Auras.StandardFinish))
                return false;

            if (Core.Me.HasAnyAura(FlourishingAuras))
                return false;

            if (Spells.StandardStep.IsKnownAndReady())
                return false;

            if (Spells.TechnicalStep.IsKnownAndReady())
                return false;

            if (Spells.Devilment.IsKnownAndReady())
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            return await Spells.Flourish.Cast(Core.Me);
        }

        /************************************************************************************************************************
         *                                                 Healing
         * **********************************************************************************************************************/
        public static async Task<bool> CuringWaltz()
        {
            if (!DancerSettings.Instance.UseCuringWaltz)
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            var cureTargets = Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.CuringWaltzHP && x.Distance(Core.Me) < 5);

            if (Core.Me.HasAura(Auras.ClosedPosition))
            {
                var DancePartner = Group.CastableParty.FirstOrDefault(x => x.HasMyAura(Auras.DancePartner));

                if (DancePartner != null)
                    cureTargets += Group.CastableParty.Count(x => x.IsValid && x.CurrentHealthPercent < DancerSettings.Instance.CuringWaltzHP && x.Distance(DancePartner) < 5);
            }

            if (cureTargets < (Globals.InParty ? DancerSettings.Instance.CuringWaltzCount : 1))
                return false;

            return await Spells.CuringWaltz.Cast(Core.Me);
        }

        public static async Task<bool> Improvisation()
        {
            if (!DancerSettings.Instance.UseImprovisation)  
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            if (ActionResourceManager.Dancer.Esprit > 80) 
                return false;

            return await Spells.Improvisation.Cast(Core.Me);
        }


        /************************************************************************************************************************
         *                                                 Dance Partner
         * **********************************************************************************************************************/
        public static async Task<bool> DancePartner()
        {
            if (!DancerSettings.Instance.UseClosedPosition)
                return false;

            if (Core.Me.HasAura(Auras.ClosedPosition))
                return false;

            if (Core.Me.HasAura(Auras.StandardStep) || Core.Me.HasAura(Auras.TechnicalStep))
                return false;

            if (!Globals.InParty)
            {
                if (DancerSettings.Instance.DancePartnerChocobo && ChocoboManager.Summoned)
                    return await Spells.ClosedPosition.Cast(ChocoboManager.Object);
                else
                    return false;
            }

            IEnumerable<Character> allyList = null;

            switch (DancerSettings.Instance.SelectedStrategy)
            {
                case DancePartnerStrategy.ClosestDps:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsDps()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.MeleeDps:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsMeleeDps()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.RangedDps:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsRangedDpsCard()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.Tank:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsTank()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.Healer:
                    allyList = Group.CastableAlliesWithin30.Where(a => a.IsAlive && !a.IsMe && a.IsHealer()).OrderBy(GetWeight);
                    break;
            }

            if (allyList == null)
                    return false;

            return await Spells.ClosedPosition.CastAura(allyList.FirstOrDefault(), Auras.DancePartner);
        }

        private static int GetWeight(Character c)
        {
            switch (c.CurrentJob)
            {
                case ClassJobType.Reaper:
                    return DancerSettings.Instance.RprPartnerWeight;

                case ClassJobType.Sage:
                    return DancerSettings.Instance.SagPartnerWeight;

                case ClassJobType.Astrologian:
                    return DancerSettings.Instance.AstPartnerWeight;

                case ClassJobType.Monk:
                case ClassJobType.Pugilist:
                    return DancerSettings.Instance.MnkPartnerWeight;

                case ClassJobType.BlackMage:
                case ClassJobType.Thaumaturge:
                    return DancerSettings.Instance.BlmPartnerWeight;

                case ClassJobType.Dragoon:
                case ClassJobType.Lancer:
                    return DancerSettings.Instance.DrgPartnerWeight;

                case ClassJobType.Samurai:
                    return DancerSettings.Instance.SamPartnerWeight;

                case ClassJobType.Machinist:
                    return DancerSettings.Instance.MchPartnerWeight;

                case ClassJobType.Summoner:
                case ClassJobType.Arcanist:
                    return DancerSettings.Instance.SmnPartnerWeight;

                case ClassJobType.Bard:
                case ClassJobType.Archer:
                    return DancerSettings.Instance.BrdPartnerWeight;

                case ClassJobType.Ninja:
                case ClassJobType.Rogue:
                    return DancerSettings.Instance.NinPartnerWeight;

                case ClassJobType.RedMage:
                    return DancerSettings.Instance.RdmPartnerWeight;

                case ClassJobType.Dancer:
                    return DancerSettings.Instance.DncPartnerWeight;

                case ClassJobType.Paladin:
                case ClassJobType.Gladiator:
                    return DancerSettings.Instance.PldPartnerWeight;

                case ClassJobType.Warrior:
                case ClassJobType.Marauder:
                    return DancerSettings.Instance.WarPartnerWeight;

                case ClassJobType.DarkKnight:
                    return DancerSettings.Instance.DrkPartnerWeight;

                case ClassJobType.Gunbreaker:
                    return DancerSettings.Instance.GnbPartnerWeight;

                case ClassJobType.WhiteMage:
                case ClassJobType.Conjurer:
                    return DancerSettings.Instance.WhmPartnerWeight;

                case ClassJobType.Scholar:
                    return DancerSettings.Instance.SchPartnerWeight;
            }

            return c.CurrentJob == ClassJobType.Adventurer ? 70 : 0;
        }

        /************************************************************************************************************************
         *                                                 Potion
         * **********************************************************************************************************************/
        public static async Task<bool> UsePotion()
        {
            if (Spells.Devilment.IsKnown() && !Spells.Devilment.IsReady(8000))
                return false;

            return await PhysicalDps.UsePotion(DancerSettings.Instance);
        }

    }
}