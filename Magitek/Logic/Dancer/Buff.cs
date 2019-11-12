using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Managers;
using ff14bot.Objects;
using Magitek.Enumerations;
using Magitek.Extensions;
using Magitek.Models.Dancer;
using Magitek.Models.Dragoon;
using Magitek.Models.Samurai;
using Magitek.Utilities;
using Auras = Magitek.Utilities.Auras;

namespace Magitek.Logic.Dancer
{
    internal static class Buff
    {
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

            if (cureTargets < (Globals.InParty ? DancerSettings.Instance.CuringWaltzCount : 1))
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
            
            if (!Core.Me.HasAura(Auras.StandardFinish))
                return false;

            if (Core.Me.HasAnyAura(FlourishingAuras))
                return false;

            return await Spells.Flourish.Cast(Core.Me);
        }

        public static async Task<bool> DancePartner()
        {
            if (!DancerSettings.Instance.UseClosedPosition)
                return false;

            if (Core.Me.HasAura(Auras.ClosedPosition))
                return false;

            if (DancerSettings.Instance.DancePartnerChocobo && ChocoboManager.Summoned)
            {
                //if (!ChocoboManager.Summoned)
                //    return false;

                return await Spells.ClosedPosition.Cast(ChocoboManager.Object);
            }

            IEnumerable<Character> allyList = null;

            switch (DancerSettings.Instance.SelectedStrategy)
            {
                case DancePartnerStrategy.ClosestDps:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsDps()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.MeleeDps:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsMeleeDps()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.RangedDps:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsRangedDpsCard()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.Tank:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsTank()).OrderBy(GetWeight);
                    break;

                case DancePartnerStrategy.Healer:
                    allyList = Group.CastableAlliesWithin10.Where(a => a.IsAlive && !a.IsMe && a.IsHealer()).OrderBy(GetWeight);
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

    }
}